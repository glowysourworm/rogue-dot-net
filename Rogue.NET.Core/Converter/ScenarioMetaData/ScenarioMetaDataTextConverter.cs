using System;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter.ScenarioMetaData
{
    public class ScenarioMetaDataTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool b = (bool)parameter;
            if (!b)
                return "???";

            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
