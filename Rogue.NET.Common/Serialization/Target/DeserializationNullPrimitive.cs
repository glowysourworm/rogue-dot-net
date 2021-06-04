using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializationNullPrimitive : DeserializationObjectBase
    {
        internal DeserializationNullPrimitive(ObjectReference reference) : base(reference, RecursiveSerializerMemberInfo.Empty)
        {

        }

        internal override PropertySpecification GetPropertySpecification()
        {
            throw new Exception("Trying to get property definitions for a null primitive:  DeserializationNullPrimitive.cs");
        }

        internal override void Construct(IEnumerable<PropertyResolvedInfo> resolvedProperties)
        {
            throw new Exception("Trying to construct a null primitive:  DeserializationNullPrimitive.cs");
        }

        protected override ObjectInfo ProvideResult()
        {
            return new ObjectInfo(this.Reference.Type);
        }
    }
}
