using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;

using System;

using static Rogue.NET.Core.Math.Geometry.Metric;

namespace Rogue.NET.Core.Math.Geometry
{
    [Serializable]
    public class GraphVertex : IGridLocator, IGraphNode
    {
        public string ReferenceId { get; private set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public MetricType Type { get; set; }

        // IGraphNode
        public int Hash { get { return this.GetHashCode(); } }

        public GraphVertex(string referenceId, int column, int row, MetricType type)
        {
            this.ReferenceId = referenceId;
            this.Column = column;
            this.Row = row;
            this.Type = type;
        }

        public GraphVertex(IPropertyReader reader)
        {
            this.ReferenceId = reader.Read<string>("ReferenceId");
            this.Column = reader.Read<int>("Column");
            this.Row = reader.Read<int>("Row");
            this.Type = reader.Read<MetricType>("Type");
        }

        public void GetProperties(IPropertyWriter writer)
        {
            writer.Write("ReferenceId", this.ReferenceId);
            writer.Write("Column", this.Column);
            writer.Write("Row", this.Row);
            writer.Write("Type", this.Type);
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
            return this.CreateHashCode(this.Column,
                                       this.Row,
                                       this.ReferenceId,
                                       this.Type);
        }

        public override string ToString()
        {
            return string.Format("Column={0} Row={1}", this.Column, this.Row);
        }
    }
}
