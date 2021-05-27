using Rogue.NET.Common.Serialization.Interface;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Formatter
{
    public class EnumFormatter : BaseFormatter<Enum>
    {
        // Supporting just int, uint base types
        readonly ByteFormatter _byteFormatter;
        readonly IntegerFormatter _integerFormatter;
        readonly UnsignedIntegerFormatter _unsignedIntegerFormatter;

        readonly Type _enumType;

        readonly IDictionary<object, IComparable> _enumValues;

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
            _enumValues = Enum.GetValues(enumType)
                              .Cast<object>()
                              .Select(enumObject => Convert.ChangeType(enumObject, _enumType.GetEnumUnderlyingType()))
                              .ToDictionary(value => value, value => (IComparable)value);
        }

        protected override Enum ReadImpl(Stream stream)
        {
            if (Enum.GetUnderlyingType(_enumType) == typeof(uint))
                return ProcessRead(stream, _unsignedIntegerFormatter);

            else if (Enum.GetUnderlyingType(_enumType) == typeof(int))
                return ProcessRead(stream, _integerFormatter);

            else if (Enum.GetUnderlyingType(_enumType) == typeof(byte))
                return ProcessRead(stream, _byteFormatter);

            else
                throw new Exception("Unhandled Enum type for EnumFormatter:  " + _enumType.FullName);
        }

        protected override void WriteImpl(Stream stream, Enum theObject)
        {
            if (Enum.GetUnderlyingType(_enumType) == typeof(uint))
                ProcessWrite(stream, Convert.ToUInt32(theObject), _unsignedIntegerFormatter);

            else if (Enum.GetUnderlyingType(_enumType) == typeof(int))
                ProcessWrite(stream, Convert.ToInt32(theObject), _integerFormatter);

            else if (Enum.GetUnderlyingType(_enumType) == typeof(byte))
                ProcessWrite(stream, Convert.ToByte(theObject), _byteFormatter);

            else
                throw new Exception("Unhandled Enum type for EnumFormatter:  " + _enumType.FullName);
        }

        // TODO: Figure out better way to validate input stream
        private void ProcessWrite(Stream stream, object theObject, IBaseFormatter formatter)
        {
            //if (!_enumValues.ContainsKey(theObject))
            //    throw new Exception("Enum value invalid:  EnumFormatter.cs");

            formatter.Write(stream, theObject);
        }

        private Enum ProcessRead(Stream stream, IBaseFormatter formatter)
        {
            var theObject = formatter.Read(stream);

            //if (!_enumValues.ContainsKey(theObject))
            //    throw new Exception("Enum value invalid:  EnumFormatter.cs");

            return (Enum)Enum.ToObject(_enumType, theObject);
        }
    }
}
