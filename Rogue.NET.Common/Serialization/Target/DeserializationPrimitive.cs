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

        internal override IEnumerable<PropertyDefinition> GetPropertyDefinitions()
        {
            throw new Exception("Trying to get property definitions for a primitive:  DeserializationPrimitive.cs");
        }

        internal override void Construct(IEnumerable<PropertyResolvedInfo> resolvedProperties)
        {
            throw new Exception("Trying to call constructor on a primitive");
        }

        protected override HashedObjectInfo ProvideResult()
        {
            return this.ObjectInfo;
        }
    }
}
