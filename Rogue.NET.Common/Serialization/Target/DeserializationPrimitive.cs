using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializationPrimitive : DeserializationObjectBase
    {
        /// <summary>
        /// Completed hashed info object with data already filled out
        /// </summary>
        public HashedObjectInfo ObjectInfo { get; private set; }

        public DeserializationPrimitive(HashedObjectInfo info) : base(new HashedObjectReference(info.Type, info.GetHashCode()), 
                                                                          RecursiveSerializerMemberInfo.Empty)
        {
            this.ObjectInfo = info;
        }

        protected override IEnumerable<PropertyDefinition> GetPropertyDefinitions(PropertyPlanner planner)
        {
            throw new Exception("Trying to call GetPropertyDefinitions(PropertyPlanner) on a primitive");
        }

        protected override void Construct()
        {
            throw new Exception("Trying to call constructor on a primitive");
        }

        protected override HashedObjectInfo ResolveImpl()
        {
            throw new NotImplementedException("Should not be calling Resolve() on a PRIMITIVE");
        }

        protected override void WriteProperties(PropertyReader reader)
        {
            throw new NotImplementedException("Should not be calling WriteProperties(PropertyReader) on a PRIMITIVE");
        }
    }
}
