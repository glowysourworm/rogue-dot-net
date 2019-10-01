using Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System.Collections.Generic;

namespace Rogue.NET.ScenarioEditor.Service.Interface
{
    /// <summary>
    /// Service that calculates projections about the scenario. This does not take into account any Alteration data
    /// except for the required Food calculations. These will be based ONLY on consumable items of type Food - Permanent
    /// Alteration to Source.
    /// </summary>
    public interface IScenarioDifficultyCalculationService
    {

    }
}
