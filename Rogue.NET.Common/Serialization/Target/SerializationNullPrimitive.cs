using Rogue.NET.Common.Serialization.Component;
using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class SerializationNullPrimitive : SerializationObjectBase
    {
        internal SerializationNullPrimitive(ObjectInfo objectInfo) : base(objectInfo, RecursiveSerializerMemberInfo.Empty)
        {
        }
    }
}
