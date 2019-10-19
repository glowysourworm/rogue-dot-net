using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service.Validation.ValidationRule.Objective
{
    public class ObjectiveLevelsSingleBranchValidationRule : IScenarioValidationRule
    {
        public ObjectiveLevelsSingleBranchValidationRule() { }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            var result = new List<ScenarioValidationResult>();

            foreach (var level in configuration.ScenarioDesign.LevelDesigns)
            {
                var consumables = level.LevelBranches
                                             .SelectMany(x => x.LevelBranch.Consumables)
                                             .Where(x => x.Asset.IsObjectiveItem);

                var equipment = level.LevelBranches
                                           .SelectMany(x => x.LevelBranch.Equipment)
                                           .Where(x => x.Asset.IsObjectiveItem);

                var doodads = level.LevelBranches
                                         .SelectMany(x => x.LevelBranch.Doodads)
                                         .Where(x => x.Asset.IsObjectiveItem);

                var enemies = level.LevelBranches
                                         .SelectMany(x => x.LevelBranch.Enemies)
                                         .Where(x => x.Asset.IsObjectiveItem ||
                                                     x.Asset.StartingConsumables.Any(z => z.TheTemplate.IsObjectiveItem) ||
                                                     x.Asset.StartingEquipment.Any(z => z.TheTemplate.IsObjectiveItem));

                var friendlies = level.LevelBranches
                                            .SelectMany(x => x.LevelBranch.Friendlies)
                                            .Where(x => x.Asset.IsObjectiveItem);

                var hasObjective = consumables.Any() || equipment.Any() || doodads.Any() || enemies.Any() || friendlies.Any();

                if (level.LevelBranches.Count > 1 && hasObjective)
                    result.Add(new ScenarioValidationResult()
                    {
                        Asset = level,
                        Message = "Levels with Scenario Objective must only have a single branch",
                        Passed = false,
                        Severity = ValidationSeverity.Error,
                        InnerMessage = level.Name + " can only have one branch because of objective assets"
                    });
            }

            return result;
        }
    }
}
