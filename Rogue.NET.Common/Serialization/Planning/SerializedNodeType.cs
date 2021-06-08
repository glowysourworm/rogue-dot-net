using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization.Planning
{
    public enum SerializedNodeType : byte
    {
        /// <summary>
        /// Serializer should store [ NullPrimitive = 0, Reference HashedType ]
        /// </summary>
        NullPrimitive = 0,

        /// <summary>
        /// Serializer should store [ Null = 0, Reference HashedType ]
        /// </summary>
        Null = 1,

        /// <summary>
        /// Serializer should store [ Primitive = 1, HashedType, Value ]
        /// </summary>
        Primitive = 2,

        /// <summary>
        /// Serializer should store [ Object = 3, Hash Info ] (Recruse Sub-graph)
        /// </summary>
        Object = 3,

        /// <summary>
        /// Serializer should store [ Reference = 4, Hash Info ]
        /// </summary>
        Reference = 4,

        /// <summary>
        /// Serializer should store [ Collection = 5, Collection Type, Child Count ] (loop) Children (Recruse Sub-graphs)
        /// </summary>
        Collection = 5
    }
}
