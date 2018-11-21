using ExpressMapper;
using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Converter;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Rogue.NET.Scenario.Converter
{
    public class ScenarioImageViewModelSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var viewModel = value as ScenarioImageViewModel;
            if (viewModel == null)
                return null;

            var model = viewModel.Map<ScenarioImageViewModel, ScenarioImage>();
            var converter = new ScenarioImageSourceConverter();
            return converter.Convert(model, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
