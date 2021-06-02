using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class SerializationCollection : SerializationObjectBase
    {
        public IEnumerable Collection { get; private set; }
        public int Count { get; private set; }

        public Type ElementType { get; private set; }
        public CollectionInterfaceType InterfaceType { get; private set; }

        public SerializationCollection(HashedObjectInfo objectInfo,
                                       RecursiveSerializerMemberInfo memberInfo,
                                       IEnumerable collection,
                                       int count,
                                       CollectionInterfaceType interfaceType,
                                       Type elementType) : base(objectInfo, memberInfo)
        {
            this.Collection = collection;
            this.Count = count;
            this.InterfaceType = interfaceType;
            this.ElementType = elementType;
        }

        protected override IEnumerable<PropertyStorageInfo> GetProperties(PropertyWriter writer)
        {
            // DEFAULT MODE - NO PROPERTY SUPPORT
            if (this.MemberInfo.Mode == SerializationMode.Default ||
                this.MemberInfo.Mode == SerializationMode.None)
                return new PropertyStorageInfo[] { };

            // CALL OBJECT'S GetPropertyDefinitions METHOD
            try
            {
                this.MemberInfo.GetMethod.Invoke(this.Collection, new object[] { writer });
            }
            catch (Exception)
            {
                throw new RecursiveSerializerException(this.ObjectInfo.Type, "Error trying to read properties");
            }

            return writer.GetResult();
        }
    }
}
