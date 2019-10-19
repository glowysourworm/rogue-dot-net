using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Service.Validation.ValidationRule.Layout
{
    public class LayoutGapGenerationValidationRule : IScenarioValidationRule
    {
        public LayoutGapGenerationValidationRule() { }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            var layoutGaps = configuration.ScenarioDesign
                                          .LevelDesigns
                                          .Where(x => x.LevelBranches.Any(z => z.LevelBranch.Layouts.None()));

            // Must have one of each objective
            return layoutGaps.Select(x =>
                new ScenarioValidationResult()
                {
                    Asset = x,
                    Severity = ValidationSeverity.Error,
                    Message = "Layouts not set for portion of scenario",
                    Passed = false,
                    InnerMessage = "Level " + x.Name + " has no layout set for one or more branches"
                });
        }
    }
}
