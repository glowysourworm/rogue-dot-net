using Rogue.NET.Core.Model.ScenarioConfiguration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Service.Validation.Interface
{
    public interface IScenarioValidationRule
    {
        IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration);
    }
}
