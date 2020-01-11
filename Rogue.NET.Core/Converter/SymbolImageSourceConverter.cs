using Microsoft.Practices.ServiceLocation;

using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Service.Interface;

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter
{
    public class SymbolImageSourceConverter : IMultiValueConverter
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public SymbolImageSourceConverter()
        {
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();
        }
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Run base routine to get scenario image
            var scenarioImage = GetScenarioImage(values, targetType, parameter, culture);

            if (scenarioImage == null)
                return Binding.DoNothing;

            // Read scale from the converter parameter
            var scale = parameter == null ? 1.0D : (double)parameter;

            return _scenarioResourceService.GetImageSource(scenarioImage, scale, 1.0, Light.White);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public ScenarioImage GetScenarioImage(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null ||
                values.Length != 12 ||
                values.Any(x => x == DependencyProperty.UnsetValue))
                return null;

            /*
                <Binding Path="BackgroundColor" />
                <Binding Path="SmileyExpression" />
                <Binding Path="SmileyBodyColor" />
                <Binding Path="SmileyLineColor" />
                <Binding Path="SymbolClampColor" />
                <Binding Path="SymbolEffectType" />
                <Binding Path="SymbolHue" />
                <Binding Path="SymbolLightness" />
                <Binding Path="SymbolPath" />
                <Binding Path="SymbolSaturation" />
                <Binding Path="SymbolSize" />
                <Binding Path="SymbolType" />
            */

            // Copy data into a ScenarioImage object
            var scenarioImage = new ScenarioImage()
            {
                BackgroundColor = (string)values[0],
                SmileyExpression = (SmileyExpression)values[1],
                SmileyBodyColor = (string)values[2],
                SmileyLineColor = (string)values[3],
                SymbolClampColor = (string)values[4],
                SymbolEffectType = (CharacterSymbolEffectType)values[5],
                SymbolHue = (double)values[6],
                SymbolLightness = (double)values[7],
                SymbolPath = (string)values[8],
                SymbolSaturation = (double)values[9],
                SymbolSize = (CharacterSymbolSize)values[10],
                SymbolType = (SymbolType)values[11]
            };

            // Have to validate the symbol data per type (These should be set in the constructor; but there's too many 
            // data changes to track. So, hopefully, this can be removed in the future). Should probably be validated
            // in the editor.
            //
            switch (scenarioImage.SymbolType)
            {
                case SymbolType.Smiley:
                    if (string.IsNullOrEmpty(scenarioImage.SmileyBodyColor) ||
                        string.IsNullOrEmpty(scenarioImage.SmileyLineColor))
                        return null;
                    break;
                case SymbolType.Character:
                case SymbolType.Symbol:
                case SymbolType.OrientedSymbol:
                case SymbolType.Terrain:
                case SymbolType.Game:
                    if (string.IsNullOrEmpty(scenarioImage.SymbolPath))
                        return null;
                    break;
                default:
                    break;
            }

            return scenarioImage;
        }
    }
}
