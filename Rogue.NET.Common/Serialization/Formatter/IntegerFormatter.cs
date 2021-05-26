using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization.Formatter
{
    public class IntegerFormatter : BaseFormatter<int>
    {
        readonly byte[] _buffer;

        public IntegerFormatter()
        {
            _buffer = new byte[sizeof(int)];
        }

        protected override int ReadImpl(Stream stream)
        {
            stream.Read(_buffer, 0, _buffer.Length);

            return BitConverter.ToInt32(_buffer, 0);
        }

        protected override void WriteImpl(Stream stream, int theObject)
        {
            var buffer = BitConverter.GetBytes(theObject);

            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
