﻿
using Rogue.NET.Common.Collection;
using Rogue.NET.Core.Model.Scenario;

namespace Rogue.NET.Scenario.Processing.Service.Interface
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
        SimpleDictionary<string, bool> GetScenarioObjectiveUpdates(ScenarioContainer scenarioContainer);
    }
}
