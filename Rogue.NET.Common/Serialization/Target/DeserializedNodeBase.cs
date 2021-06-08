using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal abstract class DeserializedNodeBase
    {
        /// <summary>
        /// The defining property for the deserialized node
        /// </summary>
        internal PropertyDefinition Property { get; private set; }

        internal HashedType Type { get; private set; }

        protected RecursiveSerializerMemberInfo MemberInfo { get; private set; }

        public DeserializedNodeBase(PropertyDefinition property, HashedType type, RecursiveSerializerMemberInfo memberInfo)
        {
            this.Property = property;
            this.Type = type;
            this.MemberInfo = memberInfo;
        }

        /// <summary>
        /// Constructs and stores the object using the appropriate constructor (See RecursiveSerializerMemberInfo)
        /// </summary>
        internal abstract void Construct(IEnumerable<PropertyResolvedInfo> resolvedProperties);

        /// <summary>
        /// Retrieves list of property definitions passed in at the constructor (SHOULD BE FOR REFERENCE TYPES ONLY)
        /// </summary>
        /// <returns></returns>
        internal abstract PropertySpecification GetPropertySpecification();

        /// <summary>
        /// Resolves the object's value with the initial reference and returns the 
        /// final result. 
        /// </summary>
        internal object Resolve()
        {
            return ProvideResult();
        }


        protected abstract object ProvideResult();

        public override string ToString()
        {
            return this.Property.ToString();
        }
    }
}
