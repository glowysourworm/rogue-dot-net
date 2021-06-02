using System;

namespace Rogue.NET.Common.Serialization
{
    public class RecursiveSerializerInnerException : Exception
    {
        protected static readonly string UsageMessage = "Use of the Recursive Serializer is as follows:  1) You can use a parameterless constructor to" +
            " work with the serializer in DEFAULT mode. Otherwise, 1 method and 1 constructor are required in your class or struct:  " +
            " GetProperties(IPropertyWriter) and ctor(IPropertyReader).";

        public RecursiveSerializerInnerException() : base(RecursiveSerializerInnerException.UsageMessage)
        {

        }

        public RecursiveSerializerInnerException(Exception innerException) : base(RecursiveSerializerInnerException.UsageMessage, innerException) { }
    }
}
