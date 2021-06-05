using System;
using System.IO;

namespace Rogue.NET.Common.Serialization.Formatter
{
    public class DateTimeFormatter : BaseFormatter<DateTime>
    {
        static Type _dataType = typeof(DateTime);

        public override Type DataType { get { return _dataType; } }

        readonly IntegerFormatter _integerFormatter;

        public DateTimeFormatter()
        {
            _integerFormatter = new IntegerFormatter();
        }

        protected override DateTime ReadImpl(Stream stream)
        {
            var year = _integerFormatter.Read(stream);
            var month = _integerFormatter.Read(stream);
            var day = _integerFormatter.Read(stream);
            var hour = _integerFormatter.Read(stream);
            var minute = _integerFormatter.Read(stream);
            var second = _integerFormatter.Read(stream);
            var milliSecond = _integerFormatter.Read(stream);

            return new DateTime(year, month, day, hour, minute, second, milliSecond);
        }

        protected override void WriteImpl(Stream stream, DateTime theObject)
        {
            _integerFormatter.Write(stream, theObject.Year);
            _integerFormatter.Write(stream, theObject.Month);
            _integerFormatter.Write(stream, theObject.Day);
            _integerFormatter.Write(stream, theObject.Hour);
            _integerFormatter.Write(stream, theObject.Minute);
            _integerFormatter.Write(stream, theObject.Second);
            _integerFormatter.Write(stream, theObject.Millisecond);
        }
    }
}
