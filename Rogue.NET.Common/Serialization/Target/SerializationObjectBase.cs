using Rogue.NET.Common.Serialization.Planning;

using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// Defines a serialization target object for our serializer. The type information can be recalled 
    /// later for creating references to targets.
    /// </summary>
    internal abstract class SerializationObjectBase
    {
        public HashedObjectInfo ObjectInfo { get; private set; }
        
        public SerializationMode Mode { get { return this.MemberInfo.Mode; } }

        protected RecursiveSerializerMemberInfo MemberInfo { get; private set; }

        protected SerializationObjectBase(HashedObjectInfo objectInfo, RecursiveSerializerMemberInfo memberInfo)
        {
            this.ObjectInfo = objectInfo;
            this.MemberInfo = memberInfo;
        }

        /// <summary>
        /// Returns property info collection for this instance - allowing the reader to "visit" the serialization
        /// target.
        /// </summary>
        internal abstract IEnumerable<PropertyStorageInfo> GetProperties(PropertyWriter writer);
    }
}
