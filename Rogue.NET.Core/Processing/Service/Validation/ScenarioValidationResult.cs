using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Processing.Service.Validation.Interface;

namespace Rogue.NET.Core.Processing.Service.Validation
{
    public class ScenarioValidationResult : NotifyViewModel, IScenarioValidationResult
    {
        ValidationSeverity _severity;
        Template _asset;
        string _message;
        string _innerMessage;
        bool _passed;

        public ValidationSeverity Severity
        {
            get { return _severity; }
            set { this.RaiseAndSetIfChanged(ref _severity, value); }
        }
        public Template Asset
        {
            get { return _asset; }
            set { this.RaiseAndSetIfChanged(ref _asset, value); }
        }
        public string Message
        {
            get { return _message; }
            set { this.RaiseAndSetIfChanged(ref _message, value); }
        }
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
