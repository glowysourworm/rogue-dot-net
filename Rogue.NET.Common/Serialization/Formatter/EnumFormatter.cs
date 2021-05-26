using System;
using System.IO;

namespace Rogue.NET.Common.Serialization.Formatter
{
    public class EnumFormatter : BaseFormatter<Enum>
    {
        // Supporting just int, uint base types
        readonly ByteFormatter _byteFormatter;
        readonly IntegerFormatter _integerFormatter;
        readonly UnsignedIntegerFormatter _unsignedIntegerFormatter;

        readonly Type _enumType;

        public EnumFormatter(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new Exception("Invalid type for EnumFormatter:  " + enumType.FullName);

            if (Enum.GetUnderlyingType(enumType) != typeof(uint) && 
                Enum.GetUnderlyingType(enumType) != typeof(int) &&
                Enum.GetUnderlyingType(enumType) != typeof(byte))
                throw new Exception("Unhandled Enum type for EnumFormatter:  " + enumType.FullName);

            _byteFormatter = new ByteFormatter();
            _integerFormatter = new IntegerFormatter();
            _unsignedIntegerFormatter = new UnsignedIntegerFormatter();
            _enumType = enumType;
        }

        protected override Enum ReadImpl(Stream stream)
        {
            if (Enum.GetUnderlyingType(_enumType) == typeof(uint))
                return (Enum)System.Convert.ChangeType(_unsignedIntegerFormatter.Read(stream), _enumType);

            else if (Enum.GetUnderlyingType(_enumType) == typeof(int))
                return (Enum)System.Convert.ChangeType(_integerFormatter.Read(stream), _enumType);

            else if (Enum.GetUnderlyingType(_enumType) == typeof(byte))
                return (Enum)System.Convert.ChangeType(_byteFormatter.Read(stream), _enumType);

            else
                throw new Exception("Unhandled Enum type for EnumFormatter:  " + _enumType.FullName);
        }

        protected override void WriteImpl(Stream stream, Enum theObject)
        {
            if (Enum.GetUnderlyingType(_enumType) == typeof(uint))
                _unsignedIntegerFormatter.Write(stream, Convert.ToUInt32(theObject));

            else if (Enum.GetUnderlyingType(_enumType) == typeof(int))
                _integerFormatter.Write(stream, Convert.ToInt32(theObject));

            else if (Enum.GetUnderlyingType(_enumType) == typeof(byte))
                _byteFormatter.Write(stream, Convert.ToByte(theObject));

            else
                throw new Exception("Unhandled Enum type for EnumFormatter:  " + _enumType.FullName);
        }
    }
}
