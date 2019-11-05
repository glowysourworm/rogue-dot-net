using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Region;
using Rogue.NET.Core.Processing.Model.Static;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Algorithm
{
    [Export(typeof(IRayTracer))]
    public class SimpleRayTracer : IRayTracer
    {
        [ImportingConstructor]
        public SimpleRayTracer() { }

        public IEnumerable<DistanceLocation>
                CalculateVisibility(LevelGrid grid,
                                    GridLocation location,
                                    double lightRadius,
                                    out IEnumerable<DistanceLocation> lineOfSightLocations)
        {
            var result = new Dictionary<int, DistanceLocation>();
            var locationCell = grid[location.Column, location.Row];
            var gridBounds = grid.Bounds;

            result.Add(locationCell.GetHashCode(), new DistanceLocation(locationCell.Location, locationCell.Location));

            var origin = GridUtility.TransformToPhysicalLayout(location);
            origin.X += ModelConstants.CellWidth / 2.0F;
            origin.Y += ModelConstants.CellHeight / 2.0F;

            int angle = 0;

            while (angle < 360)
            {
                var hitWall = false;
                var nextPoint = location;
                var angleRadians = (System.Math.PI / 180.0D) * angle;

                // 0) Calculate y = mx + b for a line passing through the cell origin (center) with the new angle

                var unitX = System.Math.Cos(angleRadians);
                var unitY = System.Math.Sin(angleRadians);

                var slope = unitY / unitX;
                var intercept = origin.Y - (slope * origin.X);

                // 1) Pick gridlines according to the angle (starting after the location which is the top left corner of the origin's cell)
                var verticalGridLines = (angle < 90 || angle >= 270) ?
                                            Enumerable.Range(location.Column + 1, gridBounds.Right - location.Column).ToList() :
                                            Enumerable.Range(gridBounds.Left, location.Column + 1).Reverse().ToList();

                var horizontalGridLines = (angle <= 180) ?
                                            Enumerable.Range(location.Row + 1, gridBounds.Bottom - location.Row).ToList() :
                                            Enumerable.Range(gridBounds.Top, location.Row + 1).Reverse().ToList();

                // 2) Calculate points of intersections in order along the ray (choose the smaller delta-magnitude)
                var xIndex = 0;
                var yIndex = 0;

                while (xIndex < verticalGridLines.Count() &&
                       yIndex < horizontalGridLines.Count() &&
                       !hitWall)
                {
                    var nextVerticalX = verticalGridLines[xIndex] * ModelConstants.CellWidth;
                    var nextVerticalY = (slope * nextVerticalX) + intercept;

                    var nextHorizontalY = horizontalGridLines[yIndex] * ModelConstants.CellHeight;
                    var nextHorizontalX = (nextHorizontalY - intercept) / slope;

                    var radiusHorizontal2 = System.Math.Pow(nextHorizontalX - origin.X, 2) + System.Math.Pow(nextHorizontalY - origin.Y, 2);
                    var radiusVertical2 = System.Math.Pow(nextVerticalX - origin.X, 2) + System.Math.Pow(nextVerticalY - origin.Y, 2);

                    if (radiusHorizontal2 < radiusVertical2)
                    {
                        // Pick the 2 involved cells - cell 2 is the one "advanced to" or the destination
                        var cellY1 = (angle < 180) ? horizontalGridLines[yIndex] - 1 : horizontalGridLines[yIndex];
                        var cellY2 = (angle < 180) ? horizontalGridLines[yIndex] : horizontalGridLines[yIndex] - 1;
                        var cellX = (int)System.Math.Floor(nextHorizontalX / ModelConstants.CellWidth); // truncate to get location

                        var cell1 = grid[cellX, cellY1];
                        var cell2 = grid[cellX, cellY2];

                        // First it hits an empty cell
                        if (cell1 == null)
                            hitWall = true;

                        else if (cell2 == null)
                        {
                            if (!result.ContainsKey(cell1.GetHashCode()))
                                result.Add(cell1.GetHashCode(), new DistanceLocation(location, cell1.Location));

                            hitWall = true;
                        }
                        else
                        {
                            if (!result.ContainsKey(cell1.GetHashCode()))
                                result.Add(cell1.GetHashCode(), new DistanceLocation(location, cell1.Location));

                            if (!result.ContainsKey(cell2.GetHashCode()))
                                result.Add(cell2.GetHashCode(), new DistanceLocation(location, cell2.Location));
                        }

                        yIndex++;
                    }
                    else
                    {
                        // Pick the 2 involved cells
                        var cellX1 = (angle < 90 || angle > 270) ? verticalGridLines[xIndex] - 1 : verticalGridLines[xIndex];
                        var cellX2 = (angle < 90 || angle > 270) ? verticalGridLines[xIndex] : verticalGridLines[xIndex] - 1;
                        var cellY = (int)System.Math.Floor(nextVerticalY / ModelConstants.CellHeight); // truncate to get location

                        var cell1 = grid[cellX1, cellY];
                        var cell2 = grid[cellX2, cellY];

                        if (cell1 == null)
                            hitWall = true;

                        else if (cell2 == null)
                        {
                            if (!result.ContainsKey(cell1.GetHashCode()))
                                result.Add(cell1.GetHashCode(), new DistanceLocation(location, cell1.Location));

                            hitWall = true;
                        }
                        else
                        {
                            if (!result.ContainsKey(cell1.GetHashCode()))
                                result.Add(cell1.GetHashCode(), new DistanceLocation(location, cell1.Location));

                            if (!result.ContainsKey(cell2.GetHashCode()))
                                result.Add(cell2.GetHashCode(), new DistanceLocation(location, cell2.Location));
                        }

                        xIndex++;
                    }
                }

                angle += 5;
            }

            // Calculate Line-Of-Sight / Visibility Radius
            lineOfSightLocations = result.Values.ToList();

            return result.Values.Where(x =>
            {
                return Metric.EuclideanDistance(x.Location, location) <= lightRadius;
            })
            .Select(x => new DistanceLocation(location, x.Location))
            .ToList(); ;
        }
    }
}
