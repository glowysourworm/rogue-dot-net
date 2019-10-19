using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Extension;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Extension
{
    /// <summary>
    /// Set of convenience methods for working with the scenario configuration collections
    /// </summary>
    public static class ScenarioConfigurationContainerExtension
    {
        public static IEnumerable<AlterationProcessingContainer> GetAllAlterations(this ScenarioConfigurationContainer configuration)
        {
            var alterations = new List<IEnumerable<AlterationProcessingContainer>>()
                    {
                        configuration.ConsumableTemplates
                                     .Where(x => x.HasAlteration)
                                     .Select(x => new AlterationProcessingContainer(x, x.ConsumableAlteration))
                                     .Actualize(),

                        configuration.ConsumableTemplates
                                     .Where(x => x.HasProjectileAlteration)
                                     .Select(x => new AlterationProcessingContainer(x, x.ConsumableProjectileAlteration))
                                     .Actualize(),

                        configuration.DoodadTemplates
                                     .Where(x => x.IsAutomatic)
                                     .Select(x => new AlterationProcessingContainer(x, x.AutomaticAlteration))
                                     .Actualize(),

                        configuration.DoodadTemplates
                                     .Where(x => x.IsInvoked)
                                     .Select(x => new AlterationProcessingContainer(x, x.InvokedAlteration))
                                     .Actualize(),

                        configuration.EquipmentTemplates
                                     .Where(x => x.HasAttackAlteration)
                                     .Select(x => new AlterationProcessingContainer(x, x.EquipmentAttackAlteration))
                                     .Actualize(),

                        configuration.EquipmentTemplates
                                     .Where(x => x.HasCurseAlteration)
                                     .Select(x => new AlterationProcessingContainer(x, x.EquipmentCurseAlteration))
                                     .Actualize(),

                        configuration.EquipmentTemplates
                                     .Where(x => x.HasEquipAlteration)
                                     .Select(x => new AlterationProcessingContainer(x, x.EquipmentEquipAlteration))
                                     .Actualize(),

                        configuration.EnemyTemplates
                                     .SelectMany(x => x.BehaviorDetails.Behaviors.Select(z => new { Asset = x, Behavior = z}))
                                     .Where(x => x.Behavior.AttackType == CharacterAttackType.Alteration)
                                     .Select(x => new AlterationProcessingContainer(x.Asset, x.Behavior.Alteration))
                                     .Actualize(),

                        configuration.FriendlyTemplates
                                     .SelectMany(x => x.BehaviorDetails.Behaviors.Select(z => new { Asset = x, Behavior = z}))
                                     .Where(x => x.Behavior.AttackType == CharacterAttackType.Alteration)
                                     .Select(x => new AlterationProcessingContainer(x.Asset, x.Behavior.Alteration))
                                     .Actualize(),

                        configuration.SkillTemplates
                                     .SelectMany(x => x.Skills.Select(z => new { Asset = x, Skill = z}))
                                     .Select(x => new AlterationProcessingContainer(x.Asset, x.Skill.SkillAlteration))
                                     .Actualize(),

                        // Create Temporary Character -> Alteration Effects
                        configuration.SkillTemplates
                                     .SelectMany(x => x.Skills.Select(z => new { Asset = x, Skill = z}))
                                     .Where(x => x.Skill != null)
                                     .Where(x => x.Skill.SkillAlteration.Effect is CreateTemporaryCharacterAlterationEffectTemplate)
                                     .Select(x => (x.Skill.SkillAlteration.Effect as CreateTemporaryCharacterAlterationEffectTemplate).TemporaryCharacter)
                                     .SelectMany(x => x.BehaviorDetails.Behaviors.Select(z => new {Asset = x, Behavior = z }))
                                     .Where(x => x.Behavior.AttackType == CharacterAttackType.Alteration)
                                     .Select(x => new AlterationProcessingContainer(x.Asset, x.Behavior.Alteration))
                                     .Actualize(),

                        configuration.EnemyTemplates
                                     .SelectMany(x => x.BehaviorDetails.Behaviors.Select(z => new { Asset = x, Behavior = z}))
                                     .Where(x => x.Behavior.AttackType == CharacterAttackType.Alteration)
                                     .Where(x => x.Behavior.Alteration != null)
                                     .Where(x => x.Behavior.Alteration.Effect is CreateTemporaryCharacterAlterationEffectTemplate)
                                     .Select(x => (x.Behavior.Alteration.Effect as CreateTemporaryCharacterAlterationEffectTemplate).TemporaryCharacter)
                                     .SelectMany(x => x.BehaviorDetails.Behaviors.Select(z => new {Asset = x, Behavior = z }))
                                     .Where(x => x.Behavior.AttackType == CharacterAttackType.Alteration)
                                     .Select(x => new AlterationProcessingContainer(x.Asset, x.Behavior.Alteration))
                                     .Actualize()
                    };

            return alterations.SelectMany(x => x);
        }
    }
}
