using Rogue.NET.Common.Extension;

using System;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// REPRESENTS A UNIQUE REFERENCE IN THE SERIALIZATION GRAPH
    /// </summary>
    internal class ObjectInfo
    {
        private static int IdCounter = 0;

        internal HashedType Type { get; private set; }
        internal int Id { get; private set; } 

        private object _theObject;

        /// <summary>
        /// Creates a null reference for the specified hashed type
        /// </summary>
        public ObjectInfo(HashedType nullObjectType)
        {
            _theObject = null;
        
            this.Type = nullObjectType;
            this.Id = ObjectInfo.IdCounter++;
        }
        public ObjectInfo(object theObject, HashedType theObjectType)
        {
            _theObject = theObject;

            this.Type = theObjectType;
            this.Id = ObjectInfo.IdCounter++;
        }

        /// <summary>
        /// Resets Id generation
        /// </summary>
        public static void ResetCounter()
        {
            ObjectInfo.IdCounter = 0;
        }

        public bool RepresentsNullReference()
        {
            return ReferenceEquals(_theObject, null);
        }

        public object GetObject()
        {
            return _theObject;
        }

        #region (public) Equality
        public static bool operator ==(ObjectInfo info1, ObjectInfo info2)
        {
            if (ReferenceEquals(info1, null))
                return ReferenceEquals(info2, null);

            else if (ReferenceEquals(info2, null))
                return false;

            else
                return info1.Equals(info2);
        }

        public static bool operator !=(ObjectInfo info1, ObjectInfo info2)
        {
            return !(info1 == info2);
        }

        public override bool Equals(object obj)
        {
            var info = obj as ObjectInfo;

            // EQUALS IS STRICTLY BY ID
            if (this.Id == info.Id &&
               !this.Type.Equals(info.Type))
                throw new Exception(string.Format("Mismatching HashedType detected!  Id={0}, Type1={1}, Type2={2}", this.Id, this.Type, info.Type));

            return this.Id == info.Id;
        }

        public override int GetHashCode()
        {
            return this.Id;
        }
        #endregion

        public override string ToString()
        {
            return this.FormatToString();
        }
    }
}
