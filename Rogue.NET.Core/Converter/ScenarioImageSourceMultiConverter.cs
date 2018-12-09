using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Service.Interface;
using System;
using System.Linq;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Rogue.NET.Core.Converter
{
    public class ScenarioImageSourceMultiConverter : IMultiValueConverter
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public ScenarioImageSourceMultiConverter()
        {
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return Binding.DoNothing;

            else if (values.Any(x => x == DependencyProperty.UnsetValue))
                return Binding.DoNothing;

            else if (values.Length != 8)
                return Binding.DoNothing;

            return _scenarioResourceService.GetImageSource(new ScenarioImage()
            {
                CharacterSymbol = (string)values[0],
                CharacterColor = (string)values[1],
                Icon = (ImageResources)values[2],
                SmileyMood = (SmileyMoods)values[3],
                SmileyBodyColor = (string)values[4],
                SmileyLineColor = (string)values[5],
                SmileyAuraColor = (string)values[6],
                SymbolType = (SymbolTypes)values[7]
            });
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
