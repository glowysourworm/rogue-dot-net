using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Processing.Model.Validation.Interface;

namespace Rogue.NET.Core.Processing.Model.Validation
{
    public class ScenarioValidationResult : NotifyViewModel, IScenarioValidationResult
    {
        string _innerMessage;
        bool _passed;

        public string InnerMessage
        {
            get { return _innerMessage; }
            set { this.RaiseAndSetIfChanged(ref _innerMessage, value); }
        }
        public bool Passed
        {
            get { return _passed; }
            set { this.RaiseAndSetIfChanged(ref _passed, value); }
        }
    }
}
