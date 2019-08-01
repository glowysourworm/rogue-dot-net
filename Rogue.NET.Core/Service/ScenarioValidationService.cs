using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Converter.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.Validation;
using Rogue.NET.Core.Model.Validation.Interface;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Core.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Service
{
    [Export(typeof(IScenarioValidationService))]
    public class ScenarioValidationService : IScenarioValidationService
    {
        private LayoutHeightConverter _layoutHeightConverter;
        private LayoutWidthConverter _layoutWidthConverter;

        public ScenarioValidationService()
        {
            _layoutHeightConverter = new LayoutHeightConverter();
            _layoutWidthConverter = new LayoutWidthConverter();
        }

        public IEnumerable<IScenarioValidationMessage> Validate(ScenarioConfigurationContainer configuration)
        {
            var validationRules = CreateValidationRules();

            return validationRules.SelectMany(x => x.Validate(configuration))
                                  .ToList();
        }

        public bool IsValid(ScenarioConfigurationContainer scenarioConfigurationContainer)
        {
            var validationRules = CreateValidationRules();

            return validationRules.SelectMany(x => x.Validate(scenarioConfigurationContainer))
                                  .Where(x => x.Severity == ValidationMessageSeverity.Error)
                                  .All(x => x.Passed);
        }

        private IEnumerable<IScenarioValidationRule> CreateValidationRules()
        {
            return new ScenarioValidationRule[] {
                // Errors
                new ScenarioValidationRule("No Scenario Objective", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var result = configuration.ConsumableTemplates.Cast<DungeonObjectTemplate>()
                                    .Union(configuration.DoodadTemplates)
                                    .Union(configuration.EnemyTemplates)
                                    .Union(configuration.EquipmentTemplates)
                                    .Any(x => x.IsObjectiveItem);

                    return new List<ScenarioValidationResult>(){ new ScenarioValidationResult(){Passed = result, InnerMessage = null } };
                })),
                new ScenarioValidationRule("Objective Generation Not Guaranteed", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var objectives = configuration.ConsumableTemplates.Cast<DungeonObjectTemplate>()
                                                    .Union(configuration.DoodadTemplates)
                                                    .Union(configuration.EnemyTemplates)
                                                    .Union(configuration.EquipmentTemplates)
                                                    .Where(x => x.IsObjectiveItem);

                    var enemyEquipment = configuration.EnemyTemplates.SelectMany(x => x.StartingEquipment.Where(z => z.TheTemplate.IsObjectiveItem && z.GenerationProbability >= 1).Select(z => z.TheTemplate)).Cast<DungeonObjectTemplate>();
                    var enemyConsumables = configuration.EnemyTemplates.SelectMany(x => x.StartingConsumables.Where(z => z.TheTemplate.IsObjectiveItem && z.GenerationProbability >= 1).Select(z => z.TheTemplate)).Cast<DungeonObjectTemplate>();



                    var objectivesNotGuaranteed = objectives.Where(z => z.GenerationRate < 1 && !enemyEquipment.Contains(z) && !enemyConsumables.Contains(z));

                    // Must have one of each objective
                    return objectivesNotGuaranteed.Select(x => new ScenarioValidationResult()
                    {
                        Passed = false,
                        InnerMessage = x.Name + " not guaranteed to be generated"
                    });
                })),
                new ScenarioValidationRule("Number of levels must be > 0 and <= 500", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    // Must have one of each objective
                    return new List<ScenarioValidationResult>(){
                        new ScenarioValidationResult()
                        {
                            Passed = configuration.DungeonTemplate.NumberOfLevels > 0 && configuration.DungeonTemplate.NumberOfLevels <= 500,
                            InnerMessage = null
                        }
                    };
                })),
                new ScenarioValidationRule("Layouts not set for portion of scenario", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var layoutGaps = Enumerable.Range(1, configuration.DungeonTemplate.NumberOfLevels)
                                               .Where(x => !configuration.DungeonTemplate.LayoutTemplates.Any(z => z.Level.Contains(x)));

                    // Must have one of each objective
                    return layoutGaps.Select(x =>
                        new ScenarioValidationResult()
                        {
                            Passed = false,
                            InnerMessage = "Level " + x.ToString() + " has no layout set"
                        });
                })),
                new ScenarioValidationRule("Layout max size must be < 10,000 cells", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var layoutSizes = configuration.DungeonTemplate
                                                   .LayoutTemplates
                                                   .Select(template =>
                                                   {
                                                       return new { Size = _layoutHeightConverter.Convert(template) * _layoutWidthConverter.Convert(template),
                                                                    TemplateName = template.Name };
                                                   })
                                                   .Where(result => result.Size >= 10000)
                                                   .Actualize();

                    return layoutSizes.Select(result =>
                        new ScenarioValidationResult()
                        {
                            Passed = false,
                            InnerMessage = "Layout Template " + result.TemplateName + " has a size of " + result.Size.ToString()
                        });
                })),
                new ScenarioValidationRule("Layout Connection Geometry Type of Rectilinear should only be paired with a Room Placement Type of RectangularGrid", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var layouts = configuration.DungeonTemplate
                                                .LayoutTemplates
                                                .Where(template =>
                                                {
                                                    return template.ConnectionGeometryType == LayoutConnectionGeometryType.Rectilinear &&
                                                           (template.RoomPlacementType != LayoutRoomPlacementType.RectangularGrid ||
                                                            template.Type == LayoutType.ConnectedCellularAutomata);
                                                })
                                                .Select(template => template.Name)
                                                .Actualize();

                    return layouts.Select(layoutName =>
                    {
                        return new ScenarioValidationResult()
                        {
                            Passed = false,
                            InnerMessage = "Layout Template " + layoutName
                        };
                    });
                })),
                new ScenarioValidationRule("Layout Corridor Connection Type must be Corridor, Teleporter, or TeleporterRandom, for any Minimum Spanning Tree type (Random Room Placement or Cellular Automata)", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var layouts = configuration.DungeonTemplate
                                                .LayoutTemplates
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
                            Passed = false,
                            InnerMessage = "Layout Template " + layoutName
                        };
                    });
                })),
                new ScenarioValidationRule("Alterations (and / or) Learned Skills have to be set for all configured assets", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var consumablesNotSet = configuration.ConsumableTemplates
                                                         .Where(x => (x.HasSpell && x.SpellTemplate == null) ||
                                                                     (x.IsProjectile && x.ProjectileSpellTemplate == null) ||
                                                                     (x.HasLearnedSkill && x.LearnedSkill == null) ||
                                                                     (x.SubType == ConsumableSubType.Ammo && x.AmmoSpellTemplate == null));

                    var equipmentNotSet = configuration.EquipmentTemplates
                                                       .Where(x => (x.HasCurseSpell && x.CurseSpell == null) ||
                                                                   (x.HasEquipSpell && x.EquipSpell == null));

                    var doodadsNotSet = configuration.DoodadTemplates
                                                     .Where(x => (x.IsAutomatic && x.AutomaticMagicSpellTemplate == null) ||
                                                                 (x.IsInvoked && x.InvokedMagicSpellTemplate == null));

                    var enemiesNotSet = configuration.EnemyTemplates
                                                     .Where(x =>
                                                            x.BehaviorDetails.Behaviors.Any(behavior =>
                                                                (behavior.AttackType == CharacterAttackType.Skill ||
                                                                 behavior.AttackType == CharacterAttackType.SkillCloseRange) &&
                                                                 behavior.EnemySpell == null))
                                                     .Actualize();

                    return consumablesNotSet
                                .Cast<DungeonObjectTemplate>()
                                .Union(equipmentNotSet)
                                .Union(doodadsNotSet)
                                .Union(enemiesNotSet)
                                .Select(x =>
                                {
                                    return new ScenarioValidationResult()
                                    {
                                        Passed = false,
                                        InnerMessage = x.Name + " has an un-set alteration (or) learned skill property"
                                    };
                                });
                })),
                new ScenarioValidationRule("Alteration type not supported for asset", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var consumablesIssues = configuration.ConsumableTemplates
                                                         .Select(x => new { Issue = ValidateConsumableAlterationTypes(x), AssetName = x.Name })
                                                         .Where(x => x.Issue != null);

                    var equipmentIssues = configuration.EquipmentTemplates
                                                       .Select(x => new { Issue = ValidateEquipmentAlterationTypes(x), AssetName = x.Name })
                                                       .Where(x => x.Issue != null);

                    var doodadsIssues = configuration.DoodadTemplates
                                                     .Select(x => new { Issue = ValidateDoodadAlterationTypes(x), AssetName = x.Name })
                                                     .Where(x => x.Issue != null);

                    var enemiesIssues = configuration.EnemyTemplates
                                                     .Select(x => new { Issues = ValidateEnemyAlterationTypes(x), AssetName = x.Name });

                    // Flatten out these issues per enemy
                    var enemyIssuesFlattened = new List<ScenarioValidationResult>();

                    foreach (var item in enemiesIssues)
                        foreach (var issue in item.Issues)
                            enemyIssuesFlattened.Add(new ScenarioValidationResult()
                            {
                                Passed = false,
                                InnerMessage = item.AssetName + " - " + issue
                            });

                    return consumablesIssues
                            .Union(equipmentIssues)
                            .Union(doodadsIssues)
                            .Select(x => new ScenarioValidationResult()
                            {
                                Passed = false,
                                InnerMessage = x.AssetName + " - " + x.Issue
                            })
                            .Union(enemyIssuesFlattened);
                })),
                new ScenarioValidationRule("Scenario Name Not Set", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var passed = !string.IsNullOrEmpty(configuration.DungeonTemplate.Name);

                    return new List<ScenarioValidationResult>(){
                        new ScenarioValidationResult()
                        {
                            Passed = passed,
                            InnerMessage = null
                        }
                    };
                })),
                new ScenarioValidationRule("Scenario Name can only contain letters, numbers, and spaces", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var scenarioName = configuration.DungeonTemplate.Name;
                    var passed = TextUtility.ValidateFileName(scenarioName);

                    return new List<ScenarioValidationResult>() {
                        new ScenarioValidationResult()
                        {
                            Passed = passed,
                            InnerMessage = null
                        }
                    };
                })),
                new ScenarioValidationRule("Remedy Alteration types must have remedied state set", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var remedyAlterations = configuration.MagicSpells
                                                         .Where(x => x.Type == AlterationType.Remedy &&
                                                                     x.Effect.RemediedState == null);

                    return remedyAlterations.Select(x =>
                        new ScenarioValidationResult()
                        {
                            Passed = false,
                            InnerMessage = x.Name + " has no remedied state set"
                        });
                })),
                new ScenarioValidationRule("Create Monster Alterations must have the monster set", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var createMonsterAlterations = configuration.MagicSpells
                                                                .Where(x => x.Type == AlterationType.OtherMagicEffect &&
                                                                            x.OtherEffectType == AlterationMagicEffectType.CreateMonster &&
                                                                            string.IsNullOrEmpty(x.CreateMonsterEnemy));

                    return createMonsterAlterations.Select(x => new ScenarioValidationResult()
                    {
                        Passed = false,
                        InnerMessage = x.Name + " has no monster set"
                    });
                })),

                // Warnings
                new ScenarioValidationRule("Asset generation rate set to zero", ValidationMessageSeverity.Warning, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var contentNotSet = configuration.ConsumableTemplates.Cast<DungeonObjectTemplate>()
                                                                        .Union(configuration.DoodadTemplates)
                                                                        .Union(configuration.EnemyTemplates)
                                                                        .Union(configuration.EquipmentTemplates)
                                                                        .Where(x => x.GenerationRate <= 0);

                    return contentNotSet.Select(x => new ScenarioValidationResult()
                    {
                        Passed = false,
                        InnerMessage = x.Name + " has generation rate of zero"
                    });
                })),
                new ScenarioValidationRule("Asset level range outside of scenario level range", ValidationMessageSeverity.Warning, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var contentInvalid = configuration.ConsumableTemplates.Cast<DungeonObjectTemplate>()
                                                                        .Union(configuration.DoodadTemplates)
                                                                        .Union(configuration.EnemyTemplates)
                                                                        .Union(configuration.EquipmentTemplates)
                                                                        .Where(x => x.Level.Low < 1 || x.Level.High > configuration.DungeonTemplate.NumberOfLevels);

                    return contentInvalid.Select(x => new ScenarioValidationResult()
                    {
                        Passed = false,
                        InnerMessage = x.Name + " has a level range of " + x.Level.ToString()
                    });
                }))
            };
        }

        private string ValidateConsumableAlterationTypes(ConsumableTemplate template)
        {
            if (template.HasSpell && template.SpellTemplate != null)
            {
                switch (template.SpellTemplate.Type)
                {
                    case AlterationType.PassiveSource:
                    case AlterationType.PassiveAura:
                        return "Consumables don't support Passive Alteration Types";
                    case AlterationType.Steal:
                    case AlterationType.RunAway:
                        return "Steal / RunAway aren't supported for consumables";                    
                    case AlterationType.AttackAttribute:
                        {
                            switch (template.SpellTemplate.AttackAttributeType)
                            {
                                case AlterationAttackAttributeType.Passive:
                                    return "Consumables don't support Passive Alteration Types";
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            if (template.SubType == ConsumableSubType.Ammo && template.AmmoSpellTemplate != null)
            {
                switch (template.AmmoSpellTemplate.Type)
                {
                    case AlterationType.PassiveSource:
                    case AlterationType.PassiveAura:
                        return "Consumables don't support Passive Alteration Types";
                    case AlterationType.PermanentSource:
                    case AlterationType.TemporarySource:
                    case AlterationType.TeleportSelf:
                    case AlterationType.Remedy:
                    case AlterationType.OtherMagicEffect:
                        return "Ammo spells require Target type Alterations";
                    case AlterationType.TemporaryAllInRange:
                    case AlterationType.TemporaryAllInRangeExceptSource:
                    case AlterationType.TeleportAllInRange:
                    case AlterationType.TeleportAllInRangeExceptSource:
                    case AlterationType.PermanentAllInRange:
                    case AlterationType.PermanentAllInRangeExceptSource:
                        return "Ammo spells don't support Effect Range Alterations";
                    case AlterationType.Steal:
                    case AlterationType.RunAway:
                        return "Steal / RunAway aren't supported for consumables";
                    case AlterationType.AttackAttribute:
                        {
                            switch (template.AmmoSpellTemplate.AttackAttributeType)
                            {
                                case AlterationAttackAttributeType.Passive:
                                    return "Consumables don't support Passive Alteration Types";
                                case AlterationAttackAttributeType.ImbueArmor:
                                case AlterationAttackAttributeType.ImbueWeapon:
                                case AlterationAttackAttributeType.TemporaryFriendlySource:
                                case AlterationAttackAttributeType.TemporaryMalignSource:
                                    return "Ammo spells require Target type Alterations";
                                case AlterationAttackAttributeType.MeleeAllInRange:
                                case AlterationAttackAttributeType.MeleeAllInRangeExceptSource:
                                case AlterationAttackAttributeType.TemporaryMalignAllInRange:
                                case AlterationAttackAttributeType.TemporaryMalignAllInRangeExceptSource:
                                    return "Ammo spells don't support Effect Range Alterations";
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            if (template.IsProjectile && template.ProjectileSpellTemplate != null)
            {
                switch (template.ProjectileSpellTemplate.Type)
                {
                    case AlterationType.PassiveSource:
                    case AlterationType.PassiveAura:
                        return "Consumables don't support Passive Alteration Types";
                    case AlterationType.PermanentSource:
                    case AlterationType.TemporarySource:
                    case AlterationType.TeleportSelf:
                    case AlterationType.Remedy:
                    case AlterationType.OtherMagicEffect:
                        return "Projectile spells require Target type Alterations";
                    case AlterationType.TemporaryAllInRange:
                    case AlterationType.TemporaryAllInRangeExceptSource:
                    case AlterationType.TeleportAllInRange:
                    case AlterationType.TeleportAllInRangeExceptSource:
                    case AlterationType.PermanentAllInRange:
                    case AlterationType.PermanentAllInRangeExceptSource:
                        return "Projectile spells don't support Effect Range Alterations";
                    case AlterationType.Steal:
                    case AlterationType.RunAway:
                        return "Steal / RunAway aren't supported for consumables";
                    case AlterationType.AttackAttribute:
                        {
                            switch (template.ProjectileSpellTemplate.AttackAttributeType)
                            {
                                case AlterationAttackAttributeType.Passive:
                                    return "Consumables don't support Passive Alteration Types";
                                case AlterationAttackAttributeType.ImbueArmor:
                                case AlterationAttackAttributeType.ImbueWeapon:
                                case AlterationAttackAttributeType.TemporaryFriendlySource:
                                case AlterationAttackAttributeType.TemporaryMalignSource:
                                    return "Projectile spells require Target type Alterations";
                                case AlterationAttackAttributeType.MeleeAllInRange:
                                case AlterationAttackAttributeType.MeleeAllInRangeExceptSource:
                                case AlterationAttackAttributeType.TemporaryMalignAllInRange:
                                case AlterationAttackAttributeType.TemporaryMalignAllInRangeExceptSource:
                                    return "Projectile spells don't support Effect Range Alterations";
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return null;
        }

        private string ValidateEquipmentAlterationTypes(EquipmentTemplate template)
        {
            if (template.HasCurseSpell && template.CurseSpell != null)
            {
                switch (template.CurseSpell.Type)
                {
                    case AlterationType.PassiveSource:
                    case AlterationType.PassiveAura:
                        return null;
                    case AlterationType.AttackAttribute:
                        {
                            switch (template.CurseSpell.AttackAttributeType)
                            {
                                case AlterationAttackAttributeType.Passive:
                                    return null;
                                default:
                                    return "Equipment ONLY support Passive Alteration Types";
                            }
                        }
                    default:
                        return "Equipment ONLY support Passive Alteration Types";
                }
            }

            if (template.HasEquipSpell && template.EquipSpell != null)
            {
                switch (template.EquipSpell.Type)
                {
                    case AlterationType.PassiveSource:
                    case AlterationType.PassiveAura:
                        return null;
                    case AlterationType.AttackAttribute:
                        {
                            switch (template.EquipSpell.AttackAttributeType)
                            {
                                case AlterationAttackAttributeType.Passive:
                                    return null;
                                default:
                                    return "Equipment ONLY support Passive Alteration Types";
                            }
                        }
                    default:
                        return "Equipment ONLY support Passive Alteration Types";
                }
            }

            return null;
        }

        private string ValidateDoodadAlterationTypes(DoodadTemplate template)
        {
            if (template.IsInvoked && template.InvokedMagicSpellTemplate != null)
            {
                switch (template.InvokedMagicSpellTemplate.Type)
                {
                    case AlterationType.TemporarySource:
                    case AlterationType.PermanentSource:
                    case AlterationType.TeleportSelf:
                    case AlterationType.OtherMagicEffect:
                    case AlterationType.Remedy:
                    case AlterationType.PermanentAllInRange:
                    case AlterationType.PermanentAllInRangeExceptSource:
                    case AlterationType.TeleportAllInRange:
                    case AlterationType.TeleportAllInRangeExceptSource:
                    case AlterationType.TemporaryAllInRange:
                    case AlterationType.TemporaryAllInRangeExceptSource:
                        return null;
                    case AlterationType.AttackAttribute:
                        {
                            switch (template.InvokedMagicSpellTemplate.AttackAttributeType)
                            {
                                case AlterationAttackAttributeType.ImbueArmor:
                                case AlterationAttackAttributeType.ImbueWeapon:
                                case AlterationAttackAttributeType.TemporaryFriendlySource:
                                case AlterationAttackAttributeType.TemporaryMalignSource:
                                case AlterationAttackAttributeType.MeleeAllInRange:
                                case AlterationAttackAttributeType.MeleeAllInRangeExceptSource:
                                case AlterationAttackAttributeType.TemporaryMalignAllInRange:
                                case AlterationAttackAttributeType.TemporaryMalignAllInRangeExceptSource:
                                    return null;
                                case AlterationAttackAttributeType.Passive:
                                default:
                                    return "Target / Passive Alerations aren't supported for Scenario Objects";
                            }
                        }
                    default:
                        return template.InvokedMagicSpellTemplate.Type.ToString() + " Alterations aren't supported for Scenario Objects";
                }
            }

            if (template.IsAutomatic && template.AutomaticMagicSpellTemplate != null)
            {
                switch (template.AutomaticMagicSpellTemplate.Type)
                {
                    case AlterationType.TemporarySource:
                    case AlterationType.PermanentSource:
                    case AlterationType.TeleportSelf:
                    case AlterationType.OtherMagicEffect:
                    case AlterationType.Remedy:
                    case AlterationType.PermanentAllInRange:
                    case AlterationType.PermanentAllInRangeExceptSource:
                    case AlterationType.TeleportAllInRange:
                    case AlterationType.TeleportAllInRangeExceptSource:
                    case AlterationType.TemporaryAllInRange:
                    case AlterationType.TemporaryAllInRangeExceptSource:
                        return null;
                    case AlterationType.AttackAttribute:
                        {
                            switch (template.AutomaticMagicSpellTemplate.AttackAttributeType)
                            {
                                case AlterationAttackAttributeType.ImbueArmor:
                                case AlterationAttackAttributeType.ImbueWeapon:
                                case AlterationAttackAttributeType.TemporaryFriendlySource:
                                case AlterationAttackAttributeType.TemporaryMalignSource:
                                case AlterationAttackAttributeType.MeleeAllInRange:
                                case AlterationAttackAttributeType.MeleeAllInRangeExceptSource:
                                case AlterationAttackAttributeType.TemporaryMalignAllInRange:
                                case AlterationAttackAttributeType.TemporaryMalignAllInRangeExceptSource:
                                    return null;
                                case AlterationAttackAttributeType.Passive:
                                default:
                                    return "Target / Passive Alerations aren't supported for Scenario Objects";
                            }
                        }
                    default:
                        return template.AutomaticMagicSpellTemplate.Type.ToString() + " Alterations aren't supported for Scenario Objects";
                }
            }

            return null;
        }

        private IEnumerable<string> ValidateEnemyAlterationTypes(EnemyTemplate template)
        {
            var result = new List<string>();

            foreach (var behavior in template.BehaviorDetails.Behaviors)
            {
                if ((behavior.AttackType == CharacterAttackType.Skill ||
                     behavior.AttackType == CharacterAttackType.SkillCloseRange) &&
                     behavior.EnemySpell != null)
                {
                    switch (behavior.EnemySpell.Type)
                    {
                        case AlterationType.AttackAttribute:
                            {
                                switch (behavior.EnemySpell.AttackAttributeType)
                                {
                                    case AlterationAttackAttributeType.ImbueArmor:
                                    case AlterationAttackAttributeType.ImbueWeapon:
                                    case AlterationAttackAttributeType.Passive:
                                        result.Add(behavior.EnemySpell.AttackAttributeType.ToString() +
                                                " Attack Attribute Alteration type is not supported for enemies");
                                        break;
                                }
                                break;
                            }
                        case AlterationType.OtherMagicEffect:
                            {
                                switch (behavior.EnemySpell.OtherEffectType)
                                {
                                    case AlterationMagicEffectType.CreateMonster:
                                        break;
                                }
                                break;
                            }
                        case AlterationType.Remedy:
                        case AlterationType.PassiveAura:
                            result.Add(behavior.EnemySpell.AttackAttributeType.ToString() +
                                        " Alteration type is not supported for enemies");
                            break;
                    }
                }
            }

            return result;
        }
    }
}
