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

            return validationRules.Select(x => x.Validate(configuration))
                                  .ToList();
        }

        private IEnumerable<IScenarioValidationRule> CreateValidationRules()
        {
            return new ScenarioValidationRule[] {
                // Errors
                new ScenarioValidationRule("No Scenario Objective", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IScenarioValidationResult>(configuration =>
                {
                    var result = !configuration.ConsumableTemplates.Cast<DungeonObjectTemplate>()
                                    .Union(configuration.DoodadTemplates)
                                    .Union(configuration.EnemyTemplates)
                                    .Union(configuration.EquipmentTemplates)
                                    .Any(x => x.IsObjectiveItem);

                    return new ScenarioValidationResult(){ Passed = result, InnerMessage = null };
                })),
                new ScenarioValidationRule("Objective Generation Not Guaranteed", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IScenarioValidationResult>(configuration =>
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
                    return new ScenarioValidationResult()
                    {
                        Passed = !objectivesNotGuaranteed.Any(),
                        InnerMessage = objectivesNotGuaranteed.Any() ? objectivesNotGuaranteed.First().Name + " not guaranteed to be generated" : null
                    };
                })),
                new ScenarioValidationRule("Number of levels must be > 0 and <= 500", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IScenarioValidationResult>(configuration =>
                {
                    // Must have one of each objective
                    return new ScenarioValidationResult()
                    {
                        Passed = configuration.DungeonTemplate.NumberOfLevels > 0 && configuration.DungeonTemplate.NumberOfLevels <= 500,
                        InnerMessage = null
                    };
                })),
                new ScenarioValidationRule("Layouts not set for portion of scenario", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IScenarioValidationResult>(configuration =>
                {
                    var layoutGaps = Enumerable.Range(1, configuration.DungeonTemplate.NumberOfLevels)
                                               .Where(x => !configuration.DungeonTemplate.LayoutTemplates.Any(z => z.Level.Contains(x)));

                    // Must have one of each objective
                    return new ScenarioValidationResult()
                    {
                        Passed = !layoutGaps.Any(),
                        InnerMessage = layoutGaps.Any() ? "Level " + layoutGaps.First().ToString() + " has no layout set" : null
                    };
                })),
                new ScenarioValidationRule("Alterations (and / or) Learned Skills have to be set for all configured assets", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IScenarioValidationResult>(configuration =>
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

                    var failed = consumablesNotSet.Any() || equipmentNotSet.Any() || doodadsNotSet.Any() || enemiesNotSet.Any();

                    var firstAsset = consumablesNotSet.Cast<DungeonObjectTemplate>()
                                        .Union(equipmentNotSet)
                                        .Union(doodadsNotSet)
                                        .Union(enemiesNotSet)
                                        .FirstOrDefault();

                    return new ScenarioValidationResult()
                    {
                        Passed = !failed,
                        InnerMessage = firstAsset != null ? firstAsset.Name + " has an un-set alteration (or) learned skill property" : null
                    };
                })),
                new ScenarioValidationRule("Alteration type not supported for asset", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IScenarioValidationResult>(configuration =>
                {
                    var consumablesIssues = configuration.ConsumableTemplates
                                                         .Select(x => new { Issue = ValidateConsumableAlterationTypes(x), AssetName = x.Name })
                                                         .Where(x => x.Issue != null);

                    var equipmentIssues = configuration.EquipmentTemplates
                                                       .Select(x => new { Issue = ValidateEquipmentAlterationTypes(x), AssetName = x.Name })
                                                       .Where(x => x != null);

                    var doodadsIssues = configuration.DoodadTemplates
                                                     .Select(x => new { Issue = ValidateDoodadAlterationTypes(x), AssetName = x.Name })
                                                     .Where(x => x != null);

                    var enemiesIssues = configuration.EnemyTemplates
                                                     .Select(x => new { Issue = ValidateEnemyAlterationTypes(x), AssetName = x.Name })
                                                     .Where(x => x != null);

                    var failed = consumablesIssues.Any() || equipmentIssues.Any() || doodadsIssues.Any() || enemiesIssues.Any();

                    var firstIssue = consumablesIssues
                                        .Union(equipmentIssues)
                                        .Union(doodadsIssues)
                                        .Union(enemiesIssues)
                                        .FirstOrDefault();

                    return new ScenarioValidationResult()
                    {
                        Passed = !failed,
                        InnerMessage = firstIssue != null ? firstIssue.AssetName + " - " + firstIssue.Issue : null
                    };
                })),
                new ScenarioValidationRule("Scenario Name Not Set", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IScenarioValidationResult>(configuration =>
                {
                    var passed = !string.IsNullOrEmpty(configuration.DungeonTemplate.Name);

                    return new ScenarioValidationResult()
                    {
                        Passed = passed,
                        InnerMessage = null
                    };
                })),
                new ScenarioValidationRule("Scenario Name can only contain letters, numbers, and spaces", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IScenarioValidationResult>(configuration =>
                {
                    var scenarioName = configuration.DungeonTemplate.Name;
                    var passed = TextUtility.ValidateFileName(scenarioName);

                    var embeddedName = scenarioName == ConfigResources.Fighter.ToString() ||
                                       scenarioName == ConfigResources.Paladin.ToString() ||
                                       scenarioName == ConfigResources.Sorcerer.ToString() ||
                                       scenarioName == ConfigResources.Witch.ToString();

                    return new ScenarioValidationResult()
                    {
                        Passed = passed,
                        InnerMessage = embeddedName ? "Can't be set to name of built-in scenario" : null
                    };
                })),
                new ScenarioValidationRule("Remedy Alteration types must have remedied state set", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IScenarioValidationResult>(configuration =>
                {
                    var remedyAlterations = configuration.MagicSpells
                                                         .Where(x => x.Type == AlterationType.Remedy &&
                                                                     x.Effect.RemediedState == null);

                    var passed = !remedyAlterations.Any();
                    return new ScenarioValidationResult()
                    {
                        Passed = passed,
                        InnerMessage = !passed ? remedyAlterations.First().Name + " has no remedied state set" : null
                    };
                })),
                new ScenarioValidationRule("Create Monster Alterations must have the monster set", ValidationMessageSeverity.Error, new Func<ScenarioConfigurationContainer, IScenarioValidationResult>(configuration =>
                {
                    var createMonsterAlterations = configuration.MagicSpells
                                                                .Where(x => x.Type == AlterationType.OtherMagicEffect &&
                                                                            x.OtherEffectType == AlterationMagicEffectType.CreateMonster &&
                                                                            string.IsNullOrEmpty(x.CreateMonsterEnemy));

                    var passed = !createMonsterAlterations.Any();
                    return new ScenarioValidationResult()
                    {
                        Passed = passed,
                        InnerMessage = !passed ? createMonsterAlterations.First().Name + " has no monster set" : null
                    };
                })),

                // Warnings
                new ScenarioValidationRule("Asset generation rate set to zero", ValidationMessageSeverity.Warning, new Func<ScenarioConfigurationContainer, IScenarioValidationResult>(configuration =>
                {
                    var contentNotSet = configuration.ConsumableTemplates.Cast<DungeonObjectTemplate>()
                                                                        .Union(configuration.DoodadTemplates)
                                                                        .Union(configuration.EnemyTemplates)
                                                                        .Union(configuration.EquipmentTemplates)
                                                                        .Where(x => x.GenerationRate <= 0);

                    var passed = !contentNotSet.Any();
                    return new ScenarioValidationResult()
                    {
                        Passed = passed,
                        InnerMessage = !passed ? contentNotSet.First().Name + " is the first of such assets" : null
                    };
                })),
                new ScenarioValidationRule("Asset level range outside of scenario level range", ValidationMessageSeverity.Warning, new Func<ScenarioConfigurationContainer, IScenarioValidationResult>(configuration =>
                {
                    var contentInvalid = configuration.ConsumableTemplates.Cast<DungeonObjectTemplate>()
                                                                        .Union(configuration.DoodadTemplates)
                                                                        .Union(configuration.EnemyTemplates)
                                                                        .Union(configuration.EquipmentTemplates)
                                                                        .Where(x => x.Level.Low < 1 || x.Level.High > configuration.DungeonTemplate.NumberOfLevels);

                    var passed = !contentInvalid.Any();
                    return new ScenarioValidationResult()
                    {
                        Passed = passed,
                        InnerMessage = !passed ? contentInvalid.First().Name + " is the first of such assets" : null
                    };
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
                    case AlterationType.PermanentSource:
                    case AlterationType.TemporarySource:
                    case AlterationType.TeleportSelf:
                    case AlterationType.Remedy:
                    case AlterationType.OtherMagicEffect:
                        return null;
                    case AlterationType.TemporaryTarget:
                    case AlterationType.TemporaryAllTargets:
                    case AlterationType.PermanentAllTargets:
                    case AlterationType.TeleportTarget:
                    case AlterationType.TeleportAllTargets:
                    case AlterationType.PermanentTarget:
                        return "Target Alterations are only supported for projectile spells";
                    case AlterationType.Steal:
                    case AlterationType.RunAway:
                        return "Steal / RunAway aren't supported for consumables";                    
                    case AlterationType.AttackAttribute:
                        {
                            switch (template.SpellTemplate.AttackAttributeType)
                            {
                                case AlterationAttackAttributeType.Passive:
                                    return "Consumables don't support Passive Alteration Types";
                                case AlterationAttackAttributeType.ImbueArmor:
                                case AlterationAttackAttributeType.ImbueWeapon:
                                case AlterationAttackAttributeType.TemporaryFriendlySource:
                                case AlterationAttackAttributeType.TemporaryMalignSource:
                                    return null;
                                case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                                case AlterationAttackAttributeType.TemporaryMalignTarget:
                                case AlterationAttackAttributeType.MeleeTarget:
                                    return "Target Alterations are only supported for projectile spells";
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
                    case AlterationType.TemporaryTarget:
                    case AlterationType.TemporaryAllTargets:
                    case AlterationType.PermanentAllTargets:
                    case AlterationType.TeleportTarget:
                    case AlterationType.TeleportAllTargets:
                    case AlterationType.PermanentTarget:
                        return null;
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
                                case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                                case AlterationAttackAttributeType.TemporaryMalignTarget:
                                case AlterationAttackAttributeType.MeleeTarget:
                                    return null;
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
                    case AlterationType.TemporaryTarget:
                    case AlterationType.TemporaryAllTargets:
                    case AlterationType.PermanentAllTargets:
                    case AlterationType.TeleportTarget:
                    case AlterationType.TeleportAllTargets:
                    case AlterationType.PermanentTarget:
                        return null;
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
                                case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                                case AlterationAttackAttributeType.TemporaryMalignTarget:
                                case AlterationAttackAttributeType.MeleeTarget:
                                    return null;
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
                                default:
                                    return template.BehaviorDetails.PrimaryBehavior.EnemySpell.AttackAttributeType.ToString() +
                                            " Attack Attribute Alteration type is not supported for enemies";
                            }
                        }
                    case AlterationType.Remedy:
                    case AlterationType.PassiveAura:
                    case AlterationType.OtherMagicEffect:
                    default:
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
                                default:
                                    return template.BehaviorDetails.SecondaryBehavior.EnemySpell.AttackAttributeType.ToString() +
                                            " Attack Attribute Alteration type is not supported for enemies";
                            }
                        }
                    case AlterationType.Remedy:
                    case AlterationType.PassiveAura:
                    case AlterationType.OtherMagicEffect:
                    default:
                        return template.BehaviorDetails.SecondaryBehavior.EnemySpell.AttackAttributeType.ToString() +
                                " Alteration type is not supported for enemies";
                }
            }

            return null;
        }
    }
}
