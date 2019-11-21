﻿using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using static Rogue.NET.Core.Math.Algorithm.Interface.INoiseGenerator;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ILayoutGenerator))]
    public class LayoutGenerator : ILayoutGenerator
    {
        readonly IRegionGeometryCreator _regionGeometryCreator;
        readonly IRectangularRegionCreator _rectangularRegionCreator;
        readonly IMazeRegionCreator _mazeRegionCreator;
        readonly ICellularAutomataRegionCreator _cellularAutomataRegionCreator;
        readonly ICorridorCreator _corridorCreator;
        readonly INoiseGenerator _noiseGenerator;
        readonly ILightingGenerator _lightingGenerator;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        // Size is great enough for an upstairs, downstairs, and 2 teleport pods (in and out of the room)
        private const int ROOM_SIZE_MIN = 4;

        // Default name used to initialize empty layer
        private const string DEFAULT_LAYER_NAME = "Default Layer";

        [ImportingConstructor]
        public LayoutGenerator(IRegionGeometryCreator regionGeometryCreator, 
                               IRectangularRegionCreator rectangularRegionCreator, 
                               IMazeRegionCreator mazeRegionCreator, 
                               ICellularAutomataRegionCreator cellularAutomataRegionCreator, 
                               ICorridorCreator corridorCreator, 
                               INoiseGenerator noiseGenerator, 
                               ILightingGenerator lightingGenerator,
                               IRandomSequenceGenerator randomSequenceGenerator)
        {
            _regionGeometryCreator = regionGeometryCreator;
            _rectangularRegionCreator = rectangularRegionCreator;
            _mazeRegionCreator = mazeRegionCreator;
            _cellularAutomataRegionCreator = cellularAutomataRegionCreator;
            _corridorCreator = corridorCreator;
            _noiseGenerator = noiseGenerator;
            _lightingGenerator = lightingGenerator;
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public LevelGrid CreateLayout(LayoutTemplate template)
        {
            switch (template.Type)
            {
                case LayoutType.Maze:
                    return CreateMaze(template);
                case LayoutType.ConnectedRectangularRooms:
                    return CreateConnectedRectangularRooms(template);
                case LayoutType.ConnectedCellularAutomata:
                    return CreateCellularAutomata(template);
                default:
                    throw new Exception("Unhandled Layout Type");
            }
        }

        private LevelGrid CreateConnectedRectangularRooms(LayoutTemplate template)
        {
            switch (template.RoomPlacementType)
            {
                case LayoutRoomPlacementType.RectangularGrid:
                    return CreateRectangularGridRooms(template);
                case LayoutRoomPlacementType.Random:
                    return CreateRandomRooms(template);
                default:
                    throw new Exception("Unhandled Room Placement Type");
            }
        }

        private LevelGrid CreateRectangularGridRooms(LayoutTemplate template)
        {
            var grid = new GridCellInfo[template.Width, template.Height];

            // Create the room rectangles
            var roomBoundaries = _regionGeometryCreator.CreateGridRectangularRegions(template.Width, template.Height, 
                                                                                     template.NumberRoomCols, template.NumberRoomRows, 
                                                                                     template.RectangularGridPadding, 
                                                                                     new Range<int>(template.RoomWidthMin, template.RoomWidthLimit), 
                                                                                     new Range<int>(template.RoomHeightMin, template.RoomHeightLimit));

            // Create cells in the regions
            foreach (var boundary in roomBoundaries)
                _rectangularRegionCreator.CreateCells(grid, boundary, false);

            return FinishLayoutRectilinear(grid, template);
        }

        private LevelGrid CreateRandomRooms(LayoutTemplate template)
        {
            var grid = new GridCellInfo[template.Width, template.Height];

            // Create the room rectangles
            var roomBoundaries = _regionGeometryCreator.CreateRandomRectangularRegions(template.Width, template.Height, template.RandomRoomCount,
                                                                                       new Range<int>(template.RoomWidthMin, template.RoomWidthLimit),
                                                                                       new Range<int>(template.RoomHeightMin, template.RoomHeightLimit),
                                                                                       template.RandomRoomSpread);

            // Create contiguous regions - OVERWRITE EXISTING CELLS BECAUSE OF RANDOM LAYOUT
            foreach (var boundary in roomBoundaries)
                _rectangularRegionCreator.CreateCells(grid, boundary, true);

            return FinishLayoutRectilinear(grid, template);
        }

        private LevelGrid CreateCellularAutomata(LayoutTemplate template)
        {
            var grid = new GridCellInfo[template.Width, template.Height];
            var boundary = new RegionBoundary(new GridLocation(0, 0), template.Width, template.Height);

            // Create cells in the contiguous rectangle groups -> Remove regions that are too small
            _cellularAutomataRegionCreator.GenerateCells(grid, boundary, template.CellularAutomataType, template.CellularAutomataFillRatio, false);

            return FinishLayout(grid, template);
        }

        private LevelGrid CreateMaze(LayoutTemplate template)
        {
            var grid = new GridCellInfo[template.Width, template.Height];
            var boundary = new RegionBoundary(new GridLocation(0, 0), template.Width, template.Height);

            // Generate cells for the whole grid
            _mazeRegionCreator.CreateCells(grid, boundary, 0, false);

            var regions = grid.IdentifyRegions();

            CreateLighting(grid, regions, template);

            return new LevelGrid(grid, new LayerInfo("Room Layer", regions), new LayerInfo[] { new LayerInfo(DEFAULT_LAYER_NAME) });
        }

        private LevelGrid CreateOpenWorldLayout(LayoutTemplate template)
        {
            template.Width = 80;
            template.Height = 50;

            var grid = new GridCellInfo[template.Width, template.Height];
            var featureMap = new double[template.Width, template.Height];

            // To avoid extra iteration - use the callback to set up the grid cells
            _noiseGenerator.Run(NoiseType.PerlinNoise, template.Width, template.Height, 0.06, new PostProcessingCallback((column, row, value) =>
            {
                // Use the loop to create the grid and save an iteration - mark valleys as "Walls" to be carved out later
                grid[column, row] = new GridCellInfo(column, row)
                {
                    IsWall = value >= 0
                };

                featureMap[column, row] = value;
            }));

            var regions = grid.IdentifyRegions().ToList();

            // Remove regions / region cells where the room size is too small
            for (int i = regions.Count - 1; i >= 0; i--)
            {
                if (regions[i].Cells.Length < ROOM_SIZE_MIN)
                {
                    // Remove cells from the grid
                    foreach (var cell in regions[i].Cells)
                        grid[cell.Column, cell.Row] = null;

                    // Remove region
                    regions.RemoveAt(i);
                }
            }

            if (!regions.Any())
                return CreateDefaultLayout();

            // Create MST
            var minimumSpanningTree = GeometryUtility.PrimsMinimumSpanningTree(regions, Metric.MetricType.Roguian);

            // Create connections by trying to "follow the valleys"
            foreach (var edge in minimumSpanningTree.Edges)
            {
                var region1 = edge.Point1.Reference;
                var region2 = edge.Point2.Reference;

                var location1 = region1.GetConnectionPoint(region2, Metric.MetricType.Roguian);
                var location2 = region1.GetAdjacentConnectionPoint(region2, Metric.MetricType.Roguian);

                var dijkstraMap = new DijkstraMap(featureMap, location1, location2);

                // Run Dijkstra's Algorithm
                dijkstraMap.Run();

                // Generate Path locations
                var path = dijkstraMap.GeneratePath(true);

                // Add path to the grid
                foreach (var location in path)
                    grid[location.Column, location.Row].IsWall = false;

            }

            return new LevelGrid(grid, new LayerInfo("Room Layer", regions.ToArray()), new LayerInfo[] { new LayerInfo(DEFAULT_LAYER_NAME) });
        }

        /// <summary>
        /// Provides default layout for layouts that didn't contain any regions of viable size
        /// </summary>
        private LevelGrid CreateDefaultLayout()
        {
            var grid = new GridCellInfo[20, 10];

            _rectangularRegionCreator.CreateCells(grid, new RegionBoundary(new GridLocation(1, 1), 18, 8), false);

            var regions = grid.IdentifyRegions();

            CreateWalls(grid);

            return new LevelGrid(grid, new LayerInfo("Room Layer", regions.ToArray()), new LayerInfo[] { });
        }

        #region (private) Layout Finishing

        /// <summary>
        /// Triangulate rooms, locate and remove small rooms, create corridors, add walls
        /// </summary>
        private LevelGrid FinishLayout(GridCellInfo[,] grid, LayoutTemplate template)
        {
            // Create base regions
            var baseRegions = grid.IdentifyRegions();

            // Triangulate room positions
            //
            var graph = GeometryUtility.PrimsMinimumSpanningTree(baseRegions, Metric.MetricType.Roguian);

            // For each edge in the triangulation - create a corridor
            //
            foreach (var edge in graph.Edges)
            {
                var location1 = edge.Point1.Reference.GetConnectionPoint(edge.Point2.Reference, Metric.MetricType.Roguian);
                var location2 = edge.Point1.Reference.GetAdjacentConnectionPoint(edge.Point2.Reference, Metric.MetricType.Roguian);

                var cell1 = grid.Get(location1.Column, location1.Row);
                var cell2 = grid.Get(location2.Column, location2.Row);

                // Create the corridor cells
                //
                _corridorCreator.CreateCorridor(grid, cell1, cell2, false, false);
            }

            // Create Terrain - Check for reconstructed room layer
            LayerInfo reconstructedRoomLayer;

            var terrainLayers = CreateTerrain(grid, template, out reconstructedRoomLayer);

            //Create walls
            CreateWalls(grid);

            // Create Lighting
            CreateLighting(grid, baseRegions, template);

            return new LevelGrid(grid, reconstructedRoomLayer ?? new LayerInfo("Room Layer", baseRegions.ToArray()), terrainLayers);
        }

        private LevelGrid FinishLayoutRectilinear(GridCellInfo[,] grid, LayoutTemplate template)
        {
            var regions = grid.IdentifyRegions();

            // Create MST
            var minimumSpanningTree = GeometryUtility.PrimsMinimumSpanningTree(regions, Metric.MetricType.Roguian);

            // Create connections by drawing wide linear connector
            foreach (var edge in minimumSpanningTree.Edges)
            {
                var includedPoints = new List<GridLocation>();

                var left = (int)System.Math.Min(edge.Point1.Vertex.Column, edge.Point2.Vertex.Column);
                var right = (int)System.Math.Max(edge.Point1.Vertex.Column, edge.Point2.Vertex.Column);
                var top = (int)System.Math.Min(edge.Point1.Vertex.Row, edge.Point2.Vertex.Row);
                var bottom = (int)System.Math.Max(edge.Point1.Vertex.Row, edge.Point2.Vertex.Row);
                var vertices = new GridLocation[]
                {
                    new GridLocation(left, top),
                    new GridLocation(right, top),
                    new GridLocation(right, bottom),
                    new GridLocation(left, bottom)
                };

                // Add points that are part of one of the rooms
                foreach (var vertex in vertices)
                {
                    if (edge.Point1.Reference.Bounds.Contains(vertex) ||
                        edge.Point2.Reference.Bounds.Contains(vertex))
                        includedPoints.Add(vertex);
                }

                // Check to see if any of the vertices lies outside one of the rooms
                var midPoint = _randomSequenceGenerator.Get() > 0.5 ? vertices.FirstOrDefault(vertex => !includedPoints.Contains(vertex)) :
                                                                      vertices.LastOrDefault(vertex => !includedPoints.Contains(vertex));

                // If there's an exterior point, then use it as the mid point for the corridor
                //
                // NOTE** This is a "null" check (for the struct)
                if (vertices.Contains(midPoint))
                {
                    _corridorCreator.CreateLinearCorridorSection(grid, edge.Point1.Vertex, midPoint, edge.Point1.Vertex.Row != midPoint.Row, true);

                    _corridorCreator.CreateLinearCorridorSection(grid, midPoint, edge.Point2.Vertex, edge.Point2.Vertex.Row != midPoint.Row, true);
                }

                // Otherwise, just draw a line from one region to the other
                else
                {
                    // NOTE*** Since all vertices lie within both regions - just draw a straight line connecting
                    //         one of the off-diagonal vertices to the opposing center
                    var northSouthOriented = edge.Point1.Reference.Bounds.Bottom < edge.Point2.Reference.Bounds.Top ||
                                             edge.Point1.Reference.Bounds.Top > edge.Point2.Reference.Bounds.Bottom;

                    // Point1 -> Point 2 (off-diangonal or the actual center)
                    _corridorCreator.CreateLinearCorridorSection(grid, edge.Point1.Vertex, edge.Point2.Vertex, northSouthOriented, true);
                }
            }

            // Create Terrain - Check for new room layer
            LayerInfo reconstructedRoomLayer;

            var terrainLayers = CreateTerrain(grid, template, out reconstructedRoomLayer);

            CreateWalls(grid);

            // Create Lighting
            CreateLighting(grid, regions, template);

            return new LevelGrid(grid, reconstructedRoomLayer ?? new LayerInfo("Room Layer", regions.ToArray()), terrainLayers);
        }

        // Credit to this fellow for the idea for maze corridors!
        //
        // https://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/
        // https://github.com/munificent/hauberk/blob/db360d9efa714efb6d937c31953ef849c7394a39/lib/src/content/dungeon.dart
        //
        private LevelGrid FinishLayoutWithMazeCorridors(GridCellInfo[,] grid, LayoutTemplate template)
        {
            // Fill in the empty cells with walls
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // Create wall cell here
                    if (grid[i, j] == null)
                        grid[i, j] = new GridCellInfo(i, j)
                        {
                            IsWall = true
                        };
                }
            }

            // Create a recursive-backtrack corridor in cell that contains 8-way walls. Continue until entire map is
            // considered.

            // Find empty regions and fill them with recrusive-backtracked corridors
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // Create a corridor here
                    if (grid.GetAdjacentElementsUnsafe(i, j).Count(cell => cell.IsWall) == 8)
                        _mazeRegionCreator.CreateCellsStartingAt(grid, grid[i, j].Location);
                }
            }

            // Finally, connect the regions

            var finalRegions = grid.IdentifyRegions();

            while (finalRegions.Count() > 1)
            {
                var region1 = finalRegions.ElementAt(0);
                var region2 = finalRegions.ElementAt(1);

                var distanceLocations = region1.EdgeCells.SelectMany(cell1 => region2.EdgeCells.Select(cell2 => new
                {
                    Distance = Metric.EuclideanSquareDistance(cell1, cell2),
                    Cell1 = grid[cell1.Column, cell1.Row],
                    Cell2 = grid[cell2.Column, cell2.Row]
                }));

                var distanceLocation = distanceLocations.MinBy(x => x.Distance);
                _corridorCreator.CreateCorridor(grid, distanceLocation.Cell1, distanceLocation.Cell2, false, false);

                finalRegions = grid.IdentifyRegions();
            }

            // Create Lighting
            CreateLighting(grid, finalRegions, template);

            return new LevelGrid(grid, new LayerInfo("Room Layer", finalRegions), new LayerInfo[] { new LayerInfo(DEFAULT_LAYER_NAME) });
        }

        /// <summary>
        /// Creates all terrain layers for this layout - WILL OBSTRUCT PATHS. Generates new room layer with new paths.
        /// </summary>
        private IEnumerable<LayerInfo> CreateTerrain(GridCellInfo[,] grid, LayoutTemplate template, out LayerInfo roomLayer)
        {
            // Procedure
            //
            // 1) Generate all layers in order for this layout as separate 2D arrays
            // 2) Identify terrain regions and set up cell infos
            //

            var terrainDict = new Dictionary<GridCellInfo[,], TerrainLayerTemplate>();

            // Use the ZOrder parameter to order the layers
            foreach (var terrain in template.TerrainLayers.OrderBy(layer => layer.TerrainLayer.Layer))
            {
                // Generate terrain layer randomly based on the weighting
                if (_randomSequenceGenerator.Get() > terrain.GenerationWeight)
                    continue;

                // Create a new grid for each terrain layer
                var terrainLayerGrid = new GridCellInfo[grid.GetLength(0), grid.GetLength(1)];

                // Store the grid with the associated terrain layer
                terrainDict.Add(terrainLayerGrid, terrain.TerrainLayer);

                switch (terrain.GenerationType)
                {
                    case TerrainGenerationType.PerlinNoise:
                        {
                            _noiseGenerator.Run(NoiseType.PerlinNoise,
                                                grid.GetLength(0),
                                                grid.GetLength(1),
                                                terrain.Frequency,
                                                new PostProcessingCallback(
                            (column, row, value) =>
                            {
                                // Translate from [-1, 1] -> [0, 1] to check fill ratio
                                if ((System.Math.Abs(value) / 2.0) < terrain.FillRatio &&
                                    grid[column, row] != null)
                                {
                                    // Check the terrain dictionary for other entries
                                    if (!terrainDict.Any(element =>
                                    {
                                        // None of the grids have terrain at this location
                                        return element.Key[column, row] != null &&

                                               // Other terrain layers at this location don't exclude this layer at the same location
                                               (element.Value.LayoutType == TerrainLayoutType.CompletelyExclusive ||

                                               // Other terrain layers at this location DO exclude other terrain; but not at this layer
                                               (element.Value.LayoutType == TerrainLayoutType.LayerExclusive &&
                                                element.Value.Layer == terrain.TerrainLayer.Layer));

                                    }))
                                    {
                                        // Add to the region
                                        terrainLayerGrid[column, row] = grid[column, row];
                                    }
                                }
                            }));
                        }
                        break;
                    default:
                        throw new Exception("Unhandled terrain layer generation type");
                }
            }

            // Detect new room regions
            var terrainBlockedGrid = new GridCellInfo[grid.GetLength(0), grid.GetLength(1)];
            var terrainBlockedInputMap = new double[grid.GetLength(0), grid.GetLength(1)];
            var foundBlockedCell = false;

            // Create new grid with removed cells for blocked terrain
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // Found a grid cell - check for blocking terrain
                    if (grid[i, j] != null)
                    {
                        // Check for any impassable terrain that has been generated
                        if (!terrainDict.Any(element => !element.Value.IsPassable && element.Key[i, j] != null))
                        {
                            // Copy cell reference
                            terrainBlockedGrid[i, j] = grid[i, j];
                        }

                        // DON'T copy cell reference -> flag the blocked cell -> set Dijkstra weight to large number
                        else
                        {
                            // FLAG BLOCKED CELL TO PREVENT EXTRA WORK IF NOT NEEDED
                            foundBlockedCell = true;

                            // Block off the tile on the Dijkstra input map
                            terrainBlockedInputMap[i, j] = 10000;
                        }
                    }
                }
            }

            if (foundBlockedCell)
            {
                // Generate the new room regions
                var roomRegions = terrainBlockedGrid.IdentifyRegions();

                // Set up the new room layer
                roomLayer = new LayerInfo("Room Layer", roomRegions);

                // Create MST for the rooms
                var roomGraph = GeometryUtility.PrimsMinimumSpanningTree(roomRegions, Metric.MetricType.Roguian);

                // For each edge in the triangulation - create a corridor
                foreach (var edge in roomGraph.Edges)
                {
                    var location1 = edge.Point1.Reference.GetConnectionPoint(edge.Point2.Reference, Metric.MetricType.Roguian);
                    var location2 = edge.Point1.Reference.GetAdjacentConnectionPoint(edge.Point2.Reference, Metric.MetricType.Roguian);

                    // Creates Dijkstra map from the input map to find paths along the edges
                    var dijkstraMap = new DijkstraMap(terrainBlockedInputMap, location1, location2);

                    dijkstraMap.Run();

                    // Generate Path locations
                    var path = dijkstraMap.GeneratePath(true);

                    // Add path to the grid
                    foreach (var location in path)
                        grid[location.Column, location.Row] = new GridCellInfo(location) { IsWall = false };
                }
            }
            else
                roomLayer = null;

            var terrainLayers = new List<LayerInfo>();

            // Identify Terrain Regions
            foreach (var element in terrainDict)
            {
                // Sets up a set of terrain regions for the specified layer
                terrainLayers.Add(new LayerInfo(element.Value.Name, element.Key.IdentifyRegions()));
            }

            return terrainLayers;
        }

        /// <summary>
        /// Creates walls on the boundary of the regions and connectors by checking for null cells
        /// </summary>
        private void CreateWalls(GridCellInfo[,] grid)
        {
            var bounds = new RegionBoundary(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1));

            // Iterate - leaving a padding of 1 on the edge to create cells for walls
            //           without stepping out of bounds
            //
            for (int i = 1; i < bounds.CellWidth - 1; i++)
            {
                for (int j = 1; j < bounds.CellHeight - 1; j++)
                {
                    if (grid[i, j] == null)
                        continue;

                    if (grid[i, j].IsWall || grid[i, j].IsDoor)
                        continue;

                    // North wall
                    if (grid.Get(i, j - 1) == null)
                        grid[i, j - 1] = new GridCellInfo(new GridLocation(i, j - 1)) { IsWall = true };

                    // South wall
                    if (grid.Get(i, j + 1) == null)
                        grid[i, j + 1] = new GridCellInfo(new GridLocation(i, j + 1)) { IsWall = true };

                    // West wall
                    if (grid.Get(i - 1, j) == null)
                        grid[i - 1, j] = new GridCellInfo(new GridLocation(i - 1, j)) { IsWall = true };

                    // East wall
                    if (grid.Get(i + 1, j) == null)
                        grid[i + 1, j] = new GridCellInfo(new GridLocation(i + 1, j)) { IsWall = true };

                    // North-East wall
                    if (grid.Get(i + 1, j - 1) == null)
                        grid[i + 1, j - 1] = new GridCellInfo(new GridLocation(i + 1, j - 1)) { IsWall = true };

                    // South-East wall
                    if (grid.Get(i + 1, j + 1) == null)
                        grid[i + 1, j + 1] = new GridCellInfo(new GridLocation(i + 1, j + 1)) { IsWall = true };

                    // North-West wall
                    if (grid.Get(i - 1, j - 1) == null)
                        grid[i - 1, j - 1] = new GridCellInfo(new GridLocation(i - 1, j - 1)) { IsWall = true };

                    // South-West wall
                    if (grid.Get(i - 1, j + 1) == null)
                        grid[i - 1, j + 1] = new GridCellInfo(new GridLocation(i - 1, j + 1)) { IsWall = true };
                }
            }
        }

        private void CreateLighting(GridCellInfo[,] grid, IEnumerable<Region> regions, LayoutTemplate template)
        {
            // Procedure
            //
            // - Create white light threshold for the level using the scenario configuration setting
            // - Create layers 1 and 2 if they're set (using RGB averages to add light color channels)
            // - Store the results as the cell's base lighting
            //

            // Create the white light threshold
            _lightingGenerator.CreateLightThreshold(grid, template);

            switch (template.LightingAmbient1.Type)
            {
                case TerrainAmbientLightingType.None:
                    break;
                case TerrainAmbientLightingType.LightedRooms:
                    _lightingGenerator.CreateLightedRooms(grid, regions, template.LightingAmbient1);
                    break;
                case TerrainAmbientLightingType.PerlinNoiseLarge:
                case TerrainAmbientLightingType.PerlinNoiseSmall:
                    _lightingGenerator.CreatePerlinNoiseLighting(grid, template.LightingAmbient1);
                    break;
                case TerrainAmbientLightingType.WhiteNoise:
                    _lightingGenerator.CreateWhiteNoiseLighting(grid, template.LightingAmbient1);
                    break;
                case TerrainAmbientLightingType.WallLighting:
                    _lightingGenerator.CreateWallLighting(grid, template.LightingAmbient1);
                    break;
                default:
                    throw new Exception("Unhandled Terrain Ambient Lighting Type");
            }

            switch (template.LightingAmbient2.Type)
            {
                case TerrainAmbientLightingType.None:
                    break;
                case TerrainAmbientLightingType.LightedRooms:
                    _lightingGenerator.CreateLightedRooms(grid, regions, template.LightingAmbient2);
                    break;
                case TerrainAmbientLightingType.PerlinNoiseLarge:
                case TerrainAmbientLightingType.PerlinNoiseSmall:
                    _lightingGenerator.CreatePerlinNoiseLighting(grid, template.LightingAmbient2);
                    break;
                case TerrainAmbientLightingType.WhiteNoise:
                    _lightingGenerator.CreateWhiteNoiseLighting(grid, template.LightingAmbient2);
                    break;
                case TerrainAmbientLightingType.WallLighting:
                    _lightingGenerator.CreateWallLighting(grid, template.LightingAmbient2);
                    break;
                default:
                    throw new Exception("Unhandled Terrain Ambient Lighting Type");
            }
        }

        /// <summary>
        /// Validates the level - returns false if there is an issue so that a default layout can be generated. NOTE*** THROWS EXCEPTIONS
        /// DURING DEBUG INSTEAD.
        /// </summary>
        private bool Validate(GridCellInfo[,] grid, IEnumerable<Region> roomRegions, IEnumerable<Region> terrainRegions)
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

            foreach (var region in roomRegions)
            {
                var roomConnector1 = false;
                var roomConnector2 = false;

                foreach (var cell in region.Cells)
                {
                    if (grid[cell.Column, cell.Row].IsMandatory &&
                        grid[cell.Column, cell.Row].MandatoryType == LayoutMandatoryLocationType.RoomConnector1)
                        roomConnector1 = true;

                    if (grid[cell.Column, cell.Row].IsMandatory &&
                        grid[cell.Column, cell.Row].MandatoryType == LayoutMandatoryLocationType.RoomConnector2)
                        roomConnector2 = true;
                }

#if DEBUG
                if (region.Cells.Length < ROOM_SIZE_MIN)
                    throw new Exception("Room Regions must have a minimum size of " + ROOM_SIZE_MIN.ToString());

                if (!roomConnector1)
                    throw new Exception("Room doesn't have a mandatory cell for connector 1");

                if (!roomConnector2)
                    throw new Exception("Room doesn't have a mandatory cell for connector 2");
#else
                if (region.Cells.Length < ROOM_SIZE_MIN)
                    return false;

                if (!roomConnector1)
                    return false;

                if (!roomConnector2)
                    return false;
#endif
            }

            return true;
        }
        #endregion
    }
}
