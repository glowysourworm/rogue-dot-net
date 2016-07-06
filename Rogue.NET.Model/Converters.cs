using Rogue.NET.Common;
using Rogue.NET.Model.Generation;
using Rogue.NET.Model.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Rogue.NET.Model
{
    public class SymbolDetailsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var template = value as SymbolDetailsTemplate;
            return template == null ? null : TemplateGenerator.GenerateSymbol(template).SymbolImageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class SymbolDetailsAuraConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return null;

            if (values[0] == DependencyProperty.UnsetValue)
                return null;

            var template = new SymbolDetailsTemplate()
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

    public class SymbolDetailsMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return null;

            if (values[0] == DependencyProperty.UnsetValue)
                return null;

            var symbolDetails = new SymbolDetailsTemplate()
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

            return TemplateGenerator.GenerateSymbol(symbolDetails).SymbolImageSource;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AttackAttributeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var attrib = value as AttackAttribute;
            if (attrib != null)
            {
                if (((int)attrib.Attack) == 0 &&
                    ((int)attrib.Resistance) == 0 &&
                    ((int)attrib.Weakness) == 0)
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DungeonMetaDataToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ScenarioMetaData m = value as ScenarioMetaData;

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
    public class DungeonMetaDataTextConverter : IValueConverter
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
    public class AttackAttributeNonzeroConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var attributeList = value as IEnumerable<AttackAttribute>;
            var filteredList = new List<AttackAttribute>();
            if (attributeList != null)
            {
                foreach (var attribute in attributeList)
                {
                    if (attribute.Weakness == 0 && attribute.Resistance == 0 && attribute.Attack == 0)
                        continue;

                    filteredList.Add(attribute);
                }
            }
            return filteredList;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
