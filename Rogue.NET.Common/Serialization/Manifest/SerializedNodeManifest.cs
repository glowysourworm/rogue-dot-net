﻿using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;

using System;

namespace Rogue.NET.Common.Serialization.Manifest
{
    [Serializable]
    public struct SerializedNodeManifest
    {
        public static SerializedNodeManifest Empty = new SerializedNodeManifest();

        public SerializedNodeType Node { get; set; }
        public SerializationMode Mode { get; set; }
        public string Type { get; set; }
        public int ObjectId { get; set; }
        public int CollectionCount { get; set; }
        public CollectionInterfaceType CollectionType { get; set; }
    }
}
