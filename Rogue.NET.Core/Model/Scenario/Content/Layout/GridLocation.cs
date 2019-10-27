using System;
using System.ComponentModel;
using System.Globalization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    [TypeConverter(typeof(GridLocationConverter))]
    [DefaultValue(typeof(GridLocationConverter), "(-1, -1)")]
    [Serializable]
    public struct GridLocation
    {
        public static GridLocation Empty = new GridLocation(-1, -1);

        public int Row { get; set; }
        public int Column { get; set; }

        public GridLocation(GridLocation copy)
        {
            this.Row = copy.Row;
            this.Column = copy.Column;
        }
        public GridLocation(int column, int row)
        {
            Row = row;
            Column = column;
        }
        public static bool operator ==(GridLocation location1, GridLocation location2)
        {
            return location1.Equals(location2);
        }
        public static bool operator !=(GridLocation location1, GridLocation location2)
        {
            return !location1.Equals(location2);
        }
        public override bool Equals(object obj)
        {
            if (obj is GridLocation)
            {
                var location = (GridLocation)obj;
                return location.Column == this.Column && location.Row == this.Row;
            }

            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return "Column=" + Column.ToString() + " Row=" + Row.ToString();
        }
    }

    public class GridLocationConverter : TypeConverter
    {
        public GridLocationConverter()
        {
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(GridLocation);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                return "(-1, -1)";

            var location = (GridLocation)value;

            return "(" + location.Column.ToString() + ", " + location.Row + ")";
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return GridLocation.Empty;

            var stringValue = value.ToString();

            if (string.IsNullOrWhiteSpace(stringValue))
                return GridLocation.Empty;

            var pieces = stringValue.Trim()
                                    .Replace("(", "")
                                    .Replace(")", "")
                                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (pieces.Length != 2)
                return GridLocation.Empty;

            var column = 0;
            var row = 0;

            if (!int.TryParse(pieces[0].Trim(), out column))
                return GridLocation.Empty;

            if (!int.TryParse(pieces[1].Trim(), out row))
                return GridLocation.Empty;

            return new GridLocation(column, row);
        }
        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return true;
        }
    }
}
