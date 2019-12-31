using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Processing.Service.Cache.Interface;

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
        static readonly List<ScenarioContainer> _scenarios;
        static readonly List<ScenarioInfo> _scenarioInfos;


        static ScenarioCache()
        {
            _scenarios = new List<ScenarioContainer>();
            _scenarioInfos = new List<ScenarioInfo>();
        }

        public ScenarioCache()
        {

        }


        public IEnumerable<string> GetScenarioNames()
        {
            return _scenarios.Select(x => x.Player.RogueName);
        }

        public ScenarioContainer GetScenario(string name)
        {
            var scenario = _scenarios.FirstOrDefault(x => x.Player.RogueName == name);

            // NOTE*** Creating clone of the scenario using binary copy
            if (scenario != null)
            {
                var buffer = BinarySerializer.Serialize(scenario);

                return (ScenarioContainer)BinarySerializer.Deserialize(buffer);
            }

            return null;
        }
        public IEnumerable<ScenarioInfo> GetScenarioInfos()
        {
            return _scenarioInfos;
        }
        public void SaveScenario(ScenarioContainer scenario)
        {
            if (!_scenarios.Any(x => x.Player.RogueName == scenario.Player.RogueName))
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
            _scenarios.Clear();

            foreach (var file in Directory.GetFiles(ResourceConstants.SavedGameDirectory)
                                          .Where(x => x.EndsWith("." + ResourceConstants.ScenarioExtension)))
            {
                AddScenario((ScenarioContainer)BinarySerializer.DeserializeFromFile(file));
            }
        }

        private static void AddScenario(ScenarioContainer scenario)
        {
            _scenarios.Add(scenario);
            _scenarioInfos.Add(new ScenarioInfo()
            {
                Name = scenario.Player.RogueName,
                SmileyExpression = scenario.Player.SmileyExpression,
                SmileyBodyColor = ColorFilter.Convert(scenario.Player.SmileyBodyColor),
                SmileyLineColor = ColorFilter.Convert(scenario.Player.SmileyLineColor)
            });
        }

        private void RemoveScenario(string scenarioName)
        {
            _scenarios.Filter(x => x.Player.RogueName == scenarioName);
            _scenarioInfos.Filter(x => x.Name == scenarioName);
        }
    }
}
