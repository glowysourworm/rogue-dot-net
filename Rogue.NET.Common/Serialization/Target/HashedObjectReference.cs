namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// Wraps the hash code for HashedObjectInfo.GetHashCode() during deserialization. HASH CODE BASED ON
    /// SERIALIZED VALUE! this.HashCode.
    /// </summary>
    internal class HashedObjectReference
    {
        public HashedType Type { get; private set; }

        /// <summary>
        /// HASH CODE FROM SERIALIZATION PROCEDURE HashObjectInfo.GetHashCode()
        /// </summary>
        public int HashCode { get; private set; }

        /// <summary>
        /// Constructor for a null object reference
        /// </summary>
        public HashedObjectReference(HashedType hashedType)
        {
            this.Type = hashedType;

            // USING TYPE + OBJECT HASH CODE CONVENTION
            this.HashCode = hashedType.GetHashCode();
        }

        public HashedObjectReference(HashedType hashedType, int hashCode)
        {
            this.Type = hashedType;

            // HASH CODE FROM HashObjectInfo.GetHashCode()
            this.HashCode = hashCode;
        }

        public override bool Equals(object obj)
        {
            var reference = obj as HashedObjectReference;

            return this.HashCode == reference.HashCode;
        }

        public override int GetHashCode()
        {
            return this.HashCode;
        }
    }
}
