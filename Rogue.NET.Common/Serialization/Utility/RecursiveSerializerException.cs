using Rogue.NET.Common.Serialization.Target;

using System;

namespace Rogue.NET.Common.Serialization.Utility
{
    public class RecursiveSerializerException : Exception
    {
        internal HashedType Type { get; private set; }

        internal RecursiveSerializerException(HashedType type) : base("", new RecursiveSerializerInnerException())
        {
            this.Type = type;
        }

        internal RecursiveSerializerException(HashedType type, string message) : base(message, new RecursiveSerializerInnerException())
        {
            this.Type = type;
        }

        internal RecursiveSerializerException(HashedType type, string message, Exception innerException) : base(message, new RecursiveSerializerInnerException(innerException))
        {
            this.Type = type;
        }
    }
}
