
using Rogue.NET.Core.Model.Scenario;
using System.Collections.Generic;

namespace Rogue.NET.Scenario.Service.Interface
{
    public interface IScenarioObjectiveService
    {
        /// <summary>
        /// Calculates whether or not the objective is acheived. This can change with player inventory.
        /// </summary>
        bool IsObjectiveAcheived(ScenarioContainer scenarioContainer);

        /// <summary>
        /// Returns a dictionary keyed by RogueName to signal when objectives are acheived.
        /// </summary>
        IDictionary<string, bool> GetScenarioObjectiveUpdates(ScenarioContainer scenarioContainer);
    }
}
