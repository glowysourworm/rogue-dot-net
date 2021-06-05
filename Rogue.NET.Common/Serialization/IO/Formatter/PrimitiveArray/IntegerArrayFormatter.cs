using Rogue.NET.Common.Serialization.Formatter;

using System;
using System.IO;

namespace Rogue.NET.Common.Serialization.IO.Formatter.PrimitiveArray
{
    internal class IntegerArrayFormatter : BaseFormatter<int[]>
    {
        static Type _dataType = typeof(int[]);

        public override Type DataType { get { return _dataType; } }

        readonly IntegerFormatter _integerFormatter;

        internal IntegerArrayFormatter()
        {
            _integerFormatter = new IntegerFormatter();
        }

        protected override int[] ReadImpl(Stream stream)
        {
            // READ BUFFER LENGTH
            var bufferLength = _integerFormatter.Read(stream);
            var buffer = new byte[bufferLength];

            var intArray = new int[bufferLength / sizeof(int)];

            // READ BUFFER
            stream.Read(buffer, 0, bufferLength);

            // COPY TO DEST ARRAY. HOPING ON HIGH PERFORMANCE!
            Buffer.BlockCopy(buffer, 0, intArray, 0, bufferLength);

            return intArray;
        }

        protected override void WriteImpl(Stream stream, int[] theObject)
        {
            var buffer = new byte[theObject.Length * sizeof(int)];

            // HOPING ON HIGH PERFORMANCE!
            Buffer.BlockCopy(theObject, 0, buffer, 0, buffer.Length);

            // WRITE BUFFER LENGTH
            _integerFormatter.Write(stream, buffer.Length);

            // WRITE BUFFER 
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
