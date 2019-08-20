using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Extension;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Extension
{
    /// <summary>
    /// Set of convenience methods for working with the scenario configuration collections
    /// </summary>
    public static class ScenarioConfigurationContainerExtension
    {
        public static IEnumerable<AlterationProcessingContainer> GetAllAlterationsForProcessing(this ScenarioConfigurationContainer configuration)
        {
            var alterations = new List<IEnumerable<AlterationProcessingContainer>>()
                    {
                        configuration.ConsumableTemplates
                                     .Where(x => x.HasAlteration)
                                     .Select(x => new AlterationProcessingContainer(x.Name, x.ConsumableAlteration))
                                     .Actualize(),

                        configuration.ConsumableTemplates
                                     .Where(x => x.HasProjectileAlteration)
                                     .Select(x => new AlterationProcessingContainer(x.Name, x.ConsumableProjectileAlteration))
                                     .Actualize(),

                        configuration.DoodadTemplates
                                     .Where(x => x.IsAutomatic)
                                     .Select(x => new AlterationProcessingContainer(x.Name, x.AutomaticAlteration))
                                     .Actualize(),

                        configuration.DoodadTemplates
                                     .Where(x => x.IsInvoked)
                                     .Select(x => new AlterationProcessingContainer(x.Name, x.InvokedAlteration))
                                     .Actualize(),

                        configuration.EquipmentTemplates
                                     .Where(x => x.HasAttackAlteration)
                                     .Select(x => new AlterationProcessingContainer(x.Name, x.EquipmentAttackAlteration))
                                     .Actualize(),

                        configuration.EquipmentTemplates
                                     .Where(x => x.HasCurseAlteration)
                                     .Select(x => new AlterationProcessingContainer(x.Name, x.EquipmentCurseAlteration))
                                     .Actualize(),

                        configuration.EquipmentTemplates
                                     .Where(x => x.HasEquipAlteration)
                                     .Select(x => new AlterationProcessingContainer(x.Name, x.EquipmentEquipAlteration))
                                     .Actualize(),

                        configuration.EnemyTemplates
                                     .SelectMany(x => x.BehaviorDetails.Behaviors.Select(z => new { Asset = x, Behavior = z}))
                                     .Where(x => x.Behavior.AttackType == Enums.CharacterAttackType.Skill ||
                                                 x.Behavior.AttackType == Enums.CharacterAttackType.SkillCloseRange)
                                     .Select(x => new AlterationProcessingContainer(x.Asset.Name, x.Behavior.EnemyAlteration))
                                     .Actualize(),

                        configuration.SkillTemplates
                                     .SelectMany(x => x.Skills.Select(z => new { Asset = x, Skill = z}))
                                     .Select(x => new AlterationProcessingContainer(x.Asset.Name, x.Skill.SkillAlteration))
                                     .Actualize()
                    };

            return alterations.SelectMany(x => x);
        }
    }
}
