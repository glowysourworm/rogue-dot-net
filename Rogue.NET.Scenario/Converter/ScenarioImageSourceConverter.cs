using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Service.Interface;
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
    public class ScenarioImageSourceConverter : IValueConverter
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public ScenarioImageSourceConverter()
        {
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var viewModel = value as ScenarioImageViewModel;
            if (viewModel == null)
                return null;

            return _scenarioResourceService.GetImageSource(
                        viewModel.RogueName,
                        viewModel.CharacterSymbol,
                        viewModel.CharacterColor,
                        viewModel.Icon,
                        viewModel.SmileyMood,
                        viewModel.SmileyBodyColor,
                        viewModel.SmileyLineColor,
                        viewModel.SmileyAuraColor,
                        viewModel.SymbolType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
