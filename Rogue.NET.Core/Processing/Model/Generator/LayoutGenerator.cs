using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Construction;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing.Interface;

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ILayoutGenerator))]
    public class LayoutGenerator : ILayoutGenerator
    {
        readonly IRegionBuilder _regionBuilder;
        readonly IConnectionBuilder _connectionBuilder;
        readonly ITerrainBuilder _terrainBuilder;
        readonly IWallFinisher _wallFinisher;
        readonly ILightingFinisher _lightingFinisher;

        [ImportingConstructor]
        public LayoutGenerator(IRegionBuilder regionBuilder,
                               IConnectionBuilder connectionBuilder,
                               ITerrainBuilder terrainBuilder,
                               IWallFinisher wallFinisher,
                               ILightingFinisher lightingFinisher)
        {
            _regionBuilder = regionBuilder;
            _connectionBuilder = connectionBuilder;
            _terrainBuilder = terrainBuilder;
            _wallFinisher = wallFinisher;
            _lightingFinisher = lightingFinisher;
        }

        public LayoutGrid CreateLayout(LayoutTemplate template)
        {
            GridCellInfo[,] grid;
            IEnumerable<ConnectedRegion<GridCellInfo>> regions;
            IEnumerable<ConnectedRegion<GridCellInfo>> modifiedRegions;
            IEnumerable<LayerInfo> terrainLayers;
            Graph regionGraph;
            Graph modifiedRegionGraph;

            // Calcualtes the grid, regions, and region graph
            if (!CreateRegionsAndCorridors(template, out grid, out regions, out regionGraph))
                return CreateDefaultLayout();

            // Calculates the terrain layers; but may not require calculating the region graph. It does so if there is
            // impassable terrain that alters the layout.
            if (!CreateTerrain(template, grid, regions, out modifiedRegions, out modifiedRegionGraph, out terrainLayers))
                return CreateDefaultLayout();

            return FinishLayout(template, grid, regions, modifiedRegions, regionGraph, modifiedRegionGraph, terrainLayers);
        }

        private LayoutGrid CreateDefaultLayout()
        {
            // TODO: CHANGE THIS TO REMOVE RECURSIVE CALL
            return CreateLayout(new LayoutTemplate()
            {
                Type = LayoutType.RectangularRegion,
                ConnectionType = LayoutConnectionType.Corridor,
                RoomColumnRatio = 0,
                RoomRowRatio = 0,
                FillRatioRooms = 1,
                RoomSize = 1,
                RoomSizeErradicity = 0
            });
        }

        /// <summary>
        /// 1st pass - creates the primary regions and corridors for the template
        /// </summary>
        private bool CreateRegionsAndCorridors(LayoutTemplate template, out GridCellInfo[,] grid, out IEnumerable<ConnectedRegion<GridCellInfo>> regions, out Graph regionGraph)
        {
            regionGraph = null;

            // Build regions of cells to initialize the layout grid
            grid = _regionBuilder.BuildRegions(template);

            // Identify and validate regions
            if (!IdentifyValidRegions(grid, out regions))
                return false;

            // Create corridors 1st time (if not a transporter connection type)
            if (template.ConnectionType != LayoutConnectionType.ConnectionPoints)
                regionGraph = _connectionBuilder.BuildConnections(grid, regions, template);

            // Make Symmetric!
            if (template.MakeSymmetric)
            {
                // First, copy cells over
                MakeSymmetric(grid, template.SymmetryType);

                // Re-validate regions -> Re-create corridors
                if (!IdentifyValidRegions(grid, out regions))
                    return false;

                // Create corridors 2nd time (if not a transporter connection type)
                if (template.ConnectionType != LayoutConnectionType.ConnectionPoints)
                    regionGraph = _connectionBuilder.BuildConnections(grid, regions, template);
            }

            return true;
        }

        /// <summary>
        /// 2nd pass - creates terrain from primary grid and regions
        /// </summary>
        private bool CreateTerrain(LayoutTemplate template,
                                   GridCellInfo[,] grid,
                                   IEnumerable<ConnectedRegion<GridCellInfo>> regions,
                                   out IEnumerable<ConnectedRegion<GridCellInfo>> modifiedRegions,
                                   out Graph modifiedRegionGraph,
                                   out IEnumerable<LayerInfo> terrainLayers)
        {
            modifiedRegions = null;
            modifiedRegionGraph = null;

            // If there are any terrain layers - proceed building them and re-creating any blocked corridors
            if (template.TerrainLayers.Any())
            {
                // Build Terrain -> Identify new regions -> Re-connect regions
                if (!_terrainBuilder.BuildTerrain(grid, regions, template, out modifiedRegions, out modifiedRegionGraph, out terrainLayers))
                    return false;
            }

            // Create empty terrain layer array
            else
                terrainLayers = new LayerInfo[] { };

            return true;
        }

        /// <summary>
        /// 3rd pass - finishes layout by creating the layers from the primary grid, regions, and terrain layers
        /// </summary>
        private LayoutGrid FinishLayout(LayoutTemplate template,
                                        GridCellInfo[,] grid,
                                        IEnumerable<Region<GridCellInfo>> baseRegions,
                                        IEnumerable<Region<GridCellInfo>> modifiedRegions,
                                        Graph baseRegionGraph,
                                        Graph modifiedRegionGraph,
                                        IEnumerable<LayerInfo> terrainLayers)
        {
            // NOTE*** "Base" Regions are those that are first calculated by the generator. The "Modified" Regions are those that are re-calculated after
            //         laying terrain.
            //
            //         The Graphs calculated by the triangulation routine are NON-DETERMINISTIC. The connections from these graphs are used to 
            //         add content.
            //
            //         The Graph of the BASE regions is either 1) lost to re-calculation (or) 2) stored in the base region graph. It just 
            //         depends on whether there was impassable terrain.
            //
            //         So, the final "Connection" regions are either 1) the BASE regions (or) 2) the MODIFIED regions - IF THE MODIFIED REGIONS ARE NOT NULL.
            //
            //         The "Room" Regions are re-calculated here IF THERE HAS BEEN ANY TERRAIN MODIFICATION. These are taken ONLY from the BASE 
            //         regions to preserve the original room structure; but there is no Graph connecting them. (This would require having a way to repair
            //         the triangulation that is damaged during terrain building)
            //
            //         The "Walkable" Regions are calculated here as a separate, non-connected layer. 
            //

            // Connection regions - BASE REGIONS ARE GUARANTEED
            var connectionRegions = modifiedRegions ?? baseRegions;
            var connectionGraph = modifiedRegionGraph ?? baseRegionGraph;

            // Re-calculate the room regions
            var roomRegions = grid.ConstructRegions(cell => !cell.IsWall &&
                                                            // !cell.IsCorridor &&  *** NOT SURE ABOUT THESE CORRIDORS - THEY INVADE THE ORIGINAL ROOMS
                                                            !terrainLayers.Any(layer => !layer.IsPassable && layer[cell.Column, cell.Row] != null) &&
                                                            baseRegions.Any(region => region[cell.Column, cell.Row] != null));
            // *** Layout Finishing

            // Build Walls around cells
            _wallFinisher.CreateWalls(grid, false);

            // Build Doors on connecting corridors - use original regions
            _wallFinisher.CreateDoors(grid, baseRegions, terrainLayers);

            // Create Lighting
            _lightingFinisher.CreateLighting(grid, template);

            // Identify final layout regions - INCLUDES ALL NON-EMPTY CELLS
            var regions = grid.ConstructRegions(cell => true);

            // Identify final walkable regions (1) Not a wall, 2) Not impassable terrain)
            var walkableRegions = grid.ConstructRegions(cell => !cell.IsWall &&
                                                               !terrainLayers.Any(layer => layer[cell.Column, cell.Row] != null && !layer.IsPassable));

            // Identify placement layer (1) Not a wall, 2) Not a door, 3) Not impassable terrain)
            var placementRegions = grid.ConstructRegions(cell => !cell.IsWall &&
                                                                !cell.IsDoor &&
                                                                !terrainLayers.Any(layer => layer[cell.Column, cell.Row] != null && !layer.IsPassable));

            // Identify final corridor regions (1) Not a wall, 2) Marked a corridor)
            var corridorRegions = grid.ConstructRegions(cell => !cell.IsWall &&
                                                                cell.IsCorridor);

            // Identify final wall regions
            var wallRegions = grid.ConstructRegions(cell => cell.IsWall);

            // *** Iterate regions to re-create using GridLocation (ONLY SUPPORTED SERIALIZED TYPE FOR REGIONS)
            var finalRegions = regions.Select(region => ConvertRegion(grid, region)).Actualize();
            var finalConnectionRegions = connectionRegions.Select(region => ConvertConnectedRegion(grid, region)).Actualize();
            var finalWalkableRegions = walkableRegions.Select(region => ConvertRegion(grid, region)).Actualize();
            var finalPlacementRegions = placementRegions.Select(region => ConvertRegion(grid, region)).Actualize();
            var finalRoomRegions = roomRegions.Select(region => ConvertRegion(grid, region)).Actualize();
            var finalCorridorRegions = corridorRegions.Select(region => ConvertRegion(grid, region)).Actualize();
            var finalWallRegions = wallRegions.Select(region => ConvertRegion(grid, region)).Actualize();

            // Build layers
            var fullLayer = new LayerInfo("Full Layer", finalRegions, false);
            var connectionLayer = new ConnectedLayerInfo("Connection Layer", connectionGraph, finalConnectionRegions, true);
            var walkableLayer = new LayerInfo("Walkable Layer", finalWalkableRegions, true);
            var placementLayer = new LayerInfo("Placement Layer", finalPlacementRegions, true);
            var roomLayer = new LayerInfo("Room Layer", finalRoomRegions, true);
            var corridorLayer = new LayerInfo("Corridor Layer", finalCorridorRegions, true);
            var wallLayer = new LayerInfo("Wall Layer", finalWallRegions, false);

            return new LayoutGrid(grid, fullLayer, connectionLayer, walkableLayer, placementLayer, roomLayer,
                                  corridorLayer, wallLayer, terrainLayers);
        }

        /// <summary>
        /// Identifies regions - removing invalid ones. Returns false if there are no valid regions.
        /// </summary>
        private bool IdentifyValidRegions(GridCellInfo[,] grid, out IEnumerable<ConnectedRegion<GridCellInfo>> validRegions)
        {
            var regions = grid.ConstructConnectedRegions(cell => !cell.IsWall);

            // Check for default room size constraints
            var invalidRegions = regions.Where(region => !RegionValidator.ValidateRoomRegion(region));

            // Set valid regions
            validRegions = regions.Except(invalidRegions);

            // Must have at least one valid region
            if (invalidRegions.Count() == regions.Count())
                return false;

            // Remove invalid regions
            else
            {
                foreach (var region in invalidRegions)
                {
                    // Remove region cells
                    foreach (var location in region.Locations)
                        grid[location.Column, location.Row] = null;
                }
            }

            return true;
        }

        /// <summary>
        /// Converts Region to use GridLocation type for serialization.
        /// </summary>
        private Region<GridLocation> ConvertRegion(GridCellInfo[,] grid, Region<GridCellInfo> region)
        {
            return new Region<GridLocation>(region.Locations.Select(location => grid[location.Column, location.Row].Location).ToArray(),
                                            region.EdgeLocations.Select(location => grid[location.Column, location.Row].Location).ToArray(),
                                            region.Boundary,
                                            new RegionBoundary(0, 0, grid.GetLength(0), grid.GetLength(1)));
        }

        private ConnectedRegion<GridLocation> ConvertConnectedRegion(GridCellInfo[,] grid, Region<GridCellInfo> region)
        {
            return new ConnectedRegion<GridLocation>(region.Locations.Select(location => grid[location.Column, location.Row].Location).ToArray(),
                                                     region.EdgeLocations.Select(location => grid[location.Column, location.Row].Location).ToArray(),
                                                     region.Boundary,
                                                     new RegionBoundary(0, 0, grid.GetLength(0), grid.GetLength(1)));
        }

        private void MakeSymmetric(GridCellInfo[,] grid, LayoutSymmetryType symmetryType)
        {
            switch (symmetryType)
            {
                case LayoutSymmetryType.LeftRight:
                    {
                        // E Half
                        for (int i = 0; i < grid.GetLength(0) / 2; i++)
                        {
                            var mirrorColumn = grid.GetLength(0) - i - 1;

                            for (int j = 0; j < grid.GetLength(1); j++)
                            {
                                // E -> W
                                if (grid[i, j] != null)
                                    grid[mirrorColumn, j] = new GridCellInfo(mirrorColumn, j) { IsWall = grid[i, j].IsWall, IsCorridor = grid[i, j].IsCorridor };

                                // W -> E
                                else if (grid[mirrorColumn, j] != null)
                                    grid[i, j] = new GridCellInfo(i, j) { IsWall = grid[mirrorColumn, j].IsWall, IsCorridor = grid[mirrorColumn, j].IsCorridor };
                            }
                        }
                    }
                    break;
                case LayoutSymmetryType.Quadrant:
                    {
                        // NE, SE, SW Quadrants
                        for (int i = 0; i < grid.GetLength(0) / 2; i++)
                        {
                            var mirrorColumn = grid.GetLength(0) - i - 1;

                            for (int j = 0; j < grid.GetLength(1) / 2; j++)
                            {
                                var mirrorRow = grid.GetLength(1) - j - 1;

                                GridCellInfo[] cells = new GridCellInfo[4];

                                // Find cell to mirror - start with NW

                                // NW
                                if (grid[i, j] != null)
                                    cells[0] = grid[i, j];

                                // NE
                                else if (grid[mirrorColumn, j] != null)
                                    cells[1] = grid[mirrorColumn, j];

                                // SE
                                else if (grid[mirrorColumn, mirrorRow] != null)
                                    cells[2] = grid[mirrorColumn, mirrorRow];

                                // SW
                                else if (grid[i, mirrorRow] != null)
                                    cells[3] = grid[i, mirrorRow];

                                var cell = cells.FirstOrDefault(x => x != null);

                                // Mirror cell over to other quadrants
                                if (cell != null)
                                {
                                    grid[i, j] = new GridCellInfo(i, j) { IsWall = cell.IsWall, IsCorridor = cell.IsCorridor };
                                    grid[mirrorColumn, j] = new GridCellInfo(mirrorColumn, j) { IsWall = cell.IsWall, IsCorridor = cell.IsCorridor };
                                    grid[i, mirrorRow] = new GridCellInfo(i, mirrorRow) { IsWall = cell.IsWall, IsCorridor = cell.IsCorridor };
                                    grid[mirrorColumn, mirrorRow] = new GridCellInfo(mirrorColumn, mirrorRow) { IsWall = cell.IsWall, IsCorridor = cell.IsCorridor };
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
