using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Component
{
    internal class SerializationPlanner<T>
    {
        // TRACK UNIQUE PROPERTY DEFINITIONS
        SimpleDictionary<PropertySpecification, PropertySpecification> _propertySpecifications;

        // GROUP SERIALIZED OBJECTS BY PROPERTY SPECIFICATION
        SimpleDictionary<PropertySpecification, List<SerializedNodeBase>> _propertySpecificationGroups;

        // Creates basic object wrappers for serialization. TRACKS HASH REFERENCES!
        SerializationObjectFactory _factory;

        // Creates primitive object infos according to HASHED TYPE RULES!
        readonly HashedTypeResolver _resolver;

        // Additional collection element types
        SimpleDictionary<int, HashedType> _elementTypeDict;

        internal SerializationPlanner(HashedTypeResolver resolver)
        {
            _propertySpecifications = new SimpleDictionary<PropertySpecification, PropertySpecification>();
            _propertySpecificationGroups = new SimpleDictionary<PropertySpecification, List<SerializedNodeBase>>();
            _elementTypeDict = new SimpleDictionary<int, HashedType>();
            _factory = new SerializationObjectFactory();
            _resolver = resolver;
        }

        internal ISerializationPlan Plan(T theObject)
        {
            // Create / validate root object
            var rootType = _resolver.Resolve(theObject, new HashedType(typeof(T)));

            // Create root node
            var node = _factory.Create(theObject, rootType);

            // Recurse
            Analyze(node);

            return new SerializationPlan(_factory.GetReferenceObjects(),
                                         _elementTypeDict,
                                         _propertySpecifications.Values,
                                         _propertySpecificationGroups,
                                         node);
        }

        private void Analyze(SerializedNodeBase node)
        {
            // Procedure
            //
            // 0) HALT RECURSION CHECK:  NULL, PRIMITIVE, REFERENCE object types
            // 1) Collections -> Read node properties (recursively) -> Read child objects (recursively)
            // 2) Property Nodes -> Read node properties (recursively)
            // 3) STORE REFERENCE
            //

            // LEAF NODE -> Halts Recursion
            if (node is SerializedLeafNode)
                return;

            // REFERENCE NODE -> Halts Recursion
            else if (node is SerializedReferenceNode)
                return;

            // COLLECTION
            else if (node is SerializedCollectionNode)
            {
                // READ PROPERTIES
                foreach (var property in ReadProperties(node))
                {
                    // Create node for the property
                    var propertyNode = _factory.Create(property.ResolvedObject, property.ResolvedType);

                    // RECURSE
                    Analyze(propertyNode);

                    // STORE AS SUB-NODE
                    (node as SerializedCollectionNode).SubNodes.Add(propertyNode);
                }

                var collection = (node as SerializedCollectionNode);

                // STORE ADDITIONAL ELEMENT DECLARING TYPES
                if (!_elementTypeDict.ContainsKey(collection.ElementDeclaringType.GetHashCode()))
                    _elementTypeDict.Add(collection.ElementDeclaringType.GetHashCode(), collection.ElementDeclaringType);

                // READ ELEMENTS
                foreach (var item in collection.Collection)
                {
                    // RESOLVE OBJECT TYPE
                    var childType = _resolver.Resolve(item, collection.ElementDeclaringType);

                    // Create child node for the element
                    var childNode = _factory.Create(item, childType);

                    // RECURSE
                    Analyze(childNode);

                    // STORE AS ELEMENT (CHILD) NODE
                    (node as SerializedCollectionNode).CollectionNodes.Add(childNode);
                }
            }

            // OBJECT
            else if (node is SerializedObjectNode)
            {
                // READ PROPERTIES
                foreach (var property in ReadProperties(node))
                {
                    // Create wrapped object for the property
                    var propertyNode = _factory.Create(property.ResolvedObject, property.ResolvedType);

                    // RECURSE
                    Analyze(propertyNode);

                    // STORE AS SUB-NODE
                    (node as SerializedObjectNode).SubNodes.Add(propertyNode);
                }
            }

            else
                throw new Exception("Unhandled SerializedNodeBase type SerializationPlanner.CreateNode");
        }

        private IEnumerable<PropertyResolvedInfo> ReadProperties(SerializedNodeBase wrappedObject)
        {
            if (wrappedObject is SerializedCollectionNode ||
                wrappedObject is SerializedObjectNode)
            {
                // INITIALIZE PROPERTY WRITER
                var writer = PropertyWriterFactory.CreateAndResolve(_resolver, wrappedObject);

                // TRACK PROPERTY SPECIFICATIONS
                var specification = writer.GetPropertySpecification();

                if (!_propertySpecifications.ContainsKey(specification))
                    _propertySpecifications.Add(specification, specification);

                // ADD TO PROPERTY SPECIFICATION GROUP FOR THIS OBJECT
                if (!_propertySpecificationGroups.ContainsKey(specification))
                    _propertySpecificationGroups.Add(specification, new List<SerializedNodeBase>() { wrappedObject });

                else
                    _propertySpecificationGroups[specification].Add(wrappedObject);

                return writer.GetProperties();
            }
            else
                throw new Exception("Invalid SerializationObjectBase type for reading properties");
        }
    }
}
