using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization
{
    public class SerializationManifest
    {
        /// <summary>
        /// Dictionary of all hashed types serialized to file. Each represents a UNIQUE object OF TYPE "Type"
        /// </summary>
        public Dictionary<int, Type> SerializerOutputDictionary { get; private set; }

        internal SerializationManifest(IDictionary<HashedObjectInfo, SerializationObjectBase> serializerOutputDict)
        {
            this.SerializerOutputDictionary = serializerOutputDict.ToDictionary(element => element.Key.GetHashCode(),
                                                                                element => element.Value.ObjectInfo.Type.Resolve());
        }
    }
}
