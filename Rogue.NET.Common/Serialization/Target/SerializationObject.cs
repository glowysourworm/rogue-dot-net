using Rogue.NET.Common.Serialization.Planning;

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
        internal SerializationObject(HashedObjectInfo objectInfo, RecursiveSerializerMemberInfo memberInfo) : base(objectInfo, memberInfo)
        {
        }

        internal override IEnumerable<PropertyStorageInfo> GetProperties(PropertyWriter writer)
        {
            // No GetProperties(PropertyWriter writer) defined -> Use reflected public properties
            if (this.MemberInfo.GetMethod == null)
                return writer.GetPropertiesReflection(this.ObjectInfo.Type.GetImplementingType(), this.ObjectInfo.GetObject());

            // CLEAR CURRENT CONTEXT
            writer.ClearContext();

            // CALL OBJECT'S GetProperties METHOD
            try
            {
                this.MemberInfo.GetMethod.Invoke(this.ObjectInfo.GetObject(), new object[] { writer });
            }
            catch (Exception innerException)
            {
                throw new Exception("Error trying to read properties from " + this.ObjectInfo.Type.DeclaringType, innerException);
            }

            return writer.GetResult();
        }
    }
}
