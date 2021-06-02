using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal class SerializationPlanner<T>
    {
        // Reads properties from objects for this serialization plan
        PropertyWriter _writer;

        // TRACK TYPES
        Dictionary<HashedType, HashedType> _typeDict;

        // Creates basic object wrappers for serialization. TRACKS HASH REFERENCES!
        SerializationObjectFactory _factory;

        internal SerializationPlanner()
        {
            _writer = new PropertyWriter();
            _typeDict = new Dictionary<HashedType, HashedType>();
            _factory = new SerializationObjectFactory();
        }

        internal ISerializationPlan Plan(T theObject)
        {
            // Create wrapper for the object
            var wrappedObject = _factory.Create(theObject, typeof(T));

            // Create root node
            var node = CreateNode(wrappedObject);

            // Recurse
            Analyze(node);

            return new SerializationPlan(_factory.GetReferences(), 
                                         _typeDict, 
                                         _factory.GetAllSerializedObjects(), 
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
                    var wrappedProperty = _factory.Create(property.PropertyValue, property.PropertyType);

                    // Create node
                    var propertyNode = CreateNode(wrappedProperty);

                    // RECURSE
                    Analyze(propertyNode);

                    // STORE AS SUB-NODE
                    (node as SerializationCollectionNode).SubNodes.Add(propertyNode);
                }

                // READ ELEMENTS
                foreach (var child in (node.NodeObject as SerializationCollection).Collection)
                {
                    // Create wrapped object for the element
                    var wrappedChild = _factory.Create(child, (node.NodeObject as SerializationCollection).ElementType);

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
                    var wrappedProperty = _factory.Create(property.PropertyValue, property.PropertyType);

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

        private IEnumerable<PropertyStorageInfo> ReadProperties(SerializationObjectBase wrappedObject)
        {
            if (wrappedObject is SerializationObject)
            {
                var referenceObject = wrappedObject as SerializationObject;

                return referenceObject.GetProperties();
            }
            else if (wrappedObject is SerializationValue)
            {
                var valueObject = wrappedObject as SerializationValue;

                return valueObject.GetProperties();
            }
            else if (wrappedObject is SerializationCollection)
            {
                var collectionObject = wrappedObject as SerializationCollection;

                return collectionObject.GetProperties();
            }
            else
                throw new Exception("Invalid SerializationObjectBase type for reading properties");
        }
    }
}
