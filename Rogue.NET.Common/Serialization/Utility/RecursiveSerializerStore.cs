using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Component.Interface;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;
using Rogue.NET.Common.Serialization.Utility;

using System;
using System.Linq;
using System.Reflection;

namespace Rogue.NET.Common.Serialization
{
    /// <summary>
    /// Static store for reflected type info
    /// </summary>
    internal static class RecursiveSerializerStore
    {
        // Keep track of property types to avoid extra reflection calls
        static SimpleDictionary<HashedType, PropertySpecification> _propertyDict;

        // Keep track of constructors and get methods
        static SimpleDictionary<HashedType, RecursiveSerializerMemberInfo> _memberInfoDict;

        internal static readonly string GetMethodName = "GetProperties";

        static RecursiveSerializerStore()
        {
            _propertyDict = new SimpleDictionary<HashedType, PropertySpecification>();
            _memberInfoDict = new SimpleDictionary<HashedType, RecursiveSerializerMemberInfo>();
        }

        /// <summary>
        /// Validates the type and CREATES its serialization mode while retrieving members for the recursive serializer. THROWS 
        /// EXCEPTIONS! (FOR SERIALIZATION)
        /// </summary>
        internal static RecursiveSerializerMemberInfo GetMemberInfo(HashedType hashedType)
        {
            // Fetch + Validate the serialization mode
            var serializationMode = GetMemberInfoMode(hashedType);

            return CreateBaseMemberInfo(hashedType, serializationMode);
        }

        internal static RecursiveSerializerMemberInfo GetMemberInfo(HashedType hashedType, SerializationMode mode)
        {
            return CreateBaseMemberInfo(hashedType, mode);
        }

        private static SerializationMode GetMemberInfoMode(HashedType hashedType)
        {
            if (_memberInfoDict.ContainsKey(hashedType))
                return _memberInfoDict[hashedType].Mode;

            var hasInterfaceImplementing = hashedType.GetImplementingType().HasInterface<IRecursiveSerializable>();
            var hasInterfaceDeclaring = hashedType.GetDeclaringType().HasInterface<IRecursiveSerializable>();
            var hasInterface = hasInterfaceDeclaring && hasInterfaceImplementing;

            if (hasInterfaceDeclaring && !hasInterfaceImplementing)
                throw new RecursiveSerializerException(hashedType, "Implementing type does not implement IRecursiveSerializable");

            if (!hasInterfaceDeclaring && hasInterfaceImplementing)
                throw new RecursiveSerializerException(hashedType, "Declaring type does not implement IRecursiveSerializable");

            if (hasInterface)
                return SerializationMode.Specified;

            else
                return SerializationMode.Default;
        }

        private static RecursiveSerializerMemberInfo CreateBaseMemberInfo(HashedType hashedType, SerializationMode mode)
        {
            // CAN ONLY UTILIZE IF MODES MATCH
            if (_memberInfoDict.ContainsKey(hashedType))
            {
                if (_memberInfoDict[hashedType].Mode != mode)
                    throw new RecursiveSerializerException(hashedType, "Serialization mode for type doesn't match the code definition");
            }

            var parameterlessCtor = hashedType.GetImplementingType().GetConstructor(new Type[] { });
            var specifiedModeCtor = hashedType.GetImplementingType().GetConstructor(new Type[] { typeof(IPropertyReader) });
            var getMethod = hashedType.GetImplementingType().GetMethod(RecursiveSerializerStore.GetMethodName, new Type[] { typeof(IPropertyWriter) });

            var hasInterfaceImplementing = hashedType.GetImplementingType().HasInterface<IRecursiveSerializable>();
            var hasInterfaceDeclaring = hashedType.GetDeclaringType().HasInterface<IRecursiveSerializable>();
            var hasInterface = hasInterfaceDeclaring && hasInterfaceImplementing;

            if (mode == SerializationMode.Default && parameterlessCtor == null)
                throw new RecursiveSerializerException(hashedType, "Improper use of Recursive Serializer - must have a parameterless constructor. (See Inner Exception)");

            if (mode == SerializationMode.Specified)
            {
                if (!hasInterface)
                    throw new RecursiveSerializerException(hashedType, "Improper use of SerializationMode.Specified - must implement IRecursiveSerializable");

                if (hasInterfaceDeclaring != hasInterfaceImplementing)
                    throw new RecursiveSerializerException(hashedType, "Improper use of IRecursiveSerializable:  Both declaring and implementing types must be marked IRecursiveSerializable:  " + hashedType.ToString());
            }

            // Create the primary members for the serializer
            var memberInfo = new RecursiveSerializerMemberInfo(parameterlessCtor,
                                                               mode == SerializationMode.Specified ? specifiedModeCtor : null,
                                                               mode == SerializationMode.Specified ? getMethod : null,
                                                               mode);

            _memberInfoDict[hashedType] = memberInfo;

            return memberInfo;
        }

        /// <summary>
        /// NOTE*** ORDERED TO KEEP SERIALIZATION CONSISTENT!!! THESE NEED TO BE RESOLVED AGAINST THE OBJECT IMPLEMENTATION!
        /// </summary>
        internal static PropertySpecification GetOrderedProperties(HashedType type)
        {
            return Impl(type);
        }

        /// <summary>
        /// PRIMARY GATEWAY FROM MSFT -> OUR NAMESPACE. HASHED TYPE IS TO BE USED FROM HERE ON.
        /// </summary>
        private static PropertySpecification Impl(HashedType type)
        {
            if (!_propertyDict.ContainsKey(type))
            {
                // ORDER BY PROPERTY NAME
                _propertyDict.Add(type, new PropertySpecification(type,
                    type.GetImplementingType()
                        .GetProperties()
                        .Where(property => property.GetMethod != null && property.SetMethod != null)
                        .OrderBy(property => property.Name)
                        .Select(property => new PropertyDefinition(property)
                        {
                            IsUserDefined = false,
                            PropertyName = property.Name,

                            // NOTE*** USING PROPERTY TYPE AS THE DECLARED TYPE
                            PropertyType = new HashedType(property.PropertyType)
                        })
                        .Actualize()));
            }

            return _propertyDict[type];
        }
    }
}
