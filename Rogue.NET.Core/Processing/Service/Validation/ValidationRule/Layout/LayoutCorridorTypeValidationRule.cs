using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service.Validation.ValidationRule.Layout
{
    public class LayoutCorridorTypeValidationRule : IScenarioValidationRule
    {
        public LayoutCorridorTypeValidationRule() { }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            var layouts = configuration.LayoutTemplates
                                        .Where(template =>
                                        {
                                            return template.ConnectionGeometryType == LayoutConnectionGeometryType.MinimumSpanningTree &&
                                                   template.ConnectionType == LayoutConnectionType.CorridorWithDoors;
                                        })
                                        .Select(template => template.Name)
                                        .Actualize();

            return layouts.Select(layoutName =>
            {
                return new ScenarioValidationResult()
                {
                    Message = "Layout Corridor Connection Type must be Corridor, Teleporter, or TeleporterRandom, for any Minimum Spanning Tree type (Random Room Placement or Cellular Automata)",
                    Passed = false,
                    Severity = ValidationSeverity.Error,
                    InnerMessage = "Layout Template " + layoutName
                };
            });
        }
    }
}
