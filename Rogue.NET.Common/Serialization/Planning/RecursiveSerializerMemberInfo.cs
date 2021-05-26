
using System.Reflection;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal class RecursiveSerializerMemberInfo
    {
        /// <summary>
        /// Should be used for PRIMITIVES and NULL reference objects
        /// </summary>
        internal static RecursiveSerializerMemberInfo Empty;

        static RecursiveSerializerMemberInfo()
        {
            RecursiveSerializerMemberInfo.Empty = new RecursiveSerializerMemberInfo(null, null, null, null, SerializationMode.None);
        }

        /// <summary>
        /// Required for DEFAULT mode
        /// </summary>
        internal ConstructorInfo ParameterlessConstructor { get; private set; }

        /// <summary>
        /// SetProperties(PropertyReader)
        /// </summary>
        internal MethodInfo SetMethod { get; private set; }

        /// <summary>
        /// GetProperties(PropertyWriter)
        /// </summary>
        internal MethodInfo GetMethod { get; private set; }

        /// <summary>
        /// GetPropertyDefinitions(PropertyPlanner)
        /// </summary>
        internal MethodInfo PlanningMethod { get; private set; }

        /// <summary>
        /// Mode implied by these members
        /// </summary>
        internal SerializationMode Mode { get; private set; }

        internal RecursiveSerializerMemberInfo(ConstructorInfo parameterlessConstructor,
                                             MethodInfo setMethod,
                                             MethodInfo getMethod,
                                             MethodInfo planningMethod,
                                             SerializationMode mode)
        {
            this.ParameterlessConstructor = parameterlessConstructor;
            this.SetMethod = setMethod;
            this.GetMethod = getMethod;
            this.PlanningMethod = planningMethod;
            this.Mode = mode;
        }
    }
}
