using Rogue.NET.Common.Collection;
using Rogue.NET.Common.CustomException;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rogue.NET.Common.Serialization.Component
{
    internal class DeserializationResolver
    {
        // Collection of UNIQUE REFERENCE OBJECT that HAVE BEEN DESERIALIZED -> By Id artifact from serialization
        SimpleDictionary<int, DeserializedObjectNode> _deserializedObjectcs;

        // DESERIALIZATION ARTIFACT - ALL LOADED TYPES
        SimpleDictionary<int, HashedType> _typeTable;

        internal DeserializationResolver()
        {
            _deserializedObjectcs = new SimpleDictionary<int, DeserializedObjectNode>();
        }

        internal SimpleDictionary<int, DeserializedObjectNode> GetDeserializedObjects()
        {
            return _deserializedObjectcs;
        }

        internal DeserializedNodeBase Resolve(DeserializedNodeBase node, SimpleDictionary<int, HashedType> typeTable)
        {
            _typeTable = typeTable;

            ResolveImpl(node);

            return node;
        }


        // Recursively resolves the graph
        private void ResolveImpl(DeserializedNodeBase node)
        {
            // Procedure:  Recursively collect PropertyStorageInfo objects to use with reflection. At
            //             the end of the loop call SetValue on the current node's object. Primitives
            //             for leaves should then be ready for resolving the node.
            //
            //             After node is resolved - STORE REFERENCE AS DESERIALIZED OBJECT ONLY IF IT
            //             IS OF A REFERENCE TYPE:  
            //
            //             DeserializedCollectionNode
            //             DeserializedObjectNode
            //

            // COLLECTION
            if (node is DeserializedCollectionNode)
            {               
                var collectionNode = node as DeserializedCollectionNode;

                var collectionProperties = new List<PropertyResolvedInfo>();

                // Loop properties: Collect property data recursively -> Store back on node
                foreach (var subNode in collectionNode.SubNodes)
                {
                    // RESOLVE (AND / OR) RECURSE
                    var propertyInfo = ResolveNodeRecurse(subNode);

                    collectionProperties.Add(propertyInfo);
                }

                // Set properties using DeserializationObjectBase
                collectionNode.Construct(collectionProperties);

                var resolvedChildNodes = new List<PropertyResolvedInfo>();

                // Iterate Elements -> Resolve recursively
                for (int index = 0; index < collectionNode.Count; index++)
                {
                    var childNode = collectionNode.CollectionNodes[index];

                    // RECURSE CHILD NODES (NOTE*** Child nodes are not properties)
                    var propertyInfo = ResolveNodeRecurse(childNode);

                    resolvedChildNodes.Add(propertyInfo);
                }

                // FINALIZE COLLECTION (MUST HAVE CALLED Construct())
                //                
                if (resolvedChildNodes.Any())
                    collectionNode.FinalizeCollection(resolvedChildNodes);

                // STORE REFERENCE (OBJECT IS READY!)
                _deserializedObjectcs.Add(collectionNode.ReferenceId, collectionNode);
            }

            // OBJECT
            else if (node is DeserializedObjectNode)
            {
                var nextNode = node as DeserializedObjectNode;

                // Properties to RESOLVE
                var properties = new List<PropertyResolvedInfo>();

                // Loop properties: Collect property data recursively -> Store back on node
                foreach (var subNode in nextNode.SubNodes)
                {
                    // RESOLVE (AND / OR) RECURSE
                    var propertyInfo = ResolveNodeRecurse(subNode);

                    properties.Add(propertyInfo);
                }

                // Construct() -> Set properties 
                nextNode.Construct(properties);

                // STORE REFERENCE (OBJECT IS READY!)
                _deserializedObjectcs.Add(nextNode.ReferenceId, nextNode);
            }

            // REFERENCE
            else if (node is DeserializedReferenceNode)
            {
                throw new Exception("Trying to recurse on DeserializedReferenceNode:  PropertyDeserializer.DeserializeRecurse");
            }

            // NULL LEAF (NULL PRIMITIVE (or) NULL REFERENCE)
            else if (node is DeserializedNullLeafNode)
            {
                throw new Exception("Trying to recurse on DeserializedNullLeafNode:  PropertyDeserializer.DeserializeRecurse");
            }

            // LEAF (PRIMITIVE)
            else if (node is DeserializedLeafNode)
            {
                throw new Exception("Trying to recurse on DeserializedLeafNode:  PropertyDeserializer.DeserializeRecurse");
            }

            else
                throw new Exception("Unhandled DeserializationNodeBase type:  PropertyDeserializer.DeserializeRecurse");
        }

        /// <summary>
        /// Attempts to RESOLVE the property defined in the DeserializationNodeBase. This will RECURSE Resolve() to
        /// fill in the node property data.
        /// </summary>
        private PropertyResolvedInfo ResolveNodeRecurse(DeserializedNodeBase node)
        {
            // Calculate the PropertyInfo
            PropertyInfo reflectionInfo = null;

            if (node.Property == PropertyDefinition.Empty ||
                node.Property == PropertyDefinition.CollectionElement ||
                node.Property.IsUserDefined)
                reflectionInfo = null;

            else
                reflectionInfo = node.Property.GetReflectedInfo();

            // NULL LEAF (NULL REFERENCE, NULL PRIMITIVE)
            if (node is DeserializedNullLeafNode)
                return new PropertyResolvedInfo(reflectionInfo)
                {
                    PropertyName = node.Property.PropertyName,
                    ResolvedObject = null,
                    ResolvedType = node.Type,
                    IsUserDefined = node.Property.IsUserDefined
                };

            // LEAF (PRIMITIVE)
            else if (node is DeserializedLeafNode)
                return new PropertyResolvedInfo(reflectionInfo)
                {
                    PropertyName = node.Property.PropertyName,
                    ResolvedObject = node.Resolve(),
                    ResolvedType = node.Type,
                    IsUserDefined = node.Property.IsUserDefined
                };

            // REFERENCE - MUST HAVE PREVIOUSLY DESERIALIZED OBJECT TO RESOLVE!
            else if (node is DeserializedReferenceNode)
            {
                var referenceNode = node as DeserializedReferenceNode;

                // Check the node object - which contains the REFERENCE ID
                if (!_deserializedObjectcs.ContainsKey(referenceNode.ReferenceId))
                    throw new FormattedException("UN-RESOLVED REFERENCE:  Id={0}, Type={1}", referenceNode.ReferenceId, node.Type.DeclaringType);

                // Return OTHER reference using DeserializationObjectBase.Resolve() (FORCES VAILDATION)
                return new PropertyResolvedInfo(reflectionInfo)
                {
                    PropertyName = node.Property.PropertyName,
                    ResolvedObject = _deserializedObjectcs[referenceNode.ReferenceId].Resolve(),
                    ResolvedType = node.Type,
                    IsUserDefined = node.Property.IsUserDefined
                };
            }

            // ELSE -> RECURSE
            else
            {
                // OBJECT, COLLECTION
                ResolveImpl(node);

                var objectNode = node as DeserializedObjectNode;

                if (!_deserializedObjectcs.ContainsKey(objectNode.ReferenceId))
                    throw new Exception("Unresolved subnode PropertyDeserializer.ResolveNodeRecurse");


                return new PropertyResolvedInfo(reflectionInfo)
                {
                    PropertyName = node.Property.PropertyName,
                    ResolvedObject = _deserializedObjectcs[node.GetHashCode()].Resolve(),
                    ResolvedType = node.Type,
                    IsUserDefined = node.Property.IsUserDefined
                };
            }
        }
    }
}
