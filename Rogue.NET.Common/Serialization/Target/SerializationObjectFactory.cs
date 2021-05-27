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
        public Dictionary<int, HashedObjectInfo> _referenceHashDict;

        public SerializationObjectFactory()
        {
            _referenceHashDict = new Dictionary<int, HashedObjectInfo>();
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
            // COLLECTION:        Validate for the types we're supporting:  List, Dictionary, Array.
            //
            // NULL REFERENCE:    Wrapped by type
            //

            var objectInfo = ReferenceTrack(theObject, theObjectType);

            // NULL
            if (theObject == null)
                return new SerializationNullObject(objectInfo);

            // STRINGS IMPLEMENT IEnumerable!
            var isCollection = objectInfo.TheObject.ImplementsInterface<IEnumerable>() &&
                              !FormatterFactory.IsPrimitiveSupported(objectInfo.Type.Resolve());

            // COLLECTION (STRINGS IMPLEMENT IEnumerable!)
            if (isCollection)
                return CreateCollection(objectInfo);

            // PRIMITIVE
            else if (FormatterFactory.IsPrimitiveSupported(objectInfo.Type.Resolve()))
                return CreatePrimitive(objectInfo);

            // VAULE TYPE
            else if (objectInfo.Type.Resolve().IsValueType)
                return CreateValue(objectInfo);

            // REFERENCE TYPE
            else
                return CreateObject(objectInfo);
        }

        /// <summary>
        /// CALCULATES HASH CODE FOR OBJECT + TYPE HASH. Throw exceptions for circular references for REFERENCE TYPES
        /// </summary>
        private HashedObjectInfo ReferenceTrack(object theObject, Type theObjectType)
        {
            // NULL
            if (theObject == null)
                return new HashedObjectInfo(theObjectType);

            // PRIMITIVE
            if (FormatterFactory.IsPrimitiveSupported(theObjectType))
            {
                return new HashedObjectInfo(theObject, theObjectType);
            }

            // POSSIBLE PERFORMANCE ISSUE CALCULATING HASH CODES
            //
            // *** NOTE:  Have to analyze type to handle polymorphism and interface properties.
            //            TRY TO CATCH WHEN:  object.GetType() != theObjectType
            //

            HashedObjectInfo hashInfo = null;

            // Property Type != Actual Object Type
            if (!theObject.GetType().Equals(theObjectType))
            {
                // INTERFACE
                if (theObjectType.IsInterface)
                {
                    hashInfo = new HashedObjectInfo(theObject, theObject.GetType());
                }
                // SUB CLASS
                else if (theObject.GetType().IsSubclassOf(theObjectType))
                {
                    hashInfo = new HashedObjectInfo(theObject, theObject.GetType());
                }
                else
                    throw new Exception("Unhandled polymorphic object type:  " + theObjectType.FullName);
            }
            // Property Type == Actual Object Type
            else
            {
                hashInfo = new HashedObjectInfo(theObject, theObjectType);
            }


            if (!_referenceHashDict.ContainsKey(hashInfo.GetHashCode()))
                _referenceHashDict.Add(hashInfo.GetHashCode(), hashInfo);

            return hashInfo;
        }

        // REFERENCE TRACKED
        private SerializationObjectBase CreateCollection(HashedObjectInfo info)
        {
            // Procedure
            //
            // 0) Determine SerializationMode by checking for constructor and get method
            // 1) Get element type
            // 2) Check that collection type is supported:  List<T>, Dictionary<K, T>, T[]
            // 3) Create the result
            //

            // Validates type and SerializationMode
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(info.Type);

            // ARRAY
            if (info.Type.Resolve().IsArray)
            {
                var childType = info.Type.Resolve().GetElementType();       // ARRAY ONLY
                var array = info.TheObject as Array;

                return new SerializationCollection(info, memberInfo, array, array.Length,CollectionInterfaceType.Array, childType);
            }
            // GENERIC ENUMERABLE
            else
            {
                if (!info.Type.Resolve().IsGenericType)
                    throw new Exception("PropertySerializer only supports Arrays, and Generic Collections:  List<T>, Dictionary<K, T>: " + info.Type.TypeName);

                else if (info.TheObject.ImplementsInterface<IDictionary>())
                {
                    var arguments = info.Type.Resolve().GetGenericArguments();

                    if (arguments.Length != 2)
                        throw new Exception("Invalid IDictionary argument list for PropertySerializer: " + info.Type.TypeName);

                    var argument = (info.TheObject as IEnumerable).GetType().GetGenericArguments()[0];

                    if (argument == null)
                        throw new Exception("Invalid IDictionary argument list for PropertySerializer: " + info.Type.TypeName);

                    var dictionary = info.TheObject as IDictionary;

                    return new SerializationCollection(info, memberInfo, dictionary, dictionary.Count, CollectionInterfaceType.IDictionary, argument);
                }
                else if (info.TheObject.ImplementsInterface<IList>())
                {
                    var argument = (info.TheObject as IEnumerable).GetType().GetGenericArguments()[0];

                    if (argument == null)
                        throw new Exception("Invalid IList argument for PropertySerializer: " + info.Type.TypeName);

                    var list = info.TheObject as IList;

                    return new SerializationCollection(info, memberInfo, list, list.Count, CollectionInterfaceType.IDictionary, argument);
                }
                else
                    throw new Exception("PropertySerializer only supports Arrays, and Generic Collections:  List<T>, Dictionary<K, T>: " + info.Type.TypeName);
            }
        }

        private SerializationObjectBase CreatePrimitive(HashedObjectInfo info)
        {
            return new SerializationPrimitive(info);
        }

        // REFERENCE TRACKED
        private SerializationObjectBase CreateValue(HashedObjectInfo info)
        {
            // Validates type and SerializationMode
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(info.Type);

            return new SerializationValue(info, memberInfo);
        }

        // REFERENCE TRACKED
        private SerializationObjectBase CreateObject(HashedObjectInfo info)
        {
            // Validates type and SerializationMode
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(info.Type);

            return new SerializationObject(info, memberInfo);
        }
    }
}
