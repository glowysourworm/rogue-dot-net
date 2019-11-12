using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Service.Interface;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter
{
    public class SymbolImageConverter : IMultiValueConverter
    {
        readonly IScenarioResourceService _scenarioResourceService;
        readonly SymbolImageSourceConverter symbolImageSourceConverter;
        public SymbolImageConverter()
        {
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();
            symbolImageSourceConverter = new SymbolImageSourceConverter();
        }
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Parse input using a common base routine
            var scenarioImage = symbolImageSourceConverter.GetScenarioImage(values, targetType, parameter, culture);

            if (scenarioImage == null)
                return Binding.DoNothing;

            // Read scale from the converter parameter
            var scale = parameter == null ? 1.0D : (double)parameter;

            return _scenarioResourceService.GetFrameworkElement(scenarioImage, scale, Light.White);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
