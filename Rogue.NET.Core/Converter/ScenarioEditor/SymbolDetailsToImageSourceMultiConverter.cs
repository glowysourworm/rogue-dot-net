using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Service.Interface;
using System;
using System.Linq;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Rogue.NET.Core.Converter.ScenarioEditor
{
    public class SymbolDetailsToImageSourceMultiConverter : IMultiValueConverter
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public SymbolDetailsToImageSourceMultiConverter()
        {
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 9)
                throw new Exception("Incorrect symbol details provided to SymbolDetailsToImageSourceMultiConverter");

            if (values.Any(x => x == DependencyProperty.UnsetValue))
                return null;

            return _scenarioResourceService.GetImageSource(
                    (string)values[0],          // RogueName (used for cache!)
                    (string)values[1],          // CharacterSymbol
                    (string)values[2],          // CharacterSymbolColor
                    (ImageResources)values[3],  // Icon
                    (SmileyMoods)values[4],     // SmileyMood
                    (string)values[5],          // SmileyBodyColor
                    (string)values[6],          // SmileyLineColor
                    (string)values[7],          // SmileyAuraColor
                    (SymbolTypes)values[8]);    // Type
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
