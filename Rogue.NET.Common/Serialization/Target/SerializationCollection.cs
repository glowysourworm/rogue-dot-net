using Rogue.NET.Common.Serialization.Planning;

using System.Collections;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class SerializationCollection : SerializationObjectBase
    {
        internal int Count { get; private set; }
        internal IEnumerable Collection { get; private set; }
        internal int[] ResolvedElementTypeHashCodes { get; private set; }
        internal CollectionInterfaceType InterfaceType { get; private set; }

        public SerializationCollection(ObjectInfo objectInfo,
                                       RecursiveSerializerMemberInfo memberInfo,
                                       IEnumerable collection,
                                       int[] resolvedElementTypeHashCodes,
                                       int count,
                                       CollectionInterfaceType interfaceType,
                                       HashedType elementDeclaringType) : base(objectInfo, memberInfo)
        {
            this.Collection = collection;
            this.Count = count;
            this.InterfaceType = interfaceType;
            this.ResolvedElementTypeHashCodes = resolvedElementTypeHashCodes;
        }
    }
}
