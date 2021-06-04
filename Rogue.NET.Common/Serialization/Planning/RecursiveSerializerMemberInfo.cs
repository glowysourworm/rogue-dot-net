
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
            RecursiveSerializerMemberInfo.Empty = new RecursiveSerializerMemberInfo(null, null, null, SerializationMode.Default);
        }

        /// <summary>
        /// Required for DEFAULT mode
        /// </summary>
        internal ConstructorInfo ParameterlessConstructor { get; private set; }

        /// <summary>
        /// ctor(PropertyReader)
        /// </summary>
        internal ConstructorInfo SpecifiedConstructor { get; private set; }

        /// <summary>
        /// GetProperties(PropertyWriter)
        /// </summary>
        internal MethodInfo GetMethod { get; private set; }

        /// <summary>
        /// Mode implied by these members
        /// </summary>
        internal SerializationMode Mode { get; private set; }

        internal RecursiveSerializerMemberInfo(ConstructorInfo parameterlessConstructor,
                                               ConstructorInfo specifiedConstructor,
                                               MethodInfo getMethod,
                                               SerializationMode mode)
        {
            this.ParameterlessConstructor = parameterlessConstructor;
            this.SpecifiedConstructor = specifiedConstructor;
            this.GetMethod = getMethod;
            this.Mode = mode;
        }
    }
}
