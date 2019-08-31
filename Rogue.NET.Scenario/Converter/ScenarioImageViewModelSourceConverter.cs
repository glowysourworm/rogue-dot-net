using Rogue.NET.Core.Converter;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Rogue.NET.Scenario.Converter
{
    public class ScenarioImageViewModelSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var viewModel = value as ScenarioImageViewModel;
            if (viewModel == null)
                return null;

            var model = new ScenarioImage()
            {
                CharacterColor = viewModel.CharacterColor,
                CharacterSymbol = viewModel.CharacterSymbol,
                Icon = viewModel.Icon,
                SmileyExpression = viewModel.SmileyExpression,
                SmileyBodyColor = viewModel.SmileyBodyColor,
                SmileyLineColor = viewModel.SmileyLineColor,
                SmileyLightRadiusColor = viewModel.SmileyAuraColor,
                SymbolType = viewModel.SymbolType,
                RogueName = viewModel.RogueName
            };
            var converter = new ScenarioImageSourceConverter();
            return converter.Convert(model, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
