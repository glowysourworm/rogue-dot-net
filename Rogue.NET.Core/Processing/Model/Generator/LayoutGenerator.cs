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
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
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
        readonly IRegionTriangulationCreator _regionTriangulationCreator;
        readonly IWallFinisher _wallFinisher;
        readonly ILightingFinisher _lightingFinisher;

        [ImportingConstructor]
        public LayoutGenerator(IRegionBuilder regionBuilder,
                               ISymmetricBuilder symmetricBuilder,
                               IConnectionBuilder connectionBuilder,
                               ITerrainBuilder terrainBuilder,
                               IRegionTriangulationCreator regionTriangulationCreator,
                               IWallFinisher wallFinisher,
                               ILightingFinisher lightingFinisher)
        {
            _regionBuilder = regionBuilder;
            _symmetricBuilder = symmetricBuilder;
            _connectionBuilder = connectionBuilder;
            _terrainBuilder = terrainBuilder;
            _regionTriangulationCreator = regionTriangulationCreator;
            _wallFinisher = wallFinisher;
            _lightingFinisher = lightingFinisher;
        }

        public LayoutGrid CreateLayout(LayoutTemplate template)
        {
            // Create BASE layer for the connection builder
            var container = CreateRegions(template);

            // Calculates the terrain layers. Sets up a new base layer
            if (!CreateTerrain(container, template))
                return CreateDefaultLayout();

            // Identify regions to triangulate
            container.RecreateRegions();

            // Triangulate the result to create the connection layer
            var regionGraph = _regionTriangulationCreator.CreateTriangulation(container.GetRegions(LayoutGrid.LayoutLayer.Room), template);

            // Create Corridors (Connections only require the triangulation graph)
            if (template.ConnectionType == LayoutConnectionType.Corridor)
                _connectionBuilder.BuildConnections(container, template, regionGraph);

            // Create Maze Corridors (Connections require some re-triangulation)
            else if (template.ConnectionType == LayoutConnectionType.Maze)
            {
                switch (template.Type)
                {
                    // "Filled" maze maps look a little nicer inside rectangular rooms
                    case LayoutType.RectangularRegion:
                    case LayoutType.RandomRectangularRegion:
                    case LayoutType.MazeMap:
                        _connectionBuilder.CreateMazeCorridors(container, template, IMazeRegionCreator.MazeType.Filled);
                        break;
                    case LayoutType.RandomSmoothedRegion:
                    case LayoutType.CellularAutomataMazeMap:
                    case LayoutType.ElevationMap:
                    case LayoutType.ElevationMazeMap:
                    case LayoutType.CellularAutomataMap:
                    default:
                        _connectionBuilder.CreateMazeCorridors(container, template, IMazeRegionCreator.MazeType.Open);
                        break;
                }

                // Re-create triangulation for the walkable regions -> Re-connect using shortest paths
                container.RecreateRegions();

                // Fetch the ENTIRE WALKABLE MAP to create MORE corridors to ADD to the container
                regionGraph = _regionTriangulationCreator.CreateTriangulation(container.GetRegions(LayoutGrid.LayoutLayer.Walkable), template);

                // Add the corridors to the container
                _connectionBuilder.ConnectUsingShortestPath(container, regionGraph);
            }

            // Re-calculate regions for the layout finisher (TERRAIN MAY BE MODIFIED)
            container.RecreateRegions();

            // *** Layout Finishing

            // Build Walls around cells
            _wallFinisher.CreateWalls(container);

            // Build Doors on connection points for room regions
            _wallFinisher.CreateDoors(container, regionGraph);

            // Re-calculate final regions for the lighting finisher (MODIFIED FOR WALLS / DOORS)
            container.RecreateRegions();

            // Create Lighting
            _lightingFinisher.CreateLighting(container, template);

            // Finalize regions for the validator
            container.RecreateRegions();

            // Check for default in case of failure
            if (!Validate(container))
                return CreateDefaultLayout();

            return FinishLayout(container, regionGraph, template);
        }

        private LayoutGrid CreateDefaultLayout()
        {
            var container = _regionBuilder.BuildDefaultLayout();

            // TODO: CREATE STATIC DEFAULT TEMPLATE 
            var template = new LayoutTemplate()
            {
                Type = LayoutType.RectangularRegion,
                ConnectionType = LayoutConnectionType.Corridor,
                RoomColumnRatio = 0,
                RoomRowRatio = 0,
                FillRatioRooms = 1,
                RoomSize = 1,
                RoomSizeErradicity = 0
            };

            // Identify regions to triangulate
            container.RecreateRegions();

            // Triangulate the result to create the connection layer
            var regionGraph = _regionTriangulationCreator.CreateTriangulation(container.GetRegions(LayoutGrid.LayoutLayer.Room), template);

            // *** Layout Finishing

            // Build Walls around cells
            _wallFinisher.CreateWalls(container);

            // Build Doors on connection points for room regions
            _wallFinisher.CreateDoors(container, regionGraph);

            // Create Lighting
            _lightingFinisher.CreateLighting(container, template);

            // Modify regions for walls and doors
            container.RecreateRegions();

            // Using simple template to finish the default layout
            return FinishLayout(container, regionGraph, template);
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

            return true;
        }

        /// <summary>
        /// 3rd pass - finishes layout by creating the layers from the primary grid, regions, and terrain layers
        /// </summary>
        private LayoutGrid FinishLayout(LayoutContainer container, GraphInfo<GridCellInfo> connectionGraph, LayoutTemplate template)
        {
            // NOTE*** "Base" Regions are those that are calculated as the primary rooms for the layout.
            //
            //         The Graphs calculated by the triangulation routine can be random. The connections from these graphs are used to 
            //         add content; and plan movement routes for characters.
            //

            // *** Convert regions to final format
            var finalRegions = container.GetRegions(LayoutGrid.LayoutLayer.Full)
                                        .Select(region => ConvertRegion(container, region))
                                        .Actualize();

            var finalWalkableRegions = container.GetRegions(LayoutGrid.LayoutLayer.Walkable)
                                                .Select(region => ConvertRegion(container, region))
                                                .Actualize();

            var finalPlacementRegions = container.GetRegions(LayoutGrid.LayoutLayer.Placement)
                                                 .Select(region => ConvertRegion(container, region))
                                                 .Actualize();

            var finalRoomRegions = container.GetRegions(LayoutGrid.LayoutLayer.Room)
                                            .Select(region => ConvertRegion(container, region))
                                            .Actualize();

            var finalCorridorRegions = container.GetRegions(LayoutGrid.LayoutLayer.Corridor)
                                                .Select(region => ConvertRegion(container, region))
                                                .Actualize();

            var finalWallRegions = container.GetRegions(LayoutGrid.LayoutLayer.Wall)
                                                 .Select(region => ConvertRegion(container, region))
                                                 .Actualize();

            var finalImpassableRegions = container.GetRegions(LayoutGrid.LayoutLayer.ImpassableTerrain)
                                                  .Select(region => ConvertRegion(container, region))
                                                  .Actualize();

            var finalTerrianLayers = container.TerrainDefinitions.Select(definition =>
            {
                var terrainRegions = container.GetTerrainRegions(definition);

                return new LayerInfo<GridLocation>(definition.Name, terrainRegions, definition.IsPassable);
            });

            // Build layers
            var fullLayer = new LayerInfo<GridLocation>("Full Layer", finalRegions, false);
            var connectionLayer = new ConnectedLayerInfo<GridLocation>("Connection Layer", ConvertGraph(connectionGraph), finalWalkableRegions, true);
            var walkableLayer = new LayerInfo<GridLocation>("Walkable Layer", finalWalkableRegions, true);
            var placementLayer = new LayerInfo<GridLocation>("Placement Layer", finalPlacementRegions, true);
            var roomLayer = new LayerInfo<GridLocation>("Room Layer", finalRoomRegions, true);
            var corridorLayer = new LayerInfo<GridLocation>("Corridor Layer", finalCorridorRegions, true);
            var wallLayer = new LayerInfo<GridLocation>("Wall Layer", finalWallRegions, false);

            // Final 2D array for the constructor
            var grid = new GridCellInfo[container.Width, container.Height];

            // (Use of encapsulation made this necessary...)
            grid.Iterate((column, row) => grid[column, row] = container.Get(column, row));

            return new LayoutGrid(grid, 
                                  fullLayer, 
                                  connectionLayer, 
                                  walkableLayer, 
                                  placementLayer, roomLayer,
                                  corridorLayer, wallLayer, 
                                  finalTerrianLayers);
        }

        /// <summary>
        /// Pulls regions from a finalized layout container and ensures they meet requirements
        /// </summary>
        private bool Validate(LayoutContainer container)
        {
            if (container.GetRegions(LayoutGrid.LayoutLayer.Walkable)
                         .Any(region => !RegionValidator.ValidateBaseRegion(region)))
                return false;

            return true;
        }

        /// <summary>
        /// Converts Region to use GridLocation type for serialization.
        /// </summary>
        private Region<GridLocation> ConvertRegion(LayoutContainer container, Region<GridCellInfo> region)
        {
            return new Region<GridLocation>(region.Id,
                                            region.Locations.Select(location => container.Get(location.Column, location.Row).Location).ToArray(),
                                            region.EdgeLocations.Select(location => container.Get(location.Column, location.Row).Location).ToArray(),
                                            region.Boundary,
                                            new RegionBoundary(0, 0, container.Width, container.Height));
        }

        private GraphInfo<GridLocation> ConvertGraph(GraphInfo<GridCellInfo> graph)
        {
            return new GraphInfo<GridLocation>(graph.Connections
                                                    .Select(connection =>
                                                    {
                                                        return new RegionConnectionInfo<GridLocation>()
                                                        {
                                                            AdjacentLocation = connection.AdjacentLocation.Location,
                                                            Location = connection.Location.Location,
                                                            AdjacentVertex = connection.AdjacentVertex,
                                                            Vertex = connection.Vertex,
                                                            EuclideanRenderedDistance = connection.EuclideanRenderedDistance
                                                        };
                                                    })
                                                    .Actualize(), graph.Edges);
        }
    }
}
