using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm;
using Rogue.NET.Core.Math.Geometry;
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

        public IEnumerable<Region> BuildRectilinearCorridors(GridCellInfo[,] grid)
        {
            var regions = grid.IdentifyRegions();

            // Create MST
            var minimumSpanningTree = GeometryUtility.PrimsMinimumSpanningTree(regions, Metric.MetricType.Euclidean);

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
                if (midPoint != null)
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

            return regions;
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
            return BuildCorridors(grid);
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

        public IEnumerable<Region> BuildCorridors(GridCellInfo[,] grid)
        {
            // Create base regions
            var regions = grid.IdentifyRegions();

            // Triangulate room positions
            //
            var graph = GeometryUtility.PrimsMinimumSpanningTree(regions, Metric.MetricType.Euclidean);

            // For each edge in the triangulation - create a corridor
            //
            foreach (var edge in graph.Edges)
            {
                var location1 = edge.Point1.Reference.GetConnectionPoint(edge.Point2.Reference, Metric.MetricType.Euclidean);
                var location2 = edge.Point1.Reference.GetAdjacentConnectionPoint(edge.Point2.Reference, Metric.MetricType.Euclidean);

                var cell1 = grid.Get(location1.Column, location1.Row);
                var cell2 = grid.Get(location2.Column, location2.Row);

                // Create the corridor cells
                //
                _corridorCreator.CreateCorridor(grid, cell1, cell2, false, false);
            }

            return regions;
        }
    }
}
