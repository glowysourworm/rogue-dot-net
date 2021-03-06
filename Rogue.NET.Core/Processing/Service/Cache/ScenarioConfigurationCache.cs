﻿using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Processing.Service.Cache.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Service.Cache
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioConfigurationCache))]
    public class ScenarioConfigurationCache : IScenarioConfigurationCache
    {
        readonly static List<ScenarioConfigurationContainer> _embeddedScenarioConfigurations;
        readonly static List<ScenarioConfigurationContainer> _userScenarioConfigurations;
        readonly static List<ScenarioConfigurationInfo> _embeddedScenarioConfigurationInfos;
        readonly static List<ScenarioConfigurationInfo> _userScenarioConfigurationInfos;

        List<SymbolDetailsTemplate> _eliminatedSmileyFaces;

        /// <summary>
        /// Statically - preloaded configuration resources 
        /// </summary>
        public IEnumerable<string> EmbeddedConfigurations
        {
            get { return _embeddedScenarioConfigurations.Select(x => x.ScenarioDesign.Name); }
        }

        /// <summary>
        /// Created by the user - stored on disk in scenarios folder 
        /// </summary>
        public IEnumerable<string> UserConfigurations
        {
            get { return _userScenarioConfigurations.Select(x => x.ScenarioDesign.Name); }
        }

        static ScenarioConfigurationCache()
        {
            _embeddedScenarioConfigurations = new List<ScenarioConfigurationContainer>();
            _userScenarioConfigurations = new List<ScenarioConfigurationContainer>();
            _embeddedScenarioConfigurationInfos = new List<ScenarioConfigurationInfo>();
            _userScenarioConfigurationInfos = new List<ScenarioConfigurationInfo>();
        }
        public ScenarioConfigurationCache()
        {
            _eliminatedSmileyFaces = new List<SymbolDetailsTemplate>();
        }
        public IEnumerable<ScenarioConfigurationInfo> GetScenarioConfigurationInfos()
        {
            return _embeddedScenarioConfigurationInfos.Union(_userScenarioConfigurationInfos);
        }
        public ScenarioConfigurationContainer GetConfiguration(string configurationName)
        {
            var configuration = _embeddedScenarioConfigurations.Union(_userScenarioConfigurations)
                                                               .FirstOrDefault(x => x.ScenarioDesign.Name == configurationName);

#if CONFIGURATION_MSFT
            return configuration;
#else
            // NOTE*** Creating clone of the configuration using binary copy
            return BinarySerializer.BinaryCopy(configuration, BinarySerializer.SerializationMode.RecursiveSerializer);
#endif
        }

        public void SaveConfiguration(ScenarioConfigurationContainer configuration)
        {
            // NOTE*** DESIGN PROBLEM WITH UNIQUE NAMES
            if (!_userScenarioConfigurations.Any(x => x.ScenarioDesign.Name == configuration.ScenarioDesign.Name))
                AddUserConfiguration(configuration);

            // Otherwise, have to replace what is in the cache
            else
            {
                RemoveUserConfiguration(configuration.ScenarioDesign.Name);
                AddUserConfiguration(configuration);
            }

            var file = Path.Combine(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.ScenarioDirectory), configuration.ScenarioDesign.Name) + "." +
                                    ResourceConstants.ScenarioConfigurationExtension;

            BinarySerializer.SerializeToFile(file, configuration, BinarySerializer.SerializationMode.RecursiveSerializer);
        }
        public void EmbedConfiguration(ScenarioConfigurationContainer configuration)
        {
            // NOTE*** DESIGN PROBLEM WITH UNIQUE NAMES
            if (!_embeddedScenarioConfigurations.Any(x => x.ScenarioDesign.Name == configuration.ScenarioDesign.Name))
                AddEmbeddedConfiguration(configuration);

            // Otherwise, have to replace what is in the cache
            else
            {
                RemoveEmbeddedConfiguration(configuration.ScenarioDesign.Name);
                AddEmbeddedConfiguration(configuration);
            }

            var file = Path.Combine(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.EmbeddedScenarioDirectory), configuration.ScenarioDesign.Name) + "." +
                                    ResourceConstants.ScenarioConfigurationExtension;

            BinarySerializer.SerializeToFile(file, configuration, BinarySerializer.SerializationMode.RecursiveSerializer);
        }
        public ScenarioImage GetRandomSmileyCharacter(bool eliminateChoice)
        {
            var smileySymbols = _embeddedScenarioConfigurations.Union(_userScenarioConfigurations)
                                                                .SelectMany(x => x.PlayerTemplates)
                                                                .Where(x => x.SymbolDetails.SymbolType == SymbolType.Smiley)
                                                                .Select(x => x.SymbolDetails);

            var randomSymbol = smileySymbols.Except(_eliminatedSmileyFaces).PickRandom();

            // Found non-used random symbol
            if (randomSymbol != null)
            {
                // Add to cache of eliminated symbols
                if (eliminateChoice)
                    _eliminatedSmileyFaces.Add(randomSymbol);

                return new ScenarioImage(randomSymbol);
            }

            return new ScenarioImage()
            {
                SymbolType = SymbolType.Smiley,
                SmileyBodyColor = Colors.Yellow.ToString(),
                SmileyLineColor = Colors.Black.ToString(),
                SmileyExpression = SmileyExpression.Happy
            };
        }
        public static void Load()
        {
            // Embedded Scenario Configurations
            _embeddedScenarioConfigurations.Clear();

            foreach (var configResource in Enum.GetValues(typeof(ConfigResources)).Cast<ConfigResources>())
            {
                var name = configResource.ToString();
                var assembly = Assembly.GetAssembly(typeof(ZipEncoder));

#if CONFIGURATION_MSFT

                var location = "Rogue.NET.Common.Resource.Configuration.MSFT." + name.ToString() + "." + ResourceConstants.ScenarioConfigurationExtension;
                using (var stream = assembly.GetManifestResourceStream(location))
                {
                    var memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);

                    AddEmbeddedConfiguration(BinarySerializer.Deserialize<ScenarioConfigurationContainer>(memoryStream.GetBuffer(), BinarySerializer.SerializationMode.MSFT));
                }
#else
                var location = "Rogue.NET.Common.Resource.Configuration." + name.ToString() + "." + ResourceConstants.ScenarioConfigurationExtension;
                using (var stream = assembly.GetManifestResourceStream(location))
                {
                    var memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);

                    AddEmbeddedConfiguration(BinarySerializer.Deserialize<ScenarioConfigurationContainer>(memoryStream.GetBuffer(), BinarySerializer.SerializationMode.RecursiveSerializer));
                }
