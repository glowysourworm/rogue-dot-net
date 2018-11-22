using AgileObjects.AgileMapper;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Converter;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Scenario.ViewModel.ItemGrid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Rogue.NET.Scenario.Converter
{
    public class ItemGridRowImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var viewModel = value as ItemGridRowViewModel;
            if (viewModel == null)
                return null;

            var model = new ScenarioImage()
            {
                CharacterColor = viewModel.CharacterColor,
                CharacterSymbol = viewModel.CharacterSymbol,
                Icon = viewModel.Icon,
                SmileyMood = viewModel.SmileyMood,
                SmileyAuraColor = viewModel.SmileyAuraColor,
                SmileyBodyColor = viewModel.SmileyBodyColor,
                SmileyLineColor = viewModel.SmileyLineColor,
                SymbolType = viewModel.SymbolType
            };
            var converter = new ScenarioImageSourceConverter();
            return converter.Convert(model, targetType, false, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
