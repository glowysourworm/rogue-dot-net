using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Target;

using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal class SerializationPlan : ISerializationPlan
    {
        public SerializationNode RootNode { get; private set; }

        public IDictionary<HashedObjectInfo, SerializationNodeBase> UniqueReferenceDict { get; private set; }

        public IDictionary<HashedType, HashedType> TypeDict { get; private set; }

        public SerializationPlan(IDictionary<HashedObjectInfo, SerializationNodeBase> referenceDict,
                                 IDictionary<HashedType, HashedType> typeDict,
                                 SerializationNode rootNode)
        {
            this.UniqueReferenceDict = referenceDict;
            this.TypeDict = typeDict;
            this.RootNode = rootNode;
        }
    }
}
