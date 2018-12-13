using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Model.Validation.Interface
{
    public interface IScenarioValidationMessage
    {
        ValidationMessageSeverity Severity { get; set; }
        string Message { get; set; }
        string InnerMessage { get; set; }
        bool Passed { get; set; }
    }
}
