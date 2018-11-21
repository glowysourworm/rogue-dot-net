using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Service.Interface;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter
{
    public class ScenarioImageElementConverter : IValueConverter
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public ScenarioImageElementConverter()
        {
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var scenarioImage = value as ScenarioImage;
            if (scenarioImage == null)
                return Binding.DoNothing;

            return _scenarioResourceService.GetFrameworkElement(scenarioImage);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
