using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class SerializationReference : SerializationObjectBase
    {
        internal SerializationReference(HashedObjectInfo objectInfo, RecursiveSerializerMemberInfo memberInfo) : base(objectInfo, memberInfo)
        {
        }

        protected override IEnumerable<PropertyStorageInfo> GetProperties(PropertyWriter writer)
        {
            throw new NotSupportedException("Trying to get properties on a referenced serialized object - should not be recursing");
        }
    }
}
