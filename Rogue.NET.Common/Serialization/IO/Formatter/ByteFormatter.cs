using System;
using System.IO;

namespace Rogue.NET.Common.Serialization.Formatter
{
    public class ByteFormatter : BaseFormatter<byte>
    {
        static Type _dataType = typeof(byte);

        public override Type DataType { get { return _dataType; } }

        protected override byte ReadImpl(Stream stream)
        {
            return System.Convert.ToByte(stream.ReadByte());
        }

        protected override void WriteImpl(Stream stream, byte theObject)
        {
            stream.WriteByte(theObject);
        }
    }
}
