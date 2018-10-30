using Rogue.NET.Core.Model.Enums;
using System;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter.ScenarioEditor
{
    public class SymbolEditorVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var desiredValue = (SymbolTypes)parameter;
            var actualValue = (SymbolTypes)value;
            return (desiredValue == actualValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
