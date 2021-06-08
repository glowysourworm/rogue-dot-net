using Rogue.NET.Common.Serialization.Planning;

using System;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class SerializedLeafNode : SerializedNodeBase
    {
        readonly object _theObject;

        /// <summary>
        /// Constructor for NULL, NULL PRIMITIVE types
        /// </summary>
        internal SerializedLeafNode(SerializedNodeType nodeType, HashedType resolvedType)
                    : base(nodeType, resolvedType, RecursiveSerializerMemberInfo.Empty)
        {
            switch (nodeType)
            {
                case SerializedNodeType.NullPrimitive:
                case SerializedNodeType.Null:
                    break;
                default:
                    throw new Exception("Improper use of SerializationLeafNode:  constructor called was for NULL, NULL PRIMITIVE, and REFERENCE types");
            }
        }

        /// <summary>
        /// Constructor for PRIMITIVE leaf type
        /// </summary>
        internal SerializedLeafNode(SerializedNodeType nodeType, HashedType resolvedType, object theObject)
            : base(nodeType, resolvedType, RecursiveSerializerMemberInfo.Empty)
        {
            switch (nodeType)
            {
                case SerializedNodeType.Primitive:
                    break;
                default:
                    throw new Exception("Improper use of SerializationLeafNode:  constructor called was for NULL and NULL PRIMITIVE types");
            }

            _theObject = theObject;
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
