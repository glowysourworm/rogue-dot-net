using Rogue.NET.Core.Model.ScenarioConfiguration;

namespace Rogue.NET.Core.Model.Validation.Interface
{
    public interface IScenarioValidationRule
    {
        IScenarioValidationMessage Validate(ScenarioConfigurationContainer configuration);
    }
}
