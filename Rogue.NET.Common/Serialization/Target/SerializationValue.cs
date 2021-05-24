using Rogue.NET.Common.Serialization.Attribute;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class SerializationValue : SerializationObjectBase
    {
        public PropertySerializableMode Mode { get; private set; }
        protected ConstructorInfo Constructor { get; private set; }
        protected MethodInfo GetMethod { get; private set; }

        internal SerializationValue(HashedObjectInfo objectInfo, ConstructorInfo constructor) : base(objectInfo)
        {
            this.Constructor = constructor;
            this.GetMethod = null;
            this.Mode = PropertySerializableMode.Default;
        }

        internal SerializationValue(HashedObjectInfo objectInfo, ConstructorInfo constructor, MethodInfo getMethod) : base(objectInfo)
        {
            this.Constructor = constructor;
            this.GetMethod = getMethod;
            this.Mode = PropertySerializableMode.Specified;
        }

        internal override IEnumerable<PropertyStorageInfo> GetProperties(PropertyReader reader)
        {
            if (this.Mode == PropertySerializableMode.Default)
                return reader.GetPropertiesReflection(this.ObjectInfo.TheObject);

            // CLEAR CURRENT CONTEXT
            reader.ClearContext();

            // CALL OBJECT'S GetProperties METHOD
            try
            {
                this.GetMethod.Invoke(this.ObjectInfo.TheObject, new object[] { reader });
            }
            catch (Exception innerException)
            {
                throw new Exception("Error trying to read properties from " + this.ObjectInfo.Type.TypeName, innerException);
            }

            return reader.GetResult();
        }
    }
}
