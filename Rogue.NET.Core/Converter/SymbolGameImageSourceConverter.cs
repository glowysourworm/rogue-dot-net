﻿using Microsoft.Practices.ServiceLocation;

using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Service.Interface;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter
{
    public class SymbolGameImageSourceConverter : IValueConverter
    {
        readonly IScenarioResourceService _scenarioResourceService;
        public SymbolGameImageSourceConverter()
        {
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                value == DependencyProperty.UnsetValue)
                return Binding.DoNothing;

            var gameSymbol = value.ToString();
            var scale = parameter == null ? 1.0D : (double)parameter;

            return _scenarioResourceService.GetImageSource(new ScenarioImage()
            {
                SymbolType = SymbolType.Game,
                SymbolPath = gameSymbol

            }, scale, 1.0, Light.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
