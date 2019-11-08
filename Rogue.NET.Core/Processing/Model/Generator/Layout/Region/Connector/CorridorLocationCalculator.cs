using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Static;
using System.Collections.Generic;
using RegionModel = Rogue.NET.Core.Model.Scenario.Content.Layout.Region;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Connector
{
    public static class CorridorLocationCalculator
    {
        static readonly IRandomSequenceGenerator _randomSequenceGenerator;

        static CorridorLocationCalculator()
        {
            _randomSequenceGenerator = ServiceLocator.Current.GetInstance<IRandomSequenceGenerator>();
        }

        public static void CalculateNearestNeighborLocations(GridCell[,] grid, RegionModel region1, RegionModel region2, out GridCell region1Location, out GridCell region2Location)
        {
            // Procedure
            //
            // Doing an O(n^2) search on the edge cells results in a total time that is computationally significant
            // (maybe as much as a couple seconds). A divide and conquer algorithm may be as good as O(n log n); but
            // the following should suffice.
            //
            // 1) Locate the relative position of room2's bounding rectangle (left, top, right, bottom)
            //
            // 2) For "corner located" (top-left, top-right, bottom-left, bottom-right) begin a search at
            //    the closest neighboring corners and iterate outward along the bounding box edge to find 
            //    the first candidate(s) for each room. Choose these at random for both rooms. (For rectangles
            //    this will be the corners 100% of the time).
            //
            // 3) For cardinally-located (top, left, right, bottom) cases - choose a point from the overlapping
            //    section of the rectangles (at random). Then, starting at room1, search directly upwards (or horizontally) 
            //    until you reach room2. This does not guarantee that the cell will be on the edge each time; but
            //    it will connect the rooms. Also, after finding the overlapping cell, start at the edge of room2
            //    to begin the search.
            //
            // 4) For overlapping or contained bounding boxes - just use a random cell from both rooms' edges. This
            //    could be improved using the O(n^2) brute force algorithm; but it may not be optimal for certain
            //    scenario generations - leaving a poor experience for those cases. (So, a TODO)

            // Have to force O(n^2) search for amorphous room shapes
            var bruteForceSearch = !region1.IsRectangular || !region2.IsRectangular;

            var room1Bounds = region1.Bounds;
            var room2Bounds = region2.Bounds;

            var top = room2Bounds.Bottom < room1Bounds.Top;
            var bottom = room2Bounds.Top > room1Bounds.Bottom;
            var left = room2Bounds.Right < room1Bounds.Left;
            var right = room2Bounds.Left > room1Bounds.Right;

            var room1Candidates = new List<GridCell>();
            var room2Candidates = new List<GridCell>();

            // Case 1: Corner-Located
            if (((top && left) ||
                 (top && right) ||
                 (bottom && left) ||
                 (bottom && right)) &&
                 !bruteForceSearch)
            {

                var sign1X = left ? 1 : -1;     // Search for room1-X is in the positive direction (for left-positioned room2)
                var sign1Y = top ? 1 : -1;      // Search for room1-Y is in the positive direction (for top-positioned room2)
                var sign2X = left ? -1 : 1;
                var sign2Y = top ? -1 : 1;
                var startLocation1 = top ?
                                     (left ? room1Bounds.TopLeft : room1Bounds.TopRight) :
                                     (left ? room1Bounds.BottomLeft : room1Bounds.BottomRight);
                var startLocation2 = top ?
                                     (left ? room2Bounds.BottomRight : room2Bounds.BottomLeft) :
                                     (left ? room2Bounds.TopRight : room2Bounds.TopLeft);

                var distance = 0;

                // This loop MUST exit because the bounding rectangle MUST contain at least one
                // cell from the room on each edge.
                while (room1Candidates.Count == 0 ||
                       room2Candidates.Count == 0)
                {
                    if (room1Candidates.Count == 0)
                    {
                        // Calculte the next pair of search cells using the provided parameters from above
                        var cell1X = grid[startLocation1.Column + (sign1X * distance),
                                          startLocation1.Row];

                        var cell1Y = grid[startLocation1.Column,
                                          startLocation1.Row + (sign1Y * distance)];

                        if (cell1X != null)
                            room1Candidates.Add(cell1X);

                        if (cell1Y != null && cell1Y != cell1X)
                            room1Candidates.Add(cell1Y);
                    }

                    if (room2Candidates.Count == 0)
                    {
                        // Calculte the next pair of search cells using the provided parameters from above
                        var cell2X = grid[startLocation2.Column + (sign2X * distance),
                                           startLocation2.Row];

                        var cell2Y = grid[startLocation2.Column,
                                           startLocation2.Row + (sign2Y * distance)];

                        if (cell2X != null)
                            room2Candidates.Add(cell2X);

                        if (cell2Y != null && cell2Y != cell2X)
                            room2Candidates.Add(cell2Y);
                    }

                    distance++;
                }
            }

            // Case 2: Edge-Located
            else if ((top || bottom || left || right) && !bruteForceSearch)
            {
                // Vertical
                if (top || bottom)
                {
                    // First, locate the overlap of the two rectangles
                    var overlapLeft = System.Math.Max(room1Bounds.Left, room2Bounds.Left);
                    var overlapRight = System.Math.Min(room1Bounds.Right, room2Bounds.Right);

                    var sign1 = top ? 1 : -1;
                    var sign2 = top ? -1 : 1;

                    var column = _randomSequenceGenerator.Get(overlapLeft, overlapRight + 1);
                    var cell1RowStart = top ? room1Bounds.Top : room1Bounds.Bottom;
                    var cell2RowStart = top ? room2Bounds.Bottom : room2Bounds.Top;
                    var distance = 0;

                    // Search from the edges until you reach the first cell
                    while (room1Candidates.Count == 0 ||
                           room2Candidates.Count == 0)
                    {
                        var cell1 = grid[column, cell1RowStart + (sign1 * distance)];
                        var cell2 = grid[column, cell2RowStart + (sign2 * distance)];

                        if (cell1 != null && room1Candidates.Count == 0)
                            room1Candidates.Add(cell1);

                        if (cell2 != null && room2Candidates.Count == 0)
                            room2Candidates.Add(cell2);

                        distance++;
                    }
                }

                // Horizontal
                else
                {
                    // First, locate the overlap of the two rectangles
                    var overlapTop = System.Math.Max(room1Bounds.Top, room2Bounds.Top);
                    var overlapBottom = System.Math.Min(room1Bounds.Bottom, room2Bounds.Bottom);

                    var sign1 = left ? 1 : -1;
                    var sign2 = left ? -1 : 1;

                    var row = _randomSequenceGenerator.Get(overlapTop, overlapBottom + 1);
                    var cell1ColumnStart = left ? room1Bounds.Left : room1Bounds.Right;
                    var cell2ColumnStart = left ? room2Bounds.Right : room2Bounds.Left;
                    var distance = 0;

                    // Search from the edges until you reach the first cell
                    while (room1Candidates.Count == 0 ||
                           room2Candidates.Count == 0)
                    {
                        var cell1 = grid[cell1ColumnStart + (sign1 * distance), row];
                        var cell2 = grid[cell2ColumnStart + (sign2 * distance), row];

                        if (cell1 != null && room1Candidates.Count == 0)
                            room1Candidates.Add(cell1);

                        if (cell2 != null && room2Candidates.Count == 0)
                            room2Candidates.Add(cell2);

                        distance++;
                    }
                }
            }

            // Case 3: Overlapping / Contained / Brute force
            else
            {
                var minimumDistance = int.MaxValue;

                foreach (var edgeLocation1 in region1.EdgeCells)
                {
                    foreach (var edgeLocation2 in region2.EdgeCells)
                    {
                        var distance = Metric.RoguianDistance(edgeLocation1, edgeLocation2);

                        // Reset candidates
                        if (distance < minimumDistance)
                        {
                            minimumDistance = distance;

                            room1Candidates.Clear();
                            room2Candidates.Clear();

                            room1Candidates.Add(grid[edgeLocation1.Column, edgeLocation1.Row]);
                            room2Candidates.Add(grid[edgeLocation2.Column, edgeLocation2.Row]);
                        }

                        // Add to candidates
                        else if (distance == minimumDistance)
                        {
                            room1Candidates.Add(grid[edgeLocation1.Column, edgeLocation1.Row]);
                            room2Candidates.Add(grid[edgeLocation2.Column, edgeLocation2.Row]);
                        }
                    }
                }
            }

            // Assign a random cell from the candidates
            region1Location = _randomSequenceGenerator.GetRandomElement(room1Candidates);
            region2Location = _randomSequenceGenerator.GetRandomElement(room2Candidates);
        }
    }
}
