using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Processing.Service.Cache.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Service.Cache
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioCache))]
    public class ScenarioCache : IScenarioCache
    {
        static readonly List<ScenarioContainer> _scenarios;


        static ScenarioCache()
        {
            _scenarios = new List<ScenarioContainer>();
        }

        public ScenarioCache()
        {

        }

        public IEnumerable<ScenarioContainer> GetScenarios()
        {
            return _scenarios;
        }

        public void SaveScenario(ScenarioContainer scenario)
        {
            if (!_scenarios.Contains(scenario))
                _scenarios.Add(scenario);

            BinarySerializer.SerializeToFile(ResourceConstants.SavedGameDirectory + "\\" + scenario.Player.RogueName + "." + ResourceConstants.ScenarioExtension, scenario);
        }

        public void DeleteScenario(ScenarioContainer scenario)
        {
            if(_scenarios.Contains(scenario))
                _scenarios.Remove(scenario);

            var path = Path.Combine(ResourceConstants.SavedGameDirectory, 
                                    scenario.Player.RogueName + "." + ResourceConstants.ScenarioExtension);

            if (File.Exists(path))
                File.Delete(path);
        }

        public static void Load()
        {
            _scenarios.Clear();

            foreach (var file in Directory.GetFiles(ResourceConstants.SavedGameDirectory)
                                          .Where(x => x.EndsWith("." + ResourceConstants.ScenarioExtension)))
            {
                _scenarios.Add((ScenarioContainer)BinarySerializer.DeserializeFromFile(file));
            }
        }
    }
}
