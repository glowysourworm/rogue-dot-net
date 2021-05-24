using Rogue.NET.Common.Serialization.Target;

using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Planning
{
    /// <summary>
    /// Supports just the collection as nodes. NO OTHER PROPERTY SUPPORT WITH SUB-NODES!
    /// </summary>
    internal class SerializationCollectionNode : SerializationNodeBase
    {
        public List<SerializationNodeBase> Children { get; private set; }

        public SerializationCollectionNode(SerializationObjectBase nodeObject) : base(nodeObject)
        {
            this.Children = new List<SerializationNodeBase>();
        }
    }
}
