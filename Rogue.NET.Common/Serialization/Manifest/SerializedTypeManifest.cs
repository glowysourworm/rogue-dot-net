using System;

namespace Rogue.NET.Common.Serialization.Manifest
{
    [Serializable]
    public class SerializedTypeManifest
    {
        public string DeclaringType { get; set; }
        public string DeclaringAssembly { get; set; }

        public string ImplementingType { get; set; }
        public string ImplementingAssembly { get; set; }

        public int HashCode { get; set; }

        public SerializedTypeManifest() { }
    }
}
