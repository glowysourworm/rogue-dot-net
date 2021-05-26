using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// Wraps the hash code for HashedObjectInfo.GetHashCode() during deserialization
    /// </summary>
    internal class HashedObjectReference
    {
        public HashedType Type { get; private set; }

        /// <summary>
        /// HASH CODE FROM HashObjectInfo.GetHashCode()
        /// </summary>
        public int HashCode { get; private set; }

        /// <summary>
        /// Constructor for a null object reference
        /// </summary>
        public HashedObjectReference(HashedType hashedType)
        {
            this.Type = hashedType;
            this.HashCode = default(int);
        }

        public HashedObjectReference(HashedType hashedType, int hashCode)
        {
            this.Type = hashedType;
            this.HashCode = hashCode;
        }

        /// <summary>
        /// Creates HashCodeInfo using the resolved object. Validates the resulting hash code against the one 
        /// stored in HashedObjectReference.
        /// </summary>
        /// <param name="actualObject">Resolved object from Deserializer</param>
        /// <returns>True if hash codes match, False otherwise.</returns>
        public bool ValidateReference(object actualObject)
        {
            var resolvedHashInfo = ReferenceEquals(actualObject, null) ? new HashedObjectInfo(this.Type.Resolve()) : 
                                                                         new HashedObjectInfo(actualObject, this.Type.Resolve());

            // SHOULD BE EQUAL IF OBJECT WAS DESERIALIZED PROPERLY
            return resolvedHashInfo.GetHashCode() == this.HashCode;
        }
    }
}
