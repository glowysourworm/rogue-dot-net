using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Region;
using System;
using System.Collections.Generic;
using System.Linq;
using static Rogue.NET.Core.Math.Geometry.Metric;

namespace Rogue.NET.Core.Math.Geometry
{
    public class NavigationTile : IGraphWeightProvider<NavigationTile>
    {
        List<NavigationTileConnectionPoint> _connectionPoints;
        NavigationTileConnectionRoute _connectionRoute;

        public IEnumerable<NavigationTileConnectionPoint> ConnectionPoints
        {
            get { return _connectionPoints; }
        }

        public NavigationTileConnectionRoute ConnectionRoute
        {
            get { return _connectionRoute; }
        }

        public VertexInt TopLeft { get; private set; }
        public VertexInt TopRight { get; private set; }
        public VertexInt BottomRight { get; private set; }
        public VertexInt BottomLeft { get; private set; }
        public VertexInt Center { get; private set; }

        public int Left { get { return this.TopLeft.X; } }
        public int Top { get { return this.TopLeft.Y; } }
        public int Right { get { return this.TopRight.X; } }
        public int Bottom { get { return this.BottomLeft.Y; } }

        public int Width { get { return this.Right - this.Left + 1; } }
        public int Height { get { return this.Bottom - this.Top + 1; } }

        public bool IsRegion { get; private set; }

        /// <summary>
        /// Gets or sets a flag for whether or not this tile is to be used in routing for the level
        /// </summary>
        public bool IsMarkedForRouting { get; set; }

        public NavigationTile(VertexInt topLeft, VertexInt bottomRight, bool isRegion)
        {
            _connectionPoints = new List<NavigationTileConnectionPoint>();
            _connectionRoute = new NavigationTileConnectionRoute(this);

            this.TopLeft = topLeft;
            this.TopRight = new VertexInt(bottomRight.X, topLeft.Y);
            this.BottomLeft = new VertexInt(topLeft.X, bottomRight.Y);
            this.BottomRight = bottomRight;
            this.Center = new VertexInt((int)((topLeft.X + bottomRight.X) / 2.0), (int)((topLeft.Y + bottomRight.Y) / 2.0));

            this.IsRegion = isRegion;
        }

        public void AddConnection(NavigationTileConnectionPoint connection)
        {
            _connectionPoints.Add(connection);
        }

