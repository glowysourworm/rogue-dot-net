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

        protected override void Construct()
        {
            throw new Exception("Trying to construct a null primitive:  DeserializationNullPrimitive.cs");
        }

        protected override IEnumerable<PropertyDefinition> GetPropertyDefinitions(PropertyPlanner planner)
        {
            throw new Exception("Trying to get property definitions for a null primitive:  DeserializationNullPrimitive.cs");
        }

        protected override HashedObjectInfo ProvideResult()
        {
            return new HashedObjectInfo(this.Reference.Type);
        }

        protected override void WriteProperties(PropertyReader reader)
        {
            throw new Exception("Trying to write properties for a null primitive:  DeserializationNullPrimitive.cs");
        }
    }
}
