﻿using Rogue.NET.Core.Converter;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Scenario.ViewModel.ItemGrid;
using System;
using System.Linq;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Service.Interface;
using Microsoft.Practices.ServiceLocation;

namespace Rogue.NET.Scenario.Converter
{
    public class ItemGridRowImageSourceMultiConverter : IMultiValueConverter
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public ItemGridRowImageSourceMultiConverter()
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

            var model = new ScenarioImage()
            {
                CharacterColor = (string)values[0],
                CharacterSymbol = (string)values[1],
                Icon = (ImageResources)values[2],
                SmileyMood = (SmileyMoods)values[3],
                SmileyAuraColor = (string)values[4],
                SmileyBodyColor = (string)values[5],
                SmileyLineColor = (string)values[6],
                SymbolType = (SymbolTypes)values[7]
            };

            return _scenarioResourceService.GetImageSource(model);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}