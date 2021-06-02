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

        readonly PropertyWriter _propertyWriter;

        bool _propertiesWritten = false;

        protected SerializationObjectBase(HashedObjectInfo objectInfo, RecursiveSerializerMemberInfo memberInfo)
        {
            this.ObjectInfo = objectInfo;
            this.MemberInfo = memberInfo;

            _propertyWriter = new PropertyWriter();
        }

        /// <summary>
        /// Returns property info collection for this instance - allowing the reader to "visit" the serialization
        /// target.
        /// </summary>
        internal IEnumerable<PropertyStorageInfo> GetProperties()
        {
            if (_propertiesWritten)
                return _propertyWriter.GetResult();

            GetProperties(_propertyWriter);

            _propertiesWritten = true;

            return _propertyWriter.GetResult();
        }

        protected abstract IEnumerable<PropertyStorageInfo> GetProperties(PropertyWriter writer);
    }
}
