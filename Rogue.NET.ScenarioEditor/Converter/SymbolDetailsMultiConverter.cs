using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Rogue.NET.ScenarioEditor.Converter
{
    public class SymbolDetailsMultiConverter : IMultiValueConverter
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public SymbolDetailsMultiConverter()
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

            return _scenarioResourceService.GetImageSource(new SymbolDetailsTemplate()
            {
                CharacterSymbol = (string)values[0],
                CharacterColor = (string)values[1],
                Icon = (ImageResources)values[2],
                SmileyMood = (SmileyMoods)values[3],
                SmileyBodyColor = (string)values[4],
                SmileyLineColor = (string)values[5],
                SmileyAuraColor = (string)values[6],
                Type = (SymbolTypes)values[7]
            });
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
