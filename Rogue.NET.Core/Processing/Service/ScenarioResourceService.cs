using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Utility;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Processing.Service.Cache;
using Rogue.NET.Core.Processing.Service.Cache.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
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

        // Basic color cache
        IEnumerable<ColorViewModel> _colors;

        public IEnumerable<string> EmbeddedConfigurations
        {
            get { return _scenarioConfigurationCache.EmbeddedConfigurations; }
        }

        public IEnumerable<string> UserConfigurations
        {
            get { return _scenarioConfigurationCache.UserConfigurations; }
        }

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
            _colors = ColorFilter.CreateColors();
        }

        public IEnumerable<ColorViewModel> GetColors()
        {
            return _colors;
        }

        public IEnumerable<string> GetScenarioNames()
        {
            return _scenarioCache.GetScenarioNames();
        }

        public IEnumerable<ScenarioInfo> GetScenarioInfos()
        {
            return _scenarioCache.GetScenarioInfos();
        }

        // NOTE*** THIS WILL RETURN A CLONE OF THE SCENARIO
        public ScenarioContainer GetScenario(string scenarioName)
        {
            return _scenarioCache.GetScenario(scenarioName);
        }

        public void SaveScenario(ScenarioContainer scenario)
        {
            _scenarioCache.SaveScenario(scenario);
        }

        public void DeleteScenario(string scenarioName)
        {
            _scenarioCache.DeleteScenario(scenarioName);
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

        public DrawingImage GetImageSource(SymbolDetailsTemplate symbolDetails, double scale, Color lighting)
        {
            return _scenarioImageSourceFactory.GetImageSource(symbolDetails, scale, lighting);
        }

        public DrawingImage GetImageSource(ScenarioImage scenarioImage, double scale, Color lighting)
        {
            return _scenarioImageSourceFactory.GetImageSource(scenarioImage, scale, lighting);
        }

        public DrawingImage GetDesaturatedImageSource(ScenarioImage scenarioImage, double scale, Color lighting)
        {
            return _scenarioImageSourceFactory.GetDesaturatedImageSource(scenarioImage, scale, lighting);
        }

        public FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage, double scale, Color lighting)
        {
            return _scenarioImageSourceFactory.GetFrameworkElement(scenarioImage, scale, lighting);
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
    }
}
