using Rogue.NET.Common.Serialization.Manifest;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections.Generic;

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

        public SerializationManifest() { }

        internal SerializationManifest(IEnumerable<SerializedNodeManifest> serializerOutput,
                                       IEnumerable<SerializedNodeManifest> deserializerOutput)
        {
            this.SerializerOutput = new List<SerializedNodeManifest>(serializerOutput);
            this.DeserializerOutput = new List<SerializedNodeManifest>(deserializerOutput);
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
