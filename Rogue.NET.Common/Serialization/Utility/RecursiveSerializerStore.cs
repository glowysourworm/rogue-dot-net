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
            var parameterlessCtor = hashedType.Resolve().GetConstructor(new Type[] { });
            var planningMethod = hashedType.Resolve().GetMethod(RecursiveSerializerStore.PlanningMethodName, new Type[] { typeof(PropertyPlanner) });
            var setMethod = hashedType.Resolve().GetMethod(RecursiveSerializerStore.SetMethodName, new Type[] { typeof(PropertyReader) });
            var getMethod = hashedType.Resolve().GetMethod(RecursiveSerializerStore.GetMethodName, new Type[] { typeof(PropertyWriter) });

            if (parameterlessCtor == null)
                throw new RecursiveSerializerException(hashedType, "Improper use of Recursive Serializer - must have a parameterless constructor. (See Inner Exception)");

            if (planningMethod != null && (setMethod == null || getMethod == null))
                throw new RecursiveSerializerException(hashedType, "Improper use of Recursive Serializer - must ALSO have get and set methods. (See Inner Exception)");

            if (setMethod != null && (planningMethod == null || getMethod == null))
                throw new RecursiveSerializerException(hashedType, "Improper use of Recursive Serializer - must ALSO have planning and get methods. (See Inner Exception)");

            if (getMethod != null && (setMethod == null || planningMethod == null))
                throw new RecursiveSerializerException(hashedType, "Improper use of Recursive Serializer - must ALSO have planning and setMethod methods. (See Inner Exception)");

            var specifiedMode = (parameterlessCtor != null && planningMethod != null && getMethod != null && setMethod != null);

            // Create the primary members for the serializer
            var memberInfo = new RecursiveSerializerMemberInfo(parameterlessCtor, 
                                                               setMethod, 
                                                               getMethod, 
                                                               planningMethod, 
                                                               specifiedMode ? SerializationMode.Specified : SerializationMode.Default);
            return memberInfo;
        }

        /// <summary>
        /// Validates the type along with its serialization mode to retrieve members for the recursive serializer. THROWS 
        /// EXCEPTIONS! (FOR DESERIALIZATION)
        /// </summary>
        internal static RecursiveSerializerMemberInfo GetMemberInfo(HashedType hashedType, SerializationMode mode)
        {
            var parameterlessCtor = hashedType.Resolve().GetConstructor(new Type[] { });
            var planningMethod = hashedType.Resolve().GetMethod(RecursiveSerializerStore.PlanningMethodName, new Type[] { typeof(PropertyPlanner) });
            var setMethod = hashedType.Resolve().GetMethod(RecursiveSerializerStore.SetMethodName, new Type[] { typeof(PropertyReader) });
            var getMethod = hashedType.Resolve().GetMethod(RecursiveSerializerStore.GetMethodName, new Type[] { typeof(PropertyWriter) });

            // Create the primary members for the serializer
            var memberInfo = new RecursiveSerializerMemberInfo(parameterlessCtor, setMethod, getMethod, planningMethod, mode);

            switch (mode)
            {
                case SerializationMode.None:
                    {
                        if (planningMethod != null || setMethod != null || getMethod != null)
                            throw new RecursiveSerializerException(hashedType, "Improper use of SerializationMode.None");
                    }
                    break;
                case SerializationMode.Default:
                    {
                        if (planningMethod != null || setMethod != null || getMethod != null)
                            throw new RecursiveSerializerException(hashedType, "Improper use of SerializationMode.Default - must NOT have planning and set methods");

                        if (parameterlessCtor == null)
                            throw new RecursiveSerializerException(hashedType, "Improper use of SerializationMode.Default - must have a parameterless constructor");
                    }
                    break;
                case SerializationMode.Specified:
                    {
                        if (planningMethod == null || setMethod == null || getMethod == null)
                            throw new RecursiveSerializerException(hashedType, "Improper use of SerializationMode.Specified - must have planning, get, and set methods");

                        if (parameterlessCtor == null)
                            throw new RecursiveSerializerException(hashedType, "Improper use of SerializationMode.Specified - must have a parameterless constructor");
                    }
                    break;
                default:
                    throw new Exception("Unhandled SerializationMode type:  RecursiveSerializerStore.cs");
            }

            return memberInfo;
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
                                            .OrderBy(property => property.Name));
            }

            return _propertyDict[type];
        }
    }
}
