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

        protected SerializationObjectBase(HashedObjectInfo objectInfo)
        {
            this.ObjectInfo = objectInfo;
        }

        /// <summary>
        /// Returns property info collection for this instance - allowing the reader to "visit" the serialization
        /// target.
        /// </summary>
        internal abstract IEnumerable<PropertyStorageInfo> GetProperties(PropertyReader reader);
    }
}
