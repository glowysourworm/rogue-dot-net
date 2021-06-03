using Rogue.NET.Common.Collection;
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
        SimpleDictionary<HashedObjectInfo, SerializationObjectBase> UniqueReferenceDict { get; }

        /// <summary>
        /// Collection of ALL SERIAILZED OBJECTS in the plan
        /// </summary>
        IEnumerable<SerializationObjectBase> AllSerializedObjects { get; }

        /// <summary>
        /// Root node for the object graph
        /// </summary>
        SerializationNodeBase RootNode { get; }
    }
}
