using Rogue.NET.Common.Serialization.Planning;

using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class SerializedObjectNode : SerializedNodeBase
    {
        internal List<SerializedNodeBase> SubNodes { get; private set; }

        readonly object _theObject;

        internal SerializedObjectNode(SerializedNodeType nodeType, HashedType resolvedType, object theObject, RecursiveSerializerMemberInfo memberInfo)
                    : base(nodeType, resolvedType, memberInfo)
        {
            _theObject = theObject;

            this.SubNodes = new List<SerializedNodeBase>();
        }

        internal override bool RepresentsNullReference()
        {
            return ReferenceEquals(_theObject, null);
        }

        internal override object GetObject()
        {
            return _theObject;
        }
    }
}
