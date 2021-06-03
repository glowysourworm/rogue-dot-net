using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Processing.Service.Cache.Interface;
using Rogue.NET.Core.Processing.Service.FileDatabase.Interface;

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service.Cache
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioCache))]
    public class ScenarioCache : IScenarioCache
    {
        // ScenarioInfo objects loaded on startup
        static readonly List<ScenarioInfo> _scenarioInfos;

        static ScenarioCache()
        {
            _scenarioInfos = new List<ScenarioInfo>();
        }

        [ImportingConstructor]
        public ScenarioCache()
        {
        }

        public IEnumerable<ScenarioInfo> GetScenarioInfos()
        {
            return _scenarioInfos;
        }

        public RogueFileDatabaseEntry CreateScenarioEntry(string rogueName, string configurationName, int seed)
        {
            using (var rogueFileDatabase = new RogueFileDatabase())
            {
                return rogueFileDatabase.Add(CreateUniqueName(rogueName, configurationName, seed), 
                                             typeof(ScenarioContainer),
                                             ResourceConstants.GetPath(ResourceConstants.ResourcePaths.SavedGameDirectory));
            }
        }

        public ScenarioContainer Get(ScenarioInfo scenarioInfo)
        {
            using (var rogueFileDatabase = new RogueFileDatabase())
            {
                // Create a unique name for the scenario instance for the entry (directory name)
                var entry = rogueFileDatabase.Get(CreateUniqueName(scenarioInfo.RogueName, scenarioInfo.ScenarioName, scenarioInfo.Seed));

                return new ScenarioContainer(entry);
            }
        }
        public void Save(ScenarioContainer scenario)
        {
            RogueFileDatabaseEntry entry;

            using (var rogueFileDatabase = new RogueFileDatabase())
            {
                // Look for existing entry
                if (rogueFileDatabase.Contains(CreateUniqueName(scenario)))
                    entry = rogueFileDatabase.Get(CreateUniqueName(scenario.Player.RogueName, scenario.Configuration.ScenarioDesign.Name, scenario.Detail.Seed));

                else
                    entry = rogueFileDatabase.Add(CreateUniqueName(scenario), typeof(ScenarioContainer));
            }

            // Saves all properties in the ScenarioContainer as separate file records. For more granularity, would make 
            // a separate method to just save the Level object.
            scenario.SaveRecords(entry);

            var uniqueName = CreateUniqueName(scenario.Player.RogueName, scenario.Configuration.ScenarioDesign.Name, scenario.Detail.Seed);

            // Add ScenarioInfo if none yet exists
            if (!_scenarioInfos.Any(x => CreateUniqueName(x.RogueName, x.ScenarioName, x.Seed) == uniqueName))
                _scenarioInfos.Add(scenario.CreateInfo());

            // Otherwise, update the cache
            else
            {
                // TODO: Save some details about the statistics (steps taken, last played time, etc...)
                //var scenarioInfo = _scenarioInfos.Find(info => info.RogueName == scenario.Player.RogueName &&
                //                                               info.ScenarioName == scenario.Configuration.ScenarioDesign.Name &&
                //                                               info.Seed == scenario.Detail.Seed);
            }
        }

        public void Delete(ScenarioInfo scenarioInfo)
        {
            using (var rogueFileDatabase = new RogueFileDatabase())
            {
                // First, delete files and directory for the scenario container
                rogueFileDatabase.Delete(CreateUniqueName(scenarioInfo));

                // Remove scenario from list of infos
                _scenarioInfos.Remove(scenarioInfo);
            }
        }

        public static void Load()
        {
            _scenarioInfos.Clear();

            // RogueFileDatabase can load statically. Using local instance - initialize
            // the rogue file database.
            using (var rogueFileDatabase = new RogueFileDatabase())
            {
                // Search for records with type ScenarioContainer
                var scenarioEntries = rogueFileDatabase.Search(entry => entry.EntryType.Name == typeof(ScenarioContainer).Name);

                // Fetch just the entries for creating a ScenarioInfo
                scenarioEntries.ForEach(entry =>
                {
                    _scenarioInfos.Add(ScenarioContainer.CreateInfo(entry));
                });
            }
        }

        public void LoadLevel(ScenarioContainer scenarioContainer, int levelNumber)
        {
            using (var rogueFileDatabase = new RogueFileDatabase())
            {
                // Create a unique name for the scenario instance for the entry (directory name)
                var entry = rogueFileDatabase.Get(CreateUniqueName(scenarioContainer));

                scenarioContainer.LoadLevel(entry, levelNumber);
            }
        }

        public void SaveLevel(ScenarioContainer scenarioContainer, Level level)
        {
            using (var rogueFileDatabase = new RogueFileDatabase())
            {
                // Create a unique name for the scenario instance for the entry (directory name)
                var entry = rogueFileDatabase.Get(CreateUniqueName(scenarioContainer));

                scenarioContainer.SaveLevel(entry, level);
            }
        }

        private string CreateUniqueName(ScenarioContainer scenarioContainer)
        {
            return CreateUniqueName(scenarioContainer.Player.RogueName, scenarioContainer.Configuration.ScenarioDesign.Name, scenarioContainer.Detail.Seed);
        }

        private string CreateUniqueName(ScenarioInfo scenarioInfo)
        {
            return CreateUniqueName(scenarioInfo.RogueName, scenarioInfo.ScenarioName, scenarioInfo.Seed);
        }

        private string CreateUniqueName(string rogueName, string scenarioName, int seed)
        {
            return string.Join(" - ", rogueName, scenarioName, seed);
        }
    }
}
