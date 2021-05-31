using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializationReference : DeserializationObjectBase
    {
        public DeserializationReference(HashedObjectReference reference, RecursiveSerializerMemberInfo memberInfo) : base(reference, memberInfo)
        {
        }

        protected override void Construct()
        {
            throw new NotSupportedException("Trying to CONSTRUCT a referenced deserialized object - should not be recursing");
        }

        protected override IEnumerable<PropertyDefinition> GetPropertyDefinitions(PropertyPlanner planner)
        {
            throw new NotSupportedException("Trying to GET PROPERTY DEFINITIONS a referenced deserialized object - should not be recursing");
        }

        protected override HashedObjectInfo ProvideResult()
        {
            throw new NotSupportedException("Trying to RESOLVE a referenced deserialized object - should be using the actual data reference");
        }

        protected override void WriteProperties(PropertyReader reader)
        {
            throw new NotSupportedException("Trying to WRITE PROPERTIES a referenced deserialized object - should not be recursing");
        }
    }
}
