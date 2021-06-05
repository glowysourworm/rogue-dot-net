using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Interface;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Formatter
{
    public class EnumFormatter : BaseFormatter<Enum>
    {
        static Type _dataType = typeof(Enum);

        public override Type DataType { get { return _dataType; } }

        // Supporting just int, uint base types
        readonly ByteFormatter _byteFormatter;
        readonly IntegerFormatter _integerFormatter;
        readonly UnsignedIntegerFormatter _unsignedIntegerFormatter;

        readonly Type _enumType;

        // Prevent casting during read
        readonly SimpleDictionary<object, Enum> _enumValues;

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

            // Load values for read / write validation (CAN'T HANDLE FLAGS / MASKS)
            _enumValues = new SimpleDictionary<object, Enum>();
        }

        protected override Enum ReadImpl(Stream stream)
        {
            if (Enum.GetUnderlyingType(_enumType) == typeof(uint))
                return ProcessReadValue(stream, _unsignedIntegerFormatter);

            else if (Enum.GetUnderlyingType(_enumType) == typeof(int))
                return ProcessReadValue(stream, _integerFormatter);

            else if (Enum.GetUnderlyingType(_enumType) == typeof(byte))
                return ProcessReadValue(stream, _byteFormatter);

            else
                throw new Exception("Unhandled Enum type for EnumFormatter:  " + _enumType.FullName);
        }

        protected override void WriteImpl(Stream stream, Enum theObject)
        {
            if (Enum.GetUnderlyingType(_enumType) == typeof(uint))
                ProcessWriteValue(stream, Convert.ToUInt32(theObject), _unsignedIntegerFormatter);

            else if (Enum.GetUnderlyingType(_enumType) == typeof(int))
                ProcessWriteValue(stream, Convert.ToInt32(theObject), _integerFormatter);

            else if (Enum.GetUnderlyingType(_enumType) == typeof(byte))
                ProcessWriteValue(stream, Convert.ToByte(theObject), _byteFormatter);

            else
                throw new Exception("Unhandled Enum type for EnumFormatter:  " + _enumType.FullName);
        }

        // TODO: Figure out better way to validate input stream
        private void ProcessWriteValue(Stream stream, object enumValue, IBaseFormatter formatter)
        {
            formatter.Write(stream, enumValue);
        }

        private Enum ProcessReadValue(Stream stream, IBaseFormatter formatter)
        {
            var enumValue = formatter.Read(stream);

            if (!_enumValues.ContainsKey(enumValue))
            {
                _enumValues.Add(enumValue, (Enum)Enum.ToObject(_enumType, enumValue));
            }

            return _enumValues[enumValue];
        }
    }
}
