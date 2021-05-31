using Rogue.NET.Common.Serialization.Manifest;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization
{
    [Serializable]
    public class SerializationManifest
    {
        /// <summary>
        /// Dictionary of all hashed types serialized to file. Each represents a UNIQUE object OF TYPE "Type"
        /// </summary>
        public List<SerializedNodeManifest> SerializerOutput { get; private set; }

        public List<SerializedNodeManifest> DeserializerOutput { get; private set; }

        public List<SerializedTypeManifest> SerializedTypeTable { get; private set; }

        public List<SerializedTypeManifest> DeserializedTypeTable { get; private set; }

        public SerializationManifest() { }

        internal SerializationManifest(IEnumerable<HashedType> serializedTypeTable,
                                       IEnumerable<SerializationObjectBase> serializerOutput,
                                       IEnumerable<HashedType> deserializedTypeTable,
                                       IEnumerable<DeserializationObjectBase> deserializerOutput)
        {
            this.SerializedTypeTable = serializedTypeTable.Select(type => new SerializedTypeManifest()
            {
                HashCode = type.GetHashCode(),
                DeclaringAssembly = type.DeclaringAssembly,
                DeclaringType = type.DeclaringType,
                ImplementingAssembly = type.ImplementingAssembly,
                ImplementingType = type.ImplementingType

            }).ToList();

            this.DeserializedTypeTable = deserializedTypeTable.Select(type => new SerializedTypeManifest()
            {
                HashCode = type.GetHashCode(),
                DeclaringAssembly = type.DeclaringAssembly,
                DeclaringType = type.DeclaringType,
                ImplementingAssembly = type.ImplementingAssembly,
                ImplementingType = type.ImplementingType

            }).ToList();

            this.SerializerOutput = serializerOutput.Select(node => new SerializedNodeManifest()
            {
                Node = GetNodeType(node),
                Mode = node.Mode,
                CollectionCount = (node as SerializationCollection)?.Count ?? 0,
                CollectionType = (node as SerializationCollection)?.InterfaceType ?? CollectionInterfaceType.Array,
                Type = node.ObjectInfo.Type.DeclaringType,
                Assembly = node.ObjectInfo.Type.DeclaringAssembly,
                IsGeneric = node.ObjectInfo.Type.DeclaringIsGeneric,
                GenericArgumentTypes = node.ObjectInfo.Type.DeclaringGenericArguments.Select(x => x.DeclaringType).ToArray(),
                NodeTypeHashCode = node.ObjectInfo.Type.GetHashCode(),
                ObjectHashCode = node.ObjectInfo.GetHashCode()

            }).ToList();

            this.DeserializerOutput = deserializerOutput.Select(node => new SerializedNodeManifest()
            {
                Node = GetNodeType(node),

                // Mode not kept for deserialization - but is used to validate the data
                Mode = Planning.SerializationMode.None,
                CollectionCount = (node as DeserializationCollection)?.Count ?? 0,
                CollectionType = (node as DeserializationCollection)?.InterfaceType ?? CollectionInterfaceType.Array,
                Type = node.Reference.Type.DeclaringType,
                Assembly = node.Reference.Type.DeclaringAssembly,
                IsGeneric = node.Reference.Type.DeclaringIsGeneric,
                GenericArgumentTypes = node.Reference.Type.DeclaringGenericArguments.Select(x => x.DeclaringType).ToArray(),
                NodeTypeHashCode = node.Reference.Type.GetHashCode(),
                ObjectHashCode = node.Reference.GetHashCode()

            }).ToList();
        }

        private SerializedNodeType GetNodeType(SerializationObjectBase objectBase)
        {
            if (objectBase is SerializationNullPrimitive)
                return SerializedNodeType.NullPrimitive;

            if (objectBase is SerializationNullObject)
                return SerializedNodeType.Null;

            else if (objectBase is SerializationPrimitive)
                return SerializedNodeType.Primitive;

            else if (objectBase is SerializationReference)
                return SerializedNodeType.Reference;

            else if (objectBase is SerializationValue)
                return SerializedNodeType.Value;

            else if (objectBase is SerializationObject)
                return SerializedNodeType.Object;

            else if (objectBase is SerializationCollection)
                return SerializedNodeType.Collection;

            else
                throw new Exception("Unhandled SerializationObjectBase type:  SerializationManifest.cs");
        }

        private SerializedNodeType GetNodeType(DeserializationObjectBase objectBase)
        {
            if (objectBase is DeserializationNullReference)
                return SerializedNodeType.Null;

            else if (objectBase is DeserializationPrimitive)
                return SerializedNodeType.Primitive;

            else if (objectBase is DeserializationReference)
                return SerializedNodeType.Reference;

            else if (objectBase is DeserializationValue)
                return SerializedNodeType.Value;

            else if (objectBase is DeserializationObject)
                return SerializedNodeType.Object;

            else if (objectBase is DeserializationCollection)
                return SerializedNodeType.Collection;

            else
                throw new Exception("Unhandled DeserializationObjectBase type:  SerializationManifest.cs");
        }
    }
}
