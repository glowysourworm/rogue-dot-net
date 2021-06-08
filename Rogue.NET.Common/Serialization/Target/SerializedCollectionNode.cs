using Rogue.NET.Common.Serialization.Planning;

using System.Collections;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class SerializedCollectionNode : SerializedObjectNode
    {
        internal List<SerializedNodeBase> CollectionNodes { get; private set; }

        internal int Count { get; private set; }
        internal IEnumerable Collection { get; private set; }
        internal HashedType ElementDeclaringType { get; private set; }
        internal CollectionInterfaceType InterfaceType { get; private set; }

        public SerializedCollectionNode(HashedType resolvedType,
                                        RecursiveSerializerMemberInfo memberInfo,
                                        IEnumerable collection,
                                        int count,
                                        CollectionInterfaceType interfaceType,
                                        HashedType elementDeclaringType) : base(SerializedNodeType.Collection, resolvedType, collection, memberInfo)
        {
            this.Collection = collection;
            this.Count = count;
            this.InterfaceType = interfaceType;
            this.ElementDeclaringType = elementDeclaringType;

            this.CollectionNodes = new List<SerializedNodeBase>(count);
        }

        internal override bool RepresentsNullReference()
        {
            return ReferenceEquals(this.Collection, null);
        }

        internal override object GetObject()
        {
            return this.Collection;
        }
    }
}
