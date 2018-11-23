using Rogue.NET.Core.Model.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.ScenarioEditor.Converter
{
    public class EnemyAttackTypeSkillVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var attackType = (CharacterAttackType)value;

            return attackType == CharacterAttackType.Skill ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
