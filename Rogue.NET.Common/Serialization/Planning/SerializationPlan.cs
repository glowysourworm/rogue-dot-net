using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Target;

using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal class SerializationPlan : ISerializationPlan
    {
        public SerializationNodeBase RootNode { get; private set; }

        public IDictionary<HashedObjectInfo, SerializationObjectBase> UniqueReferenceDict { get; private set; }

        public IEnumerable<SerializationObjectBase> AllSerializedObjects { get; private set; }

        public SerializationPlan(IDictionary<HashedObjectInfo, SerializationObjectBase> referenceDict,
                                 IDictionary<HashedType, HashedType> typeDict,
                                 IEnumerable<SerializationObjectBase> allObjects,
                                 SerializationNodeBase rootNode)
        {
            this.UniqueReferenceDict = referenceDict;
            this.AllSerializedObjects = allObjects;
            this.RootNode = rootNode;
        }
    }
}