        /// <summary>
        /// Creates route for all marked connections (IsRouted = true). The results are stored in the connection
        /// routes until called for to create corridor cells.
        /// </summary>
        public void Route()
        {
            if (!this.IsMarkedForRouting)
                throw new Exception("Trying to route un-marked tile");

            if (this.ConnectionPoints.Count(point => point.AdjacentTile.IsMarkedForRouting) < 2)
                throw new Exception("Trying to create a route for a tile with less than 2 connection points that are marked for routing");

            // Procedure
            //
            // 1) Identify the most maximal connection locations on the tile. This will form an
            //    inner rectangle inside the tile. 
            //
            //    If the resulting rectangle is mal-formed because the points are collinear or 
            //    there is only point in one of the x or y directions - then see the sub-cases.
            //
            // 2) In each of the x and y directions 
            //      - Pick one of the two maximal points. If there is only one - just use that one.
            //      - These points will be the primary points for the "bus" that will connect the tile's 
            //        connection points together with the adjacent tile. 
            //      - THERE SHOULD BE ONLY TWO OF THESE POINTS. 
            //
            // 3) If the x-direction's "inner rectangle size" is greater than the y-direction's "inner rectangle size",
            //    Then start with the x-dimension - and "draw a line" from that connection point to the outer-most 
            //    y-direction point. Then, turn - and "draw a line" to the second point (the y-direction's point)
            //
            //    The line drawn represents the smallest (probably the smallest) L-Shape that will intersect ALL of 
            //    the connection points' coordinates.
            //
            // 4) Connect each of the connection points to the "Bus" - STAYING OUTSIDE THE INNER RECTANGLE. This will
            //    ensure that the "bus" acts as the bus - or primary route through the tile. Staying outside the inner
            //    rectangle will ensure that the routes are connected without un-intended overlaps.
            //

            // Get involved connection points
            var points = this.ConnectionPoints
                             .Where(point => point.AdjacentTile.IsMarkedForRouting)
                             .Actualize();

            // Calculate x-direction extrema
            var minX = points.Min(point => point.ConnectionPoint.X);
            var maxX = points.Max(point => point.ConnectionPoint.X);

            // Calculate y-direction extrema
            var minY = points.Min(point => point.ConnectionPoint.Y);
            var maxY = points.Max(point => point.ConnectionPoint.Y);

            // Calculate N | S Edge x-direction points
            var minXPoint = points.Where(point => point.Direction == Compass.N || point.Direction == Compass.S)
                             .MinBy(point => point.ConnectionPoint.X);
            var maxXPoint = points.Where(point => point.Direction == Compass.N || point.Direction == Compass.S)
                             .MaxBy(point => point.ConnectionPoint.X);

            // Calculate E | W Edge y-direction points
            var minYPoint = points.Where(point => point.Direction == Compass.E || point.Direction == Compass.W)
                             .MinBy(point => point.ConnectionPoint.Y);
            var maxYPoint = points.Where(point => point.Direction == Compass.E || point.Direction == Compass.W)
                             .MaxBy(point => point.ConnectionPoint.Y);

            // Cases:  
            //
            // 1) Points are located on a single edge
            // 2) A single point on 2 edges
            // 3) At least 2 points in ONE dimension; but only 1 point in the OTHER dimension
            // 4) Enough data to construct an "inner rectangle"
            //

            // Case 1:  Points are all located on a single edge
            if (points.All(point => point.ConnectionPoint.X == points.First().ConnectionPoint.X))
            {
                // First, create the primary bus
                CreateRectilinearRoutePoints(minYPoint.ConnectionPoint, this.Center, false);
                CreateRectilinearRoutePoints(maxYPoint.ConnectionPoint, this.Center, false);
                CreateRectilinearRoutePoints(new VertexInt(this.Center.X, minYPoint.ConnectionPoint.Y),
                                             new VertexInt(this.Center.X, maxYPoint.ConnectionPoint.Y),
                                             true);

                // Then, connect any other points to the bus
                foreach (var point in points)
                {
                    if (point != minYPoint && point != maxYPoint)
                        CreateRectilinearRoutePoints(point.ConnectionPoint, this.Center, false);
                }
            }
            else if (points.All(point => point.ConnectionPoint.Y == points.First().ConnectionPoint.Y))
            {
                // First, create the primary bus
                CreateRectilinearRoutePoints(minXPoint.ConnectionPoint, this.Center, true);
                CreateRectilinearRoutePoints(maxXPoint.ConnectionPoint, this.Center, true);
                CreateRectilinearRoutePoints(new VertexInt(minXPoint.ConnectionPoint.X, this.Center.Y),
                                             new VertexInt(maxXPoint.ConnectionPoint.X, this.Center.Y),
                                             false);

                // Then, connect any other points to the bus
                foreach (var point in points)
                {
                    if (point != minXPoint && point != maxXPoint)
                        CreateRectilinearRoutePoints(point.ConnectionPoint, this.Center, true);
                }
            }

            // Case 2:  A single point on two separate edges (non-colinear). This condition is sufficient because the 
            //          colinear part has been ruled out.
            else if (points.Count() == 2)
            {
                var point1 = points.ElementAt(0);
                var point2 = points.ElementAt(1);

                var isYOriented = point1.Direction == Compass.N || point1.Direction == Compass.S;

                // Points are on opposing sides
                if (point1.Direction == GridUtility.GetOppositeDirection(point2.Direction))
                {
                    if (isYOriented)
                    {
                        CreateRectilinearRoutePoints(point1.ConnectionPoint, this.Center, true);
                        CreateRectilinearRoutePoints(new VertexInt(point1.ConnectionPoint.X, this.Center.Y),
                                                     new VertexInt(point2.ConnectionPoint.X, this.Center.Y), false);
                        CreateRectilinearRoutePoints(new VertexInt(point2.ConnectionPoint.X, this.Center.Y), point2.ConnectionPoint, true);
                    }
                    else
                    {
                        CreateRectilinearRoutePoints(point1.ConnectionPoint, this.Center, false);
                        CreateRectilinearRoutePoints(new VertexInt(this.Center.X, point1.ConnectionPoint.Y),
                                                     new VertexInt(this.Center.X, point2.ConnectionPoint.Y), true);
                        CreateRectilinearRoutePoints(new VertexInt(this.Center.X, point2.ConnectionPoint.Y), point2.ConnectionPoint, false);
                    }
                }
                // Points are on adjacent sides
                else
                {
                    if (isYOriented)
                    {
                        CreateRectilinearRoutePoints(point1.ConnectionPoint, new VertexInt(point1.ConnectionPoint.X, point2.ConnectionPoint.Y), true);
                        CreateRectilinearRoutePoints(new VertexInt(point1.ConnectionPoint.X, point2.ConnectionPoint.Y), point2.ConnectionPoint, false);
                    }
                    else
                    {
                        CreateRectilinearRoutePoints(point1.ConnectionPoint, new VertexInt(point2.ConnectionPoint.X, point1.ConnectionPoint.Y), false);
                        CreateRectilinearRoutePoints(new VertexInt(point2.ConnectionPoint.X, point1.ConnectionPoint.Y), point2.ConnectionPoint, true);
                    }
                }
            }

            // Case 3:  At least 2 points in ONE dimension; but only 1 point in the OTHER dimension
            else if (minX == maxX &&
                     minY != maxY)
            {
                // Create bus from the single point to the extrema in the other dimension
                if (minXPoint.Direction == Compass.N)
                    CreateRectilinearRoutePoints(minXPoint.ConnectionPoint, maxYPoint.ConnectionPoint, true);

                else if (minXPoint.Direction == Compass.S)
                    CreateRectilinearRoutePoints(minXPoint.ConnectionPoint, minYPoint.ConnectionPoint, true);

                else
                    throw new Exception("Improperly handled case NavigationTile.Route");

                // Connect the rest of the points to the bus
                foreach (var point in points)
                {
                    if (point != minXPoint)
                        CreateRectilinearRoutePoints(point.ConnectionPoint, new VertexInt(point.ConnectionPoint.X, minXPoint.ConnectionPoint.Y), false);
                }
            }
            else if (minY == maxY &&
                     minX != maxX)
            {
                // Create bus from the single point to the extrema in the other dimension
                if (minYPoint.Direction == Compass.E)
                    CreateRectilinearRoutePoints(minYPoint.ConnectionPoint, minXPoint.ConnectionPoint, false);

                else if (minYPoint.Direction == Compass.W)
                    CreateRectilinearRoutePoints(minYPoint.ConnectionPoint, maxXPoint.ConnectionPoint, false);

                else
                    throw new Exception("Improperly handled case NavigationTile.Route");

                // Connect the rest of the points to the bus
                foreach (var point in points)
                {
                    if (point != minYPoint)
                        CreateRectilinearRoutePoints(point.ConnectionPoint, new VertexInt(point.ConnectionPoint.X, minYPoint.ConnectionPoint.Y), true);
                }
            }

            // Case 4: Enough data to construct an "inner rectangle" (at least two distinct points in each dimension)
            else
            {
                // X-Dimension of the bus lies on the north edge
                if (minXPoint.Direction == Compass.N)
                {
                    // Y-Dimension of the bus lies on the east edge
                    if (maxYPoint.Direction == Compass.E)
                    {
                        // Procedure N | E
                        //
                        // 1) Draw from minX -> maxX along the north edge
                        // 2) Draw from maxX -> maxY along the east edge
                        // 3) Connect up the other points

                        // First, draw the L-Shape with the extra connection from minX to the bus
                        var busPoint1 = new VertexInt(minX, minY);
                        var busPoint2 = new VertexInt(maxX, minY);
                        var busPoint3 = new VertexInt(maxX, maxY);

                        CreateRectilinearRoutePoints(minXPoint.ConnectionPoint, busPoint1, true);
                        CreateRectilinearRoutePoints(busPoint1, busPoint2, false);
                        CreateRectilinearRoutePoints(busPoint2, busPoint3, true);

                        // Connect each of the other points to the bus
                        foreach (var point in points)
                        {
                            // North edge - links straight to the bus
                            if (point.Direction == Compass.N)
                                CreateRectilinearRoutePoints(point.ConnectionPoint, new VertexInt(point.ConnectionPoint.X, minY), true);

                            // South edge - follow the inner rectangle east to the bus
                            else if (point.Direction == Compass.S)
                            {
                                var innerRectanglePoint = new VertexInt(point.ConnectionPoint.X, maxY);

                                CreateRectilinearRoutePoints(point.ConnectionPoint, innerRectanglePoint, true);
                                CreateRectilinearRoutePoints(innerRectanglePoint, busPoint3, false);
                            }

                            // East edge - links straight to the bus
                            else if (point.Direction == Compass.E)
                                CreateRectilinearRoutePoints(point.ConnectionPoint, new VertexInt(maxX, point.ConnectionPoint.Y), false);

                            // West edge - follow inner rectangle south to the bus
                            else
                            {
                                var innerRectanglePoint = new VertexInt(minX, point.ConnectionPoint.Y);

                                CreateRectilinearRoutePoints(point.ConnectionPoint, innerRectanglePoint, false);
                                CreateRectilinearRoutePoints(innerRectanglePoint, busPoint1, true);
                            }
                        }
                    }

                    // Y-Dimension of the bus lies on the west edge
                    else
                    {
                        // Procedure N | W
                        //
                        // 1) Draw from maxX -> minX along the north edge
                        // 2) Draw from minX -> maxY along the west edge
                        // 3) Connect up the other points

                        // First, draw the L-Shape with the extra connection from maxX to the bus
                        var busPoint1 = new VertexInt(maxX, minY);
                        var busPoint2 = new VertexInt(minX, minY);
                        var busPoint3 = new VertexInt(minX, maxY);

                        CreateRectilinearRoutePoints(maxXPoint.ConnectionPoint, busPoint1, true);
                        CreateRectilinearRoutePoints(busPoint1, busPoint2, false);
                        CreateRectilinearRoutePoints(busPoint2, busPoint3, true);

                        // Connect each of the other points to the bus
                        foreach (var point in points)
                        {
                            // North edge - links straight to the bus
                            if (point.Direction == Compass.N)
                                CreateRectilinearRoutePoints(point.ConnectionPoint, new VertexInt(point.ConnectionPoint.X, minYPoint.ConnectionPoint.Y), true);

                            // South edge - follow the inner rectangle west to the bus
                            else if (point.Direction == Compass.S)
                            {
                                var innerRectanglePoint = new VertexInt(point.ConnectionPoint.X, maxY);

                                CreateRectilinearRoutePoints(point.ConnectionPoint, innerRectanglePoint, true);
                                CreateRectilinearRoutePoints(innerRectanglePoint, busPoint3, false);
                            }

                            // East edge - follow inner rectangle north to the bus
                            else if (point.Direction == Compass.E)
                            {
                                var innerRectanglePoint = new VertexInt(maxX, point.ConnectionPoint.Y);

                                CreateRectilinearRoutePoints(point.ConnectionPoint, innerRectanglePoint, false);
                                CreateRectilinearRoutePoints(innerRectanglePoint, busPoint1, true);
                            }

                            // West edge - links straight to the bus
                            else
                                CreateRectilinearRoutePoints(point.ConnectionPoint, new VertexInt(minX, point.ConnectionPoint.Y), false);

                        }
                    }
                }

                // X-Dimension of the bus lies on the south edge 
                else if (minXPoint.Direction == Compass.S)
                {
                    // Y-Dimension of the bus lies on the east edge
                    if (minYPoint.Direction == Compass.E)
                    {
                        // Procedure S | E
                        //
                        // 1) Draw from minX -> maxX along the north edge
                        // 2) Draw from maxX -> minY along the east edge
                        // 3) Connect up the other points

                        // First, draw the L-Shape with the extra connection from minX to the bus
                        var busPoint1 = new VertexInt(minX, maxY);
                        var busPoint2 = new VertexInt(maxX, maxY);
                        var busPoint3 = new VertexInt(maxX, minY);

                        CreateRectilinearRoutePoints(minXPoint.ConnectionPoint, busPoint1, true);
                        CreateRectilinearRoutePoints(busPoint1, busPoint2, false);
                        CreateRectilinearRoutePoints(busPoint2, busPoint3, true);

                        // Connect each of the other points to the bus
                        foreach (var point in points)
                        {
                            // North edge - follow the inner rectangle east to the bus
                            if (point.Direction == Compass.N)
                            {
                                var innerRectanglePoint = new VertexInt(point.ConnectionPoint.X, minY);

                                CreateRectilinearRoutePoints(point.ConnectionPoint, innerRectanglePoint, true);
                                CreateRectilinearRoutePoints(innerRectanglePoint, busPoint3, false);
                            }

                            // South edge - links directly to the bus
                            else if (point.Direction == Compass.S)
                                CreateRectilinearRoutePoints(point.ConnectionPoint, new VertexInt(point.ConnectionPoint.X, maxY), true);

                            // East edge - links straight to the bus
                            else if (point.Direction == Compass.E)
                                CreateRectilinearRoutePoints(point.ConnectionPoint, new VertexInt(maxX, point.ConnectionPoint.Y), false);

                            // West edge - follow inner rectangle south to the bus
                            else
                            {
                                var innerRectanglePoint = new VertexInt(minXPoint.ConnectionPoint.X, point.ConnectionPoint.Y);

                                CreateRectilinearRoutePoints(point.ConnectionPoint, innerRectanglePoint, false);
                                CreateRectilinearRoutePoints(innerRectanglePoint, busPoint1, true);
                            }
                        }
                    }

                    // Y-Dimension of the bus lies on the west edge
                    else
                    {
                        // Procedure S | W
                        //
                        // 1) Draw from maxX -> minX along the north edge
                        // 2) Draw from minX -> maxY along the west edge
                        // 3) Connect up the other points

                        // First, draw the L-Shape with the extra connection from maxX to the bus
                        var busPoint1 = new VertexInt(maxX, maxY);
                        var busPoint2 = new VertexInt(minX, maxY);
                        var busPoint3 = new VertexInt(minX, minY);

                        CreateRectilinearRoutePoints(maxXPoint.ConnectionPoint, busPoint1, true);
                        CreateRectilinearRoutePoints(busPoint1, busPoint2, false);
                        CreateRectilinearRoutePoints(busPoint2, busPoint3, true);

                        // Connect each of the other points to the bus
                        foreach (var point in points)
                        {
                            // North edge - follow the inner rectangle west to the bus
                            if (point.Direction == Compass.N)
                            {
                                var innerRectanglePoint = new VertexInt(point.ConnectionPoint.X, minY);

                                CreateRectilinearRoutePoints(point.ConnectionPoint, innerRectanglePoint, true);
                                CreateRectilinearRoutePoints(innerRectanglePoint, busPoint3, false);
                            }
                            
                            // South edge - links directly to the bus
                            else if (point.Direction == Compass.S)
                                CreateRectilinearRoutePoints(point.ConnectionPoint, new VertexInt(point.ConnectionPoint.X, maxY), true);

                            // East edge - follow inner rectangle south to the bus
                            else if (point.Direction == Compass.E)
                            {
                                var innerRectanglePoint = new VertexInt(maxX, point.ConnectionPoint.Y);

                                CreateRectilinearRoutePoints(point.ConnectionPoint, innerRectanglePoint, false);
                                CreateRectilinearRoutePoints(innerRectanglePoint, busPoint1, true);
                            }

                            // West edge - links straight to the bus
                            else
                                CreateRectilinearRoutePoints(point.ConnectionPoint, new VertexInt(minX, point.ConnectionPoint.Y), false);
                        }
                    }
                }

                // Mis-understood connections.. throw an exception
                else
                    throw new Exception("Mishandled connection routing NavigationTile.Route");
            }
        }

