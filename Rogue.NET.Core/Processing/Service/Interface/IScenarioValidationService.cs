using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Model.Validation.Interface;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Service.Interface
{
    /// <summary>
    /// Service to validate the ScenarioConfigurationContainer before playing
    /// </summary>
    public interface IScenarioValidationService
    {
        IEnumerable<IScenarioValidationMessage> Validate(ScenarioConfigurationContainer scenarioConfigurationContainer);
        bool IsValid(ScenarioConfigurationContainer scenarioConfigurationContainer);
    }
}
