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

        public override string ToString()
        {
            return "x=" + this.Left.ToString("F3") + 
                   " y=" + this.Top.ToString("F3") +
                   " width=" + ((this.Right - this.Left) + 1).ToString("F3") + 
                   " height=" + ((this.Bottom - this.Top) + 1).ToString("F3");
        }
    }
}