#endif
            }

            // User Scenario Configurations (On Disk)
            _userScenarioConfigurations.Clear();

            foreach (var file in Directory.GetFiles(ResourceConstants.GetPath(ResourceConstants.ResourcePaths.ScenarioDirectory))
                                          .Where(x => x.EndsWith("." + ResourceConstants.ScenarioConfigurationExtension)))
            {
                AddUserConfiguration(BinarySerializer.DeserializeFromFile<ScenarioConfigurationContainer>(file, BinarySerializer.SerializationMode.RecursiveSerializer));
            }
        }

        private static void AddEmbeddedConfiguration(ScenarioConfigurationContainer configuration)
        {
            _embeddedScenarioConfigurations.Add(configuration);
            _embeddedScenarioConfigurationInfos.Add(new ScenarioConfigurationInfo()
            {
                Name = configuration.ScenarioDesign.Name,
                Description = configuration.ScenarioDesign.ObjectiveDescription,

                // NOTE*** RUNNING DEEP CLONE ON THE PLAYER TEMPLATES
                PlayerTemplates = configuration.PlayerTemplates.Select(x => x.DeepClone()),

                // A little rough copying these details - probably want to just pass the symbol information
                // and (for now) just validate that they're smiley faces. We're gonna remove this constraint
                // later; but have to handle auras / light radii / AND SYMBOL EFFECTS / etc.. for all symbol types
                //
                SmileyExpression = configuration.PlayerTemplates.First().SymbolDetails.SmileyExpression,
                SmileyBodyColor = Media.SymbolEffect.Utility.ColorOperations.Convert(configuration.PlayerTemplates.First().SymbolDetails.SmileyBodyColor),
                SmileyLineColor = Media.SymbolEffect.Utility.ColorOperations.Convert(configuration.PlayerTemplates.First().SymbolDetails.SmileyLineColor)
            });
        }

        private static void AddUserConfiguration(ScenarioConfigurationContainer configuration)
        {
            _userScenarioConfigurations.Add(configuration);
            _userScenarioConfigurationInfos.Add(new ScenarioConfigurationInfo()
            {
                Name = configuration.ScenarioDesign.Name,
                Description = configuration.ScenarioDesign.ObjectiveDescription,

                // NOTE*** RUNNING DEEP CLONE ON THE PLAYER TEMPLATES
                PlayerTemplates = configuration.PlayerTemplates.Select(x => x.DeepClone()),

                // A little rough copying these details - probably want to just pass the symbol information
                // and (for now) just validate that they're smiley faces. We're gonna remove this constraint
                // later; but have to handle auras / light radii / AND SYMBOL EFFECTS / etc.. for all symbol types
                //
                SmileyExpression = configuration.PlayerTemplates.First().SymbolDetails.SmileyExpression,
                SmileyBodyColor = Media.SymbolEffect.Utility.ColorOperations.Convert(configuration.PlayerTemplates.First().SymbolDetails.SmileyBodyColor),
                SmileyLineColor = Media.SymbolEffect.Utility.ColorOperations.Convert(configuration.PlayerTemplates.First().SymbolDetails.SmileyLineColor)
            });
        }

        private void RemoveEmbeddedConfiguration(string configurationName)
        {
            _embeddedScenarioConfigurations.Remove(x => x.ScenarioDesign.Name == configurationName);
            _embeddedScenarioConfigurationInfos.Remove(x => x.Name == configurationName);
        }

        private void RemoveUserConfiguration(string configurationName)
        {
            _userScenarioConfigurations.Remove(x => x.ScenarioDesign.Name == configurationName);
            _userScenarioConfigurationInfos.Remove(x => x.Name == configurationName);
        }
    }
}
