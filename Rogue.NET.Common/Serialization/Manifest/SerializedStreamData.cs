using System;

namespace Rogue.NET.Common.Serialization.Manifest
{
    [Serializable]
    public struct SerializedStreamData
    {
        public long Position { get; set; }
        public long DataSize { get; set; }
        public Type DataType { get; set; }
    }
}
