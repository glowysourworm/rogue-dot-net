using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class SerializationNullPrimitive : SerializationObjectBase
    {
        internal SerializationNullPrimitive(HashedObjectInfo objectInfo) : base(objectInfo, RecursiveSerializerMemberInfo.Empty)
        {
        }

        protected override IEnumerable<PropertyStorageInfo> GetProperties(PropertyWriter writer)
        {
            throw new Exception("Invalid use of GetProperties for SerializationNullPrimitive");
        }
    }
}
