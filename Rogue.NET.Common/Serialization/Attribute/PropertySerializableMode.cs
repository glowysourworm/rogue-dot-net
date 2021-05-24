using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization.Attribute
{
    public enum PropertySerializableMode
    {
        /// <summary>
        /// By default, the serializer will try and find PUBLIC properties with PUBLIC get / set methods. There must
        /// also be a PARAMETERLESS CONSTRUCTOR.
        /// </summary>
        Default,

        /// <summary>
        /// When the PropertySerializable attribute is present, there needs to be support for the SerializationStream
        /// PUBLIC CONSTRUCTOR (AND) the PUBLIC GetProperties(SerializationStream) methods.
        /// </summary>
        Specified
    }
}
