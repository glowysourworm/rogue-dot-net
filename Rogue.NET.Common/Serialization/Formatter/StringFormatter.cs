using System;
using System.IO;

namespace Rogue.NET.Common.Serialization.Formatter
{
    public class StringFormatter : BaseFormatter<string>
    {
        readonly IntegerFormatter _integerFormatter;

        readonly byte[] _charBuffer;

        public StringFormatter()
        {
            _integerFormatter = new IntegerFormatter();
            _charBuffer = new byte[sizeof(char)];
        }

        protected override string ReadImpl(Stream stream)
        {
            // [ Char Count, char0, char1, .., charN ]
            var count = _integerFormatter.Read(stream);
            var array = new char[count];

            for (int i = 0; i < count; i++)
            {
                // Read characters from stream
                stream.Read(_charBuffer, 0, _charBuffer.Length);

                array[i] = BitConverter.ToChar(_charBuffer, 0);
            }

            return new string(array);
        }

        protected override void WriteImpl(Stream stream, string theObject)
        {
            // [ Char Count, char0, char1, .., charN ]

            // Character Count
            _integerFormatter.Write(stream, theObject.Length);

            for (int i = 0; i < theObject.Length; i++)
            {
                var charBuffer = BitConverter.GetBytes(theObject[i]);

                // Char[i]
                stream.Write(charBuffer, 0, charBuffer.Length);
            }
        }
    }
}
