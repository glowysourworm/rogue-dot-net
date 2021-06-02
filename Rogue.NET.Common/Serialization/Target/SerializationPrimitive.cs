using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// Defines OUR "Primitive" types - which should be basic value types (non-struct). Each of
    /// these MUST have a Formatter that we define for supporting serialization. 
    /// </summary>
    internal class SerializationPrimitive : SerializationObjectBase
    {
        internal SerializationPrimitive(HashedObjectInfo objectInfo) : base(objectInfo, RecursiveSerializerMemberInfo.Empty)
        {
        }

        protected override IEnumerable<PropertyStorageInfo> GetProperties(PropertyWriter writer)
        {
            throw new Exception("Invalid use of GetProperties for SerializationPrimitive");
        }
    }
}
