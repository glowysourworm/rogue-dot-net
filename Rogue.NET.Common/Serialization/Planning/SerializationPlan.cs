using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Target;

using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal class SerializationPlan : ISerializationPlan
    {
        public SerializedNodeBase RootNode { get; private set; }

        public IEnumerable<SerializedObjectNode> ReferenceObjects { get; private set; }

        public SimpleDictionary<int, HashedType> ElementTypeDict { get; private set; }

        public IEnumerable<PropertySpecification> UniquePropertySpecifications { get; private set; }

        public SimpleDictionary<PropertySpecification, List<SerializedNodeBase>> PropertySpecificationGroups { get; private set; }

        public SerializationPlan(IEnumerable<SerializedObjectNode> referenceObjects,
                                 SimpleDictionary<int, HashedType> elementTypeDict,
                                 IEnumerable<PropertySpecification> propertySpecifications,
                                 SimpleDictionary<PropertySpecification, List<SerializedNodeBase>> propertySpecificationGroups,
                                 SerializedNodeBase rootNode)
        {
            this.ReferenceObjects = referenceObjects;
            this.UniquePropertySpecifications = propertySpecifications;
            this.ElementTypeDict = elementTypeDict;
            this.PropertySpecificationGroups = propertySpecificationGroups;
            this.RootNode = rootNode;
        }
    }
}
