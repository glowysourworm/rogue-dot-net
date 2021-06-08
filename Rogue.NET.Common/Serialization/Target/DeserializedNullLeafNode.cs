using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializedNullLeafNode : DeserializedNodeBase
    {
        internal DeserializedNullLeafNode(PropertyDefinition definition, HashedType resolvedType) : base(definition, resolvedType, RecursiveSerializerMemberInfo.Empty)
        {
        }

        internal override PropertySpecification GetPropertySpecification()
        {
            throw new Exception("Trying to get property definitions for a null refereence:  DeserializationNullLeafNode.cs");
        }

        internal override void Construct(IEnumerable<PropertyResolvedInfo> resolvedProperties)
        {
            throw new Exception("Trying to construct a null reference:  DeserializationNullLeafNode.cs");
        }

        protected override object ProvideResult()
        {
            throw new Exception("Trying to provide result from a null reference:  DeserializationNullLeafNode.cs");
        }

        public override bool Equals(object obj)
        {
            var node = obj as DeserializedNullLeafNode;

            return this.GetHashCode() == node.GetHashCode();
        }
        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }
    }
}
