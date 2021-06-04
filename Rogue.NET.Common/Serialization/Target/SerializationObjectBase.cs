using Rogue.NET.Common.Serialization.Component;
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
        internal ObjectInfo ObjectInfo { get; private set; }

        internal SerializationMode Mode { get { return this.MemberInfo.Mode; } }

        internal RecursiveSerializerMemberInfo MemberInfo { get; private set; }

        protected SerializationObjectBase(ObjectInfo objectInfo, RecursiveSerializerMemberInfo memberInfo)
        {
            this.ObjectInfo = objectInfo;
            this.MemberInfo = memberInfo;
        }
    }
}
