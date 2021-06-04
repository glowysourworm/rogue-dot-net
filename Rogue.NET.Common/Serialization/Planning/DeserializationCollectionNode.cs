using Rogue.NET.Common.Serialization.Target;

using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Planning
{
    /// <summary>
    /// Supports both collection properties and child collection (elements)
    /// </summary>
    internal class DeserializationCollectionNode : DeserializationNode
    {
        public List<DeserializationNodeBase> Children { get; private set; }

        public DeserializationCollectionNode(DeserializationCollection nodeObject, PropertyDefinition definition) : base(nodeObject, definition)
        {
            this.Children = new List<DeserializationNodeBase>();
        }
    }
}
