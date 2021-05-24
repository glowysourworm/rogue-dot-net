using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Attribute;
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
        /// <summary>
        /// Name of the "Get" method for the serializer to locate on the target object
        /// </summary>
        public static string GET_METHOD_NAME = "GetProperties";

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

            // ATTRIBUTE MARKED
            var serializableAttrib = theObjectType.GetAttribute<PropertySerializable>();

            if (serializableAttrib != null)
            {
                return CreateFromAttribute(objectInfo, serializableAttrib);
            }

            // ATTRIBUTE NOT PRESENT
            else
            {
                // COLLECTION (STRINGS IMPLEMENT IEnumerable!)
                if (objectInfo.TheObject.ImplementsInterface<IEnumerable>() &&
                   !PrimitiveFormatter.IsPrimitive(objectInfo.Type.Resolve()))
                    return CreateCollection(objectInfo);

                // PRIMITIVE
                else if (PrimitiveFormatter.IsPrimitive(objectInfo.Type.Resolve()))
                    return CreatePrimitive(objectInfo);

                // VAULE TYPE
                else if (objectInfo.Type.Resolve().IsValueType)
                    return CreateValue(objectInfo);

                // REFERENCE TYPE
                else
                    return CreateObject(objectInfo);
            }
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
            if (PrimitiveFormatter.IsPrimitive(theObjectType))
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
            

            if (!_referenceHashDict.ContainsKey(hashInfo.HashCode))
                _referenceHashDict.Add(hashInfo.HashCode, hashInfo);

            return hashInfo;
        }

        // REFERENCE TRACKED
        private SerializationObjectBase CreateFromAttribute(HashedObjectInfo info, PropertySerializable attribute)
        {
            // Mode is Default -> Parameterless ctor
            if (attribute.Mode == PropertySerializableMode.Default)
            {
                var constructor = info.Type.Resolve().GetConstructor(new Type[] { });

                if (constructor == null)
                    throw new Exception("PropertySerializableMode.Default must supply parameterless ctor: " + info.Type.TypeName);

                // DEFAULT -> No Get Method
                return new SerializationObject(info, constructor);
            }

            else
            {
                var constructor = info.Type.Resolve().GetConstructor(new Type[] { typeof(PropertyReader) });
                var getMethod = info.Type.Resolve().GetMethod(GET_METHOD_NAME, new Type[] { typeof(PropertyReader) });

                if (constructor == null)
                    throw new Exception("PropertySerializableMode.Specific must supply the constructor ctor(PropertyReader): " + info.Type.TypeName);

                if (getMethod == null)
                    throw new Exception("PropertySerializableMode.Specific must supply the set method GetProperties(PropertyReader): " + info.Type.TypeName);

                // SPECIFIC -> Set Method
                return new SerializationObject(info, constructor, getMethod);
            }
        }

        // REFERENCE TRACKED
        private SerializationObjectBase CreateCollection(HashedObjectInfo info)
        {
            // Procedure
            //
            // 1) Get element type
            // 2) Check that collection type is supported:  List<T>, Dictionary<K, T>, T[]
            // 3) Create the result
            //

            // ARRAY
            if (info.Type.Resolve().IsArray)
            {
                var childType = info.Type.Resolve().GetElementType();       // ARRAY ONLY
                var array = info.TheObject as Array;

                return new SerializationCollection(info, array, array.Length, SerializationCollection.CollectionType.Array, childType);
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

                    return new SerializationCollection(info, dictionary, dictionary.Count, SerializationCollection.CollectionType.Dictionary, argument);
                }
                else if (info.TheObject.ImplementsInterface<IList>())
                {
                    var argument = (info.TheObject as IEnumerable).GetType().GetGenericArguments()[0];

                    if (argument == null)
                        throw new Exception("Invalid IList argument for PropertySerializer: " + info.Type.TypeName);

                    var list = info.TheObject as IList;

                    return new SerializationCollection(info, list, list.Count, SerializationCollection.CollectionType.List, argument);
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
            var parameterlessCtor = info.Type.Resolve().GetConstructor(new Type[] { });
            var serializationCtor = info.Type.Resolve().GetConstructor(new Type[] { typeof(PropertyReader) });
            var getMethod = info.Type.Resolve().GetMethod(GET_METHOD_NAME, new Type[] { typeof(PropertyReader) });

            // SPECIFIC MODE - CHECK FOR GET / CTOR
            if (serializationCtor != null &&
                getMethod != null)
            {
                return new SerializationValue(info, serializationCtor, getMethod);
            }

            // DEFAULT MODE
            else if (parameterlessCtor != null)
            {
                return new SerializationValue(info, parameterlessCtor);
            }

            else
                throw new Exception("PropertySerializer object must supply parameterless constructor: " + info.Type.TypeName);
        }

        // REFERENCE TRACKED
        private SerializationObjectBase CreateObject(HashedObjectInfo info)
        {
            var parameterlessCtor = info.Type.Resolve().GetConstructor(new Type[] { });
            var serializationCtor = info.Type.Resolve().GetConstructor(new Type[] { typeof(PropertyReader) });
            var getMethod = info.Type.Resolve().GetMethod(GET_METHOD_NAME, new Type[] { typeof(PropertyReader) });

            // SPECIFIC MODE - CHECK FOR GET / CTOR
            if (serializationCtor != null &&
                getMethod != null)
            {
                return new SerializationObject(info, serializationCtor, getMethod);
            }

            // DEFAULT MODE
            else if (parameterlessCtor != null)
            {
                return new SerializationObject(info, parameterlessCtor);
            }

            else
                throw new Exception("PropertySerializer object must supply parameterless constructor: " + info.Type.TypeName);
        }
    }
}
