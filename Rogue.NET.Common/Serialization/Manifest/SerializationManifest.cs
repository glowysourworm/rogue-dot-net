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
    }
}
