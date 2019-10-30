using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Math.Geometry
{
    public class RectangularTiling
    {
        RectangleInt _boundary;
        List<RectangleInt> _regionRectangles;
        List<RectangleInt> _connectingRectangles;

        public RectangleInt Boundary
        {
            get { return _boundary; }
        }

        public IEnumerable<RectangleInt> RegionRectangles
        {
            get { return _regionRectangles; }
        }

        public IEnumerable<RectangleInt> ConnectingRectangles
        {
            get { return _connectingRectangles; }
        }

        public RectangularTiling(RectangleInt boundary)
        {
            _boundary = boundary;
            _regionRectangles = new List<RectangleInt>();
            _connectingRectangles = new List<RectangleInt>();
        }

        /// <summary>
        /// Adds rectangle to the tiling. Starts a routine to iterate over all tiling rectangles to find
        /// overlaps; and processes them to re-create the affected ones. 
        /// </summary>
        /// <exception cref="ArgumentException">Provided rectangle is out of bounds of the tiling</exception>
        public void AddRegionRectangle(RectangleInt rectangle)
        {
            if (!_boundary.Contains(rectangle))
                throw new ArgumentException("Out-of-bounds rectangle in the rectangular tiling");

            // Cases:
            //
            // 0) Boundary rectangle is the only rectangle -> Add the region and subdivide the boundary into 4 connecting rectangles
            // 1) Rectangle is completely contained in a region rectangle -> Just return because parent rectangle covers the area
            // 2) Rectangle is completely or partially contained in a connecting rectangle -> Subdivide the affected rectangle
            // 3) Rectangle is partially contained in region -> Keep both regions and sub-divide any connecting rectangles
            //

            // Calculate region intersections
            var regionRectangles = _regionRectangles.Select(regionRectangle => new
            {
                Rectangle = regionRectangle,
                Contains = regionRectangle.Contains(rectangle),
                Intersects = rectangle.Intersects(regionRectangle),
                Overlap = CalculateOverlap(regionRectangle, rectangle),
            })
            .Actualize();

            // Calculate connecting region intersections
            var connectingRectangles = _connectingRectangles.Select(connectingRectangle => new
            {
                Rectangle = connectingRectangle,
                Contains = connectingRectangle.Contains(rectangle),
                Intersects = rectangle.Intersects(connectingRectangle),
                Overlap = CalculateOverlap(connectingRectangle, rectangle)
            })
            .Actualize();

            // Check for Case 0
            if (!regionRectangles.Any() && !connectingRectangles.Any())
            {
                // Boundary fully contains rectangle - subdivide it with respect to rectangle
                _connectingRectangles.AddRange(SubDivide(_boundary, rectangle, CalculateOverlap(_boundary, rectangle)));

                // Add the region rectangle
                _regionRectangles.Add(rectangle);
            }

            // Check for Case 1
            else if (regionRectangles.Any(regionRectangle => regionRectangle.Contains))
            {
                // Just return because one of the region rectangles fully contains the new one
                return;
            }

            // Deal with Cases 2 & 3
            else
            {
                var invalidConnectingRectangles = new List<RectangleInt>();

                // Sub-divide connecting rectangles that overlap or contain rectangle
                foreach (var connectingRectangle in connectingRectangles)
                {
                    if (connectingRectangle.Contains ||
                        connectingRectangle.Intersects)
                    {
                        // Check to see if the connecting rectangle LIES INSIDE the new rectangle
                        if (connectingRectangle.Overlap == Compass.Null)
                            invalidConnectingRectangles.Add(connectingRectangle.Rectangle);

                        // Connecting Rectangle protrudes from the new one
                        else
                        {
                            // Add connecting rectangle to invalid rectangles
                            invalidConnectingRectangles.Add(connectingRectangle.Rectangle);

                            // Add sub-divided rectangles to the main collection
                            _connectingRectangles.AddRange(SubDivide(connectingRectangle.Rectangle, rectangle, connectingRectangle.Overlap));
                        }
                    }
                }

                // Remove invalid rectangles
                foreach (var invalidRectangle in invalidConnectingRectangles)
                    _connectingRectangles.Remove(invalidRectangle);

                // Add the region rectangle
                _regionRectangles.Add(rectangle);
            }

            // TODO:TERRAIN REMOVE THIS
            if (_connectingRectangles.Any(rectangle1 => _connectingRectangles.Any(rectangle2 => rectangle1 != rectangle2 && rectangle1.Intersects(rectangle2))))
            {
                throw new Exception("Improperly formed connecting rectangles");
            }
        }

        /// <summary>
        /// Returns flags to show the directions that the rectangle1 overlaps rectangle2. The cardinal directions
        /// are only considered { N, W, E, S }. The result denotes the "leftover space from the rectangle1  MINUS the intersection. 
        /// Example: N => The "leftover space" after the intersection from rectangle1 lies completely to the north of rectangle2. 
        /// Example N | W => The "leftover space" after intersection lies to the north AND west. Example N | W | S | E => The "leftover space" 
        /// lies on all sides. This means rectangle1 CONTAINS rectangle2. Example (Null) => rectangle2 CONTAINS rectangle1 OR does NOT overlap at all. 
        /// In that case - must use "Intersects" as a secondary check for overlap. 
        /// </summary>
        private Compass CalculateOverlap(RectangleInt rectangle1, RectangleInt rectangle2)
        {
            var overlap = Compass.Null;

            if (rectangle1.Top < rectangle2.Top && rectangle1.Bottom >= rectangle2.Top)
                overlap |= Compass.N;

            if (rectangle1.Left < rectangle2.Left && rectangle1.Right >= rectangle2.Left)
                overlap |= Compass.W;

            if (rectangle1.Bottom > rectangle2.Bottom && rectangle1.Top <= rectangle2.Bottom)
                overlap |= Compass.S;

            if (rectangle1.Right > rectangle2.Right && rectangle1.Left <= rectangle2.Right)
                overlap |= Compass.E;

            return overlap;
        }

        /// <summary>
        /// Sub-divides rectangle1 using the intersection of rectangle2 with rectangle1. This will create new 
        /// rectangles to replace rectangle1 in the tiling. The overlap is pre-calculated using the CalculateOverlap
        /// method. WARNING** There needs to be a GUARANTEED INTERSECTION for this method to function properly.
        /// </summary>
        private IEnumerable<RectangleInt> SubDivide(RectangleInt rectangle1, RectangleInt rectangle2, Compass overlap)
        {
            // Procedure:  Take case-by-case basis - starting from full overlap (1 fully contains 2) and going down 
            //             by one overlapping side to 3, 2, and 1.
            //
            //             Use the "oriental cross-hatch pattern" to sub-divide the resulting space using rectangle1's
            //             corners like so:  { NW -> North, NE -> East, SE -> South, SW -> West }. The result looks like 
            //             the diagram below. This should create a more event pattern and prevent making decision on
            //             "how" to divide up the resulting space.
            //

            // NOTE*** USE THE CROSS-HATCH DIAGRAM TO SHOW WHERE THE BOUNDARY EDGES ARE EQUAL FOR THE SUBDIVISION. 
            //         OFF-BY-ONE ERRORS ARE DEALT WITH THIS WAY.
            //
            //         THE SOLUTION IS TO LET THE "SHORT" EDGE OF THE SUB-DIVIDED RECTANGLE SHARE THE EDGE OF THE
            //         OTHER ONE - SO DO THIS FOR EACH SUB-DIVISION. 
            //
            //         (OR) "CANNOT SHARE RECTANGLE 2'S EDGES" (PRETTY SURE THAT'S THE BEST WAY TO PUT IT)
            //
            //         (Try not to use the sub-rectangles to calculate other sub-rectangles)

            // Check the largest intersections first (contains), then smaller ones (one cardinal edge) -> one corner
            //
            if (overlap.Has(Compass.N) &&
                overlap.Has(Compass.S) &&
                overlap.Has(Compass.E) &&
                overlap.Has(Compass.W))
            {
                // Rectangle 1 fully contains rectangle 2:  Create a tiling of rectangle 1 using a "oriental cross-hatch"
                //                                          pattern
                //    |__ __
                //  __|__|
                //       |  
                //

                // NOTE*** Sub-Rectangle-1 shares the BOTTOM edge of Rectangle 2; but NOT the LEFT edge. This is the pattern
                //         to use to prevent off-by-one errors. (THE BOTTOM EDGE OF Sub-Rectangle-1 IS THE "SHORT" EDGE)
                //
                var subRectangle1 = new RectangleInt(rectangle1.TopLeft, new VertexInt(rectangle2.Left - 1, rectangle2.Bottom));
                var subRectangle2 = new RectangleInt(new VertexInt(rectangle2.Left, rectangle1.Top), new VertexInt(rectangle1.Right, rectangle2.Top - 1));
                var subRectangle3 = new RectangleInt(new VertexInt(rectangle2.Right + 1, rectangle2.Top), rectangle1.BottomRight);
                var subRectangle4 = new RectangleInt(new VertexInt(rectangle1.Left, rectangle2.Bottom + 1), new VertexInt(rectangle2.Right, rectangle1.Bottom));

                return new RectangleInt[] { subRectangle1, subRectangle2, subRectangle3, subRectangle4 };
            }

            // S | W | N (Rectangle 1 protrudes around Rectangle 2 in the S, W, and N directions)
            else if (overlap.Has(Compass.S) &&
                     overlap.Has(Compass.W) &&
                     overlap.Has(Compass.N))
            {
                var subRectangle1 = new RectangleInt(rectangle1.TopLeft, new VertexInt(rectangle2.Left - 1, rectangle2.Bottom));
                var subRectangle2 = new RectangleInt(new VertexInt(rectangle2.Left, rectangle1.Top), new VertexInt(rectangle1.Right, rectangle2.Top - 1));
                var subRectangle3 = new RectangleInt(new VertexInt(rectangle1.Left, rectangle2.Bottom + 1), rectangle1.BottomRight);

                return new RectangleInt[] { subRectangle1, subRectangle2, subRectangle3 };
            }

            // W | N | E
            else if (overlap.Has(Compass.W) &&
                     overlap.Has(Compass.N) &&
                     overlap.Has(Compass.E))
            {
                var subRectangle1 = new RectangleInt(rectangle1.TopLeft, new VertexInt(rectangle2.Left - 1, rectangle1.Bottom));
                var subRectangle2 = new RectangleInt(new VertexInt(rectangle2.Left, rectangle1.Top), new VertexInt(rectangle1.Right, rectangle2.Top - 1));
                var subRectangle3 = new RectangleInt(new VertexInt(rectangle2.Right + 1, rectangle2.Top), rectangle1.BottomRight);

                return new RectangleInt[] { subRectangle1, subRectangle2, subRectangle3 };
            }

            // N | E | S
            else if (overlap.Has(Compass.N) &&
                     overlap.Has(Compass.E) &&
                     overlap.Has(Compass.S))
            {
                var subRectangle1 = new RectangleInt(rectangle1.TopLeft, new VertexInt(rectangle1.Right, rectangle2.Top - 1));
                var subRectangle2 = new RectangleInt(new VertexInt(rectangle2.Right + 1, rectangle2.Top), rectangle1.BottomRight);
                var subRectangle3 = new RectangleInt(new VertexInt(rectangle1.Left, rectangle2.Bottom + 1), new VertexInt(rectangle2.Right, rectangle1.Bottom));

                return new RectangleInt[] { subRectangle1, subRectangle2, subRectangle3 };
            }

            // E | S | W
            else if (overlap.Has(Compass.E) &&
                     overlap.Has(Compass.S) &&
                     overlap.Has(Compass.W))
            {
                var subRectangle1 = new RectangleInt(rectangle1.TopLeft, new VertexInt(rectangle2.Left - 1, rectangle2.Bottom));
                var subRectangle2 = new RectangleInt(new VertexInt(rectangle1.Left, rectangle2.Bottom + 1), new VertexInt(rectangle2.Right, rectangle1.Bottom));
                var subRectangle3 = new RectangleInt(new VertexInt(rectangle2.Right + 1, rectangle1.Top), rectangle1.BottomRight);

                return new RectangleInt[] { subRectangle1, subRectangle2, subRectangle3 };
            }

            // N | W
            else if (overlap.Has(Compass.N) &&
                     overlap.Has(Compass.W))
            {
                var subRectangle1 = new RectangleInt(rectangle1.TopLeft, new VertexInt(rectangle2.Left - 1, rectangle1.Bottom));
                var subRectangle2 = new RectangleInt(new VertexInt(rectangle2.Left, rectangle1.Top), new VertexInt(rectangle1.Right, rectangle2.Top - 1));

                return new RectangleInt[] { subRectangle1, subRectangle2 };
            }

            // N | E
            else if (overlap.Has(Compass.N) &&
                     overlap.Has(Compass.E))
            {
                var subRectangle1 = new RectangleInt(rectangle1.TopLeft, new VertexInt(rectangle1.Right, rectangle2.Top - 1));
                var subRectangle2 = new RectangleInt(new VertexInt(rectangle2.Right + 1, rectangle2.Top), rectangle1.BottomRight);

                return new RectangleInt[] { subRectangle1, subRectangle2 };
            }

            // S | E
            else if (overlap.Has(Compass.S) &&
                     overlap.Has(Compass.E))
            {
                var subRectangle1 = new RectangleInt(new VertexInt(rectangle2.Right + 1, rectangle1.Top), rectangle1.BottomRight);
                var subRectangle2 = new RectangleInt(new VertexInt(rectangle1.Left, rectangle2.Bottom + 1), new VertexInt(rectangle2.Right, rectangle1.Bottom));

                return new RectangleInt[] { subRectangle1, subRectangle2 };
            }

            // S | W
            else if (overlap.Has(Compass.S) &&
                     overlap.Has(Compass.W))
            {
                var subRectangle1 = new RectangleInt(rectangle1.TopLeft, new VertexInt(rectangle2.Left - 1, rectangle2.Bottom));
                var subRectangle2 = new RectangleInt(new VertexInt(rectangle1.Left, rectangle2.Bottom + 1), rectangle1.BottomRight);

                return new RectangleInt[] { subRectangle1, subRectangle2 };
            }

            // N | S
            else if (overlap.Has(Compass.N) &&
                     overlap.Has(Compass.S))
            {
                var subRectangle1 = new RectangleInt(rectangle1.TopLeft, new VertexInt(rectangle1.Right, rectangle2.Top - 1));
                var subRectangle2 = new RectangleInt(new VertexInt(rectangle1.Left, rectangle2.Bottom + 1), rectangle1.BottomRight);

                return new RectangleInt[] { subRectangle1, subRectangle2 };
            }

            // W | E
            else if (overlap.Has(Compass.W) &&
                     overlap.Has(Compass.E))
            {
                var subRectangle1 = new RectangleInt(rectangle1.TopLeft, new VertexInt(rectangle2.Left - 1, rectangle1.Bottom));
                var subRectangle2 = new RectangleInt(new VertexInt(rectangle2.Right + 1, rectangle1.Top), rectangle1.BottomRight);

                return new RectangleInt[] { subRectangle1, subRectangle2 };
            }

            // N
            else if (overlap.Has(Compass.N))
            {
                var subRectangle1 = new RectangleInt(rectangle1.TopLeft, new VertexInt(rectangle1.Right, rectangle2.Top - 1));

                return new RectangleInt[] { subRectangle1 };
            }

            // S
            else if (overlap.Has(Compass.S))
            {
                var subRectangle1 = new RectangleInt(new VertexInt(rectangle1.Left, rectangle2.Bottom + 1), rectangle1.BottomRight);

                return new RectangleInt[] { subRectangle1 };
            }

            // E
            else if (overlap.Has(Compass.E))
            {
                var subRectangle1 = new RectangleInt(new VertexInt(rectangle2.Right + 1, rectangle1.Top), rectangle1.BottomRight);

                return new RectangleInt[] { subRectangle1 };
            }

            // W
            else if (overlap.Has(Compass.W))
            {
                var subRectangle1 = new RectangleInt(rectangle1.TopLeft, new VertexInt(rectangle2.Left - 1, rectangle1.Bottom));

                return new RectangleInt[] { subRectangle1 };
            }

            // Null -> Rectangle 2 Fully Contains Rectangle 1
            else if (overlap == Compass.Null)
            {
                throw new Exception("Trying to subdivide a rectangle withing the containing rectangle");
            }

            else
                throw new Exception("Unhandled Rectangle Overlap RectangularTiling");
        }
    }
}