        /// <summary>
        /// Creates points in the navigation route starting at the specified point (AND INCLUDING) - continuing towards
        /// the end point in the specified direction UNTIL THEIR VALUES MATCH IN THE CORRESPONDING DIMENSION.
        /// </summary>
        private void CreateRectilinearRoutePoints(VertexInt startPoint, VertexInt endPoint, bool yDirection)
        {
            //if ((startPoint.X == endPoint.X) && !yDirection)
            //    throw new Exception("Improper corridor specification NavigationTile.CreateRectilinearRoutePoints");

            //if ((startPoint.Y == endPoint.Y) && yDirection)
            //    throw new Exception("Improper corridor specification NavigationTile.CreateRectilinearRoutePoints");

            // Create follower for the loop
            var pointValue = yDirection ? startPoint.Y : startPoint.X;

            // Get the total count for iteration
            var count = System.Math.Abs(yDirection ? (endPoint.Y - startPoint.Y) : (endPoint.X - startPoint.X)) + 1;

            // Get the incrementing the value during iteration
            var increment = (yDirection ? (endPoint.Y - startPoint.Y) : (endPoint.X - startPoint.X)) > 0 ? 1 : -1;

            for (int i = 0; i < count; i++)
            {
                VertexInt vertex;

                if (yDirection)
                    vertex = new VertexInt(startPoint.X, pointValue);

                else
                    vertex = new VertexInt(pointValue, startPoint.Y);

                if (vertex.X > this.Right || vertex.X < this.Left)
                    throw new Exception("Trying to create point outside the bounds of the navigation tile");

                if (vertex.Y > this.Bottom || vertex.Y < this.Top)
                    throw new Exception("Trying to create point outside the bounds of the navigation tile");

                this.ConnectionRoute.IncludePoint(vertex);

                pointValue += increment;
            }
        }

