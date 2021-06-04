using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal abstract class DeserializationObjectBase
    {
        public ObjectReference Reference { get; private set; }

        protected RecursiveSerializerMemberInfo MemberInfo { get; private set; }

        public DeserializationObjectBase(ObjectReference reference, RecursiveSerializerMemberInfo memberInfo)
        {
            this.Reference = reference;
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
        /// Resolves the object's hash code with the initial reference and returns the 
        /// final result. 
        /// </summary>
        internal ObjectInfo Resolve()
        {
            var hashedInfo = ProvideResult();

            Validate(hashedInfo);

            return hashedInfo;
        }

        private void Validate(ObjectInfo providedResult)
        {
            // CAN ONLY VALIDATE TYPE BASED ON HASH CODE
            if (providedResult.Type.GetHashCode() != this.Reference.Type.GetHashCode())
            {
                // Check to see if the implementing type is equivalent to the referenced type
                if (providedResult.Type.GetImplementingType() != this.Reference.Type.GetImplementingType())
                    throw new Exception("Invalid resolved hash code for object type:  " + this.Reference.Type.ToString());
            }

            // *** PROVIDE HASH CODE VAILDATION FOR PRIMITIVE ONLY! (THIS SIMULATES A CHECKSUM FOR ALL PRIMITIVES)
            if ((this is DeserializationPrimitive) && providedResult.GetHashCode() != this.Reference.ReferenceId)
                throw new Exception("Invalid hash code for primitive type:  " + providedResult.Type.ToString());
        }

        protected abstract ObjectInfo ProvideResult();

        public override string ToString()
        {
            return this.FormatToString();
        }
    }
}
