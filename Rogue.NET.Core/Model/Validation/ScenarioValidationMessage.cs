using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Validation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Validation
{
    public class ScenarioValidationMessage : NotifyViewModel, IScenarioValidationMessage
    {
        ValidationMessageSeverity _severity;
        string _message;
        string _innerMessage;
        bool _passed;

        public ValidationMessageSeverity Severity
        {
            get { return _severity; }
            set { this.RaiseAndSetIfChanged(ref _severity, value); }
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
