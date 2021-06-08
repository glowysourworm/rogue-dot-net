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
        public IEnumerable<SerializedObjectNode> ReferenceObjects { get; }

        /// <summary>
        /// Additional element DECLARING types built by the serialization planner
        /// </summary>
        SimpleDictionary<int, HashedType> ElementTypeDict { get; }

        /// <summary>
        /// Stores collection of unique property specifications
        /// </summary>
        IEnumerable<PropertySpecification> UniquePropertySpecifications { get; }

        /// <summary>
        /// Stores grouping of objects by property specification
        /// </summary>
        SimpleDictionary<PropertySpecification, List<SerializedNodeBase>> PropertySpecificationGroups { get; }

        /// <summary>
        /// Root node for the object graph
        /// </summary>
        SerializedNodeBase RootNode { get; }
    }
}
