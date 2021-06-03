using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Target;

using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal class SerializationPlan : ISerializationPlan
    {
        public SerializationNodeBase RootNode { get; private set; }

        public SimpleDictionary<HashedObjectInfo, SerializationObjectBase> UniqueReferenceDict { get; private set; }

        public IEnumerable<SerializationObjectBase> AllSerializedObjects { get; private set; }

        public SerializationPlan(SimpleDictionary<HashedObjectInfo, SerializationObjectBase> referenceDict,
                                 SimpleDictionary<HashedType, HashedType> typeDict,
                                 IEnumerable<SerializationObjectBase> allObjects,
                                 SerializationNodeBase rootNode)
        {
            this.UniqueReferenceDict = referenceDict;
            this.AllSerializedObjects = allObjects;
            this.RootNode = rootNode;
        }
    }
}
