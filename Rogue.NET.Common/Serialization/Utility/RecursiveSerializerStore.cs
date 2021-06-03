using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections.Generic;
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
        static SimpleDictionary<Type, IEnumerable<PropertyInfo>> _propertyDict;

        internal static readonly string GetMethodName = "GetProperties";

        static RecursiveSerializerStore()
        {
            _propertyDict = new SimpleDictionary<Type, IEnumerable<PropertyInfo>>();
        }

        /// <summary>
        /// Validates the type and CREATES its serialization mode while retrieving members for the recursive serializer. THROWS 
        /// EXCEPTIONS! (FOR SERIALIZATION)
        /// </summary>
        internal static RecursiveSerializerMemberInfo GetMemberInfo(HashedType hashedType)
        {
            var memberInfo = CreateBaseMemberInfo(hashedType);

            return ValidateMemberInfo(hashedType, memberInfo.ParameterlessConstructor, memberInfo.SpecifiedConstructor, memberInfo.GetMethod, memberInfo.Mode);
        }

        internal static RecursiveSerializerMemberInfo GetMemberInfo(HashedType hashedType, SerializationMode mode)
        {
            var memberInfo = CreateBaseMemberInfo(hashedType);

            return ValidateMemberInfo(hashedType, memberInfo.ParameterlessConstructor, memberInfo.SpecifiedConstructor, memberInfo.GetMethod, mode);
        }

        private static RecursiveSerializerMemberInfo ValidateMemberInfo(HashedType hashedType, 
                                                                        ConstructorInfo parameterlessCtor, 
                                                                        ConstructorInfo specifiedCtor, 
                                                                        MethodInfo getMethod, 
                                                                        SerializationMode mode)
        {
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

            // Refine the member info parameters
            return new RecursiveSerializerMemberInfo(parameterlessCtor,
                                                     mode == SerializationMode.Specified ? specifiedCtor : null,
                                                     mode == SerializationMode.Specified ? getMethod : null,
                                                     mode);
        }

        private static RecursiveSerializerMemberInfo CreateBaseMemberInfo(HashedType hashedType)
        {
            var parameterlessCtor = hashedType.GetImplementingType().GetConstructor(new Type[] { });
            var specifiedModeCtor = hashedType.GetImplementingType().GetConstructor(new Type[] { typeof(IPropertyReader) });
            var getMethod = hashedType.GetImplementingType().GetMethod(RecursiveSerializerStore.GetMethodName, new Type[] { typeof(IPropertyWriter) });

            // Create the primary members for the serializer
            return new RecursiveSerializerMemberInfo(parameterlessCtor,
                                                     specifiedModeCtor,
                                                     getMethod,
                                                     specifiedModeCtor == null ? SerializationMode.Default : SerializationMode.Specified);
        }

        /// <summary>
        /// NOTE*** ORDERED TO KEEP SERIALIZATION CONSISTENT!!!
        /// </summary>
        internal static IEnumerable<PropertyInfo> GetOrderedProperties(Type type)
        {
            return Impl(type);
        }

        /// <summary>
        /// NOTE*** ORDERED TO KEEP SERIALIZATION CONSISTENT!!!
        /// </summary>
        internal static IEnumerable<PropertyInfo> GetOrderedProperties<T>()
        {
            var type = typeof(T);

            return Impl(type);
        }

        private static IEnumerable<PropertyInfo> Impl(Type type)
        {
            if (!_propertyDict.ContainsKey(type))
            {
                // ORDER BY PROPERTY NAME
                _propertyDict.Add(type, type.GetProperties()
                                            .Where(property => property.GetMethod != null && property.SetMethod != null)
                                            .OrderBy(property => property.Name)
                                            .Actualize());
            }

            return _propertyDict[type];
        }
    }
}
