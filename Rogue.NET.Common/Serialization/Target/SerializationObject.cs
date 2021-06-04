using Rogue.NET.Common.Serialization.Component;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Utility;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// Wrapper for the object being serialized - validates that it meets the requirements for serialization;
    /// and prepares it for processing.
    /// </summary>
    internal class SerializationObject : SerializationObjectBase
    {
        internal SerializationObject(ObjectInfo objectInfo, RecursiveSerializerMemberInfo memberInfo) : base(objectInfo, memberInfo)
        {
        }
    }
}
