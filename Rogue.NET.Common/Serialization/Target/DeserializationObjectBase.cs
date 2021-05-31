using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal abstract class DeserializationObjectBase
    {
        public HashedObjectReference Reference { get; private set; }

        protected RecursiveSerializerMemberInfo MemberInfo { get; private set; }

        public DeserializationObjectBase(HashedObjectReference reference, RecursiveSerializerMemberInfo memberInfo)
        {
            this.Reference = reference;
            this.MemberInfo = memberInfo;
        }

        /// <summary>
        /// Resolves the object's hash code with the initial reference and returns the 
        /// final result. 
        /// </summary>
        /// <exception cref="Exception">Object doesn't resolve with the same hash</exception>
        /// <returns>Resolved object</returns>
        internal HashedObjectInfo Resolve()
        {
            var hashedInfo = ProvideResult();

            // CAN ONLY VALIDATE TYPE BASED ON HASH CODE
            if (hashedInfo.Type.GetHashCode() != this.Reference.Type.GetHashCode())
            {
                // Check to see if the implementing type is equivalent to the referenced type
                if (hashedInfo.Type.GetImplementingType() != this.Reference.Type.GetImplementingType())
                    throw new Exception("Invalid resolved hash code for object type:  " + this.Reference.Type.DeclaringType);
            }

            return hashedInfo;
        }

        internal void WriteProperties(IEnumerable<PropertyResolvedInfo> properties)
        {
            WriteProperties(new PropertyReader(properties));
        }

        /// <summary>
        /// Called during first pass on the Deserializer. This will either call for reflected public properties, or
        /// query the front end object using GetPropertyDefinitions(PropertyReader reader).
        /// 
        /// FOR COLLECTIONS - NO PROPERTIES ARE SUPPORTED UNLESS THE COLLECTION USES SERIALIZATIONMODE.SPECIFIED
        /// </summary>
        internal IEnumerable<PropertyDefinition> GetPropertyDefinitions()
        {
            return GetPropertyDefinitions(new PropertyPlanner());
        }

        /// <summary>
        /// Finalizes object and returns wrapped value
        /// </summary>
        protected abstract HashedObjectInfo ProvideResult();

        protected abstract IEnumerable<PropertyDefinition> GetPropertyDefinitions(PropertyPlanner planner);

        protected abstract void WriteProperties(PropertyReader reader);

        /// <summary>
        /// Constructs and stores the object using either the supplied constructor
        /// </summary>
        protected abstract void Construct();
    }
}
