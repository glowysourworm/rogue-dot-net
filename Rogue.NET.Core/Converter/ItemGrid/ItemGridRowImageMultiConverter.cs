using System;
using System.Linq;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Service.Interface;
using Microsoft.Practices.ServiceLocation;

namespace Rogue.NET.Core.Converter.ItemGrid
{
    public class ItemGridRowImageMultiConverter : IMultiValueConverter
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public ItemGridRowImageMultiConverter()
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

            var model = new ScenarioImage()
            {
                CharacterColor = (string)values[0],
                CharacterSymbol = (string)values[1],
                Icon = (ImageResources)values[2],
                DisplayIcon = (DisplayImageResources)values[3],
                SmileyExpression = (SmileyExpression)values[4],
                SmileyLightRadiusColor = (string)values[5],
                SmileyBodyColor = (string)values[6],
                SmileyLineColor = (string)values[7],
                SymbolType = (SymbolTypes)values[8]
            };

            return _scenarioResourceService.GetFrameworkElement(model, 1.15, true);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
