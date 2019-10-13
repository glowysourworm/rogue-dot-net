using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Processing.Service.Cache;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Service.Interface
{
    /// <summary>
    /// Facade component for all resource services
    /// </summary>
    public interface IScenarioResourceService
    {
        IEnumerable<ColorViewModel> GetColors();
        ScenarioConfigurationContainer GetScenarioConfiguration(string configurationName);

        // IScenarioCache
        IEnumerable<ScenarioContainer> GetScenarios();
        void SaveScenario(ScenarioContainer scenario);
        void DeleteScenario(ScenarioContainer scenario);

        // IScenarioConfigurationCache
        IEnumerable<ScenarioConfigurationContainer> EmbeddedConfigurations { get; }
        IEnumerable<ScenarioConfigurationContainer> UserConfigurations { get; }
        ScenarioImage GetRandomSmileyCharacter();
        void SaveConfiguration(ScenarioConfigurationContainer configuration);
        void EmbedConfiguration(ScenarioConfigurationContainer configuration);

        // IScenarioImageSourceFactory
        DrawingImage GetImageSource(SymbolDetailsTemplate symbolDetails, double scale);
        DrawingImage GetImageSource(ScenarioImage scenarioImage, double scale);
        DrawingImage GetDesaturatedImageSource(ScenarioImage scenarioImage, double scale);
        FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage, double scale);
        
        // ISvgCache
        DrawingGroup GetDrawing(ScenarioCacheImage scenarioCacheImage);
        IEnumerable<string> GetResourceNames(SymbolType type);
        IEnumerable<string> GetCharacterCategories();
        IEnumerable<string> GetCharacterResourceNames(string category);
    }
}
