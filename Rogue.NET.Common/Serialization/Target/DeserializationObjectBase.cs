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
        /// Resolves the object's hash code with the initial reference and returns the 
        /// final result. 
        /// </summary>
        internal HashedObjectInfo Resolve()
        {
            var hashedInfo = ProvideResult();

            Validate(hashedInfo);

            return hashedInfo;
        }

        private void Validate(HashedObjectInfo providedResult)
        {
            // CAN ONLY VALIDATE TYPE BASED ON HASH CODE
            if (providedResult.Type.GetHashCode() != this.Reference.Type.GetHashCode())
            {
                // Check to see if the implementing type is equivalent to the referenced type
                if (providedResult.Type.GetImplementingType() != this.Reference.Type.GetImplementingType())
                    throw new Exception("Invalid resolved hash code for object type:  " + this.Reference.Type.ToString());
            }

            // *** PROVIDE HASH CODE VAILDATION FOR PRIMITIVE ONLY! (THIS SIMULATES A CHECKSUM FOR ALL PRIMITIVES)
            if ((this is DeserializationPrimitive) && providedResult.GetHashCode() != this.Reference.HashCode)
                throw new Exception("Invalid hash code for primitive type:  " + providedResult.Type.ToString());
        }

        protected abstract HashedObjectInfo ProvideResult();

        protected abstract IEnumerable<PropertyDefinition> GetPropertyDefinitions(PropertyPlanner planner);

        protected abstract void WriteProperties(PropertyReader reader);

        /// <summary>
        /// Constructs and stores the object using either the supplied constructor
        /// </summary>
        protected abstract void Construct();
    }
}
