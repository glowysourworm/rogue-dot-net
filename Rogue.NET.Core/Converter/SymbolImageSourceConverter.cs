﻿using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Processing.Service.Interface;
using System;
using System.Linq;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using Rogue.NET.Core.Model.Scenario.Content.Layout;

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
                values.Length != 15 ||
                values.Any(x => x == DependencyProperty.UnsetValue))
                return null;

            /*
                <Binding Path="CharacterColor" />
                <Binding Path="CharacterSymbol" />
                <Binding Path="CharacterSymbolCategory" />
                <Binding Path="CharacterScale" />
                <Binding Path="Symbol" />
                <Binding Path="SymbolHue" />
                <Binding Path="SymbolSaturation" />
                <Binding Path="SymbolLightness" />
                <Binding Path="SymbolScale" />
                <Binding Path="SymbolUseColorMask" />
                <Binding Path="SmileyExpression" />
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
                CharacterScale = (double)values[3],
                Symbol = (string)values[4],
                SymbolHue = (double)values[5],
                SymbolSaturation = (double)values[6],
                SymbolLightness = (double)values[7],
                SymbolScale = (double)values[8],
                SymbolUseColorMask = (bool)values[9],
                SmileyExpression = (SmileyExpression)values[10],
                SmileyBodyColor = (string)values[11],
                SmileyLineColor = (string)values[12],
                GameSymbol = (string)values[13],
                SymbolType = (SymbolType)values[14]
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
                    if (string.IsNullOrEmpty(scenarioImage.CharacterColor) ||
                        string.IsNullOrEmpty(scenarioImage.CharacterSymbol) ||
                        string.IsNullOrEmpty(scenarioImage.CharacterSymbolCategory))
                        return null;
                    break;
                case SymbolType.Symbol:
                case SymbolType.OrientedSymbol:
                case SymbolType.Terrain:
                    if (string.IsNullOrEmpty(scenarioImage.Symbol))
                        return null;
                    break;
                case SymbolType.Game:
                    if (string.IsNullOrEmpty(scenarioImage.GameSymbol))
                        return null;
                    break;
                default:
                    break;
            }

            return scenarioImage;
        }
    }
}
