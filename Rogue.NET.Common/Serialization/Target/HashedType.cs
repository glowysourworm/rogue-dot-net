using Rogue.NET.Common.Extension;

using System;
using System.Linq;
using System.Reflection;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// Type for serializing type information to file
    /// </summary>
    internal class HashedType
    {
        // PARAMETERS USED TO REFINE HASH CODE
        public string AssemblyName { get { return _type.Assembly.FullName; } }
        public string TypeName { get { return _type.Name; } }
        public string TypeFullName { get { return _type.FullName; } }
        public bool IsGeneric { get { return _type.IsGenericType; } }

        /// <summary>
        /// RECURSIVE DATA STRUCTURE
        /// </summary>
        public HashedType[] GenericArguments { get; private set; }
        
        // CACHED ONLY FOR PERFORMANCE
        Type _type;

        public HashedType(Type type)
        {
            _type = type;

            var arguments = type.GetGenericArguments() ?? new Type[] { };

            // Create generic arguments array
            this.GenericArguments = arguments.Select(argument => new HashedType(argument))
                                             .ToArray();
        }

        /// <summary>
        /// Tries to resolve Type object from type name + assembly name. ALSO USES CACHED TYPE FROM CONSTRUCTOR!
        /// </summary>
        public Type Resolve()
        {
            if (_type != null)
                return _type;

            var assembly = Assembly.Load(this.AssemblyName);

            _type = assembly.GetType(this.TypeName);

            return _type;
        }

        public override bool Equals(object obj)
        {
            var type = obj as HashedType;

            return this.GetHashCode() == type.GetHashCode();
        }

        public override int GetHashCode()
        {
            var baseHash = this.CreateHashCode(this.AssemblyName, 
                                               this.TypeName,
                                               this.TypeFullName,
                                               this.IsGeneric);

            // RECURSIVE!!
            return baseHash.ExtendHashCode(this.GenericArguments);
        }
    }
}
