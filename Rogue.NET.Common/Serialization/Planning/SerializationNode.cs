using Rogue.NET.Common.Serialization.Target;

using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal class SerializationNode : SerializationNodeBase
    {
        public List<SerializationNodeBase> SubNodes { get; private set; }

        public SerializationNode(SerializationObjectBase nodeObject) : base(nodeObject)
        {
            this.SubNodes = new List<SerializationNodeBase>();
        }
    }
}
