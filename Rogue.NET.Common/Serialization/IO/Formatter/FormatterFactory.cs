using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.IO.Formatter.PrimitiveArray;
using Rogue.NET.Common.Serialization.Target;

using System;

namespace Rogue.NET.Common.Serialization.Formatter
{
    internal static class FormatterFactory
    {
        internal static IBaseFormatter CreateFormatter(Type type)
        {
            if (type == typeof(HashedType))
                return new HashedTypeFormatter();

            else if (type == typeof(int[]))
                return new IntegerArrayFormatter();

            else
                throw new Exception("Unhandled type:  FormatterFactory.CreateFormatter: " + type.FullName);
        }

        internal static IBaseFormatter CreatePrimitiveFormatter(Type type)
        {
            if (type == typeof(bool))
                return new BooleanFormatter();

            else if (type == typeof(byte))
                return new ByteFormatter();

            else if (type == typeof(DateTime))
                return new DateTimeFormatter();

            else if (type == typeof(double))
                return new DoubleFormatter();

            else if (type == typeof(uint))
                return new UnsignedIntegerFormatter();

            else if (type == typeof(int))
                return new IntegerFormatter();

            else if (type == typeof(string))
                return new StringFormatter();

            else if (type.IsEnum)
                return new EnumFormatter(type);

            else
                throw new Exception("Unhandled type:  FormatterFactory.CreatePrimitiveFormatter: " + type.FullName);
        }

        /// <summary>
        /// Returns true if type is supported by PropertySerializer
        /// </summary>
        internal static bool IsPrimitiveSupported(Type type)
        {
            return (type == typeof(bool)) ||
                   (type == typeof(byte)) ||
                   (type == typeof(DateTime)) ||
                   (type == typeof(double)) ||
                   (type == typeof(uint)) ||
                   (type == typeof(int)) ||
                   (type == typeof(string)) ||
                   (type.IsEnum);
        }
    }
}
