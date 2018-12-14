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
        public ScenarioValidationService()
        {

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
                                                     .Where(x => (x.BehaviorDetails.PrimaryBehavior.AttackType == CharacterAttackType.Skill &&
                                                                  x.BehaviorDetails.PrimaryBehavior.EnemySpell == null) ||
                                                                 (x.BehaviorDetails.SecondaryReason != SecondaryBehaviorInvokeReason.SecondaryNotInvoked) &&
                                                                  x.BehaviorDetails.SecondaryBehavior.AttackType == CharacterAttackType.Skill &&
                                                                  x.BehaviorDetails.SecondaryBehavior.EnemySpell == null);

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
                                                     .Select(x => new { Issue = ValidateEnemyAlterationTypes(x), AssetName = x.Name })
                                                     .Where(x => x.Issue != null);

                    return consumablesIssues
                            .Union(equipmentIssues)
                            .Union(doodadsIssues)
                            .Union(enemiesIssues)
                            .Select(x => new ScenarioValidationResult()
                            {
                                Passed = false,
                                InnerMessage = x.AssetName + " - " + x.Issue
                            });
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
                        return "Projectile spells require Target type Alterations";
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
                                    return "Projectile spells require Target type Alterations";
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
                        return null;
                    case AlterationType.AttackAttribute:
                        {
                            switch (template.InvokedMagicSpellTemplate.AttackAttributeType)
                            {
                                case AlterationAttackAttributeType.ImbueArmor:
                                case AlterationAttackAttributeType.ImbueWeapon:
                                case AlterationAttackAttributeType.TemporaryFriendlySource:
                                case AlterationAttackAttributeType.TemporaryMalignSource:
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
                        return null;
                    case AlterationType.AttackAttribute:
                        {
                            switch (template.AutomaticMagicSpellTemplate.AttackAttributeType)
                            {
                                case AlterationAttackAttributeType.ImbueArmor:
                                case AlterationAttackAttributeType.ImbueWeapon:
                                case AlterationAttackAttributeType.TemporaryFriendlySource:
                                case AlterationAttackAttributeType.TemporaryMalignSource:
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

        private string ValidateEnemyAlterationTypes(EnemyTemplate template)
        {
            if (template.BehaviorDetails.PrimaryBehavior.AttackType == CharacterAttackType.Skill &&
                template.BehaviorDetails.PrimaryBehavior.EnemySpell != null)
            {
                switch (template.BehaviorDetails.PrimaryBehavior.EnemySpell.Type)
                {
                    case AlterationType.AttackAttribute:
                        {
                            switch (template.BehaviorDetails.PrimaryBehavior.EnemySpell.AttackAttributeType)
                            {
                                case AlterationAttackAttributeType.ImbueArmor:
                                case AlterationAttackAttributeType.ImbueWeapon:
                                case AlterationAttackAttributeType.Passive:
                                    return template.BehaviorDetails.PrimaryBehavior.EnemySpell.AttackAttributeType.ToString() +
                                            " Attack Attribute Alteration type is not supported for enemies";
                            }
                            break;
                        }
                    case AlterationType.OtherMagicEffect:
                        {
                            switch (template.BehaviorDetails.PrimaryBehavior.EnemySpell.OtherEffectType)
                            {
                                case AlterationMagicEffectType.CreateMonster:
                                    return null;
                            }
                            break;
                        }
                    case AlterationType.Remedy:
                    case AlterationType.PassiveAura:
                        return template.BehaviorDetails.PrimaryBehavior.EnemySpell.AttackAttributeType.ToString() +
                                " Alteration type is not supported for enemies";
                }
            }

            if (template.BehaviorDetails.SecondaryReason != SecondaryBehaviorInvokeReason.SecondaryNotInvoked &&
                template.BehaviorDetails.SecondaryBehavior.AttackType == CharacterAttackType.Skill &&
                template.BehaviorDetails.SecondaryBehavior.EnemySpell != null)
            {
                switch (template.BehaviorDetails.SecondaryBehavior.EnemySpell.Type)
                {
                    case AlterationType.AttackAttribute:
                        {
                            switch (template.BehaviorDetails.SecondaryBehavior.EnemySpell.AttackAttributeType)
                            {
                                case AlterationAttackAttributeType.ImbueArmor:
                                case AlterationAttackAttributeType.ImbueWeapon:
                                case AlterationAttackAttributeType.Passive:
                                    return template.BehaviorDetails.SecondaryBehavior.EnemySpell.AttackAttributeType.ToString() +
                                            " Attack Attribute Alteration type is not supported for enemies";
                            }
                            break;
                        }
                    case AlterationType.OtherMagicEffect:
                        {
                            switch (template.BehaviorDetails.PrimaryBehavior.EnemySpell.OtherEffectType)
                            {
                                case AlterationMagicEffectType.CreateMonster:
                                    return null;
                            }
                            break;
                        }
                    case AlterationType.Remedy:
                    case AlterationType.PassiveAura:
                        return template.BehaviorDetails.SecondaryBehavior.EnemySpell.AttackAttributeType.ToString() +
                                " Alteration type is not supported for enemies";
                }
            }

            return null;
        }
    }
}
