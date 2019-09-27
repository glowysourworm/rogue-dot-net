using Rogue.NET.Common.Utility;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Model.ResourceCache.Interface;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Processing.Symbol.Interface;

namespace Rogue.NET.Core.Processing.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioResourceService))]
    public class ScenarioResourceService : IScenarioResourceService
    {
        readonly IScenarioFileService _scenarioFileService;
        readonly IScenarioImageSourceFactory _scenarioImageSourceFactory;

        // Scenario Configuration cache
        IDictionary<string, ScenarioConfigurationContainer> _scenarioConfigurations;
        
        // Basic color cache
        IEnumerable<ColorViewModel> _colors;

        [ImportingConstructor]
        public ScenarioResourceService(
                IScenarioFileService scenarioFileService,
                IScenarioImageSourceFactory scenarioImageSourceFactory,
                IScenarioConfigurationCache scenarioConfigurationCache)
        {
            _scenarioFileService = scenarioFileService;
            _scenarioImageSourceFactory = scenarioImageSourceFactory;
            _scenarioConfigurations = scenarioConfigurationCache.EmbeddedConfigurations
                                                                .ToDictionary(x => x.DungeonTemplate.Name, x => x);
            _colors = ColorFilter.CreateColors();
        }

        #region (public) Methods
        public void LoadCustomConfigurations()
        {
            var customScenarioConfigurationFiles = Directory.GetFiles(ResourceConstants.ScenarioDirectory, "*." + ResourceConstants.ScenarioConfigurationExtension, SearchOption.TopDirectoryOnly);

            // Load Custom Scenario Configurations
            foreach (var scenarioConfigurationName in customScenarioConfigurationFiles.Select(x => Path.GetFileNameWithoutExtension(x)))
            {
                var configuration = _scenarioFileService.OpenConfiguration(scenarioConfigurationName);
                if (configuration != null)
                    _scenarioConfigurations.Add(scenarioConfigurationName, configuration);
            }
        }
        public IEnumerable<ScenarioConfigurationContainer> GetScenarioConfigurations()
        {
            return _scenarioConfigurations.Values;
        }
        public ScenarioConfigurationContainer GetScenarioConfiguration(ConfigResources configResource)
        {
            // Have to copy configuration because of the HasBeenGenerated flags in memory
            if (_scenarioConfigurations.ContainsKey(configResource.ToString()))
                return _scenarioConfigurations[configResource.ToString()].DeepClone();

            else
                throw new Exception("Configuration not pre-loaded");
        }
        public ScenarioConfigurationContainer GetScenarioConfiguration(string configurationName)
        {
            if (!_scenarioConfigurations.ContainsKey(configurationName))
                throw new Exception("Configuration not found - " + configurationName);

            // Have to copy configuration because of the HasBeenGenerated flags in memory
            return _scenarioConfigurations[configurationName].DeepClone();
        }
        public IEnumerable<ColorViewModel> GetColors()
        {
            return _colors;
        }
        public ScenarioImage GetRandomSmileyCharacter()
        {
            var characterSymbols = _scenarioConfigurations.Values
                                                          .SelectMany(x => x.PlayerTemplates.Select(z => z.SymbolDetails));

            return characterSymbols.Any() ? new ScenarioImage(characterSymbols.PickRandom()) : new ScenarioImage()
            {
                SymbolType = SymbolType.Smiley,
                SmileyBodyColor = Colors.Yellow.ToString(),
                SmileyLineColor = Colors.Black.ToString(),
                SmileyExpression = SmileyExpression.Happy
            };
        }

        public ImageSource GetImageSource(SymbolDetailsTemplate symbolDetails, double scale)
        {
            return _scenarioImageSourceFactory.GetImageSource(symbolDetails, scale);
        }

        public ImageSource GetImageSource(ScenarioImage scenarioImage, double scale)
        {
            return _scenarioImageSourceFactory.GetImageSource(scenarioImage, scale);
        }

        public ImageSource GetDesaturatedImageSource(ScenarioImage scenarioImage, double scale)
        {
            return _scenarioImageSourceFactory.GetDesaturatedImageSource(scenarioImage, scale);
        }

        public FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage, double scale)
        {
            return _scenarioImageSourceFactory.GetFrameworkElement(scenarioImage, scale);
        }
        #endregion
    }
}
