using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Planning;

using System;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// Defines a serialization target object for our serializer. The type information can be recalled 
    /// later for creating references to targets.
    /// </summary>
    internal abstract class SerializedNodeBase
    {
        private static int IdCounter = 0;

        internal int Id { get; private set; }
        internal SerializationMode Mode { get { return this.MemberInfo.Mode; } }
        internal SerializedNodeType NodeType { get; private set; }
        internal HashedType Type { get; private set; }
        internal RecursiveSerializerMemberInfo MemberInfo { get; private set; }

        internal SerializedNodeBase(SerializedNodeType nodeType, HashedType hashedType, RecursiveSerializerMemberInfo memberInfo)
        {
            this.Id = SerializedNodeBase.IdCounter++;
            this.Type = hashedType;
            this.NodeType = nodeType;
            this.MemberInfo = memberInfo;
        }

        internal static void ResetCounter()
        {
            SerializedNodeBase.IdCounter = 0;
        }

        internal abstract object GetObject();
        internal abstract bool RepresentsNullReference();

        public override string ToString()
        {
            return string.Format("Id={0}, Type={1}, NodeType={2}", this.Id, this.Type.ToString(), this.NodeType.ToString());
        }
    }
}
