using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Formatter;
using Rogue.NET.Common.Serialization.Target;
using Rogue.NET.Common.Serialization.Utility;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Component
{
    /// <summary>
    /// Component that creates wrappers for serialization objects while keeping track of references and
    /// avoiding CIRCULAR references.
    /// </summary>
    internal class SerializationObjectFactory
    {
        SimpleDictionary<ObjectInfo, SerializationObjectBase> _referenceDict;

        List<SerializationObjectBase> _allObjects;

        internal SerializationObjectBase this[ObjectInfo objectInfo]
        {
            get { return _referenceDict[objectInfo]; }
        }

        internal SerializationObjectFactory()
        {
            _referenceDict = new SimpleDictionary<ObjectInfo, SerializationObjectBase>();
            _allObjects = new List<SerializationObjectBase>();
        }

        internal bool ContainsReference(ObjectInfo reference)
        {
            return _referenceDict.ContainsKey(reference);
        }

        internal SimpleDictionary<ObjectInfo, SerializationObjectBase> GetReferences()
        {
            return _referenceDict;
        }

        internal IEnumerable<SerializationObjectBase> GetAllSerializedObjects()
        {
            return _allObjects;
        }

        /// <summary>
        /// HANDLES NULLS!  Wraps object for serialization - locating methods for ctor and get method for reproducing object. 
        /// The object type is referenced for creating a wrapper for a null ref object.
        /// </summary>
        internal SerializationObjectBase Create(ObjectInfo objectInfo)
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

            var isPrimitive = FormatterFactory.IsPrimitiveSupported(objectInfo.Type.GetImplementingType());

            // PRIMITIVE NULL
            if (objectInfo.RepresentsNullReference() && isPrimitive)
                return Finalize(new SerializationNullPrimitive(objectInfo));

            // NULL
            if (objectInfo.RepresentsNullReference())
                return Finalize(new SerializationNullObject(objectInfo));

            // STRINGS IMPLEMENT IEnumerable!
            var isCollection = objectInfo.GetObject().ImplementsInterface<IList>() && !isPrimitive;

            // PRIMITIVE
            if (isPrimitive)
                return Finalize(CreatePrimitive(objectInfo));

            // REFERENCE (Is reference type AND is OLD REFERENCE)
            else if (_referenceDict.ContainsKey(objectInfo))
                return Finalize(CreateReference(objectInfo));

            // ***** THE REST GET ADDED TO THE REFERENCE DICT

            // COLLECTION (STRINGS IMPLEMENT IEnumerable!)
            if (isCollection)
                return Finalize(CreateCollection(objectInfo));

            // VAULE TYPE
            else if (objectInfo.Type.GetImplementingType().IsValueType)
                return Finalize(CreateValue(objectInfo));

            // OBJECT TYPE
            else
                return Finalize(CreateObject(objectInfo));
        }

        private SerializationObjectBase CreateCollection(ObjectInfo info)
        {
            // Procedure
            //
            // 0) Determine SerializationMode by checking for constructor and get method
            // 1) Get element type
            // 2) Check that collection type is supported:  List<T>
            // 3) Create the result
            //

            // Validates type and SerializationMode
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(info.Type);

            if (!info.Type.GetImplementingType().IsGenericType)
                throw new Exception("PropertySerializer only supports Arrays, and Generic Collections:  List<T>: " + info.Type.DeclaringType);

            else if (info.GetObject().ImplementsInterface<IList>())
            {
                var argument = (info.GetObject() as IList).GetType().GetGenericArguments()[0];

                if (argument == null)
                    throw new Exception("Invalid IList argument for PropertySerializer: " + info.Type.DeclaringType);

                var list = info.GetObject() as IList;
                var elementDeclaringType = new HashedType(argument);

                return new SerializationCollection(info, memberInfo, list, list.Count, CollectionInterfaceType.IList, elementDeclaringType);
            }
            else
                throw new Exception("PropertySerializer only supports Arrays, and Generic Collections:  List<T>: " + info.Type.DeclaringType);
        }

        private SerializationObjectBase CreatePrimitive(ObjectInfo info)
        {
            return new SerializationPrimitive(info);
        }

        private SerializationObjectBase CreateReference(ObjectInfo referencedInfo)
        {
            // Validates type and SerializationMode
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(referencedInfo.Type);

            return new SerializationReference(referencedInfo, memberInfo);
        }

        private SerializationObjectBase CreateValue(ObjectInfo info)
        {
            // Validates type and SerializationMode
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(info.Type);

            return new SerializationValue(info, memberInfo);
        }

        private SerializationObjectBase CreateObject(ObjectInfo info)
        {
            // Validates type and SerializationMode
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(info.Type);

            return new SerializationObject(info, memberInfo);
        }

        // TRACK REFERENCES AND ALL OBJECTS -> RETURN
        private SerializationObjectBase Finalize(SerializationObjectBase result)
        {
            _allObjects.Add(result);

            if (result is SerializationNullObject ||
                result is SerializationPrimitive ||
                result is SerializationNullPrimitive)
                return result;

            var isReference = result is SerializationReference;

            if (isReference)
                return result;

            if (_referenceDict.ContainsKey(result.ObjectInfo))
                throw new Exception("Duplicate reference found:  " + result.ObjectInfo.Type.GetImplementingType().FullName);

            _referenceDict.Add(result.ObjectInfo, result);

            return result;
        }
    }
}
