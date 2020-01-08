using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;
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
        readonly ISymmetricBuilder _symmetricBuilder;
        readonly IConnectionBuilder _connectionBuilder;
        readonly ITerrainBuilder _terrainBuilder;
        readonly IWallFinisher _wallFinisher;
        readonly ILightingFinisher _lightingFinisher;

        [ImportingConstructor]
        public LayoutGenerator(IRegionBuilder regionBuilder,
                               ISymmetricBuilder symmetricBuilder,
                               IConnectionBuilder connectionBuilder,
                               ITerrainBuilder terrainBuilder,
                               IWallFinisher wallFinisher,
                               ILightingFinisher lightingFinisher)
        {
            _regionBuilder = regionBuilder;
            _symmetricBuilder = symmetricBuilder;
            _connectionBuilder = connectionBuilder;
            _terrainBuilder = terrainBuilder;
            _wallFinisher = wallFinisher;
            _lightingFinisher = lightingFinisher;
        }

        public LayoutGrid CreateLayout(LayoutTemplate template)
        {
            // Create BASE + CONNECTION layer for the connection builder
            var container = CreateRegions(template);

            // Check for default in case of failure
            if (!Validate(container))
                return CreateDefaultLayout();

            // Create Corridors (Connection points only require the triangulation graph)
            if (template.ConnectionType != LayoutConnectionType.ConnectionPoints)
                _connectionBuilder.BuildConnections(container, template);

            // Check for default in case of failure
            if (!Validate(container))
                return CreateDefaultLayout();

            // Calculates the terrain layers. Sets up a new connection layer if necessary 
            // due to impassable terrain
            if (!CreateTerrain(container, template))
                return CreateDefaultLayout();

            return FinishLayout(container, template);
        }

        private LayoutGrid CreateDefaultLayout()
        {
            var container = _regionBuilder.BuildDefaultLayout();

            container.SetTerrainLayers(new LayerInfo<GridCellInfo>[] { });

            // Using simple template to finish the default layout
            return FinishLayout(container, new LayoutTemplate()
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
        private LayoutContainer CreateRegions(LayoutTemplate template)
        {
            if (template.MakeSymmetric)
            {
                return _symmetricBuilder.CreateSymmetricLayout(template);
            }
            else
            {
                return _regionBuilder.BuildRegions(template);
            }
        }

        /// <summary>
        /// 2nd pass - creates terrain from primary grid and regions
        /// </summary>
        private bool CreateTerrain(LayoutContainer container, LayoutTemplate template)
        {
            // If there are any terrain layers - proceed building them and re-creating any blocked corridors
            if (template.TerrainLayers.Any())
            {
                // Build Terrain -> Identify new regions -> Re-connect regions
                if (!_terrainBuilder.BuildTerrain(container, template))
                    return false;
            }
            else
            {
                container.SetTerrainLayers(new LayerInfo<GridCellInfo>[] { });
            }

            return true;
        }

        /// <summary>
        /// 3rd pass - finishes layout by creating the layers from the primary grid, regions, and terrain layers
        /// </summary>
        private LayoutGrid FinishLayout(LayoutContainer container, LayoutTemplate template)
        {
            // NOTE*** "Base" Regions are those that are first calculated by the generator. The "Connection" Regions are those that are re-calculated after
            //         laying terrain (or) from using the Symmetric builder.
            //
            //         The Graphs calculated by the triangulation routine are NON-DETERMINISTIC. The connections from these graphs are used to 
            //         add content; and plan movement routes for characters.
            //
            //         The Graph of the BASE regions is either 1) lost to re-calculation (or) 2) stored in the base region graph. It just 
            //         depends on whether there was impassable terrain; or other modifications.
            //
            //         The "Room" Regions are re-calculated here IF THERE HAS BEEN ANY TERRAIN MODIFICATION. These are taken ONLY from the BASE 
            //         regions to preserve the original room structure; but there is no Graph connecting them. (This would require having a way to repair
            //         the triangulation that is damaged during terrain building)
            //
            //         The "Walkable" Regions are calculated here as a separate, non-connection layer. 
            //

            // Re-calculate the room regions
            var roomRegions = container.Grid.ConstructRegions(cell => !cell.IsWall &&
                                                                      // !cell.IsCorridor &&  *** NOT SURE ABOUT THESE CORRIDORS - THEY INVADE THE ORIGINAL ROOMS
                                                                      !container.TerrainLayers.Any(layer => !layer.IsPassable && layer[cell.Column, cell.Row] != null) &&
                                                                       container.BaseRegions.Any(region => region[cell.Column, cell.Row] != null));
            // *** Layout Finishing

            // Build Walls around cells
            _wallFinisher.CreateWalls(container.Grid, false);

            // Build Doors on connecting corridors - use original regions
            _wallFinisher.CreateDoors(container.Grid, container.BaseRegions, container.TerrainLayers);

            // Create Lighting
            _lightingFinisher.CreateLighting(container, template);

            // Identify final layout regions - INCLUDES ALL NON-EMPTY CELLS
            var regions = container.Grid.ConstructRegions(cell => true);

            // Identify final walkable regions (1) Not a wall, 2) Not impassable terrain)
            var walkableRegions = container.Grid.ConstructRegions(cell => !cell.IsWall &&
                                                               !container.TerrainLayers.Any(layer => layer[cell.Column, cell.Row] != null && !layer.IsPassable));

            // Identify placement layer (1) Not a wall, 2) Not a door, 3) Not impassable terrain)
            var placementRegions = container.Grid.ConstructRegions(cell => !cell.IsWall &&
                                                                !cell.IsDoor &&
                                                                !container.TerrainLayers.Any(layer => layer[cell.Column, cell.Row] != null && !layer.IsPassable));

            // Identify final corridor regions (1) Not a wall, 2) Marked a corridor)
            var corridorRegions = container.Grid.ConstructRegions(cell => !cell.IsWall &&
                                                                cell.IsCorridor);

            // Identify final wall regions
            var wallRegions = container.Grid.ConstructRegions(cell => cell.IsWall);

            // *** Iterate regions to re-create using GridLocation (ONLY SUPPORTED SERIALIZED TYPE FOR REGIONS)
            var finalRegions = regions.Select(region => ConvertRegion(container.Grid, region)).Actualize();
            var finalConnectionRegions = container.ConnectionLayer.ConnectionRegions.Select(region => ConvertConnectedRegion(container.Grid, region)).Actualize();
            var finalWalkableRegions = walkableRegions.Select(region => ConvertRegion(container.Grid, region)).Actualize();
            var finalPlacementRegions = placementRegions.Select(region => ConvertRegion(container.Grid, region)).Actualize();
            var finalRoomRegions = roomRegions.Select(region => ConvertRegion(container.Grid, region)).Actualize();
            var finalCorridorRegions = corridorRegions.Select(region => ConvertRegion(container.Grid, region)).Actualize();
            var finalWallRegions = wallRegions.Select(region => ConvertRegion(container.Grid, region)).Actualize();
            var finalTerrianLayers = container.TerrainLayers.Select(layerInfo =>
            {
                var terrainRegions = layerInfo.Regions.Select(region => ConvertRegion(container.Grid, region));

                return new LayerInfo<GridLocation>(layerInfo.LayerName, terrainRegions, layerInfo.IsPassable);
            });

            // Set connections from the connection regions to the finalized set
            //
            // TODO: Change triangulation code to set these from the graph after they're
            //       calculated. (Problem is that it sets ALL of the connections as it's 
            //       calculating the graph. This needs to be cleaned up after it finishes)
            //
            foreach (var edge in container.ConnectionLayer.RegionGraph.Edges)
            {
                var finalizedRegion1 = finalConnectionRegions.First(region => region.Id == edge.Point1.ReferenceId);
                var finalizedRegion2 = finalConnectionRegions.First(region => region.Id == edge.Point2.ReferenceId);

                var point1 = container.Grid[edge.Point1.Column, edge.Point1.Row].Location;
                var point2 = container.Grid[edge.Point2.Column, edge.Point2.Row].Location;

                finalizedRegion1.SetConnection(finalizedRegion2, point1, point2, edge.Distance);
                finalizedRegion2.SetConnection(finalizedRegion1, point2, point1, edge.Distance);
            }

            // Build layers
            var fullLayer = new LayerInfo<GridLocation>("Full Layer", finalRegions, false);
            var connectionLayer = new ConnectedLayerInfo<GridLocation>("Connection Layer", container.ConnectionLayer.RegionGraph, finalConnectionRegions, true);
            var walkableLayer = new LayerInfo<GridLocation>("Walkable Layer", finalWalkableRegions, true);
            var placementLayer = new LayerInfo<GridLocation>("Placement Layer", finalPlacementRegions, true);
            var roomLayer = new LayerInfo<GridLocation>("Room Layer", finalRoomRegions, true);
            var corridorLayer = new LayerInfo<GridLocation>("Corridor Layer", finalCorridorRegions, true);
            var wallLayer = new LayerInfo<GridLocation>("Wall Layer", finalWallRegions, false);

            return new LayoutGrid(container.Grid, fullLayer, connectionLayer, 
                                  walkableLayer, placementLayer, roomLayer,
                                  corridorLayer, wallLayer, finalTerrianLayers);
        }

        /// <summary>
        /// Identifies regions - removing invalid ones. Returns false if there are no valid regions.
        /// </summary>
        private bool Validate(LayoutContainer container)
        {
            if (container.BaseRegions.Any(region => !RegionValidator.ValidateBaseRegion(region)) ||
                container.ConnectionLayer.ConnectionRegions.Any(region => !RegionValidator.ValidateBaseRegion(region)))
                return false;

            return true;
        }

        /// <summary>
        /// Converts Region to use GridLocation type for serialization.
        /// </summary>
        private Region<GridLocation> ConvertRegion(GridCellInfo[,] grid, Region<GridCellInfo> region)
        {
            return new Region<GridLocation>(region.Id,
                                            region.Locations.Select(location => grid[location.Column, location.Row].Location).ToArray(),
                                            region.EdgeLocations.Select(location => grid[location.Column, location.Row].Location).ToArray(),
                                            region.Boundary,
                                            new RegionBoundary(0, 0, grid.GetLength(0), grid.GetLength(1)));
        }

        private ConnectedRegion<GridLocation> ConvertConnectedRegion(GridCellInfo[,] grid, ConnectedRegion<GridCellInfo> region)
        {
            return new ConnectedRegion<GridLocation>(region.Id,
                                                     region.Locations.Select(location => grid[location.Column, location.Row].Location).ToArray(),
                                                     region.EdgeLocations.Select(location => grid[location.Column, location.Row].Location).ToArray(),
                                                     region.Boundary,
                                                     new RegionBoundary(0, 0, grid.GetLength(0), grid.GetLength(1)));
        }
    }
}
