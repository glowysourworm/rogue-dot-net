using Rogue.NET.Core.Processing.Model.Validation.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.Validation.Interface
{
    public interface IScenarioValidationViewModel
    {
        bool ValidationPassed { get; }
        bool ValidationRequired { get; }
        int ValidationErrorCount { get; }
        int ValidationWarningCount { get; }
        int ValidationInfoCount { get; }
        ObservableCollection<IScenarioValidationMessage> ValidationMessages { get; }
        void Set(IEnumerable<IScenarioValidationMessage> validationMessages);
        void Clear();
    }
}
