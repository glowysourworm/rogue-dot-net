using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System.Runtime.Serialization;

namespace Rogue.NET.Core.Math.Geometry
{
    public class GraphVertex<T> : IGridLocator where T : class
    {
        /// <summary>
        /// The reference object for the vertex
        /// </summary>
        public T Reference { get; private set; }

        public int Column { get; set; }
        public int Row { get; set; }

        public GraphVertex(T reference, int column, int row)
        {
            this.Reference = reference;
            this.Column = column;
            this.Row = row;
        }

        public override bool Equals(object obj)
        {
            if (obj is GraphVertex<T>)
            {
                var vertex = obj as GraphVertex<T>;

                return vertex.Column == this.Column &&
                       vertex.Row == this.Row &&
                       vertex.Reference == this.Reference;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var hash = 397;

            hash = (17 * hash) + this.Column.GetHashCode();
            hash = (17 * hash) + this.Row.GetHashCode();
            hash = (17 * hash) + this.Reference.GetHashCode();

            return hash;
        }

        public override string ToString()
        {
            return string.Format("Column={0} Row={1}", this.Column, this.Row);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
