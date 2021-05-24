using System;
using System.Collections;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class SerializationCollection : SerializationObjectBase
    {
        public enum CollectionType
        {
            Array,
            List,
            Dictionary
        }

        public IEnumerable Collection { get; private set; }

        public int Count { get; private set; }

        public CollectionType SupportType { get; private set; }

        public Type ElementType { get; private set; }

        public SerializationCollection(HashedObjectInfo objectInfo,
                                       IEnumerable collection, 
                                       int count, 
                                       CollectionType type, 
                                       Type elementType) : base(objectInfo)
        {
            this.Collection = collection;
            this.Count = count;
            this.SupportType = type;
            this.ElementType = elementType;
        }

        internal override IEnumerable<PropertyStorageInfo> GetProperties(PropertyReader reader)
        {
            throw new NotSupportedException("Support for collection properties is not implemented");
        }
    }
}
