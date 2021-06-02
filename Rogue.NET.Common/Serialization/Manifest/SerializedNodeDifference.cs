using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;

using System;

namespace Rogue.NET.Common.Serialization.Manifest
{
    [Serializable]
    public class SerializedNodeDifference
    {
        public SerializedNodeManifest SerializedNode { get; set; }
        public SerializedNodeManifest DeserializedNode { get; set; }

        public SerializedNodeDifference() { }
    }
}
