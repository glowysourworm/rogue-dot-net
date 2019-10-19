using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Service.Validation.ValidationRule.Level
{
    public class LevelGenerationValidationRule : IScenarioValidationRule
    {
        public LevelGenerationValidationRule() { }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            if (configuration.ScenarioDesign.LevelDesigns.Count <= 0 ||
                configuration.ScenarioDesign.LevelDesigns.Count > 500)
            {
                return new List<ScenarioValidationResult>(){
                        new ScenarioValidationResult()
                        {
                            Message = "Number of levels must be between 0 and 500",
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
                        Message = "Scenario Generated with " + configuration.ScenarioDesign.LevelDesigns.Count.ToString() + " Levels",
                        Passed = true,
                        Severity = ValidationSeverity.Info
                    }
                };
        }
    }
}
