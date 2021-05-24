using Rogue.NET.Common.Extension;

using System;
using System.Reflection;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// Type for serializing type information to file
    /// </summary>
    internal class HashedType
    {
        public string TypeName { get; private set; }
        public string AssemblyName { get; private set; }

        // CACHED ONLY FOR PERFORMANCE
        Type _type;

        public HashedType(string typeName, string assemblyName)
        {
            this.TypeName = typeName;
            this.AssemblyName = assemblyName;
        }

        public HashedType(Type type)
        {
            _type = type;

            this.TypeName = type.FullName;
            this.AssemblyName = type.Assembly.FullName;
        }

        /// <summary>
        /// Tries to resolve Type object from type name + assembly name. ALSO USES CACHED TYPE FROM CONSTRUCTOR!
        /// </summary>
        /// <returns></returns>
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
            return this.CreateHashCode(this.TypeName, this.AssemblyName);
        }
    }
}
