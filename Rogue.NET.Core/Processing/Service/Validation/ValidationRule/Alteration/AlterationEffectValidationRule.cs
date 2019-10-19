using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Extension;
using Rogue.NET.Core.Processing.Service.Validation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Service.Validation.ValidationRule.Alteration
{
    public class AlterationEffectValidationRule : IScenarioValidationRule
    {
        public AlterationEffectValidationRule() { }

        public IEnumerable<IScenarioValidationResult> Validate(ScenarioConfigurationContainer configuration)
        {
            var alterations = configuration.GetAllAlterations();

            var unsetAlterations = alterations
                                    .Where(x => x.Alteration.Effect == null)
                                    .Select(x => new ScenarioValidationResult()
                                    {
                                        Asset = x.Asset,
                                        Passed = false,
                                        Message = "Alteration Has An Unset Effect",
                                        InnerMessage = x.Asset.Name + " has an unset Effect",
                                        Severity = ValidationSeverity.Error
                                    });

            var alterationIssues = alterations
                                    .Where(x => x.Alteration.Effect != null &&
                                                !string.IsNullOrEmpty(ValidateAlterationEffect(x.Alteration.Effect)))
                                    .Select(x => new ScenarioValidationResult()
                                    {
                                        Asset = x.Asset,
                                        Passed = false,
                                        Severity = ValidationSeverity.Error,
                                        Message = x.Asset.Name + " -> " + ValidateAlterationEffect(x.Alteration.Effect)
                                    });

            var nonUniqueAlterations = alterations.NonUnique(x => x.Alteration.Name)
                                                  .Select(x =>
                                                        new ScenarioValidationResult()
                                                        {
                                                            Asset = x.Asset,
                                                            Passed = false,
                                                            Severity = ValidationSeverity.Error,
                                                            Message = "Alterations must have unique names",
                                                            InnerMessage = x.Asset.Name + " -> (has non-unique name) -> " + x.Alteration.Name
                                                        }).Actualize();

            return unsetAlterations.Union(alterationIssues)
                                   .Union(nonUniqueAlterations);
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

            else if (template is BlockAlterationAlterationEffectTemplate)
            {
                return "TODO BLOCK ALTERATION";
            }

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

            else if (template is DetectAlterationAlterationEffectTemplate)
            {
                var effect = template as DetectAlterationAlterationEffectTemplate;

                if (effect.AlterationCategory == null)
                    return effect.Name + " has no Effect Category set";
            }

            else if (template is DetectAlterationAlignmentAlterationEffectTemplate)
                return null;

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

            else if (template is IdentifyAlterationEffectTemplate)
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

            else if (template is UncurseAlterationEffectTemplate)
                return null;

            else
                throw new Exception("Unhandled Alteration Effect Type");

            return null;
        }
    }
}
