using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Math.Algorithm;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Builder.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using static Rogue.NET.Core.Processing.Model.Generator.Layout.Component.Interface.IMazeRegionCreator;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Builder
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IConnectionBuilder))]
    public class ConnectionBuilder : IConnectionBuilder
    {
        readonly ICorridorCreator _corridorCreator;
        readonly IMazeRegionCreator _mazeRegionCreator;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        [ImportingConstructor]
        public ConnectionBuilder(ICorridorCreator corridorCreator, 
                               IMazeRegionCreator mazeRegionCreator, 
                               IRandomSequenceGenerator randomSequenceGenerator)
        {
            _corridorCreator = corridorCreator;
            _mazeRegionCreator = mazeRegionCreator;
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        // Credit to this fellow for the idea for maze corridors!
        //
        // https://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/
        // https://github.com/munificent/hauberk/blob/db360d9efa714efb6d937c31953ef849c7394a39/lib/src/content/dungeon.dart
        //
        public IEnumerable<Region> BuildMazeCorridors(GridCellInfo[,] grid, MazeType mazeType, double wallRemovalRatio, double horizontalVerticalBias)
        {
            // Procedure
            //
            // - Identify regions to pass to the maze generator (avoids these when removing walls)
            // - Fill in empty cells with walls
            // - Create mazes where there are 8-way walls surrounding a cell
            // - Since maze generator removes walls (up to the edge of the avoid regions)
            //   Must add back just those walls adjacent to the avoid regions
            //
            // - Then, BuildCorridors(...) will connect any mazes together that were interrupted
            //   (Also, by the adding back of walls)
            //

            var regions = grid.IdentifyRegions();

            // Leave room for a wall border around the outside
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // Skip region cells
                    if (grid[i, j] != null)
                        continue;

                    // Add walls in the negative space - leaving room for region cells
                    grid[i, j] = new GridCellInfo(i, j) { IsWall = true };
                }
            }

            // Create a recursive-backtrack corridor in cell that contains 8-way walls. Continue until entire map is
            // considered.

            // Find empty regions and fill them with recrusive-backtracked corridors
            //
            // NOTE*** Avoiding edges that WERE created as walls - because the "Filled" rule won't touch those.
            //
            for (int i = 1; i < grid.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < grid.GetLength(1) - 1; j++)
                {
                    // Create a corridor where all adjacent cells are walls
                    if (grid.GetAdjacentElements(i, j).All(cell => cell.IsWall))
                    {
                        // Create the maze!
                        _mazeRegionCreator.CreateCellsStartingAt(grid, regions, grid[i, j].Location, mazeType, wallRemovalRatio, horizontalVerticalBias);
                    }
                }
            }

            // Add back walls surrounding the regions
            foreach (var region in regions)
            {
                foreach (var edgeCell in region.EdgeCells)
                {
                    var adjacentCells = grid.GetAdjacentElements(edgeCell.Column, edgeCell.Row);

                    foreach (var cell in adjacentCells)
                    {
                        // Adjacent cell is not in region; but is on the edge. This should
                        // be a wall cell
                        if (region[cell.Location.Column, cell.Location.Row] == null)
                            cell.IsWall = true;
                    }
                }
            }

            // Finally, connect the regions using MST -> Linear connectors
            return BuildCorridors(grid, "Maze Corridors");
        }

        public IEnumerable<Region> BuildConnectionPoints(GridCellInfo[,] grid)
        {
            // Create base regions
            var regions = grid.IdentifyRegions().ToList();

            // Check for no regions
            if (regions.Count == 0)
                throw new Exception("No regions found ConnectionBuilder.BuildConnectionPoints");

            // Check for a single region
            if (regions.Count == 1)
                return regions;

            // Create mandatory connection points between rooms
            for (int i = 0; i < regions.Count - 1; i++)
            {
                // Get random cells from the two regions
                var region1Index = _randomSequenceGenerator.Get(0, regions[i].Cells.Length);
                var region2Index = _randomSequenceGenerator.Get(0, regions[i + 1].Cells.Length);

                // Get cells from the array ~ O(1)
                var location1 = regions[i].Cells[region1Index];
                var location2 = regions[i + 1].Cells[region2Index];

                // Set cells to mandatory
                grid[location1.Column, location1.Row].IsMandatory = true;
                grid[location1.Column, location1.Row].MandatoryType = LayoutMandatoryLocationType.RoomConnector1;

                grid[location2.Column, location2.Row].IsMandatory = true;
                grid[location2.Column, location2.Row].MandatoryType = LayoutMandatoryLocationType.RoomConnector2;
            }

            // Connect the first and last regions
            var firstIndex = _randomSequenceGenerator.Get(0, regions.First().Cells.Length);
            var lastIndex = _randomSequenceGenerator.Get(0, regions.Last().Cells.Length);

            // Get cells from the array ~ O(1)
            var firstLocation = regions.First().Cells[firstIndex];
            var lastLocation = regions.Last().Cells[lastIndex];

            // Set cells to mandatory
            grid[firstLocation.Column, firstLocation.Row].IsMandatory = true;
            grid[firstLocation.Column, firstLocation.Row].MandatoryType = LayoutMandatoryLocationType.RoomConnector1;

            grid[lastLocation.Column, lastLocation.Row].IsMandatory = true;
            grid[lastLocation.Column, lastLocation.Row].MandatoryType = LayoutMandatoryLocationType.RoomConnector2;

            return regions;
        }

        public IEnumerable<Region> BuildCorridors(GridCellInfo[,] grid, string layoutName)
        {
            // Procedure
            //
            // 1) Identify Regions
            // 2) Create cost map of terrain using edges of regions as gaussian repellers
            //    (Also, mask off region cells)
            // 3) Create region triangulation
            // 4) Use Dijkstra's algorithm to connect the regions with the cost map
            //

            // Create base regions
            var regions = grid.IdentifyRegions();

            // Create cost map from the regions
            var costMap = CreateRegionCostMap(grid, regions);

            // Triangulate room positions
            var graph = GeometryUtility.PrimsMinimumSpanningTree(regions, Metric.MetricType.Euclidean);

            // For each edge in the triangulation - create a corridor
            foreach (var edge in graph.Edges)
            {
                var location1 = edge.Point1.Reference.GetConnectionPoint(edge.Point2.Reference, Metric.MetricType.Euclidean);
                var location2 = edge.Point1.Reference.GetAdjacentConnectionPoint(edge.Point2.Reference, Metric.MetricType.Euclidean);

                var dijkstraMap = new DijkstraMap(costMap, location1, location2, true);

                if (!dijkstraMap.Run())
                {
                    dijkstraMap.OutputCSV(ResourceConstants.DijkstraOutputDirectory,
                                          layoutName +
                                          " DijkstraMap_Edge_" +
                                          edge.Point1.Vertex.Column + "_" +
                                          edge.Point1.Vertex.Row + " to " +
                                          edge.Point2.Vertex.Column + "_" +
                                          edge.Point2.Vertex.Row);

                    throw new Exception("Invalid Dijkstra Map (see output directory)");
                }

                foreach (var location in dijkstraMap.GeneratePath())
                {
                    if (location.Equals(location1) ||
                        location.Equals(location2))
                        continue;

                    // Maze levels can have walls set up to punch through
                    else if (grid[location.Column, location.Row] != null)
                        grid[location.Column, location.Row].IsWall = false;

                    // Empty cells need to be filled in
                    else
                        grid[location.Column, location.Row] = new GridCellInfo(location);
                }
            }

            return regions;
        }

        /// <summary>
        /// Creates cost map for the grid using pre-identified regions
        /// </summary>
        private double[,] CreateRegionCostMap(GridCellInfo[,] grid, IEnumerable<Region> regions)
        {
            var costMap = new double[grid.GetLength(0), grid.GetLength(1)];

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    // If grid cell is empty (null) then it does not belong to any region. Wall cells may
                    // be left from generating maze regions or corridors
                    if (grid[i, j] == null || grid[i, j].IsWall)
                        costMap[i, j] = 0;
                    
                    else
                        costMap[i, j] = DijkstraMap.RegionFeatureConstant;
                }
            }

            return costMap;
        }
    }
}
