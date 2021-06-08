using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Formatter;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;
using Rogue.NET.Common.Serialization.Utility;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Component
{
    /// <summary>
    /// Component that creates wrappers for serialization objects while keeping track of references and
    /// avoiding CIRCULAR references.
    /// </summary>
    internal class SerializationObjectFactory
    {
        // Referenced by object.GetHashCode() + HashedType.GetHashCode()
        SimpleDictionary<int, SerializedNodeBase> _primitiveReferences;

        // Referenced by object.GetHashCode() -> ReferenceEquals( , )
        SimpleDictionary<object, List<SerializedObjectNode>> _objectReferences;

        List<SerializedNodeBase> _allObjects;

        internal SerializationObjectFactory()
        {
            _primitiveReferences = new SimpleDictionary<int, SerializedNodeBase>();
            _objectReferences = new SimpleDictionary<object, List<SerializedObjectNode>>();
            _allObjects = new List<SerializedNodeBase>();
        }

        internal IEnumerable<SerializedNodeBase> GetAllSerializedObjects()
        {
            return _allObjects;
        }

        internal IEnumerable<SerializedObjectNode> GetReferenceObjects()
        {
            return _objectReferences.Values
                                    .SelectMany(list => list)
                                    .Actualize();
        }

        private int CreatePrimitiveReference(object theObject, HashedType resolvedType)
        {
            if (ReferenceEquals(theObject, null))
            {
                return resolvedType.GetHashCode();
            }
            else
                return theObject.CreateHashCode(theObject, resolvedType);
        }

        /// <summary>
        /// HANDLES NULLS!  Wraps object for serialization - locating methods for ctor and get method for reproducing object. 
        /// The object type is referenced for creating a wrapper for a null ref object.
        /// </summary>
        internal SerializedNodeBase Create(object theObject, HashedType resolvedType)
        {
            // PRIMITIVE TYPE:    Treated as primitive value type - MUST HAVE FORMATTER!
            //
            // ATTRIBUTE MARKED:  Treat as explicitly defined. Locate constructor and get method
            //
            // REFERENCE TYPE:    Treated as default 1) ctor and get method (or) 2) parameterless ctor
            //
            // VALUE TYPE:        Treated as default - Serializer will try locating a formatter. THROWS
            //                    EXCEPTION OTHERWISE!
            //
            // COLLECTION:        Validate for the types we're supporting:  List.
            //
            // NULL REFERENCE:    Wrapped by type
            //

            // FOR LEAF NODES -> RETURN EXISTING REFERENCES
            var primitiveReference = CreatePrimitiveReference(theObject, resolvedType);
            var isPrimitive = FormatterFactory.IsPrimitiveSupported(resolvedType.GetImplementingType());

            // PRIMITIVE NULL
            if (ReferenceEquals(theObject, null) && isPrimitive)
            {
                if (_primitiveReferences.ContainsKey(primitiveReference))
                    return _primitiveReferences[primitiveReference];

                return FinalizePrimitive(new SerializedLeafNode(SerializedNodeType.NullPrimitive, resolvedType));
            }

            // NULL
            if (ReferenceEquals(theObject, null))
            {
                if (_primitiveReferences.ContainsKey(primitiveReference))
                    return _primitiveReferences[primitiveReference];

                return FinalizePrimitive(new SerializedLeafNode(SerializedNodeType.Null, resolvedType));
            }

            // STRINGS IMPLEMENT IEnumerable!
            var isCollection = theObject.ImplementsInterface<IList>() && !isPrimitive;

            // PRIMITIVE
            if (isPrimitive)
            {
                if (_primitiveReferences.ContainsKey(primitiveReference))
                    return _primitiveReferences[primitiveReference];

                return FinalizePrimitive(new SerializedLeafNode(SerializedNodeType.Primitive, resolvedType, theObject));
            }

            // SEARCH FOR REFERENCE 
            var referenceNode = FindReference(theObject);

            // REFERENCE (Is reference type AND is OLD REFERENCE)
            if (referenceNode != null)
                return new SerializedReferenceNode(referenceNode.Id, resolvedType, RecursiveSerializerStore.GetMemberInfo(resolvedType));

            // ***** THE REST GET ADDED TO THE REFERENCE DICT

            // COLLECTION (STRINGS IMPLEMENT IEnumerable!)
            if (isCollection)
                return FinalizeObject(CreateCollectionNode(theObject, resolvedType));

            // OBJECT
            else
                return FinalizeObject(new SerializedObjectNode(SerializedNodeType.Object, resolvedType, theObject, RecursiveSerializerStore.GetMemberInfo(resolvedType)));
        }

        // Referenced by object.GetHashCode() -> ReferenceEquals( , )
        //
        private SerializedObjectNode FindReference(object theObject)
        {
            // NARROW SEARCH BY HASH CODE
            if (_objectReferences.ContainsKey(theObject))
            {
                // Iterate reference type nodes
                foreach (var objectNode in _objectReferences[theObject])
                {
                    // MUST USE MSFT REFERENCE EQUALS!!!
                    if (ReferenceEquals(objectNode.GetObject(), theObject))
                        return objectNode;
                }
            }

            return null;
        }

        private SerializedCollectionNode CreateCollectionNode(object theObject, HashedType resolvedType)
        {
            // Procedure
            //
            // 0) Determine SerializationMode by checking for constructor and get method
            // 1) Get element type
            // 2) Check that collection type is supported:  List<T>
            // 3) Create the result
            //

            // Validates type and SerializationMode
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(resolvedType);

            if (!resolvedType.GetImplementingType().IsGenericType)
                throw new RecursiveSerializerException(resolvedType, "PropertySerializer only supports Arrays, and Generic Collections:  List<T>");

            else if (theObject.ImplementsInterface<IList>())
            {
                var argument = (theObject as IList).GetType().GetGenericArguments()[0];

                if (argument == null)
                    throw new Exception("Invalid IList argument for PropertySerializer: " + resolvedType.DeclaringType);

                var list = theObject as IList;
                var elementDeclaringType = new HashedType(argument);

                return new SerializedCollectionNode(resolvedType, memberInfo, list, list.Count, CollectionInterfaceType.IList, elementDeclaringType);
            }
            else
                throw new RecursiveSerializerException(resolvedType, "PropertySerializer only supports Arrays, and Generic Collections:  List<T>");
        }

        // TRACK REFERENCES AND ALL OBJECTS -> RETURN
        private SerializedNodeBase FinalizePrimitive(SerializedLeafNode result)
        {
            _allObjects.Add(result);

            var primitiveReference = CreatePrimitiveReference(result.GetObject(), result.Type);

            if (!_primitiveReferences.ContainsKey(primitiveReference))
                _primitiveReferences.Add(primitiveReference, result);

            return result;
        }

        // TRACK REFERENCES AND ALL OBJECTS -> RETURN
        private SerializedNodeBase FinalizeObject(SerializedObjectNode result)
        {
            _allObjects.Add(result);

            // Existing HASH CODE -> Add REFERENCE
            if (_objectReferences.ContainsKey(result.GetObject()))
                _objectReferences[result.GetObject()].Add(result);

            else
                _objectReferences.Add(result.GetObject(), new List<SerializedObjectNode>() { result });

            return result;
        }
    }
}
