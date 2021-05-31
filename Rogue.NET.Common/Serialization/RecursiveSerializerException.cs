using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections;

namespace Rogue.NET.Common.Serialization
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

        //public override IDictionary Data
        //{
        //    get
        //    {
        //        // this.Data.Add(this.Type.DeclaringType, this.Type);

        //        return this.Data;
        //    }
        //}
    }
}
