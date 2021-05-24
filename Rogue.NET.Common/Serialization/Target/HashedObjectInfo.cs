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
        /// Calculated hash (TRYING TO DO THIS ONLY ONCE). HASH DEPENDS ON *** TYPE + OBJECT.GetHashCode() ***
        /// </summary>
        public int HashCode { get; private set; }

        /// <summary>
        /// Creates a null reference for the specified type
        /// </summary>
        public HashedObjectInfo(Type nullObjectType)
        {
            this.TheObject = null;
            this.Type = new HashedType(nullObjectType);

            // *** USING HASH CODE FOR TYPE ONLY
            this.HashCode = nullObjectType.GetHashCode();
        }
        public HashedObjectInfo(object theObject, Type type)
        {
            this.TheObject = theObject;
            this.Type = new HashedType(type);

            // *** USE BOTH TYPE AND HASH CODE FOR TRACKING!
            this.HashCode = this.CreateHashCode(type.GetHashCode(), 
                                                theObject.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            var info = obj as HashedObjectInfo;

            return this.HashCode == info.HashCode;
        }

        public override int GetHashCode()
        {
            return this.HashCode;
        }

        public override string ToString()
        {
            return string.Format("Hash={0}, Type={1}", this.HashCode, this.Type.TypeName);
        }
    }
}
