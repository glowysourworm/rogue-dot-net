﻿using Rogue.NET.Common;
using Rogue.NET.Model.Logic;
using Rogue.NET.Model.Scenario;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Model.Physics
{
    public interface IRayTracer
    {
        IEnumerable<Cell> GetLogicallyVisibleCells(LevelGrid grid, CellPoint cell, int lightRadius);
    }
    public class SimpleRayTracer : IRayTracer
    {
        #region Visible Cells Calculation
        public IEnumerable<Cell> GetLogicallyVisibleCells(LevelGrid grid, CellPoint location, int maxCellRadius)
        {
            var result = new Dictionary<int, Cell>();
            var locationCell = grid.GetCell(location);
            var gridBounds = grid.GetBounds();

            result.Add(locationCell.GetHashCode(), locationCell);

            var origin = TransformToPhysicalLayout(location);
            origin.X += ScenarioConfiguration.CELLWIDTH / 2.0F;
            origin.Y += ScenarioConfiguration.CELLHEIGHT / 2.0F;

            int angle = 0;

            while (angle < 360)
            {
                var hitWall = false;
                var nextPoint = location;
                var angleRadians = (Math.PI / 180.0D) * angle;

                // 0) Calculate y = mx + b for the "Rogue" coordinate system - which is skewed by the Cell width and height.
                //    So, use a (Skew) * (Rotation) * (Unit Vector) approach to calculating the transform.
                var xScale = 1;
                var yScale = ScenarioConfiguration.CELLHEIGHT / (double)ScenarioConfiguration.CELLWIDTH;
                //var unitEllipseX = xScale * (Math.Pow(Math.Cos(angleRadians), 2) - Math.Pow(Math.Sin(angleRadians), 2));
                //var unitEllipseY = yScale * 2 * Math.Cos(angleRadians) * Math.Sin(angleRadians);

                var unitX = Math.Cos(angleRadians);
                var unitY = Math.Sin(angleRadians);

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
                       Math.Pow(xIndex, 2) + Math.Pow(yIndex, 2) < Math.Pow(maxCellRadius, 2) &&
                       !hitWall)
                {
                    var nextVerticalX = verticalGridLines[xIndex] * ScenarioConfiguration.CELLWIDTH;
                    var nextVerticalY = (slope * nextVerticalX) + intercept;

                    var nextHorizontalY = horizontalGridLines[yIndex] * ScenarioConfiguration.CELLHEIGHT;
                    var nextHorizontalX = (nextHorizontalY - intercept) / slope;

                    var radiusHorizontal2 = Math.Pow(nextHorizontalX - origin.X, 2) + Math.Pow(nextHorizontalY - origin.Y, 2);
                    var radiusVertical2 = Math.Pow(nextVerticalX - origin.X, 2) + Math.Pow(nextVerticalY - origin.Y, 2);

                    if (radiusHorizontal2 < radiusVertical2)
                    {
                        // Pick the 2 involved cells - cell 2 is the one "advanced to" or the destination
                        var cellY1 = (angle < 180) ? horizontalGridLines[yIndex] - 1 : horizontalGridLines[yIndex];
                        var cellY2 = (angle < 180) ? horizontalGridLines[yIndex] : horizontalGridLines[yIndex] - 1;                        
                        var cellX = (int)Math.Floor(nextHorizontalX / ScenarioConfiguration.CELLWIDTH); // truncate to get location

                        var cell1 = grid.GetCell(cellX, cellY1);
                        var cell2 = grid.GetCell(cellX, cellY2);

                        if (cell1 == null)
                            hitWall = true;

                        else if (cell2 == null)
                        {
                            if (!result.ContainsKey(cell1.GetHashCode()))
                                result.Add(cell1.GetHashCode(), cell1);

                            hitWall = true;
                        }
                        else
                        {
                            if (!result.ContainsKey(cell1.GetHashCode()))
                                result.Add(cell1.GetHashCode(), cell1);

                            if (angle < 180) // cell2 is further South
                            {
                                hitWall = (cell2.Walls & Compass.N) != 0 ||
                                          (cell2.Doors & Compass.N) != 0 ||
                                          (cell1.Walls & Compass.S) != 0 ||
                                          (cell1.Doors & Compass.S) != 0;

                                if (!hitWall && !result.ContainsKey(cell2.GetHashCode()))
                                    result.Add(cell2.GetHashCode(), cell2);
                            }
                            else
                            {
                                hitWall = (cell2.Walls & Compass.S) != 0 ||
                                          (cell2.Doors & Compass.S) != 0 ||
                                          (cell1.Walls & Compass.N) != 0 ||
                                          (cell1.Doors & Compass.N) != 0;

                                if (!hitWall && !result.ContainsKey(cell2.GetHashCode()))
                                    result.Add(cell2.GetHashCode(), cell2);
                            }
                        }

                        yIndex++;
                    }
                    else
                    {
                        // Pick the 2 involved cells
                        var cellX1 = (angle < 90 || angle > 270) ? verticalGridLines[xIndex] - 1 : verticalGridLines[xIndex];
                        var cellX2 = (angle < 90 || angle > 270) ? verticalGridLines[xIndex] : verticalGridLines[xIndex] - 1;
                        var cellY = (int)Math.Floor(nextVerticalY / ScenarioConfiguration.CELLHEIGHT); // truncate to get location

                        var cell1 = grid.GetCell(cellX1, cellY);
                        var cell2 = grid.GetCell(cellX2, cellY);

                        if (cell1 == null)
                            hitWall = true;

                        else if (cell2 == null)
                        {
                            if (!result.ContainsKey(cell1.GetHashCode()))
                                result.Add(cell1.GetHashCode(), cell1);

                            hitWall = true;
                        }
                        else
                        {
                            if (!result.ContainsKey(cell1.GetHashCode()))
                                result.Add(cell1.GetHashCode(), cell1);

                            if (angle < 90 || angle > 270) // cell2 is further East
                            {
                                hitWall= (cell2.Walls & Compass.W) != 0 ||
                                          (cell2.Doors & Compass.W) != 0 ||
                                          (cell1.Walls & Compass.E) != 0 ||
                                          (cell1.Doors & Compass.E) != 0;

                                if (!hitWall && !result.ContainsKey(cell2.GetHashCode()))
                                    result.Add(cell2.GetHashCode(), cell2);
                            }
                            else
                            {
                                hitWall = (cell2.Walls & Compass.E) != 0 ||
                                          (cell2.Doors & Compass.E) != 0 ||
                                          (cell1.Walls & Compass.W) != 0 ||
                                          (cell1.Doors & Compass.W) != 0;

                                if (!hitWall && !result.ContainsKey(cell2.GetHashCode()))
                                    result.Add(cell2.GetHashCode(), cell2);
                            }
                        }

                        xIndex++;
                    }
                }

                angle += 5;
            }

            return result.Values;
        }
        private PointF TransformToPhysicalLayout(CellPoint p)
        {
            float x = (float)(ScenarioConfiguration.CELLWIDTH * p.Column);
            float y = (float)(ScenarioConfiguration.CELLHEIGHT * p.Row);
            return new PointF(x, y);
        }
        #endregion
    }
}