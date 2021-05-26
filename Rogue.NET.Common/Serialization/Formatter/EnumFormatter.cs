using System;
using System.IO;

namespace Rogue.NET.Common.Serialization.Formatter
{
    public class EnumFormatter : BaseFormatter<Enum>
    {
        // Supporting just int, uint base types
        readonly IntegerFormatter _integerFormatter;
        readonly UnsignedIntegerFormatter _unsignedIntegerFormatter;

        readonly Type _enumType;

        bool _unsigned = false;

        public EnumFormatter(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new Exception("Invalid type for EnumFormatter:  " + enumType.FullName);

            if (Enum.GetUnderlyingType(enumType) == typeof(uint))
                _unsigned = true;

            else if (Enum.GetUnderlyingType(enumType) != (typeof(int)))
                throw new Exception("Unhandled Enum type for EnumFormatter:  " + enumType.FullName);

            _integerFormatter = new IntegerFormatter();
            _unsignedIntegerFormatter = new UnsignedIntegerFormatter();
            _enumType = enumType;
        }

        protected override Enum ReadImpl(Stream stream)
        {
            if (_unsigned)
            {
                return (Enum)System.Convert.ChangeType(_unsignedIntegerFormatter.Read(stream), _enumType);
            }
            else
            {
                return (Enum)System.Convert.ChangeType(_integerFormatter.Read(stream), _enumType);
            }
        }

        protected override void WriteImpl(Stream stream, Enum theObject)
        {
            if (_unsigned)
            {
                _unsignedIntegerFormatter.Write(stream, Convert.ToUInt32(theObject));
            }
            else
            {
                _integerFormatter.Write(stream, Convert.ToInt32(theObject));
            }
        }
    }
}
