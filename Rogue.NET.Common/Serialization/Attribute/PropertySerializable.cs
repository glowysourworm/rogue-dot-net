using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization.Attribute
{
    public class PropertySerializable : System.Attribute
    {
        public PropertySerializableMode Mode { get; set; }
    }
}
