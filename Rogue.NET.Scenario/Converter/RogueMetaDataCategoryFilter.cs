using Rogue.NET.Scenario.Content.ViewModel.Content;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Rogue.NET.Scenario.Converter
{
    public class RogueMetaDataCategoryFilter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as IEnumerable<RogueEncyclopediaCategoryViewModel>;
            if (collection == null)
                return Binding.DoNothing;

            return collection.Where(x => x.Items.Count > 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
