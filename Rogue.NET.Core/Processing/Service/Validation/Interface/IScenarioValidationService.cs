using Rogue.NET.Core.Model.ScenarioConfiguration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Service.Validation.Interface
{
    /// <summary>
    /// Service to validate the ScenarioConfigurationContainer before playing
    /// </summary>
    public interface IScenarioValidationService
    {
        IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer scenarioConfigurationContainer);

        bool IsValid(ScenarioConfigurationContainer scenarioConfigurationContainer);
    }
}
