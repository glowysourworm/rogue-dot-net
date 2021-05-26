using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization
{
    public class RecursiveSerializerInnerException : Exception
    {
        public RecursiveSerializerInnerException() : base("Use of the Recursive Serializer is as follows:  1) You can use a parameterless constructor to" +
            " work with the serializer in DEFAULT mode. Otherwise, 3 methods are required in your class or struct:  GetPropertyDefinitions(PropertyPlanner), " +
            " GetProperties(PropertyWriter) and SetProperties(PropertyReader).")
        {
            
        }
    }
}
