using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializedLeafNode : DeserializedNodeBase
    {
        readonly object _theObject;

        public DeserializedLeafNode(PropertyDefinition definition, HashedType resolvedType, object theObject) : base(definition, resolvedType, RecursiveSerializerMemberInfo.Empty)
        {
            _theObject = theObject;
        }

        internal override PropertySpecification GetPropertySpecification()
        {
            throw new Exception("Trying to get property definitions for a primitive:  DeserializationPrimitive.cs");
        }

        internal override void Construct(IEnumerable<PropertyResolvedInfo> resolvedProperties)
        {
            throw new Exception("Trying to call constructor on a primitive");
        }

        public override int GetHashCode()
        {
            if (ReferenceEquals(_theObject, null))
                return this.Type.GetHashCode();

            else
                return this.CreateHashCode(_theObject, this.Type);
        }

        public override bool Equals(object obj)
        {
            var node = obj as DeserializedLeafNode;

            return this.GetHashCode() == node.GetHashCode();
        }

        protected override object ProvideResult()
        {
            return _theObject;
        }
    }
}
