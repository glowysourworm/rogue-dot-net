using Rogue.NET.Core.IO;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Rogue.NET.Core.Service.Interface
{
    public interface IScenarioResourceService
    {
        void SetCacheMode(ResourceCacheMode resourceCacheMode);

        void ClearCache();

        /// <summary>
        /// Tells service to load and cache specified configuration.
        /// </summary>
        void LoadScenarioConfiguration(ConfigResources configResource);

        IEnumerable<ScenarioConfigurationContainer> GetScenarioConfigurations();

        IDictionary<string, ScenarioFileHeader> GetScenarioHeaders();

        ScenarioConfigurationContainer GetEmbeddedScenarioConfiguration(ConfigResources configResource);

        ScenarioConfigurationContainer OpenScenarioConfigurationFile(string file);

        void SaveConfig(string name, ScenarioConfigurationContainer config);

        BitmapSource GetImageSource(ScenarioImage scenarioImage);

        BitmapSource GetImageSource(
                string rogueName,
                string symbol,
                string symbolColor,
                ImageResources icon,
                SmileyMoods smileyMood,
                string smileyBodyColor,
                string smileyLineColor,
                string smileyAuraColor,
                SymbolTypes type);

        BitmapSource GetImage(ImageResources imageResources);
    }
}
