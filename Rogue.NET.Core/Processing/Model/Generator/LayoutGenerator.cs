
using Microsoft.Practices.ServiceLocation;

using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using static Rogue.NET.Core.Model.Scenario.Content.Layout.LayoutGrid;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ILayoutGenerator))]
    public class LayoutGenerator : ILayoutGenerator
    {
        readonly IRegionBuilder _regionBuilder;
        readonly ISymmetricBuilder _symmetricBuilder;
        readonly IConnectionBuilder _connectionBuilder;
        readonly IRegionTriangulationCreator _regionTriangulationCreator;
        readonly IWallFinisher _wallFinisher;
        readonly ILightingFinisher _lightingFinisher;

        [ImportingConstructor]
        public LayoutGenerator(IRegionBuilder regionBuilder,
                               ISymmetricBuilder symmetricBuilder,
                               IConnectionBuilder connectionBuilder,
                               IRegionTriangulationCreator regionTriangulationCreator,
                               IWallFinisher wallFinisher,
                               ILightingFinisher lightingFinisher)
        {
            _regionBuilder = regionBuilder;
            _symmetricBuilder = symmetricBuilder;
            _connectionBuilder = connectionBuilder;
            _regionTriangulationCreator = regionTriangulationCreator;
            _wallFinisher = wallFinisher;
            _lightingFinisher = lightingFinisher;
        }

        public LayoutGrid CreateLayout(LayoutTemplate template)
        {
            // Create BASE layer for the container
            var baseGrid = CreateRegions(template);

            // INJECTING STATEFUL SERVICE
            var container = new LayoutContainer(ServiceLocator.Current.GetInstance<ITerrainBuilder>(),
                                                template, baseGrid);

            // PRE-CALCULATE MAZE CORRIDORS AS CONNECTION ROOMS
            if (template.ConnectionType == LayoutConnectionType.Maze)
            {
                switch (template.Type)
                {
                    // "Filled" maze maps look a little nicer inside rectangular rooms
                    case LayoutType.RectangularRegion:
                    case LayoutType.RandomRectangularRegion:
                    case LayoutType.MazeMap:
                        _connectionBuilder.CreateMazeRegionsInEmptySpace(container, template, IMazeRegionCreator.MazeType.Filled);
                        break;
                    case LayoutType.RandomSmoothedRegion:
                    case LayoutType.CellularAutomataMazeMap:
                    case LayoutType.ElevationMap:
                    case LayoutType.ElevationMazeMap:
                    case LayoutType.CellularAutomataMap:
                        _connectionBuilder.CreateMazeRegionsInEmptySpace(container, template, IMazeRegionCreator.MazeType.Open);
                        break;
                    default:
                        throw new System.Exception("Unhandled layout type:  LayoutGenerator.CreateLayout");
                }
            }

            // Identify CONNECTION ROOMS to triangulate
            container.ProcessRegions();

            // Triangulate the CONNECTION ROOM layer to create the connection graph
            _regionTriangulationCreator.CreateTriangulation(container, template);

            // Build connections for the graph
            if (container.GetConnectionGraph().HasEdges())
            {
                // Build connections on the triangulation
                _connectionBuilder.BuildConnections(container, template);
            }

            // *** Layout Finishing: Finalize Terrain -> Create Walls -> Process Regions (Walls) -> Create Doors
            //                                        -> Finalize Layout -> Add Lighting -> Validate()

            container.FinalizeTerrain();

            // Build Walls around cells
            _wallFinisher.CreateWalls(container);

            // WALL layer changed
            container.ProcessRegions();

            // Build Doors on connection points for room regions
            // _wallFinisher.CreateDoors(container);

            // FINALIZE THE CONTAINER - NO MORE MODIFICATIONS TO LAYERS
            var finalizedContainer = container.Finalize();

            // Create Lighting
            _lightingFinisher.CreateLighting(finalizedContainer, template);

            // TODO: CREATE MORE IMPORTANT LAYOUT VALIDATOR
            if (container.GetRegions(LayoutGrid.LayoutLayer.Walkable)
                         .Sum(region => region.Locations.Length) < 4)
                return CreateDefaultLayout();

            // RUN FINAL VALIDATION ON THE LAYOUT CONTAINER
            if (!container.Validate())
                throw new System.Exception("Layout Container failed validation:  LayoutGenerator.CreateLayout");

            return FinishLayout(finalizedContainer, template);
        }

        private LayoutGrid CreateDefaultLayout()
        {
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

            // Create base regions
            var grid = _regionBuilder.BuildDefaultLayout(template);

            // Create the layout container
            var container = new LayoutContainer(ServiceLocator.Current.GetInstance<ITerrainBuilder>(), template, grid);

            // Identify regions to triangulate
            container.ProcessRegions();

            // Triangulate the result to create the connection layer
            _regionTriangulationCreator.CreateTriangulation(container, template);

            // *** Layout Finishing

            container.FinalizeTerrain();

            // Build Walls around cells
            _wallFinisher.CreateWalls(container);

            // Build Doors on connection points for room regions
            // _wallFinisher.CreateDoors(container);

            // FINALIZE THE CONTAINER - NO MORE MODIFICATIONS TO LAYERS
            var finalizedContainer = container.Finalize();

            // Create Lighting
            _lightingFinisher.CreateLighting(finalizedContainer, template);

            // Modify regions for walls and doors
            container.ProcessRegions();

            // Using simple template to finish the default layout
            return FinishLayout(finalizedContainer, template);
        }

        /// <summary>
        /// 1st pass - creates the primary regions and corridors for the template
        /// </summary>
        private GridCellInfo[,] CreateRegions(LayoutTemplate template)
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
        /// Finishes layout by creating the layers from the primary grid, regions, and terrain layers
        /// </summary>
        private LayoutGrid FinishLayout(FinalizedLayoutContainer container, LayoutTemplate template)
        {
            // NOTE*** "Base" Regions are those that are calculated as the primary rooms for the layout.
            //
            //         The Graphs calculated by the triangulation routine can be random. The connections from these graphs are used to 
            //         add content; and plan movement routes for characters.
            //

            // *** CONVERT THE LAYOUT
            var finalRegions = new Dictionary<LayoutLayer, IEnumerable<Region<GridLocation>>>();

            foreach (var layer in Enum.GetValues(typeof(LayoutLayer))
                                      .Cast<LayoutLayer>())
            {
                var regions = container.RegionDict[layer]
                                            .Select(region => ConvertRegion(region, (location) => location))
                                            .Actualize();

                finalRegions.Add(layer, regions);
            }

            var finalTerrianLayers = container.TerrainDict
                                              .Where(element => element.Value.Any())        // Remove layers with no terrain
                                              .Select(element => element.Key)
                                              .Select(definition => 
            {
                var terrainRegions = container.TerrainDict[definition]
                                              .Select(region => ConvertRegion(region, (terrainLocation) => terrainLocation));

                return new LayerInfo<GridLocation>(definition.Name, terrainRegions, definition.IsPassable);
            });

            // Build layers
            var connectionLayer = new ConnectedLayerInfo<GridLocation>("Connection Layer", ConvertGraph(container.Graph,
                                                                                           finalRegions[LayoutLayer.ConnectionRoom]),
                                                                                           finalRegions[LayoutLayer.ConnectionRoom],
                                                                                           true);

            // NOTE*** Impassability flag is separated in the LayoutGrid using the TerrainLayerMap.Impassable flag.
            //         So, treating the "full, and wall" layers as "impassable" won't have an effect.
            //
            var fullLayer = new LayerInfo<GridLocation>("Full No Terrain Support Layer", finalRegions[LayoutLayer.FullNoTerrainSupport], false);
            var walkableLayer = new LayerInfo<GridLocation>("Walkable Layer", finalRegions[LayoutLayer.Walkable], true);
            var placementLayer = new LayerInfo<GridLocation>("Placement Layer", finalRegions[LayoutLayer.Placement], true);
            var roomLayer = new LayerInfo<GridLocation>("Room Layer", finalRegions[LayoutLayer.Room], true);
            var corridorLayer = new LayerInfo<GridLocation>("Corridor Layer", finalRegions[LayoutLayer.Corridor], true);
            var wallLayer = new LayerInfo<GridLocation>("Wall Layer", finalRegions[LayoutLayer.Wall], true);
            var terrainSupportLayer = new LayerInfo<GridLocation>("Terrain Support Layer", finalRegions[LayoutLayer.TerrainSupport], true);

            return new LayoutGrid(container.Grid,
                                  connectionLayer,
                                  fullLayer,
                                  walkableLayer,
                                  placementLayer, 
                                  roomLayer,
                                  corridorLayer, 
                                  wallLayer,
                                  terrainSupportLayer,
                                  finalTerrianLayers);
        }

        /// <summary>
        /// Converts Region to use GridLocation type for serialization.
        /// </summary>
        private Region<T> ConvertRegion<T, TOther>(RegionInfo<TOther> region, Func<TOther, T> locationConverter) where T : class, IGridLocator
                                                                                                                 where TOther : class, IGridLocator
        {
            return new Region<T>(region.Id,
                                    region.Locations.Select(location => locationConverter(location)).ToArray(),
                                    region.EdgeLocations.Select(location => locationConverter(location)).ToArray(),
                                    region.Boundary,
                                    new RegionBoundary(0, 0, region.ParentBoundary.Width, region.ParentBoundary.Height));
        }

        private RegionGraph ConvertGraph(RegionGraphInfo<GridLocation> graph, IEnumerable<Region<GridLocation>> convertedRegions)
        {
            var convertedConnections = new List<RegionConnection>();

            // Graph with edges
            if (graph.HasEdges())
            {
                foreach (var connection in graph.GetConnections())
                {
                    var region1 = convertedRegions.First(region => region.Id == connection.Node.Id);
                    var region2 = convertedRegions.First(region => region.Id == connection.AdjacentNode.Id);

                    convertedConnections.Add(new RegionConnection(region1,
                                                                  region2,
                                                                  connection.Location,
                                                                  connection.AdjacentLocation,
                                                                  connection.EuclideanRenderedDistance));
                }

                return new RegionGraph(convertedConnections);
            }
            // Trivial graph
            else
            {
                return new RegionGraph(convertedRegions.Single());
            }
        }
    }
}
