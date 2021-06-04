using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;
using Rogue.NET.Common.Serialization.Utility;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Component
{
    internal class SerializationPlanner<T>
    {
        // TRACK UNIQUE PROPERTY DEFINITIONS
        SimpleDictionary<PropertySpecification, PropertySpecification> _propertySpecifications;

        // GROUP SERIALIZED OBJECTS BY PROPERTY SPECIFICATION
        SimpleDictionary<PropertySpecification, List<SerializationObjectBase>> _propertySpecificationGroups;

        // Creates basic object wrappers for serialization. TRACKS HASH REFERENCES!
        SerializationObjectFactory _factory;

        // Creates primitive object infos according to HASHED TYPE RULES!
        readonly ObjectInfoResolver _resolver;

        internal SerializationPlanner(ObjectInfoResolver resolver)
        {
            _propertySpecifications = new SimpleDictionary<PropertySpecification, PropertySpecification>();
            _propertySpecificationGroups = new SimpleDictionary<PropertySpecification, List<SerializationObjectBase>>();
            _factory = new SerializationObjectFactory(resolver);
            _resolver = resolver;
        }

        internal ISerializationPlan Plan(T theObject)
        {
            // Create / validate root object
            var rootInfo = _resolver.Resolve(theObject, new HashedType(typeof(T)));

            // Create wrapper for the object
            var wrappedObject = _factory.Create(rootInfo);

            // Create root node
            var node = CreateNode(wrappedObject);

            // Recurse
            Analyze(node);

            return new SerializationPlan(_factory.GetReferences(), 
                                         _propertySpecifications.Values, 
                                         _propertySpecificationGroups,
                                         node);
        }

        private void Analyze(SerializationNodeBase node)
        {
            // Procedure
            //
            // 0) HALT RECURSION CHECK:  NULL, PRIMITIVE, REFERENCE object types
            // 1) Collections -> Read node properties (recursively) -> Read child objects (recursively)
            // 2) Property Nodes -> Read node properties (recursively)
            // 3) STORE REFERENCE
            //

            // NULL PRIMITIVE
            if (node.NodeObject is SerializationNullPrimitive)
                return;

            // NULL
            else if (node.NodeObject is SerializationNullObject)
                return;

            // PRIMITIVE
            else if (node.NodeObject is SerializationPrimitive)
                return;

            // REFERENCE (NOTE*** THESE ARE CREATED BY THE FACTORY!!) (HALTS RECURSION)
            else if (node.NodeObject is SerializationReference)
                return;

            // COLLECTION
            else if (node.NodeObject is SerializationCollection)
            {
                // READ PROPERTIES
                foreach (var property in ReadProperties(node.NodeObject))
                {
                    // Create wrapped object for the property
                    var wrappedProperty = _factory.Create(property.ResolvedInfo);

                    // Create node
                    var propertyNode = CreateNode(wrappedProperty);

                    // RECURSE
                    Analyze(propertyNode);

                    // STORE AS SUB-NODE
                    (node as SerializationCollectionNode).SubNodes.Add(propertyNode);
                }

                var collection = (node.NodeObject as SerializationCollection);
                var counter = 0;

                // READ ELEMENTS
                foreach (var item in collection.Collection)
                {
                    var elementType = collection.ResolvedElementTypes[counter++];

                    // RESOLVE OBJECT INFO
                    var childInfo = _resolver.Resolve(item, elementType);

                    // Create wrapped object for the element
                    var wrappedChild = _factory.Create(childInfo);

                    // Create node
                    var childNode = CreateNode(wrappedChild);

                    // RECURSE
                    Analyze(childNode);

                    // STORE AS ELEMENT (CHILD) NODE
                    (node as SerializationCollectionNode).Children.Add(childNode);
                }
            }

            // VALUE or OBJECT
            else if (node.NodeObject is SerializationValue ||
                     node.NodeObject is SerializationObject)
            {
                // READ PROPERTIES
                foreach (var property in ReadProperties(node.NodeObject))
                {
                    // Create wrapped object for the property
                    var wrappedProperty = _factory.Create(property.ResolvedInfo);

                    // Create node
                    var propertyNode = CreateNode(wrappedProperty);

                    // RECURSE
                    Analyze(propertyNode);

                    // STORE AS SUB-NODE
                    (node as SerializationNode).SubNodes.Add(propertyNode);
                }
            }   

            else
                throw new Exception("Unhandled SerializationObjectBase type SerializationPlanner.CreateNode");
        }

        private SerializationNodeBase CreateNode(SerializationObjectBase wrappedObject)
        {
            // NULL PRIMITIVE
            if (wrappedObject is SerializationNullPrimitive)
                return new SerializationNode(wrappedObject);

            // NULL
            if (wrappedObject is SerializationNullObject)
                return new SerializationNode(wrappedObject);

            // PRIMITIVE
            else if (wrappedObject is SerializationPrimitive)
                return new SerializationNode(wrappedObject);

            // REFERENCE
            else if (wrappedObject is SerializationReference)
                return new SerializationNode(wrappedObject);

            // VALUE
            else if (wrappedObject is SerializationValue)
                return new SerializationNode(wrappedObject);

            // OBJECT
            else if (wrappedObject is SerializationObject)
                return new SerializationNode(wrappedObject);

            // COLLECTION
            else if (wrappedObject is SerializationCollection)
                return new SerializationCollectionNode(wrappedObject);

            else
                throw new Exception("Unhandled SerializationObjectBase type SerializationPlanner.CreateNode");
        }

        private IEnumerable<PropertyResolvedInfo> ReadProperties(SerializationObjectBase wrappedObject)
        {
            if (wrappedObject is SerializationCollection ||
                wrappedObject is SerializationValue ||
                wrappedObject is SerializationObject)
            {
                // INITIALIZE PROPERTY WRITER
                var writer = PropertyWriterFactory.CreateAndResolve(_resolver, wrappedObject);

                // TRACK PROPERTY SPECIFICATIONS
                var specification = writer.GetPropertySpecification();

                if (!_propertySpecifications.ContainsKey(specification))
                    _propertySpecifications.Add(specification, specification);

                // ADD TO PROPERTY SPECIFICATION GROUP FOR THIS OBJECT
                if (!_propertySpecificationGroups.ContainsKey(specification))
                    _propertySpecificationGroups.Add(specification, new List<SerializationObjectBase>() { wrappedObject });

                else
                    _propertySpecificationGroups[specification].Add(wrappedObject);

                return writer.GetProperties();
            }
            else
                throw new Exception("Invalid SerializationObjectBase type for reading properties");


        }
    }
}
