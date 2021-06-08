using Rogue.NET.Common.Collection;
using Rogue.NET.Common.CustomException;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.IO;
using System.Reflection;

namespace Rogue.NET.Common.Serialization.Formatter
{
    /// <summary>
    /// RECURSIVE FORMATTER! NEEDED TO STORE TYPE DATA FOR OBJECTS THAT DON'T MATCH
    /// THEIR DECLARED TYPES
    /// </summary>
    internal class HashedTypeFormatter : BaseFormatter<HashedType>
    {
        static Type _dataType = typeof(HashedType);

        public override Type DataType { get { return _dataType; } }

        static SimpleDictionary<string, Assembly> _assemblyLookup;
        static SimpleDictionary<string, SimpleDictionary<string, Type>> _typeLookup;

        readonly StringFormatter _stringFormatter;
        readonly IntegerFormatter _integerFormatter;
        readonly BooleanFormatter _booleanFormatter;

        // Create static type lookup for formatter
        static HashedTypeFormatter()
        {
            _assemblyLookup = AppDomain.CurrentDomain.GetAssemblies().ToSimpleDictionary(assembly => assembly.FullName, assembly => assembly);
            _typeLookup = AppDomain.CurrentDomain
                                   .GetAssemblies()
                                   .ToSimpleDictionary(assembly => assembly.FullName,
                                                       assembly =>
                                                       {
                                                           return assembly.GetTypes()
                                                                          .ToSimpleDictionary(type => type.FullName, type => type);
                                                       });
        }

        internal HashedTypeFormatter()
        {
            _stringFormatter = new StringFormatter();
            _integerFormatter = new IntegerFormatter();
            _booleanFormatter = new BooleanFormatter();
        }

        protected override HashedType ReadImpl(Stream stream)
        {
            var hasTypeDiscrepancy = _booleanFormatter.Read(stream);

            var declaringAssembly = _stringFormatter.Read(stream);
            var declaringType = _stringFormatter.Read(stream);
            var declaringIsGeneric = _booleanFormatter.Read(stream);

            // RECURSE USING LOOP -> EXPECTED TO SELF TERMINATE!
            var declaringGenericArgumentsCount = _integerFormatter.Read(stream);
            var declaringGenericArguments = new HashedType[declaringGenericArgumentsCount];

            for (int index = 0; index < declaringGenericArgumentsCount; index++)
            {
                declaringGenericArguments[index] = ReadImpl(stream);
            }

            // NOTE*** NOT ALL DATA FROM HASHED TYPE WAS NECESSARY TO SERIALIZE
            var declaringTypeResolved = ResolveFromFields(declaringAssembly, declaringType);

            if (hasTypeDiscrepancy)
            {
                var implementingAssembly = _stringFormatter.Read(stream);
                var implementingType = _stringFormatter.Read(stream);
                var implementingIsGeneric = _booleanFormatter.Read(stream);

                // RECURSE USING LOOP -> EXPECTED TO SELF TERMINATE!
                var implementingGenericArgumentsCount = _integerFormatter.Read(stream);
                var implementingGenericArguments = new HashedType[implementingGenericArgumentsCount];

                for (int index = 0; index < implementingGenericArgumentsCount; index++)
                {
                    implementingGenericArguments[index] = ReadImpl(stream);
                }

                // NOTE*** NOT ALL DATA FROM HASHED TYPE WAS NECESSARY TO SERIALIZE
                var implementingTypeResolved = ResolveFromFields(implementingAssembly, implementingType);

                return new HashedType(declaringTypeResolved, implementingTypeResolved);
            }

            return new HashedType(declaringTypeResolved);
        }

        protected override void WriteImpl(Stream stream, HashedType theObject)
        {
            // WRITE EXTRA BOOLEAN -> "HAS TYPE DISCREPANCY"
            _booleanFormatter.Write(stream, theObject.HasTypeDiscrepancy());

            _stringFormatter.Write(stream, theObject.DeclaringAssembly);
            _stringFormatter.Write(stream, theObject.DeclaringType);
            _booleanFormatter.Write(stream, theObject.DeclaringIsGeneric);

            // RECURSE USING LOOP -> EXPECTED TO SELF TERMINATE!
            _integerFormatter.Write(stream, theObject.DeclaringGenericArguments.Length);

            foreach (var argument in theObject.DeclaringGenericArguments)
                WriteImpl(stream, argument);

            if (theObject.HasTypeDiscrepancy())
            {
                _stringFormatter.Write(stream, theObject.ImplementingAssembly);
                _stringFormatter.Write(stream, theObject.ImplementingType);
                _booleanFormatter.Write(stream, theObject.ImplementingIsGeneric);

                // RECURSE USING LOOP -> EXPECTED TO SELF TERMINATE!
                _integerFormatter.Write(stream, theObject.ImplementingGenericArguments.Length);

                foreach (var argument in theObject.ImplementingGenericArguments)
                    WriteImpl(stream, argument);
            }
        }

        private static Type ResolveFromFields(string assemblyName, string typeName)
        {
            if (!_typeLookup.ContainsKey(assemblyName))
                throw new Exception("No assembly found for type:  " + typeName);

            if (!_typeLookup[assemblyName].ContainsKey(typeName))
            {
                // Try resolving using the assembly
                var type = _assemblyLookup[assemblyName].GetType(typeName);

                if (type == null)
                    throw new FormattedException("No type {0} found in assembly {1}", typeName, assemblyName);

                // Cache resolved type
                _typeLookup[assemblyName].Add(typeName, type);
            }

            return _typeLookup[assemblyName][typeName];
        }
    }
}
