using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using System;
using System.Runtime.Serialization;

using static Rogue.NET.Core.Math.Geometry.Metric;

namespace Rogue.NET.Core.Math.Geometry
{
    [Serializable]
    public class GraphVertex : IGridLocator
    {
        public string ReferenceId { get; private set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public MetricType Type { get; set; }

        public GraphVertex(string referenceId, int column, int row, MetricType type)
        {
            this.ReferenceId = referenceId;
            this.Column = column;
            this.Row = row;
            this.Type = type;
        }

        public GraphVertex(SerializationInfo info, StreamingContext context)
        {
            this.ReferenceId = info.GetString("ReferenceId");
            this.Column = info.GetInt32("Column");
            this.Row = info.GetInt32("Row");
            this.Type = (MetricType)info.GetValue("Type", typeof(MetricType));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ReferenceId", this.ReferenceId);
            info.AddValue("Column", this.Column);
            info.AddValue("Row", this.Row);
            info.AddValue("Type", this.Type);
        }

        public override bool Equals(object obj)
        {
            if (obj is GraphVertex)
            {
                var vertex = obj as GraphVertex;

                return vertex.Column == this.Column &&
                       vertex.Row == this.Row &&
                       vertex.ReferenceId == this.ReferenceId &&
                       vertex.Type == this.Type;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var hash = 397;

            hash = (17 * hash) + this.Column.GetHashCode();
            hash = (17 * hash) + this.Row.GetHashCode();
            hash = (17 * hash) + this.ReferenceId.GetHashCode();
            hash = (17 * hash) + this.Type.GetHashCode();

            return hash;
        }

        public override string ToString()
        {
            return string.Format("Column={0} Row={1}", this.Column, this.Row);
        }
    }
}
