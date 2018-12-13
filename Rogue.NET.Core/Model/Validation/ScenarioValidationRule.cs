using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.Validation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Validation
{
    public class ScenarioValidationRule : NotifyViewModel, IScenarioValidationRule
    {
        readonly Func<ScenarioConfigurationContainer, IScenarioValidationResult> _validationFunc;

        string _name;
        ValidationMessageSeverity _severity;

        public ScenarioValidationRule(string name, ValidationMessageSeverity severity, Func<ScenarioConfigurationContainer, IScenarioValidationResult> validationFunc)
        {
            _name = name;
            _severity = severity;
            _validationFunc = validationFunc;
        }

        public IScenarioValidationMessage Validate(ScenarioConfigurationContainer configuration)
        {
            var result = _validationFunc(configuration);

            return new ScenarioValidationMessage()
            {
                InnerMessage = result.InnerMessage,
                Message = _name + (result.Passed ? " - Passed" : " - Failed"),
                Passed = result.Passed,
                Severity = result.Passed ? ValidationMessageSeverity.Info : _severity
            };
        }
    }
}
