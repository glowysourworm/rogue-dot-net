using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Model.Validation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Validation
{
    public class ScenarioValidationRule : NotifyViewModel, IScenarioValidationRule
    {
        readonly Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>> _validationFunc;

        string _name;
        ValidationMessageSeverity _severity;

        public ScenarioValidationRule(string name, ValidationMessageSeverity severity, Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>> validationFunc)
        {
            _name = name;
            _severity = severity;
            _validationFunc = validationFunc;
        }

        public IEnumerable<IScenarioValidationMessage> Validate(ScenarioConfigurationContainer configuration)
        {
            var result = _validationFunc(configuration);

            return result.Select(x => new ScenarioValidationMessage()
            {
                InnerMessage = x.InnerMessage,
                Message = _name + (x.Passed ? " - Passed" : " - Failed"),
                Passed = x.Passed,
                Severity = x.Passed ? ValidationMessageSeverity.Info : _severity
            });
        }
    }
}
