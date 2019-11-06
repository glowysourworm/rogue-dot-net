using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Math.Geometry
{
    public struct VertexInt
    {
        public int X { get; set; }
        public int Y { get; set; }

        public VertexInt(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public VertexInt(GridLocation location)
        {
            this.X = location.Column;
            this.Y = location.Row;
        }
        public VertexInt(Vertex vertex)
        {
            this.X = (int)vertex.X;
            this.Y = (int)vertex.Y;
        }

        /// <summary>
        /// Returns a new vertex with the values subtracted from the X and Y coordinates
        /// </summary>
        public VertexInt Subtract(int deltaX, int deltaY)
        {
            return new VertexInt(this.X - deltaX, this.Y - deltaY);
        }

        public static bool operator ==(VertexInt v1, VertexInt v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(VertexInt v1, VertexInt v2)
        {
            return !v1.Equals(v2);
        }

        public override bool Equals(object obj)
        {
            if (obj is VertexInt)
            {
                var vertex = (VertexInt)obj;

                return vertex.X == this.X && vertex.Y == this.Y;
            }
            else
                throw new Exception("Unhandled case VertexInt.Equals");
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "x=" + this.X.ToString() + "  y=" + this.Y.ToString();
        }
    }
}
