using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service.Validation.ValidationRule.Objective
{
    public class ObjectiveGenerationValidationRule : IScenarioValidationRule
    {
        public ObjectiveGenerationValidationRule() { }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            var result = new List<ScenarioValidationResult>();

            foreach (var level in configuration.ScenarioDesign.LevelDesigns)
            {
                var consumables = level.LevelBranches
                                             .Select(x => new { Assets = x.LevelBranch.Consumables, Branch = x.LevelBranch })
                                             .Where(x => x.Assets.Any(z => z.Asset.IsObjectiveItem));

                var equipment = level.LevelBranches
                                             .Select(x => new { Assets = x.LevelBranch.Equipment, Branch = x.LevelBranch })
                                             .Where(x => x.Assets.Any(z => z.Asset.IsObjectiveItem));

                var doodads = level.LevelBranches
                                             .Select(x => new { Assets = x.LevelBranch.Doodads, Branch = x.LevelBranch })
                                             .Where(x => x.Assets.Any(z => z.Asset.IsObjectiveItem));

                var enemies = level.LevelBranches
                                         .Select(x => new { Assets = x.LevelBranch.Enemies, Branch = x.LevelBranch })
                                         .Where(x => x.Assets.Any(z => z.Asset.IsObjectiveItem) ||
                                                     x.Assets.Any(z => z.Asset.StartingConsumables.Any(a => a.TheTemplate.IsObjectiveItem)) ||
                                                     x.Assets.Any(z => z.Asset.StartingEquipment.Any(a => a.TheTemplate.IsObjectiveItem)));

                var friendlies = level.LevelBranches
                                             .Select(x => new { Assets = x.LevelBranch.Friendlies, Branch = x.LevelBranch })
                                             .Where(x => x.Assets.Any(z => z.Asset.IsObjectiveItem));

                result.AddRange(consumables.Where(x => x.Branch.ConsumableGenerationRange.GetAverage() < 1).Select(x => new ScenarioValidationResult()
                {
                    Message = "Scenario Objective Generation Not Guaranteed",
                    Severity = ValidationSeverity.Error,
                    Passed = false,
                    InnerMessage = "Asset Generation Not Guaranteed on Level: " + level.Name + " Branch:  " + x.Branch.Name
                }));

                result.AddRange(equipment.Where(x => x.Branch.EquipmentGenerationRange.GetAverage() < 1).Select(x => new ScenarioValidationResult()
                {
                    Message = "Scenario Objective Generation Not Guaranteed",
                    Severity = ValidationSeverity.Error,
                    Passed = false,
                    InnerMessage = "Asset Generation Not Guaranteed on Level: " + level.Name + " Branch:  " + x.Branch.Name
                }));

                result.AddRange(doodads.Where(x => x.Branch.DoodadGenerationRange.GetAverage() < 1).Select(x => new ScenarioValidationResult()
                {
                    Message = "Scenario Objective Generation Not Guaranteed",
                    Severity = ValidationSeverity.Error,
                    Passed = false,
                    InnerMessage = "Asset Generation Not Guaranteed on Level: " + level.Name + " Branch:  " + x.Branch.Name
                }));

                result.AddRange(enemies.Where(x => x.Branch.EnemyGenerationRange.GetAverage() < 1).Select(x => new ScenarioValidationResult()
                {
                    Message = "Scenario Objective Generation Not Guaranteed",
                    Severity = ValidationSeverity.Error,
                    Passed = false,
                    InnerMessage = "Asset Generation Not Guaranteed on Level: " + level.Name + " Branch:  " + x.Branch.Name
                }));

                result.AddRange(friendlies.Where(x => x.Branch.FriendlyGenerationRange.GetAverage() < 1).Select(x => new ScenarioValidationResult()
                {
                    Message = "Scenario Objective Generation Not Guaranteed",
                    Severity = ValidationSeverity.Error,
                    Passed = false,
                    InnerMessage = "Asset Generation Not Guaranteed on Level: " + level.Name + " Branch:  " + x.Branch.Name
                }));
            }
            return result;
        }
    }
}
