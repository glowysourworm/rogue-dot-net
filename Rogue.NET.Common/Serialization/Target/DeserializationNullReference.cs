using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializationNullReference : DeserializationObjectBase
    {
        internal DeserializationNullReference(HashedObjectReference reference) : base(reference, RecursiveSerializerMemberInfo.Empty)
        {
        }

        internal override IEnumerable<PropertyDefinition> GetPropertyDefinitions()
        {
            throw new Exception("Trying to get property definitions for a null refereence:  DeserializationNullReference.cs");
        }

        internal override void Construct(IEnumerable<PropertyResolvedInfo> resolvedProperties)
        {
            throw new Exception("Trying to construct a null reference:  DeserializationNullReference.cs");
        }

        protected override HashedObjectInfo ProvideResult()
        {
            return new HashedObjectInfo(this.Reference.Type);
        }
    }
}
