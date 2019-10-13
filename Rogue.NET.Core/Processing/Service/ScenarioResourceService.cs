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

        public IEnumerable<ScenarioConfigurationContainer> EmbeddedConfigurations
        {
            get { return _scenarioConfigurationCache.EmbeddedConfigurations; }
        }

        public IEnumerable<ScenarioConfigurationContainer> UserConfigurations
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

        public FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage, double scale)
        {
            return _scenarioImageSourceFactory.GetFrameworkElement(scenarioImage, scale);
        }

        public IEnumerable<ScenarioContainer> GetScenarios()
        {
            return _scenarioCache.GetScenarios();
        }

        public void SaveScenario(ScenarioContainer scenario)
        {
            _scenarioCache.SaveScenario(scenario);
        }

        public void DeleteScenario(ScenarioContainer scenario)
        {
            _scenarioCache.DeleteScenario(scenario);
        }

        public ScenarioImage GetRandomSmileyCharacter()
        {
            return _scenarioConfigurationCache.GetRandomSmileyCharacter();
        }

        public ScenarioConfigurationContainer GetScenarioConfiguration(string configurationName)
        {
            return _scenarioConfigurationCache.EmbeddedConfigurations
                                              .Union(_scenarioConfigurationCache.UserConfigurations)
                                              .First(x => x.ScenarioDesign.Name == configurationName);
        }

        public void SaveConfiguration(ScenarioConfigurationContainer configuration)
        {
            _scenarioConfigurationCache.SaveConfiguration(configuration);
        }

        public void EmbedConfiguration(ScenarioConfigurationContainer configuration)
        {
            _scenarioConfigurationCache.EmbedConfiguration(configuration);
        }

        public DrawingImage GetImageSource(SymbolDetailsTemplate symbolDetails, double scale)
        {
            return _scenarioImageSourceFactory.GetImageSource(symbolDetails, scale);
        }

        public DrawingImage GetImageSource(ScenarioImage scenarioImage, double scale)
        {
            return _scenarioImageSourceFactory.GetImageSource(scenarioImage, scale);
        }

        public DrawingImage GetDesaturatedImageSource(ScenarioImage scenarioImage, double scale)
        {
            return _scenarioImageSourceFactory.GetDesaturatedImageSource(scenarioImage, scale);
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
