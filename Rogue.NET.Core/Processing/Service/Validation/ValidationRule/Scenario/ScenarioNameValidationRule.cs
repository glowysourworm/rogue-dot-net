using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using Rogue.NET.Core.Utility;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Service.Validation.ValidationRule.Scenario
{
    public class ScenarioNameValidationRule : IScenarioValidationRule
    {
        public ScenarioNameValidationRule() { }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            if (string.IsNullOrEmpty(configuration.ScenarioDesign.Name))
            {
                return new ScenarioValidationResult[]
                {
                    new ScenarioValidationResult()
                    {
                        Message = "Scenario Name Not Set",
                        Severity = ValidationSeverity.Error,
                        Passed = false,
                        InnerMessage = null
                    }
                };
            }

            else if (!TextUtility.ValidateFileName(configuration.ScenarioDesign.Name))
            {
                return new ScenarioValidationResult[]
                {
                    new ScenarioValidationResult()
                    {
                        Message = "Scenario Name Does Not Contain Valid File Characters (0-9), (A-Z)",
                        Severity = ValidationSeverity.Error,
                        Passed = false,
                        InnerMessage = null
                    }
                };
            }

            else
                return new ScenarioValidationResult[]
                {
                    new ScenarioValidationResult()
                    {
                        Severity = ValidationSeverity.Info,
                        Passed = true,
                        Message = "Scenario Name Set:  " + configuration.ScenarioDesign.Name
                    }
                };
        }
    }
}
