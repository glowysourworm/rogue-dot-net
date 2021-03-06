﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
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
        // IScenarioCache
        IEnumerable<ScenarioInfo> GetScenarioInfos();
        ScenarioContainer GetScenario(ScenarioInfo scenarioInfo);
        void SaveScenario(ScenarioContainer scenario);
        void DeleteScenario(ScenarioInfo scenarioInfo);
        void LoadLevel(ScenarioContainer scenarioContainer, int levelNumber);
        void SaveLevel(ScenarioContainer scenario, Level level);
        RogueFileDatabaseEntry CreateScenarioEntry(string rogueName, string configurationName, int seed);

        // IScenarioConfigurationCache
        IEnumerable<string> EmbeddedConfigurations { get; }
        IEnumerable<string> UserConfigurations { get; }
        IEnumerable<ScenarioConfigurationInfo> GetScenarioConfigurationInfos();
        ScenarioImage GetRandomSmileyCharacter(bool eliminateChoice);
        ScenarioConfigurationContainer GetScenarioConfiguration(string configurationName);
        void SaveConfiguration(ScenarioConfigurationContainer configuration);
        void EmbedConfiguration(ScenarioConfigurationContainer configuration);

        // IScenarioImageSourceFactory
        DrawingImage GetImageSource(SymbolDetailsTemplate symbolDetails, double scale, double effectiveVision, params Light[] lighting);
        DrawingImage GetImageSource(ScenarioImage scenarioImage, double scale, double effectiveVision, params Light[] lighting);
        DrawingImage GetDesaturatedImageSource(SymbolDetailsTemplate symbolDetails, double scale, double effectiveVision, params Light[] lighting);
        DrawingImage GetDesaturatedImageSource(ScenarioImage scenarioImage, double scale, double effectiveVision, params Light[] lighting);
        FrameworkElement GetFrameworkElement(ScenarioImage scenarioImage, double scale, double effectiveVision, params Light[] lighting);

        // ISvgCache
        DrawingGroup GetDrawing(ScenarioCacheImage scenarioCacheImage);
        IEnumerable<string> GetResourceNames(SymbolType type);
        IEnumerable<string> GetCharacterCategories();
        IEnumerable<string> GetCharacterResourceNames(string category);

        // TEMP FILES
        public string SaveToTempFile<T>(T theObject, bool compress);
        public T LoadFromTempFile<T>(string fileName, bool compressed);
        public void DeleteTempFile(string fileName);
    }
}
