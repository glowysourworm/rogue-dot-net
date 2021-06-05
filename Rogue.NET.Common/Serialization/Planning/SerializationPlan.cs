using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Target;

using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal class SerializationPlan : ISerializationPlan
    {
        public SerializationNodeBase RootNode { get; private set; }

        public SimpleDictionary<ObjectInfo, SerializationObjectBase> UniqueReferenceDict { get; private set; }

        public SimpleDictionary<int, HashedType> ElementTypeDict { get; private set; }

        public IEnumerable<PropertySpecification> UniquePropertySpecifications { get; private set; }

        public SimpleDictionary<PropertySpecification, List<SerializationObjectBase>> PropertySpecificationGroups { get; private set; }

        public SerializationPlan(SimpleDictionary<ObjectInfo, SerializationObjectBase> referenceDict,
                                 SimpleDictionary<int, HashedType> elementTypeDict,
                                 IEnumerable<PropertySpecification> propertySpecifications,
                                 SimpleDictionary<PropertySpecification, List<SerializationObjectBase>> propertySpecificationGroups,
                                 SerializationNodeBase rootNode)
        {
            this.UniqueReferenceDict = referenceDict;
            this.UniquePropertySpecifications = propertySpecifications;
            this.ElementTypeDict = elementTypeDict;
            this.PropertySpecificationGroups = propertySpecificationGroups;
            this.RootNode = rootNode;
        }
    }
}
