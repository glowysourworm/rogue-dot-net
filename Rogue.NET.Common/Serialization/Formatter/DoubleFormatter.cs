using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization.Formatter
{
    public class DoubleFormatter : BaseFormatter<double>
    {
        readonly byte[] _buffer;

        public DoubleFormatter()
        {
            _buffer = new byte[sizeof(long)];
        }

        protected override double ReadImpl(Stream stream)
        {
            // Reads byte as int
            stream.Read(_buffer, 0, _buffer.Length);

            // Get the byte[] for this int, convert to boolean
            var longValue = BitConverter.ToInt64(_buffer, 0);

            return BitConverter.Int64BitsToDouble(longValue);
        }

        protected override void WriteImpl(Stream stream, double theObject)
        {
            // Get buffer for the double as 64 bit
            var double64 = BitConverter.DoubleToInt64Bits(theObject);
            var buffer = BitConverter.GetBytes(double64);

            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
