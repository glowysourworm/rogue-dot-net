using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing.Interface;
using System;
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

        public LevelGrid CreateLayout(LayoutTemplate template)
        {
            // Build regions of cells to initialize the layout grid
            var grid = _regionBuilder.BuildRegions(template);

            // Identify Regions and create connectors
            IEnumerable<Region<GridCellInfo>> regions;

            if (IdentifyValidRegions(grid, out regions))
                _connectionBuilder.BuildConnections(grid, regions, template);

            // No Valid Regions -> return default layout
            else
                return CreateDefaultLayout();

            // Make Symmetric!
            if (template.MakeSymmetric)
            {
                // First, copy cells over
                MakeSymmetric(grid, template.SymmetryType);

                // Second, re-create corridors
                if (IdentifyValidRegions(grid, out regions))
                    _connectionBuilder.BuildConnections(grid, regions, template);

                // No Valid Regions -> return default layout
                else
                    return CreateDefaultLayout();
            }

            // Final room layer is calculated by the terrain builder
            IEnumerable<LayerInfo> terrainLayers;

            // If there are any terrain layers - proceed building them and re-creating any blocked corridors
            if (template.TerrainLayers.Any())
            {
                // Build Terrain -> Identify new regions -> Re-connect regions
                if (!_terrainBuilder.BuildTerrain(grid, regions, template, out terrainLayers))
                    return CreateDefaultLayout();
            }

            // Create empty terrain layer array
            else
                terrainLayers = new LayerInfo[] { };

            // Identify final room regions
            var roomRegions = grid.IdentifyRegions(cell => !cell.IsWall && !cell.IsCorridor);

            // Identify final corridor regions (1) Not a wall, 2) Marked a corridor, and 3) Not in any original region)
            var corridorRegions = grid.IdentifyRegions(cell => !cell.IsWall && cell.IsCorridor && !regions.Any(region => region[cell.Location.Column, cell.Location.Row] != null));

            // *** Iterate regions to re-create using GridLocation (ONLY SUPPORTED SERIALIZED TYPE FOR REGIONS)
            var finalRoomRegions = roomRegions.Select(region => ConvertRegion(grid, region));
            var finalCorridorRegions = corridorRegions.Select(region => ConvertRegion(grid, region));

            // Build layers
            var roomLayer = new LayerInfo("Room Layer", finalRoomRegions);
            var corridorLayer = new LayerInfo("Corridor Layer", finalCorridorRegions);

            // Build Walls around cells
            _wallFinisher.CreateWalls(grid, false);

            // Create Lighting
            _lightingFinisher.CreateLighting(grid, roomRegions, template);

            return new LevelGrid(grid, roomLayer, corridorLayer, terrainLayers);
        }

        /// <summary>
        /// Identifies regions - removing invalid ones. Returns false if there are no valid regions.
        /// </summary>
        private bool IdentifyValidRegions(GridCellInfo[,] grid, out IEnumerable<Region<GridCellInfo>> validRegions)
        {
            var regions = grid.IdentifyRegions(cell => !cell.IsWall);

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

        private LevelGrid CreateDefaultLayout()
        {
            var grid = _regionBuilder.BuildDefaultRegion();

            var roomRegions = grid.IdentifyRegions(cell => !cell.IsWall && !cell.IsCorridor);
            var corridorRegions = grid.IdentifyRegions(cell => cell.IsCorridor);

            // *** Iterate regions to re-create using GridLocation (ONLY SUPPORTED SERIALIZED TYPE FOR REGIONS)
            var finalRoomRegions = roomRegions.Select(region => ConvertRegion(grid, region));
            var finalCorridorRegions = corridorRegions.Select(region => ConvertRegion(grid, region));

            var roomLayer = new LayerInfo("Room Layer", finalRoomRegions);
            var corridorLayer = new LayerInfo("Corridor Layer", finalCorridorRegions);

            // Build Walls around cells
            _wallFinisher.CreateWalls(grid, false);

            // Create Lighting
            _lightingFinisher.CreateDefaultLighting(grid);

            return new LevelGrid(grid, roomLayer, corridorLayer, new LayerInfo[] { });
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

        /// <summary>
        /// Validates the level - returns false if there is an issue so that a default layout can be generated. NOTE*** THROWS EXCEPTIONS
        /// DURING DEBUG INSTEAD.
        /// </summary>
        private bool Validate(GridCellInfo[,] grid, LayerInfo roomLayer, IEnumerable<LayerInfo> terrainLayers)
        {
            var savePoint = false;
            var stairsUp = false;
            var stairsDown = false;

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] != null &&
                        grid[i, j].IsMandatory)
                    {
                        savePoint |= grid[i, j].MandatoryType == LayoutMandatoryLocationType.SavePoint;
                        stairsUp |= grid[i, j].MandatoryType == LayoutMandatoryLocationType.StairsUp;
                        stairsDown |= grid[i, j].MandatoryType == LayoutMandatoryLocationType.StairsDown;
                    }
                }
            }

#if DEBUG
            if (!savePoint)
                throw new Exception("Layout must have a mandatory cell for the save point");

            if (!stairsUp)
                throw new Exception("Layout must have a mandatory cell for the stairs up");

            if (!stairsDown)
                throw new Exception("Layout must have a mandatory cell for the stairs down");
#else
            if (!savePoint)
                return false;

            if (!stairsUp)
                return false;

            if (!stairsDown)
                return false;
#endif

            foreach (var region in roomLayer.Regions)
            {
                var roomConnector1 = false;
                var roomConnector2 = false;

                foreach (var location in region.Locations)
                {
                    if (grid[location.Column, location.Row].IsMandatory &&
                        grid[location.Column, location.Row].MandatoryType == LayoutMandatoryLocationType.RoomConnector1)
                        roomConnector1 = true;

                    if (grid[location.Column, location.Row].IsMandatory &&
                        grid[location.Column, location.Row].MandatoryType == LayoutMandatoryLocationType.RoomConnector2)
                        roomConnector2 = true;
                }

#if DEBUG
                if (!RegionValidator.ValidateRoomRegion(region))
                    throw new Exception("Room Region invalid");

                if (!roomConnector1)
                    throw new Exception("Room doesn't have a mandatory cell for connector 1");

                if (!roomConnector2)
                    throw new Exception("Room doesn't have a mandatory cell for connector 2");
#else
                if (!_regionValidator.ValidateRoomRegion(region))
                    return false;

                if (!roomConnector1)
                    return false;

                if (!roomConnector2)
                    return false;
#endif
            }

            return true;
        }
    }
}
