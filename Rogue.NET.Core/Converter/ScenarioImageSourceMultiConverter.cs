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

            else if (values.Length != 9)
                return Binding.DoNothing;

            return _scenarioResourceService.GetImageSource(new ScenarioImage()
            {
                CharacterSymbol = (string)values[0],
                CharacterColor = (string)values[1],
                Icon = (ImageResources)values[2],
                DisplayIcon = (DisplayImageResources)values[3],
                SmileyMood = (SmileyMoods)values[4],
                SmileyBodyColor = (string)values[5],
                SmileyLineColor = (string)values[6],
                SmileyAuraColor = (string)values[7],
                SymbolType = (SymbolTypes)values[8]
            });
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
