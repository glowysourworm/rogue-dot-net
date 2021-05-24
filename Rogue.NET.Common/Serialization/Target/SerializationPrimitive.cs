using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// Defines OUR "Primitive" types - which should be basic value types (non-struct). Each of
    /// these MUST have a Formatter that we define for supporting serialization. 
    /// </summary>
    internal class SerializationPrimitive : SerializationObjectBase
    {
        public SerializationPrimitive(HashedObjectInfo objectInfo) : base(objectInfo)
        {
        }

        internal override IEnumerable<PropertyStorageInfo> GetProperties(PropertyReader reader)
        {
            throw new Exception("Invalid use of GetProperties for SerializationPrimitive");
        }
    }
}
