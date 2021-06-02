using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializationNullPrimitive : DeserializationObjectBase
    {
        internal DeserializationNullPrimitive(HashedObjectReference reference) : base(reference, RecursiveSerializerMemberInfo.Empty)
        {

        }

        internal override IEnumerable<PropertyDefinition> GetPropertyDefinitions()
        {
            throw new Exception("Trying to get property definitions for a null primitive:  DeserializationNullPrimitive.cs");
        }

        internal override void Construct(IEnumerable<PropertyResolvedInfo> resolvedProperties)
        {
            throw new Exception("Trying to construct a null primitive:  DeserializationNullPrimitive.cs");
        }

        protected override HashedObjectInfo ProvideResult()
        {
            return new HashedObjectInfo(this.Reference.Type);
        }
    }
}
