using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Rogue.NET.Core.Service.Interface
{
    public interface IScenarioResourceService
    {
        void LoadAllConfigurations();
        IEnumerable<ScenarioConfigurationContainer> GetScenarioConfigurations();
        ScenarioConfigurationContainer GetScenarioConfiguration(ConfigResources configResource);
        ScenarioConfigurationContainer GetScenarioConfiguration(string configurationName);
        BitmapSource GetImageSource(SymbolDetailsTemplate symbolDetails, double scale);
        BitmapSource GetImageSource(ScenarioImage scenarioImage, double scale);
        BitmapSource GetDesaturatedImageSource(ScenarioImage scenarioImage, double scale);
        FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage, double scale);
        IEnumerable<ColorViewModel> GetColors();
    }
}
