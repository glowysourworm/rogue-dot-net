using Rogue.NET.Common.Extension;
using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Rogue.NET.ScenarioEditor.Converter
{
    public class BehaviorHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = parameter as IList;
            if (value == null ||
                list == null)
                return Binding.DoNothing;

            var index = list.IndexOf(value);

            return (index + 1).ToOrdinal() + " Behavior";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
