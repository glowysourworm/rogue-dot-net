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
            var hashedInfo = ResolveImpl();

            if (hashedInfo.GetHashCode() != this.Reference.HashCode)
                throw new Exception("Invalid resolved hash code for object:  " + this.Reference.Type.TypeName);

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

        protected abstract HashedObjectInfo ResolveImpl();

        protected abstract IEnumerable<PropertyDefinition> GetPropertyDefinitions(PropertyPlanner planner);

        protected abstract void WriteProperties(PropertyReader reader);

        /// <summary>
        /// Constructs and stores the object using either the supplied constructor
        /// </summary>
        protected abstract void Construct();
    }
}
