using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Processing.Service.Interface;
using System;
using System.Linq;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Converter
{
    public class SymbolImageConverter : IMultiValueConverter
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public SymbolImageConverter()
        {
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();
        }
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null ||
                values.Length != 13 ||
                values.Any(x => x == DependencyProperty.UnsetValue))
                return Binding.DoNothing;

            /*
                <Binding Path="CharacterColor" />
                <Binding Path="CharacterSymbol" />
                <Binding Path="CharacterSymbolCategory" />
                <Binding Path="Symbol" />
                <Binding Path="SymbolHue" />
                <Binding Path="SymbolSaturation" />
                <Binding Path="SymbolLightness" />
                <Binding Path="SmileyExpression" />
                <Binding Path="SmileyAuraColor" />
                <Binding Path="SmileyBodyColor" />
                <Binding Path="SmileyLineColor" />
                <Binding Path="GameSymbol" />
                <Binding Path="SymbolType" />
            */

            // Copy data into a ScenarioImage object
            var scenarioImage = new ScenarioImage()
            {
                CharacterColor = (string)values[0],
                CharacterSymbol = (string)values[1],
                CharacterSymbolCategory = (string)values[2],
                Symbol = (string)values[3],
                SymbolHue = (double)values[4],
                SymbolSaturation = (double)values[5],
                SymbolLightness = (double)values[6],
                SmileyExpression = (SmileyExpression)values[7],
                SmileyLightRadiusColor = (string)values[8],
                SmileyBodyColor = (string)values[9],
                SmileyLineColor = (string)values[10],
                GameSymbol = (string)values[11],
                SymbolType = (SymbolType)values[12]
            };

            // Have to validate the symbol data per type (These should be set in the constructor; but there's too many 
            // data changes to track. So, hopefully, this can be removed in the future). Should probably be validated
            // in the editor.
            //
            switch (scenarioImage.SymbolType)
            {
                case SymbolType.Smiley:
                    if (string.IsNullOrEmpty(scenarioImage.SmileyBodyColor) ||
                        string.IsNullOrEmpty(scenarioImage.SmileyLightRadiusColor) ||
                        string.IsNullOrEmpty(scenarioImage.SmileyLineColor))
                        return Binding.DoNothing;
                    break;
                case SymbolType.Character:
                    if (string.IsNullOrEmpty(scenarioImage.CharacterColor) ||
                        string.IsNullOrEmpty(scenarioImage.CharacterSymbol) ||
                        string.IsNullOrEmpty(scenarioImage.CharacterSymbolCategory))
                        return Binding.DoNothing;
                    break;
                case SymbolType.Symbol:
                    if (string.IsNullOrEmpty(scenarioImage.Symbol))
                        return Binding.DoNothing;
                    break;
                case SymbolType.Game:
                    if (string.IsNullOrEmpty(scenarioImage.GameSymbol))
                        return Binding.DoNothing;
                    break;
                default:
                    break;
            }

            // Read scale from the converter parameter
            var scale = parameter == null ? 1.0D : (double)parameter;

            return _scenarioResourceService.GetFrameworkElement(scenarioImage, scale);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
