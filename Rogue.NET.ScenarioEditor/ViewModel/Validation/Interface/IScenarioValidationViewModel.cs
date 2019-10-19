using Rogue.NET.Core.Processing.Service.Validation.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.Validation.Interface
{
    public interface IScenarioValidationViewModel
    {
        bool ValidationPassed { get; }
        bool ValidationRequired { get; }
        int ValidationErrorCount { get; }
        int ValidationWarningCount { get; }
        int ValidationInfoCount { get; }
        ObservableCollection<IScenarioValidationResult> ValidationMessages { get; }
        void Set(IEnumerable<IScenarioValidationResult> validationMessages);
        void Clear();
    }
}
