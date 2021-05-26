using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class SerializationNullObject : SerializationObjectBase
    {
        public SerializationNullObject(HashedObjectInfo nullInfo) : base(nullInfo, RecursiveSerializerMemberInfo.Empty)
        {
        }

        internal override IEnumerable<PropertyStorageInfo> GetProperties(PropertyWriter writer)
        {
            throw new Exception("Invalid use of GetProperties for null-referenced object");
        }
    }
}
