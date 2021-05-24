using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal class SerializationPlanner<T>
    {
        // Reads properties from objects for this serialization plan
        PropertyReader _reader;

        // TRACK REFERENCES BY HASH CODE
        Dictionary<HashedObjectInfo, SerializationNodeBase> _referenceDict;

        // Creates basic object wrappers for serialization. TRACKS HASH REFERENCES!
        SerializationObjectFactory _factory;

        public SerializationPlanner()
        {
            _reader = new PropertyReader();
            _referenceDict = new Dictionary<HashedObjectInfo, SerializationNodeBase>();
            _factory = new SerializationObjectFactory();
        }

        public ISerializationPlan Plan(T theObject)
        {
            if (ReferenceEquals(theObject, null))
                throw new ArgumentException("Trying to serialize a null object reference:  SerializationPlanner.Plan");

            var wrappedObject = _factory.Create(theObject, typeof(T));
            var node = new SerializationNode(wrappedObject);

            // Recursively analyze the graph
            foreach (var property in ReadProperties(wrappedObject))
            {
                var subNode = Analyze(property);

                if (subNode != null)
                    node.SubNodes.Add(subNode);
            }

            // STORE REFERENCE TO BASE OBJECT (KEEP TRACK!)
            _referenceDict.Add(wrappedObject.ObjectInfo, node);

            return new SerializationPlan(_referenceDict, node);
        }

        protected SerializationNodeBase Analyze(PropertyStorageInfo info)
        {
            // Create wrapped object - HANDLES NULLS
            var wrappedObject = _factory.Create(info.PropertyValue, info.PropertyType);

            // NULL
            if (wrappedObject is SerializationNullObject)
            {
                return new SerializationNode(wrappedObject);
            }

            // PRIMITIVE
            if (wrappedObject is SerializationPrimitive)
            {
                return new SerializationNode(wrappedObject);
            }

            // Create reference, and node
            var node = (wrappedObject is SerializationCollection) ? (SerializationNodeBase)new SerializationCollectionNode(wrappedObject) :
                                                                    (SerializationNodeBase)new SerializationNode(wrappedObject);

            // VALUE, OBJECT, COLLECTION -> ADD REFERENCE (OR) HALT RECURSION!!!
            if (wrappedObject is SerializationValue ||
                wrappedObject is SerializationObject ||
                wrappedObject is SerializationCollection)
            {
                if (!_referenceDict.ContainsKey(wrappedObject.ObjectInfo))
                    _referenceDict.Add(wrappedObject.ObjectInfo, node);

                // HALT RECURSION -> RETURN REFERENCED NODE
                else
                    return _referenceDict[wrappedObject.ObjectInfo];
            }

            // COLLECTION
            if (wrappedObject is SerializationCollection)
            {
                // NOTE*** Serializer will store a reference to the collection with the COUNT
                //         included. This will allow deserialization of the elements in the
                //         collection.
                //

                var wrappedCollection = wrappedObject as SerializationCollection;
                var collectionNode = node as SerializationCollectionNode;

                // ENUMERATE -> ANALYZE
                foreach (var element in wrappedCollection.Collection)
                {
                    // Create child object
                    var wrappedChild = _factory.Create(element, wrappedCollection.ElementType);

                    // Analyze
                    foreach (var property in ReadProperties(wrappedChild))
                    {
                        // RECURSE
                        collectionNode.Children.Add(Analyze(property));
                    }
                }
            }

            // OBJECT / VALUE -> RECURSE
            else if (wrappedObject is SerializationValue ||
                     wrappedObject is SerializationObject)
            {
                var referenceNode = node as SerializationNode;

                // Store sub-nodes for each sub-property
                foreach (var property in ReadProperties(referenceNode.NodeObject))
                {
                    referenceNode.SubNodes.Add(Analyze(property));
                }
            }

            return node;
        }

        private IEnumerable<PropertyStorageInfo> ReadProperties(SerializationObjectBase wrappedObject)
        {
            if (wrappedObject is SerializationObject)
            {
                var referenceObject = wrappedObject as SerializationObject;

                return referenceObject.GetProperties(_reader);
            }
            else if (wrappedObject is SerializationValue)
            {
                var valueObject = wrappedObject as SerializationValue;

                return valueObject.GetProperties(_reader);
            }
            // EXPECTING NESTED COLLECTIONS TO CRASH    
            else
                throw new Exception("Invalid SerializationObjectBase type for reading properties");


        }
    }
}
