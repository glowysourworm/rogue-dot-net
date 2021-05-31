using Rogue.NET.Common.CustomException;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal class DeserializationResolver
    {
        // Collection of UNIQUE objects that HAVE BEEN DESERIALIZED
        Dictionary<HashedObjectReference, DeserializationObjectBase> _deserializedObjectcs;

        internal DeserializationResolver()
        {
            _deserializedObjectcs = new Dictionary<HashedObjectReference, DeserializationObjectBase>();
        }

        internal IDictionary<HashedObjectReference, DeserializationObjectBase> GetDeserializedObjects()
        {
            return _deserializedObjectcs;
        }

        internal DeserializationNodeBase Resolve(DeserializationNodeBase node)
        {
            ResolveImpl(node);

            return node;
        }


        // Recursively resolves the graph
        private void ResolveImpl(DeserializationNodeBase node)
        {
            // Procedure:  Recursively collect PropertyStorageInfo objects to use with reflection. At
            //             the end of the loop call SetValue on the current node's object. Primitives
            //             for leaves should then be ready for resolving the node.
            //
            //             After node is resolved - STORE REFERENCE AS DESERIALIZED OBJECT ONLY IF IT
            //             IS OF A REFERENCE TYPE:  
            //
            //             DeserializationObject
            //             DeserializationValue
            //             DeserializationCollection
            //

            // COLLECTION
            if (node is DeserializationCollectionNode)
            {
                var collectionNode = node as DeserializationCollectionNode;

                var collectionProperties = new List<PropertyResolvedInfo>();

                // Loop properties: Collect property data recursively -> Store back on node
                foreach (var subNode in collectionNode.SubNodes)
                {
                    // RESOLVE (AND / OR) RECURSE
                    var propertyInfo = ResolveNodeRecurse(subNode);

                    collectionProperties.Add(propertyInfo);
                }

                // Set properties using DeserializationObjectBase
                if (collectionProperties.Any())
                    collectionNode.NodeObject.WriteProperties(collectionProperties);

                var resolvedChildNodes = new List<HashedObjectInfo>();

                // Iterate Elements -> Resolve recursively
                for (int index = 0; index < collectionNode.Count; index++)
                {
                    var childNode = collectionNode.Children[index];

                    if (childNode.Property != PropertyDefinition.CollectionElement)
                        throw new Exception("Invalid Property Definition for collection element:  DeserializationResolver.cs");

                    // RECURSE TO RESOLVE (Child Nodes are NON-PROPERTY NODES)
                    Resolve(childNode);

                    // NOTE*** Data from these child nodes MUST BE PASSED to the DeserializationObjectBase WITH REFERENCES
                    //         RESOLVED. 
                    //
                    //         This is because of references to OTHER resolved objects already in our posession. 
                    //
                    //         To solve this, just check for hashed objects that are in the deserialzied object dictionary.
                    //

                    // REFERENCE
                    if (_deserializedObjectcs.ContainsKey(childNode.NodeObject.Reference))
                        resolvedChildNodes.Add(_deserializedObjectcs[childNode.NodeObject.Reference].Resolve());

                    // ANY OTHER
                    else
                        resolvedChildNodes.Add(childNode.NodeObject.Resolve());
                }

                // FINALIZE COLLECTION
                (collectionNode.NodeObject as DeserializationCollection).FinalizeCollection(resolvedChildNodes);
            }
            // NODE
            else if (node is DeserializationNode)
            {
                var nextNode = node as DeserializationNode;

                // Properties to RESOLVE
                var properties = new List<PropertyResolvedInfo>();

                // Loop properties: Collect property data recursively -> Store back on node
                foreach (var subNode in nextNode.SubNodes)
                {
                    // RESOLVE (AND / OR) RECURSE
                    var propertyInfo = ResolveNodeRecurse(subNode);

                    properties.Add(propertyInfo);
                }

                // Set properties using DeserializationObjectBase
                if (nextNode.NodeObject is DeserializationCollection ||
                    nextNode.NodeObject is DeserializationObject ||
                    nextNode.NodeObject is DeserializationValue)
                    nextNode.NodeObject.WriteProperties(properties);
            }
            else
                throw new Exception("Unhandled DeserializationNodeBase type:  PropertyDeserializer.DeserializeRecurse");

            var isReferenced = node.NodeObject is DeserializationCollection ||
                               node.NodeObject is DeserializationObject ||
                               node.NodeObject is DeserializationValue;

            // CHECK FOR DUPLICATE REFERENCES
            if (_deserializedObjectcs.ContainsKey(node.NodeObject.Reference) && isReferenced)
                throw new Exception("Duplicate reference for deserialized object:  " + node.NodeObject.Reference.Type);

            // STORE REFERENCE (OBJECT IS READY!)
            if (isReferenced)
                _deserializedObjectcs.Add(node.NodeObject.Reference, node.NodeObject);
        }

        /// <summary>
        /// Attempts to RESOLVE the property defined in the DeserializationNodeBase. This will RECURSE Resolve() to
        /// fill in the node property data.
        /// </summary>
        private PropertyResolvedInfo ResolveNodeRecurse(DeserializationNodeBase node)
        {
            // NULL PRIMITIVE
            if (node.NodeObject is DeserializationNullPrimitive)
                return new PropertyResolvedInfo()
                {
                    PropertyName = node.Property.PropertyName,
                    PropertyType = node.Property.PropertyType,
                    ResolvedInfo = node.NodeObject.Resolve()
                };

            // NULL
            else if (node.NodeObject is DeserializationNullReference)
                return new PropertyResolvedInfo()
                {
                    PropertyName = node.Property.PropertyName,
                    PropertyType = node.Property.PropertyType,
                    ResolvedInfo = node.NodeObject.Resolve()
                };

            // PRIMITIVE
            else if (node.NodeObject is DeserializationPrimitive)
                return new PropertyResolvedInfo()
                {
                    PropertyName = node.Property.PropertyName,
                    PropertyType = node.Property.PropertyType,
                    ResolvedInfo = node.NodeObject.Resolve()
                };

            // REFERENCE - MUST HAVE PREVIOUSLY DESERIALIZED OBJECT TO RESOLVE!
            else if (node.NodeObject is DeserializationReference)
            {
                // Check the node object - which contains the REFERENCE HASH
                if (!_deserializedObjectcs.ContainsKey(node.NodeObject.Reference))
                    throw new FormattedException("UN-RESOLVED REFERENCE:  Hash={0}, Type={1}", node.NodeObject.Reference.HashCode, node.NodeObject.Reference.Type.DeclaringType);

                // Return OTHER reference using DeserializationObjectBase.Resolve() (FORCES VAILDATION)
                return new PropertyResolvedInfo()
                {
                    PropertyName = node.Property.PropertyName,
                    PropertyType = node.Property.PropertyType,
                    ResolvedInfo = _deserializedObjectcs[node.NodeObject.Reference].Resolve()
                };
            }

            // ELSE -> RECURSE
            else
            {
                // VALUE, OBJECT, COLLECTION
                Resolve(node);

                if (!_deserializedObjectcs.ContainsKey(node.NodeObject.Reference))
                    throw new Exception("Unresolved subnode PropertyDeserializer.ResolveNodeRecurse");

                return new PropertyResolvedInfo()
                {
                    PropertyName = node.Property.PropertyName,
                    PropertyType = node.Property.PropertyType,
                    ResolvedInfo = _deserializedObjectcs[node.NodeObject.Reference].Resolve()
                };
            }
        }
    }
}
