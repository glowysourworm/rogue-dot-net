using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;

using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Interface
{
    internal interface ISerializationPlan
    {
        /// <summary>
        /// Stores dictionary of unique references to reference-type objects
        /// </summary>
        IDictionary<HashedObjectInfo, SerializationNodeBase> UniqueReferenceDict { get; }

        /// <summary>
        /// Stores hashed type information for EVERY serialized type
        /// </summary>
        IDictionary<HashedType, HashedType> TypeDict { get; }

        /// <summary>
        /// Root node for the object graph
        /// </summary>
        SerializationNode RootNode { get; }
    }
}
