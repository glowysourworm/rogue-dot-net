using Rogue.NET.Core.Model.Enums;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using SymbolDetailsTemplateClass = Rogue.NET.Core.Model.ScenarioConfiguration.Abstract.SymbolDetailsTemplate;

namespace Rogue.NET.Core.Converter.SymbolDetailsTemplate
{
    public class SymbolDetailsTemplateAuraToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return null;

            if (values[0] == DependencyProperty.UnsetValue)
                return null;

            var template = new SymbolDetailsTemplateClass()
            {
                CharacterColor = (string)values[0],
                CharacterSymbol = (string)values[1],
                Icon = (ImageResources)values[2],
                SmileyAuraColor = (string)values[3],
                SmileyBodyColor = (string)values[4],
                SmileyLineColor = (string)values[5],
                SmileyMood = (SmileyMoods)values[6],
                Type = (SymbolTypes)values[7]
            };

            return template == null ? null :
                    (template.Type != SymbolTypes.Smiley ? Colors.Transparent :
                        ColorConverter.ConvertFromString(template.SmileyAuraColor));
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
