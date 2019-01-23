using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Rogue.NET.Core.Converter.ItemGrid
{
    public class ItemGridIconImageConverter : IValueConverter
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public ItemGridIconImageConverter()
        {
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            var icon = (ImageResources)value;

            return _scenarioResourceService.GetImageSource(new ScenarioImage("", icon));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
