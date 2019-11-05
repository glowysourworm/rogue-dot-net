using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Math.Geometry
{
    public class NavigationTiling
    {
        readonly static IRandomSequenceGenerator _randomSequenceGenerator;

        NavigationTile _boundary;
        List<NavigationTile> _regionTiles;
        List<NavigationTile> _connectingTiles;

        public NavigationTile Boundary
        {
            get { return _boundary; }
        }

        public IEnumerable<NavigationTile> RegionTiles
        {
            get { return _regionTiles; }
        }

        public IEnumerable<NavigationTile> ConnectingTiles
        {
            get { return _connectingTiles; }
        }

        static NavigationTiling()
        {
            _randomSequenceGenerator = ServiceLocator.Current.GetInstance<IRandomSequenceGenerator>();
        }

        public NavigationTiling(NavigationTile boundary)
        {
            _boundary = boundary;
            _regionTiles = new List<NavigationTile>();
            _connectingTiles = new List<NavigationTile>();
        }

        /// <summary>
        /// Creates a connected graph using adjacent tiles to find edges. NOTE*** BE SURE TO CALL
        /// CalculateConnections() FIRST.
        /// </summary>
        public Graph<NavigationTile> CreateGraph()
        {
            var allTiles = this.RegionTiles.Union(this.ConnectingTiles).Actualize();
            
            // Construct connections for all tiles based on pre-calculated connection points
            var connections = allTiles.SelectMany(tile =>
            {
                return tile.ConnectionPoints.Select(connection =>
                {
                    return new ReferencedEdge<NavigationTile>(new ReferencedVertex<NavigationTile>(tile, new Vertex(tile.Center)),
                                                              new ReferencedVertex<NavigationTile>(connection.AdjacentTile, new Vertex(connection.AdjacentTile.Center)));
                });
            });

            // Create distinct connections using both edge orientations for equality check
            var distinctConnections = connections.DistinctWith((edge1, edge2) =>
            {
                // Check both orientations for edge equality
                return ((edge1.Point1.Reference == edge2.Point1.Reference) && (edge1.Point2.Reference == edge2.Point2.Reference)) ||
                       ((edge1.Point1.Reference == edge2.Point2.Reference) && (edge1.Point2.Reference == edge2.Point1.Reference));
            });

            return new Graph<NavigationTile>(distinctConnections);
        }

        /// <summary>
        /// Adds rectangle to the tiling. Starts a routine to iterate over all tiling rectangles to find
        /// overlaps; and processes them to re-create the affected ones. 
        /// </summary>
        /// <exception cref="ArgumentException">Provided rectangle is out of bounds of the tiling</exception>
        public void AddTile(NavigationTile tile)
        {
            if (!_boundary.Contains(tile))
                throw new ArgumentException("Out-of-bounds rectangle in the rectangular tiling");

            // Cases:
            //
            // 0) Boundary rectangle is the only rectangle -> Add the region and subdivide the boundary into 4 connecting rectangles
            // 1) Rectangle is completely contained in a region rectangle -> Just return because parent rectangle covers the area
            // 2) Rectangle is completely or partially contained in a connecting rectangle -> Subdivide the affected rectangle
            // 3) Rectangle is partially contained in region -> Keep both regions and sub-divide any connecting rectangles
            //

            // Calculate region intersections
            var regionRectangles = _regionTiles.Select(regionRectangle => new
            {
                Rectangle = regionRectangle,
                Contains = regionRectangle.Contains(tile),
                Intersects = tile.Intersects(regionRectangle),
                Overlap = CalculateOverlap(regionRectangle, tile),
            })
            .Actualize();

            // Calculate connecting region intersections
            var connectingRectangles = _connectingTiles.Select(connectingRectangle => new
            {
                Rectangle = connectingRectangle,
                Contains = connectingRectangle.Contains(tile),
                Intersects = tile.Intersects(connectingRectangle),
                Overlap = CalculateOverlap(connectingRectangle, tile)
            })
            .Actualize();

            // Check for Case 0
            if (!regionRectangles.Any() && !connectingRectangles.Any())
            {
                // Boundary fully contains rectangle - subdivide it with respect to rectangle
                _connectingTiles.AddRange(SubDivide(_boundary, tile, CalculateOverlap(_boundary, tile)));

                // Add the region rectangle
                _regionTiles.Add(tile);
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
                var invalidConnectingRectangles = new List<NavigationTile>();

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
                            _connectingTiles.AddRange(SubDivide(connectingRectangle.Rectangle, tile, connectingRectangle.Overlap));
                        }
                    }
                }

                // Remove invalid rectangles
                foreach (var invalidRectangle in invalidConnectingRectangles)
                    _connectingTiles.Remove(invalidRectangle);

                // Add the region rectangle
                _regionTiles.Add(tile);
            }

            // TODO:TERRAIN REMOVE THIS
            if (_connectingTiles.Any(rectangle1 => _connectingTiles.Any(rectangle2 => rectangle1 != rectangle2 && rectangle1.Intersects(rectangle2))))
            {
                throw new Exception("Improperly formed connecting rectangles");
            }
        }

        /// <summary>
        /// Calculates connection points for each of the tiles in the tiling.
        /// </summary>
        public void CalculateConnections()
        {
            var allTiles = _regionTiles.Union(_connectingTiles).Actualize();

            // Check each tile with every other tile
            foreach (var tile1 in allTiles)
            {
                foreach (var tile2 in allTiles)
                {
                    if (tile1 == tile2)
                        continue;

                    // Check for bordering tiles
                    if (!tile1.Borders(tile2))
                        continue;

                    // Calculate the connection points from tile1 -> tile2 AND tile2 -> tile1
                    if (!tile1.ConnectionPoints.Any(connection => connection.AdjacentTile == tile2))
                    {
                        NavigationTileConnectionPoint adjacentTilePoint;

                        // Calculate tile1's point
                        tile1.AddConnection(CalculateConnectionPoint(tile1, tile2, out adjacentTilePoint));

                        // Add tile2's point
                        tile2.AddConnection(adjacentTilePoint);
                    }
                }
            }
        }

        /// <summary>
        /// Routes all tiles that are marekd for routing
        /// </summary>
        public void RouteConnections()
        {
            // Procedure Rules
            //
            // - Only want to route connecting tiles (Region tiles are assumed to be filled with cells)
            // - Each connecting tile must be marked for routing
            // - Each connecting tile MUST have 2 adjacent tiles that are ALSO marked for routing
            //
            foreach (var tile in this.ConnectingTiles
                                     .Where(x => x.IsMarkedForRouting)
                                     .Where(x => x.ConnectionPoints.Count(point => point.AdjacentTile.IsMarkedForRouting) >= 2))
                tile.Route();
        }

        /// <summary>
        /// Returns flags to show the directions that the rectangle1 overlaps rectangle2. The cardinal directions
        /// are only considered { N, W, E, S }. The result denotes the "leftover space from the rectangle1  MINUS the intersection. 
        /// Example: N => The "leftover space" after the intersection from rectangle1 lies completely to the north of rectangle2. 
        /// Example N | W => The "leftover space" after intersection lies to the north AND west. Example N | W | S | E => The "leftover space" 
        /// lies on all sides. This means rectangle1 CONTAINS rectangle2. Example (Null) => rectangle2 CONTAINS rectangle1 OR does NOT overlap at all. 
        /// In that case - must use "Intersects" as a secondary check for overlap. 
        /// </summary>
        private Compass CalculateOverlap(NavigationTile rectangle1, NavigationTile rectangle2)
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
        private IEnumerable<NavigationTile> SubDivide(NavigationTile tile1, NavigationTile tile2, Compass overlap)
        {
            // Procedure:   Take case-by-case basis - starting from full overlap (1 fully contains 2) and going down 
            //              by one overlapping side to 3, 2, and 1.
            //
            //              Use the "oriental cross-hatch pattern" to sub-divide the resulting space using rectangle1's
            //              corners like so:  { NW -> North, NE -> East, SE -> South, SW -> West }. The result looks like 
            //              the diagram below. This should create a more event pattern and prevent making decision on
            //              "how" to divide up the resulting space.
            //
            // Connections: After sub-dividing Rectangle1, remove it's connection list from the dictionary; and add
            //              new entries for each sub-divisition - which will create connections to the surrounding
            //              rectangles in the tiling.

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

                // NOTE*** Sub-Tile-1 shares the BOTTOM edge of Tile 2; but NOT the LEFT edge. This is the pattern
                //         to use to prevent off-by-one errors. (THE BOTTOM EDGE OF Sub-Tile-1 IS THE "SHORT" EDGE)
                //
                var subTile1 = new NavigationTile(tile1.TopLeft, new VertexInt(tile2.Left - 1, tile2.Bottom), false);
                var subTile2 = new NavigationTile(new VertexInt(tile2.Left, tile1.Top), new VertexInt(tile1.Right, tile2.Top - 1), false);
                var subTile3 = new NavigationTile(new VertexInt(tile2.Right + 1, tile2.Top), tile1.BottomRight, false);
                var subTile4 = new NavigationTile(new VertexInt(tile1.Left, tile2.Bottom + 1), new VertexInt(tile2.Right, tile1.Bottom), false);

                return new NavigationTile[] { subTile1, subTile2, subTile3, subTile4 };
            }

            // S | W | N (Rectangle 1 protrudes around Rectangle 2 in the S, W, and N directions)
            else if (overlap.Has(Compass.S) &&
                     overlap.Has(Compass.W) &&
                     overlap.Has(Compass.N))
            {
                var subRectangle1 = new NavigationTile(tile1.TopLeft, new VertexInt(tile2.Left - 1, tile2.Bottom), false);
                var subRectangle2 = new NavigationTile(new VertexInt(tile2.Left, tile1.Top), new VertexInt(tile1.Right, tile2.Top - 1), false);
                var subRectangle3 = new NavigationTile(new VertexInt(tile1.Left, tile2.Bottom + 1), tile1.BottomRight, false);

                return new NavigationTile[] { subRectangle1, subRectangle2, subRectangle3 };
            }

            // W | N | E
            else if (overlap.Has(Compass.W) &&
                     overlap.Has(Compass.N) &&
                     overlap.Has(Compass.E))
            {
                var subRectangle1 = new NavigationTile(tile1.TopLeft, new VertexInt(tile2.Left - 1, tile1.Bottom), false);
                var subRectangle2 = new NavigationTile(new VertexInt(tile2.Left, tile1.Top), new VertexInt(tile1.Right, tile2.Top - 1), false);
                var subRectangle3 = new NavigationTile(new VertexInt(tile2.Right + 1, tile2.Top), tile1.BottomRight, false);

                return new NavigationTile[] { subRectangle1, subRectangle2, subRectangle3 };
            }

            // N | E | S
            else if (overlap.Has(Compass.N) &&
                     overlap.Has(Compass.E) &&
                     overlap.Has(Compass.S))
            {
                var subRectangle1 = new NavigationTile(tile1.TopLeft, new VertexInt(tile1.Right, tile2.Top - 1), false);
                var subRectangle2 = new NavigationTile(new VertexInt(tile2.Right + 1, tile2.Top), tile1.BottomRight, false);
                var subRectangle3 = new NavigationTile(new VertexInt(tile1.Left, tile2.Bottom + 1), new VertexInt(tile2.Right, tile1.Bottom), false);

                return new NavigationTile[] { subRectangle1, subRectangle2, subRectangle3 };
            }

            // E | S | W
            else if (overlap.Has(Compass.E) &&
                     overlap.Has(Compass.S) &&
                     overlap.Has(Compass.W))
            {
                var subRectangle1 = new NavigationTile(tile1.TopLeft, new VertexInt(tile2.Left - 1, tile2.Bottom), false);
                var subRectangle2 = new NavigationTile(new VertexInt(tile1.Left, tile2.Bottom + 1), new VertexInt(tile2.Right, tile1.Bottom), false);
                var subRectangle3 = new NavigationTile(new VertexInt(tile2.Right + 1, tile1.Top), tile1.BottomRight, false);

                return new NavigationTile[] { subRectangle1, subRectangle2, subRectangle3 };
            }

            // N | W
            else if (overlap.Has(Compass.N) &&
                     overlap.Has(Compass.W))
            {
                var subRectangle1 = new NavigationTile(tile1.TopLeft, new VertexInt(tile2.Left - 1, tile1.Bottom), false);
                var subRectangle2 = new NavigationTile(new VertexInt(tile2.Left, tile1.Top), new VertexInt(tile1.Right, tile2.Top - 1), false);

                return new NavigationTile[] { subRectangle1, subRectangle2 };
            }

            // N | E
            else if (overlap.Has(Compass.N) &&
                     overlap.Has(Compass.E))
            {
                var subRectangle1 = new NavigationTile(tile1.TopLeft, new VertexInt(tile1.Right, tile2.Top - 1), false);
                var subRectangle2 = new NavigationTile(new VertexInt(tile2.Right + 1, tile2.Top), tile1.BottomRight, false);

                return new NavigationTile[] { subRectangle1, subRectangle2 };
            }

            // S | E
            else if (overlap.Has(Compass.S) &&
                     overlap.Has(Compass.E))
            {
                var subRectangle1 = new NavigationTile(new VertexInt(tile2.Right + 1, tile1.Top), tile1.BottomRight, false);
                var subRectangle2 = new NavigationTile(new VertexInt(tile1.Left, tile2.Bottom + 1), new VertexInt(tile2.Right, tile1.Bottom), false);

                return new NavigationTile[] { subRectangle1, subRectangle2 };
            }

            // S | W
            else if (overlap.Has(Compass.S) &&
                     overlap.Has(Compass.W))
            {
                var subRectangle1 = new NavigationTile(tile1.TopLeft, new VertexInt(tile2.Left - 1, tile2.Bottom), false);
                var subRectangle2 = new NavigationTile(new VertexInt(tile1.Left, tile2.Bottom + 1), tile1.BottomRight, false);

                return new NavigationTile[] { subRectangle1, subRectangle2 };
            }

            // N | S
            else if (overlap.Has(Compass.N) &&
                     overlap.Has(Compass.S))
            {
                var subRectangle1 = new NavigationTile(tile1.TopLeft, new VertexInt(tile1.Right, tile2.Top - 1), false);
                var subRectangle2 = new NavigationTile(new VertexInt(tile1.Left, tile2.Bottom + 1), tile1.BottomRight, false);

                return new NavigationTile[] { subRectangle1, subRectangle2 };
            }

            // W | E
            else if (overlap.Has(Compass.W) &&
                     overlap.Has(Compass.E))
            {
                var subRectangle1 = new NavigationTile(tile1.TopLeft, new VertexInt(tile2.Left - 1, tile1.Bottom), false);
                var subRectangle2 = new NavigationTile(new VertexInt(tile2.Right + 1, tile1.Top), tile1.BottomRight, false);

                return new NavigationTile[] { subRectangle1, subRectangle2 };
            }

            // N
            else if (overlap.Has(Compass.N))
            {
                var subRectangle1 = new NavigationTile(tile1.TopLeft, new VertexInt(tile1.Right, tile2.Top - 1), false);

                return new NavigationTile[] { subRectangle1 };
            }

            // S
            else if (overlap.Has(Compass.S))
            {
                var subRectangle1 = new NavigationTile(new VertexInt(tile1.Left, tile2.Bottom + 1), tile1.BottomRight, false);

                return new NavigationTile[] { subRectangle1 };
            }

            // E
            else if (overlap.Has(Compass.E))
            {
                var subRectangle1 = new NavigationTile(new VertexInt(tile2.Right + 1, tile1.Top), tile1.BottomRight, false);

                return new NavigationTile[] { subRectangle1 };
            }

            // W
            else if (overlap.Has(Compass.W))
            {
                var subRectangle1 = new NavigationTile(tile1.TopLeft, new VertexInt(tile2.Left - 1, tile1.Bottom), false);

                return new NavigationTile[] { subRectangle1 };
            }

            // Null -> Rectangle 2 Fully Contains Rectangle 1
            else if (overlap == Compass.Null)
            {
                throw new Exception("Trying to subdivide a rectangle withing the containing rectangle");
            }

            else
                throw new Exception("Unhandled Rectangle Overlap RectangularTiling");
        }

        /// <summary>
        /// Calculates the connection point between two adjacent tiles
        /// </summary>
        private NavigationTileConnectionPoint CalculateConnectionPoint(NavigationTile tile, NavigationTile adjacentTile, out NavigationTileConnectionPoint adjacentTileConnectionPoint)
        {
            // N
            if (adjacentTile.Bottom == tile.Top - 1)
            {
                var left = System.Math.Max(adjacentTile.Left, tile.Left);
                var right = System.Math.Min(adjacentTile.Right, tile.Right);
                var connectingColumn = _randomSequenceGenerator.Get(left, right + 1);

                adjacentTileConnectionPoint = new NavigationTileConnectionPoint(tile, new VertexInt(connectingColumn, adjacentTile.Bottom), Compass.S);

                return new NavigationTileConnectionPoint(adjacentTile, new VertexInt(connectingColumn, tile.Top), Compass.N);
            }

            // S
            else if (adjacentTile.Top == tile.Bottom + 1)
            {
                var left = System.Math.Max(adjacentTile.Left, tile.Left);
                var right = System.Math.Min(adjacentTile.Right, tile.Right);
                var connectingColumn = _randomSequenceGenerator.Get(left, right + 1);

                adjacentTileConnectionPoint = new NavigationTileConnectionPoint(tile, new VertexInt(connectingColumn, adjacentTile.Top), Compass.N);

                return new NavigationTileConnectionPoint(adjacentTile, new VertexInt(connectingColumn, tile.Bottom), Compass.S);
            }

            // E
            else if (adjacentTile.Left == tile.Right + 1)
            {
                var top = System.Math.Max(adjacentTile.Top, tile.Top);
                var bottom = System.Math.Min(adjacentTile.Bottom, tile.Bottom);
                var connectingRow = _randomSequenceGenerator.Get(top, bottom + 1);

                adjacentTileConnectionPoint = new NavigationTileConnectionPoint(tile, new VertexInt(adjacentTile.Left, connectingRow), Compass.W);

                return new NavigationTileConnectionPoint(adjacentTile, new VertexInt(tile.Right, connectingRow), Compass.E);
            }

            // W
            else if (adjacentTile.Right == tile.Left - 1)
            {
                var top = System.Math.Max(adjacentTile.Top, tile.Top);
                var bottom = System.Math.Min(adjacentTile.Bottom, tile.Bottom);
                var connectingRow = _randomSequenceGenerator.Get(top, bottom + 1);

                adjacentTileConnectionPoint = new NavigationTileConnectionPoint(tile, new VertexInt(adjacentTile.Right, connectingRow), Compass.E);

                return new NavigationTileConnectionPoint(adjacentTile, new VertexInt(tile.Left, connectingRow), Compass.W);
            }
            else
                throw new Exception("Improperly defined adjacent tiles NavigationTiling.CalculateConnectionPoint");
        }
    }
}
