using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;

namespace Rogue.NET.Core.Processing.Service.Validation.Interface
{
    public interface IScenarioValidationResult
    {
        ValidationSeverity Severity { get; set; }
        Template Asset { get; set; }
        string Message { get; set; }
        string InnerMessage { get; set; }
        bool Passed { get; set; }
    }
}
