using Rogue.NET.Common.Serialization.Manifest;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.IO.Interface
{
    internal interface ISerializationStreamWriter
    {
        void Write<T>(T theObject);
        void Write(object theObject, Type theObjectType);

        IEnumerable<SerializedStreamData> GetStreamData();
    }
}
