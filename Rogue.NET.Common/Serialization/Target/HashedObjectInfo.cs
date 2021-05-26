using Rogue.NET.Common.Extension;

using System;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// REPRESENTS A UNIQUE REFERENCE IN THE SERIALIZATION GRAPH. UNIQUENESS BASED ON HASHCODE!
    /// </summary>
    internal class HashedObjectInfo
    {
        public object TheObject { get; private set; }
        public HashedType Type { get; private set; }

        /// <summary>
        /// Creates a null reference for the specified type
        /// </summary>
        public HashedObjectInfo(Type nullObjectType)
        {
            this.TheObject = null;
            this.Type = new HashedType(nullObjectType);
        }
        public HashedObjectInfo(object theObject, Type type)
        {
            this.TheObject = theObject;
            this.Type = new HashedType(type);
        }

        public override bool Equals(object obj)
        {
            var info = obj as HashedObjectInfo;

            return this.GetHashCode() == info.GetHashCode();
        }

        public override int GetHashCode()
        {
            // RETURN TYPE + OBJECT.GetHashCode() UNLESS NULL REFERENCE INFO
            //
            if (this.TheObject == null)
                return this.Type.GetHashCode();

            else
                return this.CreateHashCode(this.TheObject, this.Type);
        }

        public override string ToString()
        {
            return string.Format("Hash={0}, Type={1}", this.GetHashCode(), this.Type.TypeName);
        }
    }
}
