using System;

namespace Rogue.NET.Common.Serialization
{
    [Serializable]
    internal class PropertySerializerHeader
    {
        public Type TheObjectType { get; set; }
        public int Checksum { get; set; }

        public PropertySerializerHeader(Type objectType, int checksum)
        {
            this.TheObjectType = objectType;
            this.Checksum = checksum;
        }
    }
}
