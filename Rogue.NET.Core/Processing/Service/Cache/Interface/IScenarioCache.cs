using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Service.Cache.Interface
{
    public interface IScenarioCache
    {
        IEnumerable<ScenarioInfo> GetScenarioInfos();
        ScenarioContainer Get(ScenarioInfo scenarioInfo);
        void Save(ScenarioContainer scenario);
        void Delete(ScenarioInfo scenarioInfo);

        RogueFileDatabaseEntry CreateScenarioEntry(string rogueName, string configurationName, int seed);

        // Methods for managing level data

        /// <summary>
        /// Loads the level requested and modifies the ScenarioContainer to update it for the next level.
        /// </summary>
        void LoadLevel(ScenarioContainer scenarioContainer, int levelNumber);

        /// <summary>
        /// Saves the specified level to the IRogueFileDatabase entry for the specified ScenarioContainer
        /// </summary>
        void SaveLevel(ScenarioContainer scenario, Level level);
    }
}
