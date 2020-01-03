using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Construction;
using Rogue.NET.Core.Processing.Model.Extension;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Rogue.NET.Core.Processing.Model.Algorithm
{
    /// <summary>
    /// Calculates visibility using "shadow casting" algorithm
    /// </summary>
    public static class VisibilityCalculator
    {
        /// <summary>
        /// Callback from the visibility calculation to allow setting of data in the primary grid (prevents allocation)
        /// </summary>
        public delegate void VisibilityCalculatorCallback(int column, int row, bool isVisible);

        const int MAX_RADIUS = 10;

        protected enum Octant
        {
            NNW,
            NNE,
            ENE,
            ESE,
            SSE,
            SSW,
            WSW,
            WNW
        }

        protected struct OctantSlope
        {
            public double DeltaX { get; private set; }
            public double DeltaY { get; private set; }

            public double Magnitude
            {
                get
                {
                    return this.DeltaX == 0 ? (this.DeltaY > 0 ? double.PositiveInfinity :
                                               this.DeltaY < 0 ? double.NegativeInfinity :
                                               double.NaN) : this.DeltaY / this.DeltaX;
                }
            }

            public OctantSlope(Point point, Point center)
            {
                this.DeltaX = point.X - center.X;
                this.DeltaY = point.Y - center.Y;
            }

            public static OctantSlope CreateLeadingSlope(GridLocation location, GridLocation center)
            {
                double deltaX, deltaY;

                CalculateLeadingSlope(location, center, out deltaX, out deltaY);

                return new OctantSlope()
                {
                    DeltaX = deltaX,
                    DeltaY = deltaY
                };
            }

            public static OctantSlope CreateTrailingSlope(GridLocation location, GridLocation center)
            {
                double deltaX, deltaY;

                CalculateTrailingSlope(location, center, out deltaX, out deltaY);

                return new OctantSlope()
                {
                    DeltaX = deltaX,
                    DeltaY = deltaY
                };
            }

            public static OctantSlope CreateCenterSlope(GridLocation location, GridLocation center)
            {
                double deltaX, deltaY;

                CalculateCenterSlope(location, center, out deltaX, out deltaY);

                return new OctantSlope()
                {
                    DeltaX = deltaX,
                    DeltaY = deltaY
                };
            }
        }

        protected struct ShadowCastingFeature
        {
            public GridLocation BeginningLocation { get; private set; }
            public GridLocation EndingLocation { get; private set; }
            public OctantSlope LeadingSlope { get; private set; }
            public OctantSlope TrailingSlope { get; private set; }
            public Octant Octant { get; private set; }

            public ShadowCastingFeature(GridLocation beginningLocation, GridLocation endingLocation, GridLocation center, Octant octant)
            {
                this.BeginningLocation = beginningLocation;
                this.EndingLocation = endingLocation;
                this.Octant = octant;
                this.LeadingSlope = OctantSlope.CreateLeadingSlope(beginningLocation, center);
                this.TrailingSlope = OctantSlope.CreateTrailingSlope(endingLocation, center);
            }
        }

        public static void CalculateVisibility(LayoutGrid grid, GridLocation location, VisibilityCalculatorCallback callback)
        {
            CalculateVisibilityImpl((column, row) =>
            {
                return grid[column, row]?.Location;
            },
            (gridLocation) =>
            {
                return grid[gridLocation.Column, gridLocation.Row] == null ||
                       grid[gridLocation.Column, gridLocation.Row].IsWall ||
                       grid[gridLocation.Column, gridLocation.Row].IsDoor;
            }, location, callback);
        }

        public static void CalculateVisibility(GridCellInfo[,] grid, GridLocation location, VisibilityCalculatorCallback callback)
        {
            CalculateVisibilityImpl((column, row) =>
            {
                return grid.Get(column, row)?.Location;
            },
            (gridLocation) =>
            {
                return grid[gridLocation.Column, gridLocation.Row] == null ||
                       grid[gridLocation.Column, gridLocation.Row].IsWall ||
                       grid[gridLocation.Column, gridLocation.Row].IsDoor;
            }, location, callback);
        }

        private static void CalculateVisibilityImpl(Func<int, int, GridLocation> getter, Func<GridLocation, bool> getterIsLightBlocking, GridLocation center, VisibilityCalculatorCallback callback)
        {
            // Implementing Shadow Casting - by Björn Bergström [bjorn.bergstrom@roguelikedevelopment.org] 
            //
            // http://www.roguebasin.com/index.php?title=FOV_using_recursive_shadowcasting

            // Scan all octants
            foreach (Octant octant in Enum.GetValues(typeof(Octant)))
            {
                var lightBlockingFeatures = new List<ShadowCastingFeature>();
                var iterate = true;
                var radius = 1;

                while (iterate)
                {
                    int location1Column, location1Row, location2Column, location2Row;
                    bool yDirection = false;

                    CalculateIterationLocations(center, radius, octant, out location1Column, out location1Row, out location2Column, out location2Row, out yDirection);

                    var negative = yDirection ? location2Row - location1Row < 0 : location2Column - location1Column < 0;
                    var startIndex = yDirection ? location1Row : location1Column;
                    var endIndex = yDirection ? location2Row : location2Column;
                    var index = startIndex;
                    var finalIteration = true;
                    GridLocation firstLightBlockingLocation = null;
                    GridLocation lastLightBlockingLocation = null;

                    while (index != (negative ? endIndex - 1 : endIndex + 1))
                    {
                        var column = yDirection ? location1Column : location1Column + (index - startIndex);
                        var row = yDirection ? location1Row + (index - startIndex) : location1Row;

                        // CHECK MAX RADIUS FOR THIS OCTANT
                        if (Metric.EuclideanDistance(column, row, center.Column, center.Row) > MAX_RADIUS)
                            break;

                        // Current location
                        var location = getter(column, row);

                        // First, check whether location is null
                        var isNull = location == null;

                        // NOTE*** ALLOCATING THIS MEMORY IS NOT LIKED BECAUSE OF GC PERFORMANCE. BEST TO 
                        //         INSTANTIATE ALL GRID LOCATIONS IN THE PRIMARY GRID INSTEAD OF HAVING NULLS.
                        //
                        // Next, set location to a new grid location if it is null
                        if (location == null)
                            location = new GridLocation(column, row);

                        // If Non-Null, then check other conditions and store result
                        if (!isNull)
                        {
                            // Calculate whether location is dark
                            var isDark = IsLocationBlocked(lightBlockingFeatures, location, center, octant);

                            // Calculate whether location is light blocking
                            var isLightBlocking = getterIsLightBlocking(location);

                            // Detect light blocking features
                            if (isLightBlocking && firstLightBlockingLocation == null)
                            {
                                firstLightBlockingLocation = location;
                                lastLightBlockingLocation = location;
                            }

                            else if (isLightBlocking && firstLightBlockingLocation != null)
                                lastLightBlockingLocation = location;

                            else if (!isLightBlocking && lastLightBlockingLocation != null)
                            {
                                lightBlockingFeatures.Add(new ShadowCastingFeature(firstLightBlockingLocation, lastLightBlockingLocation, center, octant));

                                // Reset feature detectors
                                firstLightBlockingLocation = null;
                                lastLightBlockingLocation = null;
                            }

                            // Check for final iteration
                            finalIteration &= (isDark || isLightBlocking);

                            // Callback to notify visibility calculated for this location
                            callback(location.Column, location.Row, !isDark);
                        }
                        // If Null, then it is also light blocking
                        else
                        {
                            // AND in final iteration - true if all locations are null, light blocking, or dark
                            finalIteration &= true;

                            // Detect light blocking features
                            if (firstLightBlockingLocation == null)
                            {
                                firstLightBlockingLocation = location;
                                lastLightBlockingLocation = location;
                            }
                            else
                                lastLightBlockingLocation = location;
                        }

                        index += (negative ? -1 : 1);

                        // Finish up light blocking feature for this iteration
                        if (firstLightBlockingLocation != null && (index == (negative ? endIndex - 1 : endIndex + 1)))
                            lightBlockingFeatures.Add(new ShadowCastingFeature(firstLightBlockingLocation, lastLightBlockingLocation, center, octant));
                    }

                    radius++;

                    // Re-evaluate loop condition
                    iterate = !finalIteration;
                }
            }
        }

        private static void CalculateIterationLocations(GridLocation center, int radius, Octant octant, out int location1Column, out int location1Row, out int location2Column, out int location2Row, out bool yDirection)
        {
            // NOTE*** MUST ITERATE CLOCKWISE AROUND THE CIRCLE
            switch (octant)
            {
                case Octant.NNW:
                    {
                        location1Column = center.Column - radius;
                        location1Row = center.Row - radius;
                        location2Column = center.Column;
                        location2Row = center.Row - radius;
                        yDirection = false;
                    }
                    break;
                case Octant.NNE:
                    {
                        location1Column = center.Column;
                        location1Row = center.Row - radius;
                        location2Column = center.Column + radius;
                        location2Row = center.Row - radius;
                        yDirection = false;
                    }
                    break;
                case Octant.ENE:
                    {
                        location1Column = center.Column + radius;
                        location1Row = center.Row - radius;
                        location2Column = center.Column + radius;
                        location2Row = center.Row;
                        yDirection = true;
                    }
                    break;
                case Octant.ESE:
                    {
                        location1Column = center.Column + radius;
                        location1Row = center.Row;
                        location2Column = center.Column + radius;
                        location2Row = center.Row + radius;
                        yDirection = true;
                    }
                    break;
                case Octant.SSE:
                    {
                        location1Column = center.Column + radius;
                        location1Row = center.Row + radius;
                        location2Column = center.Column;
                        location2Row = center.Row + radius;
                        yDirection = false;
                    }
                    break;
                case Octant.SSW:
                    {
                        location1Column = center.Column;
                        location1Row = center.Row + radius;
                        location2Column = center.Column - radius;
                        location2Row = center.Row + radius;
                        yDirection = false;
                    }
                    break;
                case Octant.WSW:
                    {
                        location1Column = center.Column - radius;
                        location1Row = center.Row + radius;
                        location2Column = center.Column - radius;
                        location2Row = center.Row;
                        yDirection = true;
                    }
                    break;
                case Octant.WNW:
                    {
                        location1Column = center.Column - radius;
                        location1Row = center.Row;
                        location2Column = center.Column - radius;
                        location2Row = center.Row - radius;
                        yDirection = true;
                    }
                    break;
                default:
                    throw new Exception("Unhandled Grid Octant - ShadowCastScan");
            }
        }

        /// <summary>
        /// Calculates slope of angle of ray passing through the "First (leading)" point on the specified location sweeping clockwise
        /// </summary>
        private static void CalculateLeadingSlope(GridLocation location, GridLocation center, out double deltaX, out double deltaY)
        {
            var north = location.Row < center.Row;
            var south = location.Row > center.Row;
            var east = location.Column > center.Column;
            var west = location.Column < center.Column;

            var centerPoint = CalculateCellCenter(center);
            var bounds = CalculateCellBoundary(location);

            // Wall Lights - Use point to create minimum angle for the leading slope
            //
            if (north && west)
            {
                deltaY = bounds.BottomLeft.Y - centerPoint.Y;
                deltaX = bounds.BottomLeft.X - centerPoint.X;
            }
            else if (north && east)
            {
                deltaY = bounds.TopLeft.Y - centerPoint.Y;
                deltaX = bounds.TopLeft.X - centerPoint.X;
            }
            else if (south && east)
            {
                deltaY = bounds.TopRight.Y - centerPoint.Y;
                deltaX = bounds.TopRight.X - centerPoint.X;
            }
            else if (south && west)
            {
                deltaY = bounds.BottomRight.Y - centerPoint.Y;
                deltaX = bounds.BottomRight.X - centerPoint.X;
            }
            else if (north)
            {
                deltaY = bounds.BottomLeft.Y - centerPoint.Y;
                deltaX = bounds.BottomLeft.X - centerPoint.X;
            }
            else if (east)
            {
                deltaY = bounds.TopLeft.Y - centerPoint.Y;
                deltaX = bounds.TopLeft.X - centerPoint.X;
            }
            else if (south)
            {
                deltaY = bounds.TopRight.Y - centerPoint.Y;
                deltaX = bounds.TopRight.X - centerPoint.X;
            }
            else if (west)
            {
                deltaY = bounds.BottomRight.Y - centerPoint.Y;
                deltaX = bounds.BottomRight.X - centerPoint.X;
            }
            else
                throw new Exception("Unhandled direction SimpleRayTracer.CalculateFirstAngle()");
        }

        /// <summary>
        /// Calculates slope of angle of ray passing through the "Second (trailing)" point on the specified location sweeping clockwise
        /// </summary>
        private static void CalculateTrailingSlope(GridLocation location, GridLocation center, out double deltaX, out double deltaY)
        {
            var north = location.Row < center.Row;
            var south = location.Row > center.Row;
            var east = location.Column > center.Column;
            var west = location.Column < center.Column;

            var centerPoint = CalculateCellCenter(center);
            var centerBoundary = CalculateCellBoundary(center);
            var bounds = CalculateCellBoundary(location);

            // Wall Lights - Use point to create maximum angle for the trailing slope
            //
            if (north && west)
            {
                deltaY = bounds.TopRight.Y - centerPoint.Y;
                deltaX = bounds.TopRight.X - centerPoint.X;
            }
            else if (north && east)
            {
                deltaY = bounds.BottomRight.Y - centerPoint.Y;
                deltaX = bounds.BottomRight.X - centerPoint.X;
            }
            else if (south && east)
            {
                deltaY = bounds.BottomLeft.Y - centerPoint.Y;
                deltaX = bounds.BottomLeft.X - centerPoint.X;
            }
            else if (south && west)
            {
                deltaY = bounds.TopLeft.Y - centerPoint.Y;
                deltaX = bounds.TopLeft.X - centerPoint.X;
            }
            else if (north)
            {
                deltaY = bounds.BottomRight.Y - centerPoint.Y;
                deltaX = bounds.BottomRight.X - centerPoint.X;
            }
            else if (east)
            {
                deltaY = bounds.BottomLeft.Y - centerPoint.Y;
                deltaX = bounds.BottomLeft.X - centerPoint.X;
            }
            else if (south)
            {
                deltaY = bounds.TopLeft.Y - centerPoint.Y;
                deltaX = bounds.TopLeft.X - centerPoint.X;
            }
            else if (west)
            {
                deltaY = bounds.TopRight.Y - centerPoint.Y;
                deltaX = bounds.TopRight.X - centerPoint.X;
            }
            else
                throw new Exception("Unhandled direction SimpleRayTracer.CalculateFirstAngle()");
        }

        /// <summary>
        /// Calculates slope of angle of ray passing through the center of the specified point
        /// </summary>
        private static void CalculateCenterSlope(GridLocation location, GridLocation center, out double deltaX, out double deltaY)
        {
            var centerPoint = CalculateCellCenter(center);
            var locationCenterPoint = CalculateCellCenter(location);

            deltaY = locationCenterPoint.Y - centerPoint.Y;
            deltaX = locationCenterPoint.X - centerPoint.X;
        }

        /// <summary>
        /// Calculates whether the given location is blocked by another light blocking location - given the provided center and octant
        /// </summary>
        private static bool IsLocationBlocked(IEnumerable<ShadowCastingFeature> lightBlockingFeatures, GridLocation location, GridLocation center, Octant octant)
        {
            if (lightBlockingFeatures.None())
                return false;

            // Visibility Blocking Rules: Location must have blocked leading and trailing points. Center point is used to determine 
            //                            axis crossing features.
            //
            var slopeCenter = OctantSlope.CreateCenterSlope(location, center);
            var slopeLeading = OctantSlope.CreateLeadingSlope(location, center);
            var slopeTrailing = OctantSlope.CreateTrailingSlope(location, center);

            return lightBlockingFeatures.Any(feature =>
            {
                // Need to detect features that cross over the cardinal axes

                // N | S -> Magnitude (Abs()) of location slopes must be GREATER than the feature's magnitudes (Abs())
                if (feature.LeadingSlope.Magnitude > 0 && feature.TrailingSlope.Magnitude < 0)
                {
                    // Case 1:  Location is also crossing over the cardinal axis
                    if (slopeLeading.Magnitude.Sign() != slopeTrailing.Magnitude.Sign())
                        return slopeLeading.Magnitude.Abs() >= feature.LeadingSlope.Magnitude.Abs() &&
                               slopeTrailing.Magnitude.Abs() >= feature.TrailingSlope.Magnitude.Abs();

                    // Case 2:  Location is in the quadrant; but the feature is crossing the axis
                    else
                    {
                        switch (octant)
                        {
                            case Octant.NNW:
                            case Octant.SSE:
                                return slopeCenter.Magnitude >= feature.LeadingSlope.Magnitude;
                            case Octant.NNE:
                            case Octant.SSW:
                                return slopeCenter.Magnitude <= feature.TrailingSlope.Magnitude;
                            default:
                                throw new Exception("Unhandled octant IsLightBlocking");
                        }
                    }
                }

                // E | W -> Magnitude (Abs()) of location slopes must be LESS than the feature's magnitudes (Abs())
                if (feature.LeadingSlope.Magnitude < 0 && feature.TrailingSlope.Magnitude > 0)
                {
                    // Case 1:  Location is also crossing over the cardinal axis
                    if (slopeLeading.Magnitude.Sign() != slopeTrailing.Magnitude.Sign())
                        return slopeLeading.Magnitude.Abs() <= feature.LeadingSlope.Magnitude.Abs() &&
                               slopeTrailing.Magnitude.Abs() <= feature.TrailingSlope.Magnitude.Abs();

                    // Case 2:  Location is in the quadrant; but the feature is crossing the axis
                    else
                    {
                        switch (octant)
                        {
                            case Octant.ENE:
                            case Octant.WSW:
                                return slopeCenter.Magnitude >= feature.LeadingSlope.Magnitude;
                            case Octant.ESE:
                            case Octant.WNW:
                                return slopeCenter.Magnitude <= feature.TrailingSlope.Magnitude;
                            default:
                                throw new Exception("Unhandled octant IsLightBlocking");
                        }
                    }
                }

                // Default Case:  All features within the same quadrant - the slope magnitude is always increasing
                return (slopeLeading.Magnitude.Between(feature.LeadingSlope.Magnitude, feature.TrailingSlope.Magnitude, true) &&
                        slopeTrailing.Magnitude.Between(feature.LeadingSlope.Magnitude, feature.TrailingSlope.Magnitude, true));
            });
        }

        private static Point CalculateCellCenter(GridLocation location)
        {
            return new Point((location.Column * ModelConstants.CellWidth) + (ModelConstants.CellWidth / 2.0),
                             (location.Row * ModelConstants.CellHeight) + (ModelConstants.CellHeight / 2.0));
        }

        private static Rect CalculateCellBoundary(GridLocation location)
        {
            return new Rect(new Point(location.Column * ModelConstants.CellWidth,
                                      location.Row * ModelConstants.CellHeight),
                            new Size(ModelConstants.CellWidth, ModelConstants.CellHeight));
        }
    }
}
