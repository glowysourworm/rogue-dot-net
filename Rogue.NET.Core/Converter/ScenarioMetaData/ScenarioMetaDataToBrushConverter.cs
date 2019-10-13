using System;
using System.Windows.Data;
using System.Windows.Media;

using ScenarioMetaDataClass = Rogue.NET.Core.Model.Scenario.Abstract.ScenarioMetaData;

namespace Rogue.NET.Core.Converter.ScenarioMetaData
{
    public class ScenarioMetaDataToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var m = value as ScenarioMetaDataClass;

            if (m != null)
            {
                if (!m.IsIdentified)
                    return Brushes.White;

                else
                {
                    if (m.IsUnique)
                        return Brushes.Beige;

                    if (m.IsObjective)
                        return Brushes.Goldenrod;
                }
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
