using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;

using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Provides grid location instances for the specified indices
        /// </summary>
        public delegate GridLocation VisibilityGridCallback(int column, int row);

        /// <summary>
        /// Provides callback to specify whether location is light blocking
        /// </summary>
        public delegate bool IsLightBlockingGridCallback(int column, int row);

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
            private double _deltaX;
            private double _deltaY;
            private double _magnitude;

            public double Magnitude { get { return _magnitude; } }

            public OctantSlope(Point point, Point center)
            {
                _deltaX = point.X - center.X;
                _deltaY = point.Y - center.Y;

                if (_deltaX != 0)
                    _magnitude = _deltaY / _deltaX;

                else
                {
                    if (_deltaY > 0)
                        _magnitude = double.PositiveInfinity;

                    else if (_deltaY < 0)
                        _magnitude = double.NegativeInfinity;

                    else
                        _magnitude = double.NaN;
                }
            }
            public OctantSlope(double deltaX, double deltaY)
            {
                _deltaX = deltaX;
                _deltaY = deltaY;

                if (_deltaX != 0)
                    _magnitude = _deltaY / _deltaX;

                else
                {
                    if (_deltaY > 0)
                        _magnitude = double.PositiveInfinity;

                    else if (_deltaY < 0)
                        _magnitude = double.NegativeInfinity;

                    else
                        _magnitude = double.NaN;
                }
            }

            public static OctantSlope CreateLeadingSlope(GridLocation location, GridLocation center)
            {
                double deltaX, deltaY;

                CalculateLeadingSlope(location, center, out deltaX, out deltaY);

                return new OctantSlope(deltaX, deltaY);
            }

            public static OctantSlope CreateTrailingSlope(GridLocation location, GridLocation center)
            {
                double deltaX, deltaY;

                CalculateTrailingSlope(location, center, out deltaX, out deltaY);

                return new OctantSlope(deltaX, deltaY);
            }

            public static OctantSlope CreateCenterSlope(GridLocation location, GridLocation center)
            {
                double deltaX, deltaY;

                CalculateCenterSlope(location, center, out deltaX, out deltaY);

                return new OctantSlope(deltaX, deltaY);
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

        public static void CalculateVisibility(LayoutGrid grid, GridLocation location, int radius, VisibilityCalculatorCallback callback)
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
            }, location, radius, callback);
        }

        /// <summary>
        /// Routine for calculating visibility during the layout generation process. This is used to initialize the layout lighting
        /// </summary>
        public static void CalculateVisibility(GridLocation location,
                                               int radius,
                                               VisibilityGridCallback gridCallback,
                                               IsLightBlockingGridCallback lightBlockingCallback,
                                               VisibilityCalculatorCallback visibilityCallback)
        {
            CalculateVisibilityImpl((column, row) =>
            {
                return gridCallback(column, row);
            },
            (gridLocation) =>
            {
                return lightBlockingCallback(gridLocation.Column, gridLocation.Row);

            }, location, radius, visibilityCallback);
        }

        private static void CalculateVisibilityImpl(Func<int, int, GridLocation> getter, Func<GridLocation, bool> getterIsLightBlocking, GridLocation center, int maxRadius, VisibilityCalculatorCallback callback)
        {
            // Implementing Shadow Casting - by Björn Bergström [bjorn.bergstrom@roguelikedevelopment.org] 
            //
            // http://www.roguebasin.com/index.php?title=FOV_using_recursive_shadowcasting

            // Scan all octants
            foreach (Octant octant in Enum.GetValues(typeof(Octant)))
            {
                var lightBlockingFeatures = new List<ShadowCastingFeature>();
                var iterate = true;
                var currentRadius = 1;

                while (iterate)
                {
                    int location1Column, location1Row, location2Column, location2Row;
                    bool yDirection = false;

                    CalculateIterationLocations(center, currentRadius, maxRadius, octant, out location1Column, out location1Row, out location2Column, out location2Row, out yDirection);

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

                        //// CHECK MAX RADIUS FOR THIS OCTANT
                        //if (Metric.EuclideanDistance(column, row, center.Column, center.Row) > MAX_RADIUS)
                        //    break;

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

                    currentRadius++;

                    // Re-evaluate loop condition
                    iterate = !finalIteration && (currentRadius <= maxRadius);
                }
            }
        }

        private static void CalculateIterationLocations(GridLocation center, int radius, int maxRadius, Octant octant, out int location1Column, out int location1Row, out int location2Column, out int location2Row, out bool yDirection)
        {
            // Calculate a max iterating distance based on the radius and MAX_RADIUS. Solve the right triangle 
            // inside a circle of MAX_RADIUS - this will show the below solution.
            //
            var circleCoordinate = System.Math.Sqrt((maxRadius * maxRadius) - (radius * radius));
            var circleIteratingDistance = (int)System.Math.Min(radius, circleCoordinate);

            // NOTE*** MUST ITERATE CLOCKWISE AROUND THE CIRCLE
            switch (octant)
            {
                case Octant.NNW:
                    {
                        location1Column = center.Column - circleIteratingDistance;
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
                        location2Column = center.Column + circleIteratingDistance;
                        location2Row = center.Row - radius;
                        yDirection = false;
                    }
                    break;
                case Octant.ENE:
                    {
                        location1Column = center.Column + radius;
                        location1Row = center.Row - circleIteratingDistance;
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
                        location2Row = center.Row + circleIteratingDistance;
                        yDirection = true;
                    }
                    break;
                case Octant.SSE:
                    {
                        location1Column = center.Column + circleIteratingDistance;
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
                        location2Column = center.Column - circleIteratingDistance;
                        location2Row = center.Row + radius;
                        yDirection = false;
                    }
                    break;
                case Octant.WSW:
                    {
                        location1Column = center.Column - radius;
                        location1Row = center.Row + circleIteratingDistance;
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
                        location2Row = center.Row - circleIteratingDistance;
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

            var featureFound = false;

            foreach (var feature in lightBlockingFeatures)
            {
                // Need to detect features that cross over the cardinal axes

                // N | S -> Magnitude (Abs()) of location slopes must be GREATER than the feature's magnitudes (Abs())
                if (feature.LeadingSlope.Magnitude > 0 && feature.TrailingSlope.Magnitude < 0)
                {
                    // Case 1:  Location is also crossing over the cardinal axis
                    if (slopeLeading.Magnitude.Sign() != slopeTrailing.Magnitude.Sign())
                        featureFound |= slopeLeading.Magnitude.Abs() >= feature.LeadingSlope.Magnitude.Abs() &&
                                        slopeTrailing.Magnitude.Abs() >= feature.TrailingSlope.Magnitude.Abs();

                    // Case 2:  Location is in the quadrant; but the feature is crossing the axis
                    else
                    {
                        switch (octant)
                        {
                            case Octant.NNW:
                            case Octant.SSE:
                                featureFound |= slopeCenter.Magnitude >= feature.LeadingSlope.Magnitude;
                                break;
                            case Octant.NNE:
                            case Octant.SSW:
                                featureFound |= slopeCenter.Magnitude <= feature.TrailingSlope.Magnitude;
                                break;
                            default:
                                throw new Exception("Unhandled octant IsLightBlocking");
                        }
                    }
                }

                // PERFORMANCE OPTIMIZATION
                if (featureFound)
                    return featureFound;

                // E | W -> Magnitude (Abs()) of location slopes must be LESS than the feature's magnitudes (Abs())
                if (feature.LeadingSlope.Magnitude < 0 && feature.TrailingSlope.Magnitude > 0)
                {
                    // Case 1:  Location is also crossing over the cardinal axis
                    if (slopeLeading.Magnitude.Sign() != slopeTrailing.Magnitude.Sign())
                        featureFound |= slopeLeading.Magnitude.Abs() <= feature.LeadingSlope.Magnitude.Abs() &&
                                        slopeTrailing.Magnitude.Abs() <= feature.TrailingSlope.Magnitude.Abs();

                    // Case 2:  Location is in the quadrant; but the feature is crossing the axis
                    else
                    {
                        switch (octant)
                        {
                            case Octant.ENE:
                            case Octant.WSW:
                                featureFound |= slopeCenter.Magnitude >= feature.LeadingSlope.Magnitude;
                                break;
                            case Octant.ESE:
                            case Octant.WNW:
                                featureFound |= slopeCenter.Magnitude <= feature.TrailingSlope.Magnitude;
                                break;
                            default:
                                throw new Exception("Unhandled octant IsLightBlocking");
                        }
                    }
                }

                // PERFORMANCE OPTIMIZATION
                if (featureFound)
                    return featureFound;

                // Default Case:  All features within the same quadrant - the slope magnitude is always increasing
                featureFound |= (slopeLeading.Magnitude.Between(feature.LeadingSlope.Magnitude, feature.TrailingSlope.Magnitude, true) &&
                                slopeTrailing.Magnitude.Between(feature.LeadingSlope.Magnitude, feature.TrailingSlope.Magnitude, true));

                if (featureFound)
                    return true;
            }

            return false;
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
