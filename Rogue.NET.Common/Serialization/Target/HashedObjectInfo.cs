using Rogue.NET.Common.Extension;

using System;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// REPRESENTS A UNIQUE REFERENCE IN THE SERIALIZATION GRAPH. UNIQUENESS BASED ON HASHCODE!
    /// </summary>
    internal class HashedObjectInfo
    {
        public HashedType Type { get; private set; }

        private object _theObject;
        private bool _representsNullReference;

        /// <summary>
        /// Creates a null reference for the specified type
        /// </summary>
        public HashedObjectInfo(Type nullObjectType)
        {
            _theObject = null;
            _representsNullReference = true;
            this.Type = new HashedType(nullObjectType);
        }
        /// <summary>
        /// Creates a null reference for the specified hashed type
        /// </summary>
        public HashedObjectInfo(HashedType nullObjectType)
        {
            _theObject = null;
            _representsNullReference = true;
            this.Type = nullObjectType;
        }
        public HashedObjectInfo(object theObject, Type declaringType)
        {
            _theObject = theObject;
            _representsNullReference = false;
            this.Type = new HashedType(declaringType, theObject.GetType());
        }

        public bool RepresentsNullReference()
        {
            return _representsNullReference;
        }

        public object GetObject()
        {
            return _theObject;
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
            if (_representsNullReference)
                return this.Type.GetHashCode();

            else
                return this.CreateHashCode(_theObject, this.Type);
        }

        public override string ToString()
        {
            return string.Format("Hash={0}, Type={1}", this.GetHashCode(), this.Type.DeclaringType);
        }
    }
}
