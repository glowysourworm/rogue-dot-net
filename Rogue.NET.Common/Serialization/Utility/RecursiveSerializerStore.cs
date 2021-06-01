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
        static Dictionary<Type, IEnumerable<PropertyInfo>> _propertyDict;

        internal static readonly string PlanningMethodName = "GetPropertyDefinitions";
        internal static readonly string SetMethodName = "SetProperties";
        internal static string GetMethodName = "GetProperties";

        static RecursiveSerializerStore()
        {
            _propertyDict = new Dictionary<Type, IEnumerable<PropertyInfo>>();
        }

        /// <summary>
        /// Validates the type and CREATES its serialization mode while retrieving members for the recursive serializer. THROWS 
        /// EXCEPTIONS! (FOR SERIALIZATION)
        /// </summary>
        internal static RecursiveSerializerMemberInfo GetMemberInfo(HashedType hashedType)
        {
            var parameterlessCtor = hashedType.GetImplementingType().GetConstructor(new Type[] { });
            var planningMethod = hashedType.GetImplementingType().GetMethod(RecursiveSerializerStore.PlanningMethodName, new Type[] { typeof(PropertyPlanner) });
            var setMethod = hashedType.GetImplementingType().GetMethod(RecursiveSerializerStore.SetMethodName, new Type[] { typeof(PropertyReader) });
            var getMethod = hashedType.GetImplementingType().GetMethod(RecursiveSerializerStore.GetMethodName, new Type[] { typeof(PropertyWriter) });

            var hasInterfaceImplementing = hashedType.GetImplementingType().HasInterface<IRecursiveSerializable>();
            var hasInterfaceDeclaring = hashedType.GetDeclaringType().HasInterface<IRecursiveSerializable>();
            var hasInterface = hasInterfaceDeclaring && hasInterfaceImplementing;

            if (hasInterfaceDeclaring != hasInterfaceImplementing)
                throw new RecursiveSerializerException(hashedType, "Improper use of IRecursiveSerializable:  Both declaring and implementing types must be marked IRecursiveSerializable:  " + hashedType.ToString());

            if (parameterlessCtor == null)
                throw new RecursiveSerializerException(hashedType, "Improper use of Recursive Serializer - must have a parameterless constructor. (See Inner Exception)");

            if (hasInterface && (planningMethod != null || setMethod == null || getMethod == null))
                throw new RecursiveSerializerException(hashedType, "Improper use of Recursive Serializer - must implement IRecursiveSerializable. (See Inner Exception)");

            // Create the primary members for the serializer
            var memberInfo = new RecursiveSerializerMemberInfo(parameterlessCtor, 
                                                               hasInterface ? setMethod : null, 
                                                               hasInterface ? getMethod : null, 
                                                               hasInterface ? planningMethod : null,
                                                               hasInterface ? SerializationMode.Specified : SerializationMode.Default);
            return memberInfo;
        }

        /// <summary>
        /// Validates the type along with its serialization mode to retrieve members for the recursive serializer. THROWS 
        /// EXCEPTIONS! (FOR DESERIALIZATION)
        /// </summary>
        internal static RecursiveSerializerMemberInfo GetMemberInfo(HashedType hashedType, SerializationMode mode)
        {
            var parameterlessCtor = hashedType.GetImplementingType().GetConstructor(new Type[] { });
            var planningMethod = hashedType.GetImplementingType().GetMethod(RecursiveSerializerStore.PlanningMethodName, new Type[] { typeof(PropertyPlanner) });
            var setMethod = hashedType.GetImplementingType().GetMethod(RecursiveSerializerStore.SetMethodName, new Type[] { typeof(PropertyReader) });
            var getMethod = hashedType.GetImplementingType().GetMethod(RecursiveSerializerStore.GetMethodName, new Type[] { typeof(PropertyWriter) });

            var hasInterfaceImplementing = hashedType.GetImplementingType().HasInterface<IRecursiveSerializable>();
            var hasInterfaceDeclaring = hashedType.GetDeclaringType().HasInterface<IRecursiveSerializable>();
            var hasInterface = hasInterfaceDeclaring && hasInterfaceImplementing;

            if (hasInterfaceDeclaring != hasInterfaceImplementing)
                throw new RecursiveSerializerException(hashedType, "Improper use of IRecursiveSerializable:  Both declaring and implementing types must be marked IRecursiveSerializable:  " + hashedType.ToString());

            if (parameterlessCtor == null)
                throw new RecursiveSerializerException(hashedType, "Improper use of Recursive Serializer - must have a parameterless constructor. (See Inner Exception)");

            if (mode == SerializationMode.Specified && !hasInterface)
                throw new RecursiveSerializerException(hashedType, "Improper use of SerializationMode.Specified - must implement IRecursiveSerializable");

            // Create the primary members for the serializer
            return new RecursiveSerializerMemberInfo(parameterlessCtor,
                                                     hasInterface ? setMethod : null,
                                                     hasInterface ? getMethod : null,
                                                     hasInterface ? planningMethod : null,
                                                     mode);
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
                                            .OrderBy(property => property.Name)
                                            .Actualize());
            }

            return _propertyDict[type];
        }
    }
}
