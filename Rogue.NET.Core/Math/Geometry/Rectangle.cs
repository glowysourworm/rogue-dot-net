using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using System.Collections.Generic;

namespace Rogue.NET.Core.Math.Geometry
{
    public class Rectangle
    {
        public Vertex TopLeft { get; private set; }
        public Vertex TopRight { get; private set; }
        public Vertex BottomRight { get; private set; }
        public Vertex BottomLeft { get; private set; }

        public double Left { get { return this.TopLeft.X; } }
        public double Top { get { return this.TopLeft.Y; } }
        public double Right { get { return this.TopRight.X; } }
        public double Bottom { get { return this.BottomLeft.Y; } }

        public double Width { get { return this.Right - this.Left; } }
        public double Height { get { return this.Bottom - this.Top; } }

        public Rectangle(Vertex topLeft, Vertex bottomRight)
        {
            this.TopLeft = topLeft;
            this.TopRight = new Vertex(bottomRight.X, topLeft.Y);
            this.BottomLeft = new Vertex(topLeft.X, bottomRight.Y);
            this.BottomRight = bottomRight;
        }

        /// <summary>
        /// Constructs a super-rectangle from the point list. This rectangle will contain all the points in the list.
        /// </summary>
        public Rectangle(IEnumerable<Vertex> points)
        {
            var left = double.MaxValue;
            var right = double.MinValue;
            var top = double.MaxValue;
            var bottom = double.MinValue;

            foreach (var point in points)
            {
                if (point.X < left)
                    left = point.X;

                if (point.X > right)
                    right = point.X;

                if (point.Y < top)
                    top = point.Y;

                if (point.Y > bottom)
                    bottom = point.Y;
            }

            this.TopLeft = new Vertex(left, top);
            this.TopRight = new Vertex(right, top);
            this.BottomRight = new Vertex(right, bottom);
            this.BottomLeft = new Vertex(left, bottom);
        }

        /// <summary>
        /// Calculates intersection - excluding edges and vertices.
        /// </summary>
        public bool Intersects(Rectangle rectangle)
        {
            if (rectangle.Left > this.Right)
                return false;

            if (rectangle.Right < this.Left)
                return false;

            if (rectangle.Top > this.Bottom)
                return false;

            if (rectangle.Bottom < this.Top)
                return false;

            return true;
        }

        public bool Contains(Rectangle rectangle, bool allowEdgeOverlap)
        {
            if (!allowEdgeOverlap)
            {
                if (rectangle.Right >= this.Right)
                    return false;

                if (rectangle.Left <= this.Left)
                    return false;

                if (rectangle.Top <= this.Top)
                    return false;

                if (rectangle.Bottom >= this.Bottom)
                    return false;
            }
            else
            {
                if (rectangle.Right > this.Right)
                    return false;

                if (rectangle.Left < this.Left)
                    return false;

                if (rectangle.Top < this.Top)
                    return false;

                if (rectangle.Bottom > this.Bottom)
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            return "x=" + this.Left.ToString("F3") + 
                   " y=" + this.Top.ToString("F3") +
                   " width=" + this.Width.ToString("F3") + 
                   " height=" + this.Height.ToString("F3");
        }
    }
}
