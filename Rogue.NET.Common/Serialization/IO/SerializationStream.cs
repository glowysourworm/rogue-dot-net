using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Serialization.Formatter;
using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.IO.Interface;
using Rogue.NET.Common.Serialization.Manifest;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization.IO
{
    /// <summary>
    /// Wrapper for USER STREAM to perform serialization
    /// </summary>
    internal class SerializationStream : ISerializationStreamReader, ISerializationStreamWriter
    {
        // Collection of formatters for serialization
        SimpleDictionary<Type, IBaseFormatter> _formatters;

        readonly Stream _stream;
        readonly List<SerializedStreamData> _streamData;

        public SerializationStream(Stream stream)
        {
            _stream = stream;
            _streamData = new List<SerializedStreamData>();
            _formatters = new SimpleDictionary<Type, IBaseFormatter>();
        }

        public IEnumerable<SerializedStreamData> GetStreamData()
        {
            return _streamData;
        }

        public void Write<T>(T theObject)
        {
            Write(theObject, typeof(T));
        }

        public void Write(object theObject, Type theObjectType)
        {
            var formatter = SelectFormatter(theObjectType);

            Write(formatter, theObject);
        }

        public T Read<T>()
        {
            return (T)Read(typeof(T));
        }

        public object Read(Type type)
        {
            var formatter = SelectFormatter(type);

            object result = null;

            Read(formatter, out result);

            return result;
        }

        private void Read(IBaseFormatter formatter, out object theObject)
        {
            var position = _stream.Position;

            theObject = formatter.Read(_stream);

            //_streamData.Add(new SerializedStreamData()
            //{
            //    DataType = formatter.DataType,
            //    DataSize = _stream.Position - position,
            //    Position = position
            //});
        }

        private void Write(IBaseFormatter formatter, object theObject)
        {
            var position = _stream.Position;

            formatter.Write(_stream, theObject);

            //_streamData.Add(new SerializedStreamData()
            //{
            //    DataType = formatter.DataType,
            //    DataSize = _stream.Position - position,
            //    Position = position
            //});
        }

        private IBaseFormatter SelectFormatter(Type type)
        {
            if (_formatters.ContainsKey(type))
                return _formatters[type];

            IBaseFormatter formatter = null;

            if (FormatterFactory.IsPrimitiveSupported(type))
            {
                formatter = FormatterFactory.CreatePrimitiveFormatter(type);
            }
            else
            {
                formatter = FormatterFactory.CreateFormatter(type);
            }

            _formatters.Add(type, formatter);

            return formatter;
        }
    }
}
