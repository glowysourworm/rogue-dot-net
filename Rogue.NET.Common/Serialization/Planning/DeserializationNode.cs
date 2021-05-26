using Rogue.NET.Common.Serialization.Target;

using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal class DeserializationNode : DeserializationNodeBase
    {
        public List<DeserializationNodeBase> SubNodes { get; private set; }

        public DeserializationNode(DeserializationObjectBase nodeObject, PropertyDefinition definition) : base(nodeObject, definition)
        {
            this.SubNodes = new List<DeserializationNodeBase>();
        }
    }
}
