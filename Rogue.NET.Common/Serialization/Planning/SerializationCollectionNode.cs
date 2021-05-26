using Rogue.NET.Common.Serialization.Target;

using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Planning
{
    /// <summary>
    /// Supports both collection properties and child collection (elements)
    /// </summary>
    internal class SerializationCollectionNode : SerializationNode
    {
        public List<SerializationNodeBase> Children { get; private set; }

        public SerializationCollectionNode(SerializationObjectBase nodeObject) : base(nodeObject)
        {
            this.Children = new List<SerializationNodeBase>();
        }
    }
}
