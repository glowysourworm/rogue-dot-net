
using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class SerializationValue : SerializationObjectBase
    {
        internal SerializationValue(HashedObjectInfo objectInfo, RecursiveSerializerMemberInfo memberInfo) : base(objectInfo, memberInfo)
        {
        }

        protected override IEnumerable<PropertyStorageInfo> GetProperties(PropertyWriter writer)
        {
            // No GetProperties(PropertyWriter writer) defined -> Use reflected public properties
            if (this.MemberInfo.Mode == SerializationMode.Default)
                return writer.GetPropertiesReflection(this.ObjectInfo.Type.GetImplementingType(), this.ObjectInfo.GetObject());

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
