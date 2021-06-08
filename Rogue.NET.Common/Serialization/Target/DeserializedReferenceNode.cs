using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializedReferenceNode : DeserializedNodeBase
    {
        /// <summary>
        /// ID FROM SERIALIZATION PROCEDURE
        /// </summary>
        internal int ReferenceId { get; private set; }

        internal DeserializedReferenceNode(PropertyDefinition definition, int referenceId, HashedType type) : base(definition, type, RecursiveSerializerMemberInfo.Empty)
        {
            this.ReferenceId = referenceId;
        }

        internal override PropertySpecification GetPropertySpecification()
        {
            throw new Exception("Trying to get property definitions for a reference:  DeserializationReference.cs");
        }

        internal override void Construct(IEnumerable<PropertyResolvedInfo> resolvedProperties)
        {
            throw new NotSupportedException("Trying to CONSTRUCT a referenced deserialized object - should not be recursing");
        }

        protected override object ProvideResult()
        {
            throw new NotSupportedException("Trying to RESOLVE a referenced deserialized object - should be using the actual data reference");
        }

        public override bool Equals(object obj)
        {
            var node = obj as DeserializedReferenceNode;

            return this.ReferenceId == node.ReferenceId;
        }
        public override int GetHashCode()
        {
            return this.ReferenceId;
        }
    }
}