        /// <summary>
        /// Calculates intersection - excluding edges and vertices.
        /// </summary>
        public bool Intersects(NavigationTile tile)
        {
            if (tile.Left > this.Right)
                return false;

            if (tile.Right < this.Left)
                return false;

            if (tile.Top > this.Bottom)
                return false;

            if (tile.Bottom < this.Top)
                return false;

            return true;
        }

        public bool Contains(NavigationTile tile)
        {
            if (tile.Right > this.Right)
                return false;

            if (tile.Left < this.Left)
                return false;

            if (tile.Top < this.Top)
                return false;

            if (tile.Bottom > this.Bottom)
                return false;

            return true;
        }

        public bool Borders(NavigationTile tile)
        {
            // N
            if (tile.Bottom + 1 == this.Top)
                return System.Math.Max(this.Left, tile.Left) <= this.Right &&
                       System.Math.Min(this.Right, tile.Right) >= this.Left;

            // S
            if (tile.Top - 1 == this.Bottom)
                return System.Math.Max(this.Left, tile.Left) <= this.Right &&
                       System.Math.Min(this.Right, tile.Right) >= this.Left;

            // E
            if (tile.Left - 1 == this.Right)
                return System.Math.Max(this.Top, tile.Top) <= this.Bottom &&
                       System.Math.Min(this.Bottom, tile.Bottom) >= this.Top;

            // W
            if (tile.Right + 1 == this.Left)
                return System.Math.Max(this.Top, tile.Top) <= this.Bottom &&
                       System.Math.Min(this.Bottom, tile.Bottom) >= this.Top;

            return false;
        }

