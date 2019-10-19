using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service.Validation.ValidationRule.Objective
{
    public class ObjectiveValidationRule : IScenarioValidationRule
    {
        public ObjectiveValidationRule() { }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            var assets = configuration.ConsumableTemplates.Cast<DungeonObjectTemplate>()
                            .Union(configuration.DoodadTemplates)
                            .Union(configuration.EnemyTemplates)
                            .Union(configuration.FriendlyTemplates)
                            .Union(configuration.EquipmentTemplates)
                            .Where(x => x.IsObjectiveItem);

            if (assets.Any())
            {
                return assets.Select(x => new ScenarioValidationResult()
                {
                    Asset = x,
                    Message = "Scenario Objective Set:  " + x.Name,
                    Passed = true,
                    Severity = ValidationSeverity.Info,
                    InnerMessage = null
                });
            }
            else
            {
                return new ScenarioValidationResult[]
                {
                    new ScenarioValidationResult()
                    {
                        Message = "No Scenario Objective Set",
                        Passed = false,
                        Severity = ValidationSeverity.Error
                    }
                };
            }
        }
    }
}
