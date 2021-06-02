using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// Creates wrapped types with information for constructing / reading data from the stream to write
    /// to them in a default / specific constructor mode.
    /// </summary>
    internal class DeserializationObjectFactory
    {
        internal DeserializationObjectFactory()
        {
        }

        internal DeserializationObjectBase CreateCollection(HashedObjectReference reference, int childCount, CollectionInterfaceType interfaceType, SerializationMode mode, IEnumerable<PropertyDefinition> definitions)
        {
            // Procedure
            //
            // 0) Validate Constructor, Set, and Planning Methods
            // 1) Get element type
            // 2) Check that collection type is supported:  List<T>, Dictionary<K, T>, T[]
            // 3) Create the result
            //

            // Validate type and get members
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(reference.Type, mode);

            if (!reference.Type.GetImplementingType().IsGenericType)
                throw new RecursiveSerializerException(reference.Type, "PropertySerializer only supports Arrays, and Generic Collections:  List<T>, Dictionary<K, T>");

            else if (reference.Type.GetImplementingType().HasInterface<IDictionary>())
            {
                var arguments = reference.Type.GetImplementingType().GetGenericArguments();

                if (arguments.Length != 2)
                    throw new RecursiveSerializerException(reference.Type, "Invalid IDictionary argument list for PropertySerializer");

                var argument = reference.Type.GetImplementingType().GetGenericArguments()[0];

                if (argument == null)
                    throw new RecursiveSerializerException(reference.Type, "Invalid IDictionary argument list for PropertySerializer");

                return new DeserializationCollection(reference, memberInfo, definitions, argument, childCount, CollectionInterfaceType.IDictionary);
            }
            else if (reference.Type.GetImplementingType().HasInterface<IList>())
            {
                var argument = reference.Type.GetImplementingType().GetGenericArguments()[0];

                if (argument == null)
                    throw new RecursiveSerializerException(reference.Type, "Invalid IList argument for PropertySerializer");

                return new DeserializationCollection(reference, memberInfo, definitions, argument, childCount, CollectionInterfaceType.IList);
            }
            else
                throw new RecursiveSerializerException(reference.Type, "PropertySerializer only supports Arrays, and Generic Collections:  List<T>, Dictionary<K, T>");
        }

        internal DeserializationObjectBase CreateNullReference(HashedObjectReference reference, SerializationMode mode)
        {
            if (mode != SerializationMode.None)
                throw new Exception("Invalid Serialization Mode for null referenced type:  " + reference.Type.GetImplementingType().ToString());

            return new DeserializationNullReference(reference);
        }

        internal DeserializationObjectBase CreateNullPrimitive(HashedObjectReference reference, SerializationMode mode)
        {
            if (mode != SerializationMode.None)
                throw new Exception("Invalid Serialization Mode for null primitive type:  " + reference.Type.GetImplementingType().ToString());

            return new DeserializationNullPrimitive(reference);
        }

        internal DeserializationObjectBase CreatePrimitive(HashedObjectInfo info, SerializationMode mode)
        {
            if (mode != SerializationMode.None)
                throw new Exception("Invalid Serialization Mode for null referenced type:  " + info.Type.GetImplementingType().ToString());

            return new DeserializationPrimitive(info);
        }

        internal DeserializationObjectBase CreateReference(HashedObjectReference reference, SerializationMode mode)
        {
            // Validate type and get members
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(reference.Type, mode);

            return new DeserializationReference(reference, memberInfo);
        }

        internal DeserializationObjectBase CreateValue(HashedObjectReference reference, SerializationMode mode, IEnumerable<PropertyDefinition> definitions)
        {
            // Validate type and get members
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(reference.Type, mode);

            return new DeserializationValue(reference, memberInfo, definitions);
        }

        internal DeserializationObjectBase CreateObject(HashedObjectReference reference, SerializationMode mode, IEnumerable<PropertyDefinition> definitions)
        {
            // Validate type and get members
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(reference.Type, mode);

            return new DeserializationObject(reference, memberInfo, definitions);
        }
    }
}
