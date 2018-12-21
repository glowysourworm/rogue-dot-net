using Rogue.NET.Core.Model.Enums;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.ScenarioEditor.Converter
{
    public class AlterationHasEffectSymbolChangeVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Binding.DoNothing;

            else if (values.Length != 2)
                return Binding.DoNothing;

            else if (values.Any(x => x == DependencyProperty.UnsetValue))
                return Binding.DoNothing;

            var type = (AlterationType)values[0];
            var attackAttributeType = (AlterationAttackAttributeType)values[1];

            return (type == AlterationType.PassiveAura ||
                   type == AlterationType.PassiveSource ||
                   type == AlterationType.TemporaryAllTargets ||
                   type == AlterationType.TemporarySource ||
                   type == AlterationType.TemporaryTarget ||
                   (type == AlterationType.AttackAttribute &&
                    (attackAttributeType == AlterationAttackAttributeType.Passive ||
                     attackAttributeType == AlterationAttackAttributeType.TemporaryFriendlySource ||
                     attackAttributeType == AlterationAttackAttributeType.TemporaryFriendlyTarget ||
                     attackAttributeType == AlterationAttackAttributeType.TemporaryMalignSource ||
                     attackAttributeType == AlterationAttackAttributeType.TemporaryMalignTarget))) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
