using Rogue.NET.Common.Serialization.Manifest;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.IO.Interface
{
    internal interface ISerializationStreamReader
    {
        T Read<T>();
        object Read(Type type);

        IEnumerable<SerializedStreamData> GetStreamData();
    }
}
