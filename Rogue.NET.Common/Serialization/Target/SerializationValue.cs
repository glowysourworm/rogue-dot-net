
using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class SerializationValue : SerializationObjectBase
    {
        internal SerializationValue(HashedObjectInfo objectInfo, RecursiveSerializerMemberInfo memberInfo) : base(objectInfo, memberInfo)
        {
        }

        internal override IEnumerable<PropertyStorageInfo> GetProperties(PropertyWriter writer)
        {
            // No GetProperties(PropertyWriter writer) defined -> Use reflected public properties
            if (this.MemberInfo.GetMethod == null)
                return writer.GetPropertiesReflection(this.ObjectInfo.TheObject);

            // CLEAR CURRENT CONTEXT
            writer.ClearContext();

            // CALL OBJECT'S GetProperties METHOD
            try
            {
                this.MemberInfo.GetMethod.Invoke(this.ObjectInfo.TheObject, new object[] { writer });
            }
            catch (Exception innerException)
            {
                throw new Exception("Error trying to read properties from " + this.ObjectInfo.Type.TypeName, innerException);
            }

            return writer.GetResult();
        }
    }
}
