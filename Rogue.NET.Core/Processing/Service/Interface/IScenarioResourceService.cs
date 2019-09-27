using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rogue.NET.Core.Processing.Service.Interface
{
    public interface IScenarioResourceService
    {
        void LoadCustomConfigurations();
        IEnumerable<ScenarioConfigurationContainer> GetScenarioConfigurations();
        ScenarioConfigurationContainer GetScenarioConfiguration(ConfigResources configResource);
        ScenarioConfigurationContainer GetScenarioConfiguration(string configurationName);
        ImageSource GetImageSource(SymbolDetailsTemplate symbolDetails, double scale);
        ImageSource GetImageSource(ScenarioImage scenarioImage, double scale);
        ImageSource GetDesaturatedImageSource(ScenarioImage scenarioImage, double scale);
        FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage, double scale);
        IEnumerable<ColorViewModel> GetColors();

        // Returns a random character smiley face from the collection of combined characters from all scenario configurations
        ScenarioImage GetRandomSmileyCharacter();
    }
}
