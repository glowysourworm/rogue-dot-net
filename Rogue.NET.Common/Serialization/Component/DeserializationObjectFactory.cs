using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;
using Rogue.NET.Common.Serialization.Utility;

using System.Collections;

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

        internal DeserializedNodeBase CreateCollection(PropertyDefinition definingProperty, int referenceId, HashedType type,
                                                       CollectionInterfaceType interfaceType,
                                                       int childCount,
                                                       SerializationMode mode,
                                                       PropertySpecification specification,
                                                       HashedType elementType)
        {
            // Procedure
            //
            // 0) Validate Constructor, Set, and Planning Methods
            // 1) Get element type
            // 2) Check that collection type is supported:  List<T>
            // 3) Create the result
            //

            // Validate type and get members
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(type, mode);

            if (!type.GetImplementingType().IsGenericType)
                throw new RecursiveSerializerException(type, "PropertySerializer only supports Arrays, and Generic Collections:  List<T>");

            else if (type.GetImplementingType().HasInterface<IList>())
            {
                var argument = type.GetImplementingType().GetGenericArguments()[0];

                if (argument == null)
                    throw new RecursiveSerializerException(type, "Invalid IList argument for PropertySerializer");

                // NOTE*** Element IMPLEMENTING type is not tracked. So, the declaring type should be equal to the implemeting type.
                if (!argument.IsAssignableFrom(elementType.GetDeclaringType()))
                    throw new RecursiveSerializerException(elementType, "Invalid collection element type against referenced argument type");

                return new DeserializedCollectionNode(definingProperty, type, referenceId, memberInfo, specification, elementType, childCount, CollectionInterfaceType.IList);
            }
            else
                throw new RecursiveSerializerException(type, "PropertySerializer only supports Arrays, and Generic Collections:  List<T>");
        }

        internal DeserializedNodeBase CreateNullReference(PropertyDefinition definingProperty, HashedType type, SerializationMode mode)
        {
            return new DeserializedNullLeafNode(definingProperty, type);
        }

        internal DeserializedNodeBase CreateNullPrimitive(PropertyDefinition definingProperty, HashedType type, SerializationMode mode)
        {
            return new DeserializedNullLeafNode(definingProperty, type);
        }

        internal DeserializedNodeBase CreatePrimitive(PropertyDefinition definingProperty, object theObject, HashedType type, SerializationMode mode)
        {
            return new DeserializedLeafNode(definingProperty, type, theObject);
        }

        internal DeserializedNodeBase CreateReference(PropertyDefinition definingProperty, int referenceId, HashedType type, SerializationMode mode)
        {
            // Validate type and get members
            // var memberInfo = RecursiveSerializerStore.GetMemberInfo(type, mode);

            return new DeserializedReferenceNode(definingProperty, referenceId, type);
        }

        internal DeserializedNodeBase CreateObject(PropertyDefinition definingProperty, int referenceId, HashedType type, SerializationMode mode, PropertySpecification specification)
        {
            // Validate type and get members
            var memberInfo = RecursiveSerializerStore.GetMemberInfo(type, mode);

            return new DeserializedObjectNode(definingProperty, type, referenceId, memberInfo, specification);
        }
    }
}
