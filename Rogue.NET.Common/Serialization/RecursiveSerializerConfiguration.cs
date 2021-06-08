using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization
{
    public class RecursiveSerializerConfiguration
    {
        /// <summary>
        /// This can occur when the type tree has changed in the code; and the serialized
        /// graph differs because a property has been REMOVED from the code tree.
        /// </summary>
        public bool IgnoreRemovedProperties { get; set; }
    }
}
