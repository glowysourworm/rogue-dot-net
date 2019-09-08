﻿using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Rogue.NET.ScenarioEditor.Converter
{
    public class SymbolDetailsImageSourceConverter : IValueConverter
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public SymbolDetailsImageSourceConverter()
        {
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var symbolDetails = value as SymbolDetailsTemplateViewModel;
            if (symbolDetails == null)
                return Binding.DoNothing;

            var model = symbolDetails.Map<SymbolDetailsTemplateViewModel, SymbolDetailsTemplate>();

            return _scenarioResourceService.GetImageSource(model, 1.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
