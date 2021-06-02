using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializationReference : DeserializationObjectBase
    {
        internal DeserializationReference(HashedObjectReference reference, RecursiveSerializerMemberInfo memberInfo) : base(reference, memberInfo)
        {
        }

        internal override IEnumerable<PropertyDefinition> GetPropertyDefinitions()
        {
            throw new Exception("Trying to get property definitions for a reference:  DeserializationReference.cs");
        }

        internal override void Construct(IEnumerable<PropertyResolvedInfo> resolvedProperties)
        {
            throw new NotSupportedException("Trying to CONSTRUCT a referenced deserialized object - should not be recursing");
        }

        protected override HashedObjectInfo ProvideResult()
        {
            throw new NotSupportedException("Trying to RESOLVE a referenced deserialized object - should be using the actual data reference");
        }
    }
}
