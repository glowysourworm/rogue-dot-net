using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Processing.Service.Cache;
using Rogue.NET.Core.Processing.Service.Cache.Interface;
using Rogue.NET.Core.Processing.Service.Interface;

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioResourceService))]
    public class ScenarioResourceService : IScenarioResourceService
    {
        readonly IScenarioConfigurationCache _scenarioConfigurationCache;
        readonly IScenarioCache _scenarioCache;
        readonly ISvgCache _svgCache;
        readonly IScenarioImageSourceFactory _scenarioImageSourceFactory;

        public IEnumerable<string> EmbeddedConfigurations
        {
            get { return _scenarioConfigurationCache.EmbeddedConfigurations; }
        }

        public IEnumerable<string> UserConfigurations
        {
            get { return _scenarioConfigurationCache.UserConfigurations; }
        }

        protected IList<string> TempFiles = new List<string>();

        [ImportingConstructor]
        public ScenarioResourceService(IScenarioConfigurationCache scenarioConfigurationCache,
                                       IScenarioCache scenarioCache,
                                       ISvgCache svgCache,
                                       IScenarioImageSourceFactory scenarioImageSourceFactory)
        {
            _scenarioConfigurationCache = scenarioConfigurationCache;
            _scenarioCache = scenarioCache;
            _svgCache = svgCache;
            _scenarioImageSourceFactory = scenarioImageSourceFactory;
        }

        public IEnumerable<ScenarioInfo> GetScenarioInfos()
        {
            return _scenarioCache.GetScenarioInfos();
        }

        public ScenarioContainer GetScenario(ScenarioInfo scenarioInfo)
        {
            return _scenarioCache.Get(scenarioInfo);
        }

        public void SaveScenario(ScenarioContainer scenario)
        {
            _scenarioCache.Save(scenario);
        }

        public void DeleteScenario(ScenarioInfo scenarioInfo)
        {
            _scenarioCache.Delete(scenarioInfo);
        }

        public void LoadLevel(ScenarioContainer scenarioContainer, int levelNumber)
        {
            _scenarioCache.LoadLevel(scenarioContainer, levelNumber);
        }

        public void SaveLevel(ScenarioContainer scenario, Level level)
        {
            _scenarioCache.SaveLevel(scenario, level);
        }

        public RogueFileDatabaseEntry CreateScenarioEntry(string rogueName, string configurationName, int seed)
        {
            return _scenarioCache.CreateScenarioEntry(rogueName, configurationName, seed);
        }

        public ScenarioImage GetRandomSmileyCharacter(bool eliminateChoice)
        {
            return _scenarioConfigurationCache.GetRandomSmileyCharacter(eliminateChoice);
        }

        public IEnumerable<ScenarioConfigurationInfo> GetScenarioConfigurationInfos()
        {
            return _scenarioConfigurationCache.GetScenarioConfigurationInfos();
        }

        // NOTE*** THIS WILL RETURN A CLONE OF THE CONFIGURATION
        public ScenarioConfigurationContainer GetScenarioConfiguration(string configurationName)
        {
            return _scenarioConfigurationCache.GetConfiguration(configurationName);
        }

        public void SaveConfiguration(ScenarioConfigurationContainer configuration)
        {
            _scenarioConfigurationCache.SaveConfiguration(configuration);
        }

        public void EmbedConfiguration(ScenarioConfigurationContainer configuration)
        {
            _scenarioConfigurationCache.EmbedConfiguration(configuration);
        }

        public DrawingImage GetImageSource(SymbolDetailsTemplate symbolDetails, double scale, double effectiveVision, params Light[] lighting)
        {
            return _scenarioImageSourceFactory.GetImageSource(symbolDetails, scale, effectiveVision, lighting);
        }

        public DrawingImage GetImageSource(ScenarioImage scenarioImage, double scale, double effectiveVision, params Light[] lighting)
        {
            return _scenarioImageSourceFactory.GetImageSource(scenarioImage, scale, effectiveVision, lighting);
        }

        public DrawingImage GetDesaturatedImageSource(SymbolDetailsTemplate symbolDetails, double scale, double effectiveVision, params Light[] lighting)
        {
            return _scenarioImageSourceFactory.GetDesaturatedImageSource(symbolDetails, scale, effectiveVision, lighting);
        }

        public DrawingImage GetDesaturatedImageSource(ScenarioImage scenarioImage, double scale, double effectiveVision, Light[] lighting)
        {
            return _scenarioImageSourceFactory.GetDesaturatedImageSource(scenarioImage, scale, effectiveVision, lighting);
        }

        public FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage, double scale, double effectiveVision, Light[] lighting)
        {
            return _scenarioImageSourceFactory.GetFrameworkElement(scenarioImage, scale, effectiveVision, lighting);
        }

        public DrawingGroup GetDrawing(ScenarioCacheImage scenarioCacheImage)
        {
            return _svgCache.GetDrawing(scenarioCacheImage);
        }

        public IEnumerable<string> GetResourceNames(SymbolType type)
        {
            return _svgCache.GetResourceNames(type);
        }

        public IEnumerable<string> GetCharacterCategories()
        {
            return _svgCache.GetCharacterCategories();
        }

        public IEnumerable<string> GetCharacterResourceNames(string category)
        {
            return _svgCache.GetCharacterResourceNames(category);
        }

        // TEMP FILE STORAGE
        public string SaveToTempFile<T>(T theObject, bool compress)
        {
            var buffer = BinarySerializer.Serialize(theObject);

            if (compress)
                buffer = ZipEncoder.Compress(buffer);

            var fileName = Path.Combine(ResourceConstants.TempDirectory, System.Guid.NewGuid().ToString());

            File.WriteAllBytes(fileName, buffer);

            return fileName;
        }
        public T LoadFromTempFile<T>(string fileName, bool compressed)
        {
            var buffer = File.ReadAllBytes(fileName);

            if (compressed)
                buffer = ZipEncoder.Decompress(buffer);

            return (T)BinarySerializer.Deserialize(buffer);
        }
        public void DeleteTempFile(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }
    }
}
