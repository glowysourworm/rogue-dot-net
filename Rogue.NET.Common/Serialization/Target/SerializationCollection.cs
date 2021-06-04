using Rogue.NET.Common.Serialization.Component;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Utility;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class SerializationCollection : SerializationObjectBase
    {
        internal int Count { get; private set; }
        internal IEnumerable Collection { get; private set; }
        internal IList<HashedType> ResolvedElementTypes { get; private set; }
        internal CollectionInterfaceType InterfaceType { get; private set; }

        public SerializationCollection(ObjectInfo objectInfo,
                                       RecursiveSerializerMemberInfo memberInfo,
                                       IEnumerable collection,
                                       IList<HashedType> resolvedElementTypes,
                                       int count,
                                       CollectionInterfaceType interfaceType,
                                       HashedType elementDeclaringType) : base(objectInfo, memberInfo)
        {
            this.Collection = collection;
            this.Count = count;
            this.InterfaceType = interfaceType;           
            this.ResolvedElementTypes = new List<HashedType>(resolvedElementTypes);
        }
    }
}
