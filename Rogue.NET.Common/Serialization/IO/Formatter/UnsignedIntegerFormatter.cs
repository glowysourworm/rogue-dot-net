using System;
using System.IO;

namespace Rogue.NET.Common.Serialization.Formatter
{
    public class UnsignedIntegerFormatter : BaseFormatter<uint>
    {
        static Type _dataType = typeof(uint);

        public override Type DataType { get { return _dataType; } }

        readonly byte[] _buffer;

        public UnsignedIntegerFormatter()
        {
            _buffer = new byte[sizeof(uint)];
        }

        protected override uint ReadImpl(Stream stream)
        {
            stream.Read(_buffer, 0, _buffer.Length);

            return BitConverter.ToUInt32(_buffer, 0);
        }

        protected override void WriteImpl(Stream stream, uint theObject)
        {
            var buffer = BitConverter.GetBytes(theObject);

            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
