using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;
using Rogue.NET.Common.Serialization.Utility;

using System.Collections;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Component
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

        internal DeserializationObjectBase CreateCollection(ObjectReference reference,
                                                            CollectionInterfaceType interfaceType,
                                                            int childCount,
                                                            SerializationMode mode,
                                                            PropertySpecification specification,
                                                            int[] elementTypeHashCodes)
        {
            // Procedure
            //
            // 0) Validate Constructor, Set, and Planning Methods
            // 1) Get element type
            // 2) Check that collection type is supported:  List<T>
            // 3) Create the result
            //

            // Validate type and get members
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(reference.Type, mode);

            if (!reference.Type.GetImplementingType().IsGenericType)
                throw new RecursiveSerializerException(reference.Type, "PropertySerializer only supports Arrays, and Generic Collections:  List<T>");

            else if (reference.Type.GetImplementingType().HasInterface<IList>())
            {
                var argument = reference.Type.GetImplementingType().GetGenericArguments()[0];

                if (argument == null)
                    throw new RecursiveSerializerException(reference.Type, "Invalid IList argument for PropertySerializer");

                return new DeserializationCollection(reference, memberInfo, specification, elementTypeHashCodes, childCount, CollectionInterfaceType.IList);
            }
            else
                throw new RecursiveSerializerException(reference.Type, "PropertySerializer only supports Arrays, and Generic Collections:  List<T>");
        }

        internal DeserializationObjectBase CreateNullReference(ObjectReference reference, SerializationMode mode)
        {
            return new DeserializationNullReference(reference);
        }

        internal DeserializationObjectBase CreateNullPrimitive(ObjectReference reference, SerializationMode mode)
        {
            return new DeserializationNullPrimitive(reference);
        }

        internal DeserializationObjectBase CreatePrimitive(ObjectInfo info, SerializationMode mode)
        {
            return new DeserializationPrimitive(info);
        }

        internal DeserializationObjectBase CreateReference(ObjectReference reference, SerializationMode mode)
        {
            // Validate type and get members
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(reference.Type, mode);

            return new DeserializationReference(reference, memberInfo);
        }

        internal DeserializationObjectBase CreateValue(ObjectReference reference, SerializationMode mode, PropertySpecification specification)
        {
            // Validate type and get members
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(reference.Type, mode);

            return new DeserializationValue(reference, memberInfo, specification);
        }

        internal DeserializationObjectBase CreateObject(ObjectReference reference, SerializationMode mode, PropertySpecification specification)
        {
            // Validate type and get members
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(reference.Type, mode);

            return new DeserializationObject(reference, memberInfo, specification);
        }
    }
}
