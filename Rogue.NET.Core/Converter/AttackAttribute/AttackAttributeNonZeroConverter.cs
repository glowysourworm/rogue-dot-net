using System;
using System.Collections.Generic;
using System.Windows.Data;

using AttackAttributeClass = Rogue.NET.Core.Model.Scenario.Alteration.AttackAttribute;

namespace Rogue.NET.Core.Converter.AttackAttribute
{
    public class AttackAttributeNonzeroConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var attributeList = value as IEnumerable<AttackAttributeClass>;
            var filteredList = new List<AttackAttributeClass>();
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
