using Rogue.NET.Common.Extension;

using System;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// Type for serializing type information to file
    /// </summary>
    internal class HashedType
    {
        internal string DeclaringAssembly { get { return _declaringType.Assembly.FullName; } }
        internal string DeclaringType { get { return _declaringType.FullName; } }
        internal bool DeclaringIsGeneric { get { return _declaringType.IsGenericType; } }

        internal string ImplementingAssembly
        {
            get
            {
                if (_implementingType == null)
                    throw new NullReferenceException("HashedType.ImplementingAssembly must have a implementing type");

                return _implementingType.Assembly.FullName;
            }
        }
        internal string ImplementingType
        {
            get
            {
                if (_implementingType == null)
                    throw new NullReferenceException("HashedType.ImplementingType must have a implementing type");

                return _implementingType.FullName;
            }
        }
        internal bool ImplementingIsGeneric
        {
            get
            {
                if (_implementingType == null)
                    throw new NullReferenceException("HashedType.ImplementingIsGeneric must have a implementing type");

                return _implementingType.IsGenericType;
            }
        }

        /// <summary>
        /// RECURSIVE DATA STRUCTURE
        /// </summary>
        internal HashedType[] DeclaringGenericArguments { get; private set; }
        internal HashedType[] ImplementingGenericArguments { get; private set; }

        // CACHED FOR PERFORMANCE ONLY
        Type _declaringType;
        Type _implementingType;

        // CACHE THE HASH CODE FOR PERFORMANCE!
        int _calculatedHashCode;

        internal HashedType(Type declaringType)
        {
            Initialize(declaringType, declaringType);
        }

        /// <summary>
        /// Private method used for deserializing
        /// </summary>
        internal HashedType(Type declaringType, Type implementingType)
        {
            _declaringType = declaringType;
            _implementingType = implementingType;

            Initialize(declaringType, implementingType);
        }

        private void Initialize(Type declaringType, Type implementingType)
        {
            if (declaringType == null)
                throw new NullReferenceException("HashedType must have a declaring type");

            _declaringType = declaringType;
            _implementingType = implementingType;

            var arguments = declaringType.GetGenericArguments() ?? new Type[] { };
            var implementingArguments = _implementingType.GetGenericArguments() ?? new Type[] { };

            // Create generic arguments array (PERFORMANCE TUNED)
            this.DeclaringGenericArguments = arguments.Transform(type => new HashedType(type));
            this.ImplementingGenericArguments = implementingArguments.Transform(type => new HashedType(type));

            _calculatedHashCode = default(int);
        }

        /// <summary>
        /// Used for creating instance with NO TYPE DISCREPANCY
        /// </summary>
        public static HashedType Create(string declaringAssembly, string declaringType, bool declaringIsGeneric, HashedType[] declaringGenericArguments)
        {
            return new HashedType(ResolveFromFields(declaringAssembly, declaringType));
        }

        /// <summary>
        /// Used for creating instance with TYPE DISCREPANCY
        /// </summary>
        public static HashedType Create(string declaringAssembly, string declaringType, bool declaringIsGeneric, HashedType[] declaringGenericArguments,
                                        string implementingAssembly, string implementingType, bool implementingIsGeneric, HashedType[] implementingGenericArguments)
        {
            var typeDeclaring = ResolveFromFields(declaringAssembly, declaringType);
            var typeImplementing = ResolveFromFields(implementingAssembly, implementingType);

            return new HashedType(typeDeclaring, typeImplementing);
        }

        private static Type ResolveFromFields(string assemblyName, string typeName)
        {
            var assembly = AppDomain.CurrentDomain
                                    .GetAssemblies()
                                    .FirstOrDefault(assembly => assembly.FullName == assemblyName);

            if (assembly == null)
                throw new Exception("No assembly found for type:  " + typeName);

            var type = assembly.GetType(typeName);

            if (type == null)
                throw new Exception("No type found in loaded assembly:  " + typeName);

            return type;
        }

        /// <summary>
        /// Tries to resolve Type object from type name + assembly name. ALSO USES CACHED TYPE FROM CONSTRUCTOR!
        /// </summary>
        public Type GetImplementingType()
        {
            return _implementingType;
        }

        /// <summary>
        /// Tries to resolve Type object from type name + assembly name. ALSO USES CACHED TYPE FROM CONSTRUCTOR!
        /// </summary>
        public Type GetDeclaringType()
        {
            return _declaringType;
        }

        public bool HasTypeDiscrepancy()
        {
            // return !_declaringType.Equals(_implementingType);

            return ((this.DeclaringType != this.ImplementingType) && !string.IsNullOrEmpty(this.ImplementingType)) ||
                   ((this.DeclaringAssembly != this.ImplementingAssembly) && !string.IsNullOrEmpty(this.ImplementingAssembly));
        }

        public override bool Equals(object obj)
        {
            var type = obj as HashedType;

            return this.GetHashCode() == type.GetHashCode();
        }

        public override int GetHashCode()
        {
            if (_calculatedHashCode == default(int))
            {

                var baseHash = this.CreateHashCode(this.DeclaringAssembly,
                                                   this.DeclaringType,
                                                   this.DeclaringIsGeneric);

                // RECURSIVE!!
                baseHash = baseHash.ExtendHashCode(this.DeclaringGenericArguments);

                if (HasTypeDiscrepancy())
                {
                    baseHash = baseHash.ExtendHashCode(this.ImplementingAssembly,
                                                       this.ImplementingType,
                                                       this.ImplementingIsGeneric);

                    // RECURSIVE!!
                    baseHash = baseHash.ExtendHashCode(this.ImplementingGenericArguments);
                }

                _calculatedHashCode = baseHash;
            }
            return _calculatedHashCode;
        }

        public override string ToString()
        {
            return "Declaring Type=" + _declaringType.Name + ", Implementing Type=" + _implementingType.Name;
        }
    }
}
