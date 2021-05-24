using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization.Formatter
{
    /// <summary>
    /// Formatter for selected types that can be supported by MSFT's default BinaryFormatter. We're going
    /// to be explicit to VERY PRIMITIVE types AND DateTime.
    /// </summary>
    public class PrimitiveFormatter : BaseFormatter
    {
        BinaryFormatter _formatter;

        public PrimitiveFormatter(Type primitiveType) : base(primitiveType)
        {
            _formatter = new BinaryFormatter();

            if (!IsPrimitive(primitiveType))
                throw new Exception("Un-supported formatter type:  PrimitiveFormatter.cs " + primitiveType.ToString());
        }

        /// <summary>
        /// Returns true if type is supported by PrimitiveFormatter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPrimitive(Type type)
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

        protected override void WriteImpl(Stream stream, object theObject)
        {
            // Expecting that this should work for most primitives
            _formatter.Serialize(stream, theObject);
        }
    }
}
