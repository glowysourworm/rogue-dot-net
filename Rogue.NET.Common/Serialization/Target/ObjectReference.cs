using Rogue.NET.Common.Extension;

using System;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// Reference to serialized object
    /// </summary>
    internal class ObjectReference
    {
        internal HashedType Type { get; private set; }

        /// <summary>
        /// ID FROM SERIALIZATION PROCEDURE
        /// </summary>
        internal int ReferenceId { get; private set; }

        /// <summary>
        /// Constructor for a null object reference
        /// </summary>
        internal ObjectReference(HashedType hashedType, int referenceId)
        {
            this.Type = hashedType;
            this.ReferenceId = referenceId;
        }

        #region (public) Equality
        public static bool operator ==(ObjectReference reference1, ObjectReference reference2)
        {
            if (ReferenceEquals(reference1, null))
                return ReferenceEquals(reference2, null);

            else if (ReferenceEquals(reference2, null))
                return false;

            else
                return reference1.Equals(reference2);
        }

        public static bool operator !=(ObjectReference reference1, ObjectReference reference2)
        {
            return !(reference1 == reference2);
        }

        public override bool Equals(object obj)
        {
            var reference = obj as ObjectReference;

            // EQUALS IS STRICTLY BY ID
            if (this.ReferenceId == reference.ReferenceId &&
               !this.Type.Equals(reference.Type))
                throw new Exception(string.Format("Mismatching HashedType detected!  Id={0}, Type1={1}, Type2={2}", this.ReferenceId, this.Type, reference.Type));

            return this.ReferenceId == reference.ReferenceId;
        }

        public override int GetHashCode()
        {
            return this.ReferenceId;
        }
        #endregion

        public override string ToString()
        {
            return this.FormatToString();
        }
    }
}
