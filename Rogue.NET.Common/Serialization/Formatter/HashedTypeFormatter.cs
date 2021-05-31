using Rogue.NET.Common.Serialization.Target;

using System.IO;

namespace Rogue.NET.Common.Serialization.Formatter
{
    /// <summary>
    /// RECURSIVE FORMATTER! NEEDED TO STORE TYPE DATA FOR OBJECTS THAT DON'T MATCH
    /// THEIR DECLARED TYPES
    /// </summary>
    internal class HashedTypeFormatter : BaseFormatter<HashedType>
    {
        readonly StringFormatter _stringFormatter;
        readonly IntegerFormatter _integerFormatter;
        readonly BooleanFormatter _booleanFormatter;

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

                return HashedType.Create(declaringAssembly, declaringType, declaringIsGeneric, declaringGenericArguments,
                                         implementingAssembly, implementingType, implementingIsGeneric, implementingGenericArguments);
            }

            return HashedType.Create(declaringAssembly, declaringType, declaringIsGeneric, declaringGenericArguments);
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
    }
}
