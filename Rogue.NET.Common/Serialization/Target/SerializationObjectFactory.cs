using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Formatter;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// Component that creates wrappers for serialization objects while keeping track of references and
    /// avoiding CIRCULAR references. TRACKING VIA HASH CODE!!!
    /// </summary>
    internal class SerializationObjectFactory
    {
        SimpleDictionary<HashedObjectInfo, SerializationObjectBase> _referenceDict;

        List<SerializationObjectBase> _allObjects;

        internal SerializationObjectBase this[HashedObjectInfo objectInfo]
        {
            get { return _referenceDict[objectInfo]; }
        }

        internal SerializationObjectFactory()
        {
            _referenceDict = new SimpleDictionary<HashedObjectInfo, SerializationObjectBase>();
            _allObjects = new List<SerializationObjectBase>();
        }

        internal bool ContainsReference(HashedObjectInfo reference)
        {
            return _referenceDict.ContainsKey(reference);
        }

        internal SimpleDictionary<HashedObjectInfo, SerializationObjectBase> GetReferences()
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
        internal SerializationObjectBase Create(object theObject, Type theObjectType)
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

            var objectInfo = CreateHashInfo(theObject, theObjectType);

            var isPrimitive = FormatterFactory.IsPrimitiveSupported(objectInfo.Type.GetImplementingType());

            // PRIMITIVE NULL
            if (ReferenceEquals(theObject, null) && isPrimitive)
                return Finalize(new SerializationNullPrimitive(objectInfo));

            // NULL
            if (ReferenceEquals(theObject, null))
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

        /// <summary>
        /// CALCULATES HASH CODE FOR OBJECT + TYPE HASH. Throw exceptions for circular references for REFERENCE TYPES
        /// </summary>
        private HashedObjectInfo CreateHashInfo(object theObject, Type theObjectType)
        {
            var isPrimitive = FormatterFactory.IsPrimitiveSupported(theObjectType);

            // PRIMITIVE NULL
            if (isPrimitive && ReferenceEquals(theObject, null))
                return new HashedObjectInfo(theObjectType);

            // NULL
            if (theObject == null)
                return new HashedObjectInfo(theObjectType);

            // PRIMITIVE
            if (isPrimitive)
                return new HashedObjectInfo(theObject, theObjectType);

            // POSSIBLE PERFORMANCE ISSUE CALCULATING HASH CODES
            //
            // *** NOTE:  Have to analyze type to handle polymorphism and interface properties.
            //            TRY TO CATCH WHEN:  object.GetType() != theObjectType
            //

            HashedObjectInfo hashInfo = null;

            // *** NOTE:  Trying to work with MSFT Type... So, just using this to catch things we 
            //            might have missed. PASS IN the object type called for. THIS IS TO WORK WITH
            //            the property definitions. (Keeps the type consistent)
            //

            // Property Type != Actual Object Type
            if (!theObject.GetType().Equals(theObjectType))
            {
                // INTERFACE
                if (theObjectType.IsInterface)
                {
                    hashInfo = new HashedObjectInfo(theObject, theObjectType);
                }
                // SUB CLASS
                else if (theObject.GetType().IsSubclassOf(theObjectType))
                {
                    hashInfo = new HashedObjectInfo(theObject, theObjectType);
                }
                else
                    throw new Exception("Unhandled polymorphic object type:  " + theObjectType.FullName);
            }
            // Property Type == Actual Object Type
            else
            {
                hashInfo = new HashedObjectInfo(theObject, theObjectType);
            }

            return hashInfo;
        }

        private SerializationObjectBase CreateCollection(HashedObjectInfo info)
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

                return new SerializationCollection(info, memberInfo, list, list.Count, CollectionInterfaceType.IList, argument);
            }
            else
                throw new Exception("PropertySerializer only supports Arrays, and Generic Collections:  List<T>: " + info.Type.DeclaringType);
        }

        private SerializationObjectBase CreatePrimitive(HashedObjectInfo info)
        {
            return new SerializationPrimitive(info);
        }

        private SerializationObjectBase CreateReference(HashedObjectInfo referencedInfo)
        {
            // Validates type and SerializationMode
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(referencedInfo.Type);

            return new SerializationReference(referencedInfo, memberInfo);
        }

        private SerializationObjectBase CreateValue(HashedObjectInfo info)
        {
            // Validates type and SerializationMode
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(info.Type);

            return new SerializationValue(info, memberInfo);
        }

        private SerializationObjectBase CreateObject(HashedObjectInfo info)
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
