using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializationNullReference : DeserializationObjectBase
    {
        public DeserializationNullReference(HashedObjectReference reference) : base(reference, RecursiveSerializerMemberInfo.Empty)
        {
        }

        protected override void Construct()
        {
            throw new Exception("Trying to construct a null reference:  DeserializationNullReference.cs");
        }

        protected override IEnumerable<PropertyDefinition> GetPropertyDefinitions(PropertyPlanner planner)
        {
            throw new Exception("Trying to get property definitions for a null reference:  DeserializationNullReference.cs");
        }

        protected override HashedObjectInfo ResolveImpl()
        {
            throw new Exception("Trying to resolve implementation for a null reference:  DeserializationNullReference.cs");
        }

        protected override void WriteProperties(PropertyReader reader)
        {
            throw new Exception("Trying to write properties for a null reference:  DeserializationNullReference.cs");
        }
    }
}
