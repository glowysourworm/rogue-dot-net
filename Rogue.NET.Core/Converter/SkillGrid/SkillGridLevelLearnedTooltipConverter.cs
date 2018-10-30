using System;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter.SkillGrid
{
    public class SkillGridLevelLearnedTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int lvl = (int)value;
            return "Skill Unlocked at Level " + lvl.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
