using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Validation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Validation
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
