using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Converter.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Extension;
using Rogue.NET.Core.Processing.Model.Validation;
using Rogue.NET.Core.Processing.Model.Validation.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service
{
    [Export(typeof(IScenarioValidationService))]
    public class ScenarioValidationService : IScenarioValidationService
    {
        private LayoutHeightConverter _layoutHeightConverter;
        private LayoutWidthConverter _layoutWidthConverter;

        [ImportingConstructor]
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
                //new ScenarioValidationRule("Objective Generation Not Guaranteed", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                //{
                //    var objectives = configuration.ConsumableTemplates.Cast<DungeonObjectTemplate>()
                //                                    .Union(configuration.DoodadTemplates)
                //                                    .Union(configuration.EnemyTemplates)
                //                                    .Union(configuration.EquipmentTemplates)
                //                                    .Where(x => x.IsObjectiveItem);

                //    var enemyEquipment = configuration.EnemyTemplates.SelectMany(x => x.StartingEquipment.Where(z => z.TheTemplate.IsObjectiveItem && z.GenerationProbability >= 1).Select(z => z.TheTemplate)).Cast<DungeonObjectTemplate>();
                //    var enemyConsumables = configuration.EnemyTemplates.SelectMany(x => x.StartingConsumables.Where(z => z.TheTemplate.IsObjectiveItem && z.GenerationProbability >= 1).Select(z => z.TheTemplate)).Cast<DungeonObjectTemplate>();



                //    var objectivesNotGuaranteed = objectives.Where(z => z.GenerationRate < 1 && !enemyEquipment.Contains(z) && !enemyConsumables.Contains(z));

                //    // Must have one of each objective
                //    return objectivesNotGuaranteed.Select(x => new ScenarioValidationResult()
                //    {
                //        Passed = false,
                //        InnerMessage = x.Name + " not guaranteed to be generated"
                //    });
                //})),
                new ScenarioValidationRule("Number of levels must be > 0 and <= 500", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    // Must have one of each objective
                    return new List<ScenarioValidationResult>(){
                        new ScenarioValidationResult()
                        {
                            Passed = configuration.ScenarioDesign.LevelDesigns.Count > 0 && configuration.ScenarioDesign.LevelDesigns.Count <= 500,
                            InnerMessage = null
                        }
                    };
                })),
                //new ScenarioValidationRule("Layouts not set for portion of scenario", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                //{
                //    var layoutGaps = configuration.ScenarioDesign.LevelDesigns.Where(x => x.LevelBranches)
                //    // Must have one of each objective
                //    return layoutGaps.Select(x =>
                //        new ScenarioValidationResult()
                //        {
                //            Passed = false,
                //            InnerMessage = "Level " + x.ToString() + " has no layout set"
                //        });
                //})),
                new ScenarioValidationRule("Layout max size must be < 10,000 cells", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var layoutSizes = configuration.LayoutTemplates
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
                    var layouts = configuration.LayoutTemplates
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
                            Passed = false,
                            InnerMessage = "Layout Template " + layoutName
                        };
                    });
                })),
                new ScenarioValidationRule("Alteration Effects (and / or) Learned Skills have to be set for all configured assets", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var consumablesNotSet = configuration.ConsumableTemplates
                                                         .Where(x => (x.HasAlteration && x.ConsumableAlteration == null) ||
                                                                     (x.HasProjectileAlteration && x.ConsumableProjectileAlteration == null) ||
                                                                     (x.HasLearnedSkill && x.LearnedSkill == null));

                    var equipmentNotSet = configuration.EquipmentTemplates
                                                       .Where(x => (x.HasAttackAlteration && x.EquipmentAttackAlteration == null) ||
                                                                   (x.HasCurseAlteration && x.EquipmentCurseAlteration == null) ||
                                                                   (x.HasEquipAlteration && x.EquipmentEquipAlteration == null));

                    var doodadsNotSet = configuration.DoodadTemplates
                                                     .Where(x => (x.IsAutomatic && x.AutomaticAlteration == null) ||
                                                                 (x.IsInvoked && x.InvokedAlteration == null));

                    var enemiesNotSet = configuration.EnemyTemplates
                                                     .Where(x =>
                                                            x.BehaviorDetails.Behaviors.Any(behavior =>
                                                                (behavior.AttackType == CharacterAttackType.Alteration) &&
                                                                 behavior.Alteration == null))
                                                     .Actualize();

                    var skillsNotSet = configuration.SkillTemplates
                                                    .Where(x => x.Skills.Any(z => z.SkillAlteration == null ||
                                                                                  z.SkillAlteration.Effect == null))
                                                    .Actualize();

                    return consumablesNotSet
                                .Cast<DungeonObjectTemplate>()
                                .Union(equipmentNotSet)
                                .Union(doodadsNotSet)
                                .Union(enemiesNotSet)
                                .Union(skillsNotSet)
                                .Select(x =>
                                {
                                    return new ScenarioValidationResult()
                                    {
                                        Passed = false,
                                        InnerMessage = x.Name + " has an un-set alteration effect (or) learned skill property"
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
                    var passed = !string.IsNullOrEmpty(configuration.ScenarioDesign.Name);

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
                    var scenarioName = configuration.ScenarioDesign.Name;
                    var passed = TextUtility.ValidateFileName(scenarioName);

                    return new List<ScenarioValidationResult>() {
                        new ScenarioValidationResult()
                        {
                            Passed = passed,
                            InnerMessage = null
                        }
                    };
                })),
                new ScenarioValidationRule("Alteration Effect Issues Found", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var alterations = configuration.GetAllAlterationsForProcessing();

                    var results = alterations
                                    .Where(x => x.Effect == null)
                                    .Select(x => x.AssetName + " has an unset Effect")
                                    .Union(alterations
                                        .Where(x => x.Effect != null &&
                                                    !string.IsNullOrEmpty(ValidateAlterationEffect(x.Effect)))
                                        .Select(x => x.AssetName + " -> " + ValidateAlterationEffect(x.Effect)))
                                    .Actualize();

                    return results
                              .Select(x =>
                        new ScenarioValidationResult()
                        {
                            Passed = false,
                            InnerMessage = x
                        });
                })),
                new ScenarioValidationRule("Alterations must have unique names", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    return configuration.GetAllAlterationsForProcessing()
                                        .NonUnique(x => x.AlterationName)
                                        .Select(x =>
                        new ScenarioValidationResult()
                        {
                            Passed = false,
                            InnerMessage = x.AssetName + " -> (has non-unique name) -> " + x.AlterationName

                        }).Actualize();
                })),
                new ScenarioValidationRule("Enemies must have Parameters > 0", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    return configuration.EnemyTemplates
                                        .Where(x => x.Hp.High <= 0 || x.Hp.Low <= 0 ||
                                                    x.Mp.High <= 0 || x.Mp.Low <= 0 ||
                                                    x.Agility.High <= 0 || x.Agility.Low <= 0 ||
                                                    x.Intelligence.High <= 0 || x.Intelligence.Low <= 0 ||
                                                    x.LightRadius.High <= 0 || x.LightRadius.Low <= 0 ||
                                                    x.Speed.High <= 0 || x.Speed.Low <= 0 ||
                                                    x.Strength.High <= 0 || x.Strength.Low <= 0)
                                        .Select(x =>
                        new ScenarioValidationResult()
                        {
                            Passed = false,
                            InnerMessage = x.Name + " must have parameters > 0"

                        }).Actualize();
                })),
                new ScenarioValidationRule("Improper Animation Parameters: Projectile and Chain Animations must not have the Source as an Affected Character", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    return configuration.GetAllAlterationsForProcessing()
                                        // ...Where Animation
                                        .Where(x => x.HasAnimation)
                                        // ...Is Trying to Create a Projectile
                                        .Where(x => x.AnimationGroup.Animations.Any(z => z.BaseType == AnimationBaseType.Chain ||
                                                                                         z.BaseType == AnimationBaseType.ChainReverse ||
                                                                                         z.BaseType == AnimationBaseType.Projectile ||
                                                                                         z.BaseType == AnimationBaseType.ProjectileReverse))
                                        // ...That will fire at the source character
                                        .Where(x => x.AnimationGroup.TargetType == AlterationTargetType.AllInRange ||
                                                    x.AnimationGroup.TargetType == AlterationTargetType.Source)
                                        .Select(x =>
                        new ScenarioValidationResult()
                        {
                            Passed = false,
                            InnerMessage = x.AssetName + " Animation is improperly set. Try setting Target Type to 'Target'"

                        }).Actualize();
                })),
                new ScenarioValidationRule("Equipment Attack Alteration must be applied to either One or Two handed melee weapons ONLY", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    return configuration.EquipmentTemplates
                                        .Where(x => x.HasAttackAlteration)
                                        .Where(x => x.Type != EquipmentType.OneHandedMeleeWeapon &&
                                                    x.Type != EquipmentType.TwoHandedMeleeWeapon)
                                        .Select(x =>
                        new ScenarioValidationResult()
                        {
                            Passed = false,
                            InnerMessage = x.Name + " cannot support Equipment Attack Alteration"

                        }).Actualize();
                })),
                new ScenarioValidationRule("Character Class Requirement must have a Character Class Set", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
                {
                    var equipment = configuration.EquipmentTemplates
                                                 .Where(x => x.HasCharacterClassRequirement)
                                                 .Where(x => string.IsNullOrEmpty(x.CharacterClass))
                                                 .Select(x => "Equipment -> " + x.Name)
                                                 .Actualize();

                    var consumables = configuration.EquipmentTemplates
                                                 .Where(x => x.HasCharacterClassRequirement)
                                                 .Where(x => string.IsNullOrEmpty(x.CharacterClass))
                                                 .Select(x => "Consumable -> " + x.Name)
                                                 .Actualize();

                    var doodads = configuration.DoodadTemplates
                                                 .Where(x => x.HasCharacterClassRequirement)
                                                 .Where(x => string.IsNullOrEmpty(x.CharacterClass))
                                                 .Select(x => "Doodad -> " + x.Name)
                                                 .Actualize();

                    var skills = configuration.SkillTemplates
                                              .SelectMany(x => x.Skills)
                                                 .Where(x => x.HasCharacterClassRequirement)
                                                 .Where(x => string.IsNullOrEmpty(x.CharacterClass))
                                                 .Select(x => "Skill -> " + x.Name)
                                                 .Actualize();

                    return equipment.Union(consumables)
                                    .Union(doodads)
                                    .Union(skills)
                                    .Select(x => new ScenarioValidationResult()
                                    {
                                        InnerMessage = x,
                                        Passed = false
                                    })
                                    .Actualize();
                })) }; //,

            //// Warnings
            //new ScenarioValidationRule("Asset generation rate set to zero", ValidationMessageSeverity.Warning, new Func<ScenarioConfigurationContainer, IEnumerable<IScenarioValidationResult>>(configuration =>
            //    {
            //        var contentNotSet = configuration.ConsumableTemplates.Cast<DungeonObjectTemplate>()
            //                                                            .Union(configuration.DoodadTemplates)
            //                                                            .Union(configuration.EnemyTemplates)
            //                                                            .Union(configuration.EquipmentTemplates)
            //                                                            .Where(x => x.GenerationRate <= 0);

            //        return contentNotSet.Select(x => new ScenarioValidationResult()
            //        {
            //            Passed = false,
            //            InnerMessage = x.Name + " has generation rate of zero"
            //        });
            //    }))
            //};
        }

        private string ValidateConsumableAlterationTypes(ConsumableTemplate template)
        {
            // TODO:ALTERATION
            //if (template.HasSpell && template.SpellTemplate != null)
            //{
            //    switch (template.SpellTemplate.Type)
            //    {
            //        case AlterationType.PassiveSource:
            //        case AlterationType.PassiveAura:
            //            return "Consumables don't support Passive Alteration Types";
            //        case AlterationType.Steal:
            //        case AlterationType.RunAway:
            //            return "Steal / RunAway aren't supported for consumables";                    
            //        case AlterationType.AttackAttribute:
            //            {
            //                switch (template.SpellTemplate.AttackAttributeType)
            //                {
            //                    case AlterationAttackAttributeType.Passive:
            //                        return "Consumables don't support Passive Alteration Types";
            //                    default:
            //                        break;
            //                }
            //            }
            //            break;
            //        default:
            //            break;
            //    }
            //}

            //if (template.SubType == ConsumableSubType.Ammo && template.AmmoSpellTemplate != null)
            //{
            //    switch (template.AmmoSpellTemplate.Type)
            //    {
            //        case AlterationType.PassiveSource:
            //        case AlterationType.PassiveAura:
            //            return "Consumables don't support Passive Alteration Types";
            //        case AlterationType.PermanentSource:
            //        case AlterationType.TemporarySource:
            //        case AlterationType.TeleportSelf:
            //        case AlterationType.Remedy:
            //        case AlterationType.OtherMagicEffect:
            //            return "Ammo spells require Target type Alterations";
            //        case AlterationType.TemporaryAllInRange:
            //        case AlterationType.TemporaryAllInRangeExceptSource:
            //        case AlterationType.TeleportAllInRange:
            //        case AlterationType.TeleportAllInRangeExceptSource:
            //        case AlterationType.PermanentAllInRange:
            //        case AlterationType.PermanentAllInRangeExceptSource:
            //            return "Ammo spells don't support Effect Range Alterations";
            //        case AlterationType.Steal:
            //        case AlterationType.RunAway:
            //            return "Steal / RunAway aren't supported for consumables";
            //        case AlterationType.AttackAttribute:
            //            {
            //                switch (template.AmmoSpellTemplate.AttackAttributeType)
            //                {
            //                    case AlterationAttackAttributeType.Passive:
            //                        return "Consumables don't support Passive Alteration Types";
            //                    case AlterationAttackAttributeType.ImbueArmor:
            //                    case AlterationAttackAttributeType.ImbueWeapon:
            //                    case AlterationAttackAttributeType.TemporaryFriendlySource:
            //                    case AlterationAttackAttributeType.TemporaryMalignSource:
            //                        return "Ammo spells require Target type Alterations";
            //                    case AlterationAttackAttributeType.MeleeAllInRange:
            //                    case AlterationAttackAttributeType.MeleeAllInRangeExceptSource:
            //                    case AlterationAttackAttributeType.TemporaryMalignAllInRange:
            //                    case AlterationAttackAttributeType.TemporaryMalignAllInRangeExceptSource:
            //                        return "Ammo spells don't support Effect Range Alterations";
            //                    default:
            //                        break;
            //                }
            //            }
            //            break;
            //        default:
            //            break;
            //    }
            //}

            //if (template.IsProjectile && template.ProjectileSpellTemplate != null)
            //{
            //    switch (template.ProjectileSpellTemplate.Type)
            //    {
            //        case AlterationType.PassiveSource:
            //        case AlterationType.PassiveAura:
            //            return "Consumables don't support Passive Alteration Types";
            //        case AlterationType.PermanentSource:
            //        case AlterationType.TemporarySource:
            //        case AlterationType.TeleportSelf:
            //        case AlterationType.Remedy:
            //        case AlterationType.OtherMagicEffect:
            //            return "Projectile spells require Target type Alterations";
            //        case AlterationType.TemporaryAllInRange:
            //        case AlterationType.TemporaryAllInRangeExceptSource:
            //        case AlterationType.TeleportAllInRange:
            //        case AlterationType.TeleportAllInRangeExceptSource:
            //        case AlterationType.PermanentAllInRange:
            //        case AlterationType.PermanentAllInRangeExceptSource:
            //            return "Projectile spells don't support Effect Range Alterations";
            //        case AlterationType.Steal:
            //        case AlterationType.RunAway:
            //            return "Steal / RunAway aren't supported for consumables";
            //        case AlterationType.AttackAttribute:
            //            {
            //                switch (template.ProjectileSpellTemplate.AttackAttributeType)
            //                {
            //                    case AlterationAttackAttributeType.Passive:
            //                        return "Consumables don't support Passive Alteration Types";
            //                    case AlterationAttackAttributeType.ImbueArmor:
            //                    case AlterationAttackAttributeType.ImbueWeapon:
            //                    case AlterationAttackAttributeType.TemporaryFriendlySource:
            //                    case AlterationAttackAttributeType.TemporaryMalignSource:
            //                        return "Projectile spells require Target type Alterations";
            //                    case AlterationAttackAttributeType.MeleeAllInRange:
            //                    case AlterationAttackAttributeType.MeleeAllInRangeExceptSource:
            //                    case AlterationAttackAttributeType.TemporaryMalignAllInRange:
            //                    case AlterationAttackAttributeType.TemporaryMalignAllInRangeExceptSource:
            //                        return "Projectile spells don't support Effect Range Alterations";
            //                    default:
            //                        break;
            //                }
            //            }
            //            break;
            //        default:
            //            break;
            //    }
            //}

            return null;
        }

        private string ValidateEquipmentAlterationTypes(EquipmentTemplate template)
        {
            // TODO:ALTERATION
            //if (template.HasCurseSpell && template.CurseSpell != null)
            //{
            //    switch (template.CurseSpell.Type)
            //    {
            //        case AlterationType.PassiveSource:
            //        case AlterationType.PassiveAura:
            //            return null;
            //        case AlterationType.AttackAttribute:
            //            {
            //                switch (template.CurseSpell.AttackAttributeType)
            //                {
            //                    case AlterationAttackAttributeType.Passive:
            //                        return null;
            //                    default:
            //                        return "Equipment ONLY support Passive Alteration Types";
            //                }
            //            }
            //        default:
            //            return "Equipment ONLY support Passive Alteration Types";
            //    }
            //}

            //if (template.HasEquipSpell && template.EquipSpell != null)
            //{
            //    switch (template.EquipSpell.Type)
            //    {
            //        case AlterationType.PassiveSource:
            //        case AlterationType.PassiveAura:
            //            return null;
            //        case AlterationType.AttackAttribute:
            //            {
            //                switch (template.EquipSpell.AttackAttributeType)
            //                {
            //                    case AlterationAttackAttributeType.Passive:
            //                        return null;
            //                    default:
            //                        return "Equipment ONLY support Passive Alteration Types";
            //                }
            //            }
            //        default:
            //            return "Equipment ONLY support Passive Alteration Types";
            //    }
            //}

            return null;
        }

        private string ValidateDoodadAlterationTypes(DoodadTemplate template)
        {
            // TODO:ALTERATION
            //if (template.IsInvoked && template.InvokedMagicSpellTemplate != null)
            //{
            //    switch (template.InvokedMagicSpellTemplate.Type)
            //    {
            //        case AlterationType.TemporarySource:
            //        case AlterationType.PermanentSource:
            //        case AlterationType.TeleportSelf:
            //        case AlterationType.OtherMagicEffect:
            //        case AlterationType.Remedy:
            //        case AlterationType.PermanentAllInRange:
            //        case AlterationType.PermanentAllInRangeExceptSource:
            //        case AlterationType.TeleportAllInRange:
            //        case AlterationType.TeleportAllInRangeExceptSource:
            //        case AlterationType.TemporaryAllInRange:
            //        case AlterationType.TemporaryAllInRangeExceptSource:
            //            return null;
            //        case AlterationType.AttackAttribute:
            //            {
            //                switch (template.InvokedMagicSpellTemplate.AttackAttributeType)
            //                {
            //                    case AlterationAttackAttributeType.ImbueArmor:
            //                    case AlterationAttackAttributeType.ImbueWeapon:
            //                    case AlterationAttackAttributeType.TemporaryFriendlySource:
            //                    case AlterationAttackAttributeType.TemporaryMalignSource:
            //                    case AlterationAttackAttributeType.MeleeAllInRange:
            //                    case AlterationAttackAttributeType.MeleeAllInRangeExceptSource:
            //                    case AlterationAttackAttributeType.TemporaryMalignAllInRange:
            //                    case AlterationAttackAttributeType.TemporaryMalignAllInRangeExceptSource:
            //                        return null;
            //                    case AlterationAttackAttributeType.Passive:
            //                    default:
            //                        return "Target / Passive Alerations aren't supported for Scenario Objects";
            //                }
            //            }
            //        default:
            //            return template.InvokedMagicSpellTemplate.Type.ToString() + " Alterations aren't supported for Scenario Objects";
            //    }
            //}

            //if (template.IsAutomatic && template.AutomaticMagicSpellTemplate != null)
            //{
            //    switch (template.AutomaticMagicSpellTemplate.Type)
            //    {
            //        case AlterationType.TemporarySource:
            //        case AlterationType.PermanentSource:
            //        case AlterationType.TeleportSelf:
            //        case AlterationType.OtherMagicEffect:
            //        case AlterationType.Remedy:
            //        case AlterationType.PermanentAllInRange:
            //        case AlterationType.PermanentAllInRangeExceptSource:
            //        case AlterationType.TeleportAllInRange:
            //        case AlterationType.TeleportAllInRangeExceptSource:
            //        case AlterationType.TemporaryAllInRange:
            //        case AlterationType.TemporaryAllInRangeExceptSource:
            //            return null;
            //        case AlterationType.AttackAttribute:
            //            {
            //                switch (template.AutomaticMagicSpellTemplate.AttackAttributeType)
            //                {
            //                    case AlterationAttackAttributeType.ImbueArmor:
            //                    case AlterationAttackAttributeType.ImbueWeapon:
            //                    case AlterationAttackAttributeType.TemporaryFriendlySource:
            //                    case AlterationAttackAttributeType.TemporaryMalignSource:
            //                    case AlterationAttackAttributeType.MeleeAllInRange:
            //                    case AlterationAttackAttributeType.MeleeAllInRangeExceptSource:
            //                    case AlterationAttackAttributeType.TemporaryMalignAllInRange:
            //                    case AlterationAttackAttributeType.TemporaryMalignAllInRangeExceptSource:
            //                        return null;
            //                    case AlterationAttackAttributeType.Passive:
            //                    default:
            //                        return "Target / Passive Alerations aren't supported for Scenario Objects";
            //                }
            //            }
            //        default:
            //            return template.AutomaticMagicSpellTemplate.Type.ToString() + " Alterations aren't supported for Scenario Objects";
            //    }
            //}

            return null;
        }

        private IEnumerable<string> ValidateEnemyAlterationTypes(EnemyTemplate template)
        {
            var result = new List<string>();

            foreach (var behavior in template.BehaviorDetails.Behaviors)
            {
                if ((behavior.AttackType == CharacterAttackType.Alteration) &&
                     behavior.Alteration != null)
                {
                    if (behavior.Alteration.Effect is CreateEnemyAlterationEffectTemplate)
                    {
                        var effect = behavior.Alteration.Effect as CreateEnemyAlterationEffectTemplate;

                        if (effect.Enemy == null)
                            result.Add("Create Enemy not set for enemy behavior:  " + template.Name);
                    }

                    if (behavior.Alteration.Effect is OtherAlterationEffectTemplate)
                        result.Add("Enemy OtherAlterationEffect is deprecated and has to be removed");
                }
            }

            return result;
        }

        private string ValidateAlterationEffect(IAlterationEffectTemplate template)
        {
            if (template is AttackAttributeAuraAlterationEffectTemplate)
                return null;

            else if (template is AttackAttributeMeleeAlterationEffectTemplate)
                return null;

            else if (template is AttackAttributeTemporaryAlterationEffectTemplate)
            {
                var effect = template as AttackAttributeTemporaryAlterationEffectTemplate;

                if (effect.HasAlteredState &&
                    effect.AlteredState == null)
                    return effect.Name + " has no Altered State set";
            }

            else if (template is AttackAttributePassiveAlterationEffectTemplate)
                return null;

            else if (template is AuraAlterationEffectTemplate)
                return null;

            else if (template is ChangeLevelAlterationEffectTemplate)
            {
                var effect = template as ChangeLevelAlterationEffectTemplate;

                if (effect.LevelChange.High < 1)
                    return effect.Name + " has an invalid Level Change parameter";
            }

            else if (template is CreateEnemyAlterationEffectTemplate)
            {
                var effect = template as CreateEnemyAlterationEffectTemplate;

                if (effect.Enemy == null)
                    return effect.Name + " has no Enemy set";
            }

            else if (template is CreateFriendlyAlterationEffectTemplate)
            {
                var effect = template as CreateFriendlyAlterationEffectTemplate;

                if (effect.Friendly == null)
                    return effect.Name + " has no Friendly set";
            }

            else if (template is CreateTemporaryCharacterAlterationEffectTemplate)
            {
                var effect = template as CreateTemporaryCharacterAlterationEffectTemplate;

                if (effect.TemporaryCharacter == null)
                    return effect.Name + " has no Temporary Character set";
            }

            else if (template is DrainMeleeAlterationEffectTemplate)
                return null;

            else if (template is EquipmentEnhanceAlterationEffectTemplate)
            {
                var effect = template as EquipmentEnhanceAlterationEffectTemplate;

                switch (effect.Type)
                {
                    case AlterationModifyEquipmentType.ArmorClass:
                    case AlterationModifyEquipmentType.WeaponClass:
                        if (effect.ClassChange == 0)
                            return effect.Name + " has no class change parameter set";
                        break;
                    case AlterationModifyEquipmentType.ArmorImbue:
                    case AlterationModifyEquipmentType.WeaponImbue:
                        if (effect.AttackAttributes.All(x => !x.Attack.IsSet() &&
                                                             !x.Resistance.IsSet() &&
                                                             !x.Weakness.IsSet()))
                            return effect.Name + " has no imbue parameters set";
                        break;
                    case AlterationModifyEquipmentType.ArmorQuality:
                    case AlterationModifyEquipmentType.WeaponQuality:
                        if (effect.QualityChange == 0)
                            return effect.Name + " has no quality change parameter set";
                        break;
                    default:
                        throw new Exception("Unhandled Equipment Modify Type");
                }
            }

            else if (template is EquipmentDamageAlterationEffectTemplate)
            {
                var effect = template as EquipmentDamageAlterationEffectTemplate;

                switch (effect.Type)
                {
                    case AlterationModifyEquipmentType.ArmorClass:
                    case AlterationModifyEquipmentType.WeaponClass:
                        if (effect.ClassChange == 0)
                            return effect.Name + " has no class change parameter set";
                        break;
                    case AlterationModifyEquipmentType.ArmorImbue:
                    case AlterationModifyEquipmentType.WeaponImbue:
                        if (effect.AttackAttributes.All(x => !x.Attack.IsSet() &&
                                                             !x.Resistance.IsSet() &&
                                                             !x.Weakness.IsSet()))
                            return effect.Name + " has no imbue parameters set";
                        break;
                    case AlterationModifyEquipmentType.ArmorQuality:
                    case AlterationModifyEquipmentType.WeaponQuality:
                        if (effect.QualityChange == 0)
                            return effect.Name + " has no quality change parameter set";
                        break;
                    default:
                        throw new Exception("Unhandled Equipment Modify Type");
                }
            }

            else if (template is OtherAlterationEffectTemplate)
                return null;

            else if (template is PassiveAlterationEffectTemplate)
                return null;

            else if (template is PermanentAlterationEffectTemplate)
                return null;

            else if (template is RemedyAlterationEffectTemplate)
            {
                var effect = template as RemedyAlterationEffectTemplate;

                if (effect.RemediedState == null)
                    return effect.Name + " has no Remedied State set";
            }

            else if (template is RevealAlterationEffectTemplate)
                return null;

            else if (template is RunAwayAlterationEffectTemplate)
                return null;

            else if (template is StealAlterationEffectTemplate)
                return null;

            else if (template is TeleportManualAlterationEffectTemplate)
                return null;

            else if (template is TeleportRandomAlterationEffectTemplate)
                return null;

            else if (template is TemporaryAlterationEffectTemplate)
            {
                var effect = template as TemporaryAlterationEffectTemplate;

                if (effect.HasAlteredState &&
                    effect.AlteredState == null)
                    return effect.Name + " has no Altered State set";
            }

            else if (template is TransmuteAlterationEffectTemplate)
            {
                var effect = template as TransmuteAlterationEffectTemplate;

                if (effect.ProbabilityOfSuccess <= 0)
                    return effect.Name + " has no probability of success";

                if (effect.TransmuteItems.Any(x => x.IsConsumableProduct &&
                                                   x.ConsumableProduct == null))
                    return effect.Name + " has no product set (for one item)";

                if (effect.TransmuteItems.Any(x => x.IsEquipmentProduct &&
                                                   x.EquipmentProduct == null))
                    return effect.Name + " has no product set (for one item)";

                if (effect.TransmuteItems.Any(x => x.IsEquipmentProduct &&
                                                   x.IsConsumableProduct))
                    return effect.Name + " has both product types set (for one item)";

                if (effect.TransmuteItems.Any(x => x.Weighting <= 0))
                    return effect.Name + " has weighting <= 0 (for one item)";
            }

            else
                throw new Exception("Unhandled Alteration Effect Type");

            return null;
        }
    }
}
