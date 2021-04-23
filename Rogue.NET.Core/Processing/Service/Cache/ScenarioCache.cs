using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Processing.Service.Cache.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service.Cache
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioCache))]
    public class ScenarioCache : IScenarioCache
    {
        static readonly List<ScenarioInfo> _scenarioInfos;


        static ScenarioCache()
        {
            _scenarioInfos = new List<ScenarioInfo>();
        }

        public ScenarioCache()
        {

        }


        public IEnumerable<string> GetScenarioNames()
        {
            return _scenarioInfos.Select(x => x.Name);
        }

        public ScenarioContainer GetScenario(string name)
        {
            var scenarioFile = Path.Combine(ResourceConstants.SavedGameDirectory, name + "." + ResourceConstants.ScenarioExtension);

            foreach (var file in Directory.GetFiles(ResourceConstants.SavedGameDirectory)
                                          .Where(x => x.EndsWith("." + ResourceConstants.ScenarioExtension)))
            {
                if (file == scenarioFile)
                    return (ScenarioContainer)BinarySerializer.DeserializeFromFile(file);
            }

            throw new Exception("Scenario file not found:  " + scenarioFile);
        }
        public IEnumerable<ScenarioInfo> GetScenarioInfos()
        {
            return _scenarioInfos;
        }
        public void SaveScenario(ScenarioContainer scenario)
        {
            if (!_scenarioInfos.Any(x => x.Name == scenario.Player.RogueName))
                AddScenario(scenario);

            // Otherwise, evict the cache
            else
            {
                RemoveScenario(scenario.Player.RogueName);
                AddScenario(scenario);
            }

            BinarySerializer.SerializeToFile(ResourceConstants.SavedGameDirectory + "\\" + scenario.Player.RogueName + "." + ResourceConstants.ScenarioExtension, scenario);
        }

        public void DeleteScenario(string scenarioName)
        {
            RemoveScenario(scenarioName);

            var path = Path.Combine(ResourceConstants.SavedGameDirectory, scenarioName + "." + ResourceConstants.ScenarioExtension);

            if (File.Exists(path))
                File.Delete(path);
        }

        public static void Load()
        {
            _scenarioInfos.Clear();

            foreach (var file in Directory.GetFiles(ResourceConstants.SavedGameDirectory)
                                          .Where(x => x.EndsWith("." + ResourceConstants.ScenarioExtension)))
            {
                AddScenario((ScenarioContainer)BinarySerializer.DeserializeFromFile(file));
            }
        }

        private static void AddScenario(ScenarioContainer scenario)
        {
            _scenarioInfos.Add(new ScenarioInfo()
            {
                Name = scenario.Player.RogueName,
                SmileyExpression = scenario.Player.SmileyExpression,
                SmileyBodyColor = ColorOperations.Convert(scenario.Player.SmileyBodyColor),
                SmileyLineColor = ColorOperations.Convert(scenario.Player.SmileyLineColor)
            });
        }

        private void RemoveScenario(string scenarioName)
        {
            _scenarioInfos.Filter(x => x.Name == scenarioName);
        }
    }
}