        public double CalculateWeight(NavigationTile adjacentNode, Metric.MetricType metricType)
        {
            if (metricType != MetricType.Roguian)
                throw new Exception("Currently not supporting other metric types besides the Roguian metric NavigationTile.CalculateWeight");

            // These MUST be bordering tiles
            if (!this.Borders(adjacentNode))
                throw new Exception("Trying to calculate a graph weight for non-bordering nagivation tiles");

            var connectionPoint1 = this.ConnectionPoints.FirstOrDefault(point => point.AdjacentTile == adjacentNode);
            var connectionPoint2 = adjacentNode.ConnectionPoints.FirstOrDefault(point => point.AdjacentTile == this);

            // The nodes must already have calculated connections
            if (connectionPoint1 == null || connectionPoint2 == null)
                throw new Exception("Trying to calculate a graph weight for two non-connected nodes.");

            // Must handle route between tiles - which includes the path from the connection points to the center
            // as part of the graph weight
            //
            // The route should be node1 -> connectionPoint1 -> connectionPoint2 -> node2
            //
            return Metric.RoguianDistance(this.Center, connectionPoint1.ConnectionPoint) +
                   Metric.RoguianDistance(connectionPoint1.ConnectionPoint, connectionPoint2.ConnectionPoint) +
                   Metric.RoguianDistance(connectionPoint2.ConnectionPoint, adjacentNode.Center);
        }

        public override string ToString()
        {
            return "x=" + this.Left.ToString() +
                   " y=" + this.Top.ToString() +
                   " width=" + this.Width.ToString() +
                   " height=" + this.Height.ToString();
        }
    }
}
