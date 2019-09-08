using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Processing.Service.Interface;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter
{
    public class ScenarioImageSourceConverter : IValueConverter
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public ScenarioImageSourceConverter()
        {
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var scenarioImage = value as ScenarioImage;
            if (scenarioImage == null)
                return Binding.DoNothing;

            return _scenarioResourceService.GetImageSource(scenarioImage, 1.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
