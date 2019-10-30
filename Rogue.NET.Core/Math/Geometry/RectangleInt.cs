namespace Rogue.NET.Core.Math.Geometry
{
    public class RectangleInt
    {
        public VertexInt TopLeft { get; private set; }
        public VertexInt TopRight { get; private set; }
        public VertexInt BottomRight { get; private set; }
        public VertexInt BottomLeft { get; private set; }

        public int Left { get { return this.TopLeft.X; } }
        public int Top { get { return this.TopLeft.Y; } }
        public int Right { get { return this.TopRight.X; } }
        public int Bottom { get { return this.BottomLeft.Y; } }

        public int Width { get { return this.Right - this.Left + 1; } }
        public int Height { get { return this.Bottom - this.Top + 1; } }

        public RectangleInt(VertexInt topLeft, VertexInt bottomRight)
        {
            this.TopLeft = topLeft;
            this.TopRight = new VertexInt(bottomRight.X, topLeft.Y);
            this.BottomLeft = new VertexInt(topLeft.X, bottomRight.Y);
            this.BottomRight = bottomRight;
        }

        /// <summary>
        /// Calculates intersection - excluding edges and vertices.
        /// </summary>
        public bool Intersects(RectangleInt rectangle)
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

        public bool Contains(RectangleInt rectangle)
        {

            if (rectangle.Right > this.Right)
                return false;

            if (rectangle.Left < this.Left)
                return false;

            if (rectangle.Top < this.Top)
                return false;

            if (rectangle.Bottom > this.Bottom)
                return false;

            return true;
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
