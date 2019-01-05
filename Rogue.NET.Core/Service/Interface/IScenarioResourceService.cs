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
        BitmapSource GetImageSource(SymbolDetailsTemplate symbolDetails);
        BitmapSource GetImageSource(ScenarioImage scenarioImage);
        BitmapSource GetImageSource(DisplayImageResources displayImageResources);
        BitmapSource GetDesaturatedImageSource(ScenarioImage scenarioImage);
        FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage);
        IEnumerable<ColorViewModel> GetColors();
    }
}
