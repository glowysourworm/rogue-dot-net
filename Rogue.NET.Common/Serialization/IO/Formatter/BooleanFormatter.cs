using System;
using System.IO;

namespace Rogue.NET.Common.Serialization.Formatter
{
    public class BooleanFormatter : BaseFormatter<bool>
    {
        static Type _dataType = typeof(bool);

        public override Type DataType { get { return _dataType; } }

        protected override bool ReadImpl(Stream stream)
        {
            // Reads byte as int
            var value = stream.ReadByte();

            // Get the byte[] for this int, convert to boolean
            return BitConverter.ToBoolean(BitConverter.GetBytes(value), 0);
        }

        protected override void WriteImpl(Stream stream, bool theObject)
        {
            // Get buffer for the boolean 
            var buffer = BitConverter.GetBytes(theObject);

            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
