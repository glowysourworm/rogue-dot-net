using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.Common.Extension;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common;
using System;

namespace Rogue.NET.ScenarioEditor.Service
{
    [Export(typeof(IScenarioAssetReferenceService))]
    public class ScenarioAssetReferenceService : IScenarioAssetReferenceService
    {
        /*
             For building these methods - start with the lowest level asset first (Brush) and
             move up the tree to see what's been affected

             TODO - Forward these affected assets to a notification service
         */

        #region (public) Methods
        public void UpdateAttackAttributes(ScenarioConfigurationContainerViewModel configuration)
        {
            // Alteration Effects - (Update en-mas)
            foreach (var effect in GetAllAlterationEffects(configuration))
                UpdateAttackAttributeAlterationEffect(effect, configuration.AttackAttributes);

            // TODO:CHARACTERCLASS
            // Equipment
            foreach (var equipment in configuration.EquipmentTemplates)
                UpdateAttackAttributeCollection(configuration.AttackAttributes, equipment.AttackAttributes);

            // Enemies
            foreach (var enemy in configuration.EnemyTemplates)
                UpdateAttackAttributeCollection(configuration.AttackAttributes, enemy.AttackAttributes);

            // Player Template

        }

        public void UpdateCharacterClasses(ScenarioConfigurationContainerViewModel configuration)
        {
            // Consumables
            foreach (var consumable in configuration.ConsumableTemplates.Where(x => x.HasCharacterClassRequirement))
            {
                //consumable.CharacterClass = MatchByName(configuration.CharacterClasses, consumable.CharacterClass);

                //if (consumable.CharacterClass == null)
                //    consumable.HasCharacterClassRequirement = false;
            }

            // Equipment
            foreach (var equipment in configuration.EquipmentTemplates.Where(x => x.HasCharacterClassRequirement))
            {
                // TODO:CHARACTERCLASS
                //equipment.CharacterClass = MatchByName(configuration.CharacterClasses, equipment.CharacterClass);

                //if (equipment.CharacterClass == null)
                //    equipment.HasCharacterClassRequirement = false;
            }

            // Doodads
            foreach (var doodad in configuration.DoodadTemplates.Where(x => x.HasCharacterClassRequirement))
            {
                // TODO:CHARACTERCLASS
                //doodad.CharacterClass = MatchByName(configuration.CharacterClasses, doodad.CharacterClass);

                //if (doodad.CharacterClass == null)
                //    doodad.HasCharacterClassRequirement = false;
            }

            // Skills
            foreach (var skillSet in configuration.SkillTemplates)
            {
                foreach (var skill in skillSet.Skills)
                {
                    // TODO:CHARACTERCLASS
                    //skill.CharacterClass = MatchByName(configuration.CharacterClasses, skill.CharacterClass);

                    //if (skill.CharacterClass == null)
                    //    skill.HasCharacterClassRequirement = false;
                }
            }
        }

        public void UpdateAlteredCharacterStates(ScenarioConfigurationContainerViewModel configuration)
        {
            // Alteration Effects - Update Altered Character States
            foreach (var alterationEffect in GetAllAlterationEffects(configuration))
            {
                if (alterationEffect is AttackAttributeTemporaryAlterationEffectTemplateViewModel)
                {
                    var effect = alterationEffect as AttackAttributeTemporaryAlterationEffectTemplateViewModel;

                    effect.AlteredState = MatchByName(configuration.AlteredCharacterStates, effect.AlteredState);

                    if (effect.AlteredState == null)
                        effect.HasAlteredState = false;
                }

                else if (alterationEffect is TemporaryAlterationEffectTemplateViewModel)
                {
                    var effect = alterationEffect as TemporaryAlterationEffectTemplateViewModel;

                    effect.AlteredState = MatchByName(configuration.AlteredCharacterStates, effect.AlteredState);

                    if (effect.AlteredState == null)
                        effect.HasAlteredState = false;
                }
            }
        }

        public void UpdateEnemies(ScenarioConfigurationContainerViewModel configuration)
        {
            // Alteration Effects - Update Create Monster 
            foreach (var alterationEffect in GetAllAlterationEffects(configuration))
            {
                if (alterationEffect is CreateMonsterAlterationEffectTemplateViewModel)
                {
                    var effect = alterationEffect as CreateMonsterAlterationEffectTemplateViewModel;

                    // Enemy no longer exists in the scenario
                    if (!configuration.EnemyTemplates.Any(x => x.Name == effect.CreateMonsterEnemy))
                    {
                        // TODO:ALTERATION Then, have to validate this in the validator
                        effect.CreateMonsterEnemy = null;
                    }
                }
            }
        }

        public void UpdateItems(ScenarioConfigurationContainerViewModel configuration)
        {
            // Enemies
            foreach (var enemy in configuration.EnemyTemplates)
            {
                UpdateStartingConsumablesCollection(configuration.ConsumableTemplates, enemy.StartingConsumables);
                UpdateStartingEquipmentCollection(configuration.EquipmentTemplates, enemy.StartingEquipment);
            }

            // TODO:CHARACTERCLASS
            // Player
            //UpdateStartingConsumablesCollection(configuration.ConsumableTemplates, configuration.PlayerTemplate.StartingConsumables);
            //UpdateStartingEquipmentCollection(configuration.EquipmentTemplates, configuration.PlayerTemplate.StartingEquipment);
        }

        public void UpdateSkillSets(ScenarioConfigurationContainerViewModel configuration)
        {
            // Consumables
            foreach (var consumable in configuration.ConsumableTemplates)
            {
                consumable.LearnedSkill = MatchByName(configuration.SkillTemplates, consumable.LearnedSkill);

                if (consumable.LearnedSkill == null)
                    consumable.HasLearnedSkill = false;
            }

            // TODO:CHARACTERCLASS
            // Player Starting Skills
            //UpdateCollection(configuration.SkillTemplates, configuration.PlayerTemplate.Skills);
        }

        #endregion      

        #region (private) Collection Methods

        private T MatchByName<T>(IList<T> source, T dest) where T : TemplateViewModel
        {
            return (dest == null) ? dest : source.FirstOrDefault(x => x.Name == dest.Name);
        }

        /// <summary>
        /// Removes items from the destination that don't exist in the source (BY NAME)
        /// </summary>
        private void UpdateCollection<T>(IList<T> source, IList<T> dest) where T : TemplateViewModel
        {
            // Remove
            for (int i = dest.Count - 1; i >= 0; i--)
            {
                if (!source.Any(a => a.Name == dest[i].Name))
                    dest.RemoveAt(i);
            }
        }
        private void UpdateStartingConsumablesCollection(IList<ConsumableTemplateViewModel> source, IList<ProbabilityConsumableTemplateViewModel> dest)
        {
            // Remove
            for (int i = dest.Count - 1; i >= 0; i--)
            {
                if (!source.Any(a => a.Name == dest[i].TheTemplate.Name))
                    dest.RemoveAt(i);
            }
        }
        private void UpdateStartingEquipmentCollection(IList<EquipmentTemplateViewModel> source, IList<ProbabilityEquipmentTemplateViewModel> dest)
        {
            // Remove
            for (int i = dest.Count - 1; i >= 0; i--)
            {
                if (!source.Any(a => a.Name == dest[i].TheTemplate.Name))
                    dest.RemoveAt(i);
            }
        }

        private void UpdateAttackAttributeCollection(IList<AttackAttributeTemplateViewModel> source, IList<AttackAttributeTemplateViewModel> dest)
        {
            // Create
            foreach (var attrib in source)
            {
                if (!dest.Any(a => a.Name == attrib.Name))
                    dest.Add(attrib.DeepClone());

                // Update
                else
                {
                    var existing = dest.First(a => a.Name == attrib.Name);
                    existing.SymbolDetails.CharacterColor = attrib.SymbolDetails.CharacterColor;
                    existing.SymbolDetails.CharacterSymbol = attrib.SymbolDetails.CharacterSymbol;
                    existing.SymbolDetails.Icon = attrib.SymbolDetails.Icon;
                    existing.SymbolDetails.SmileyAuraColor = attrib.SymbolDetails.SmileyAuraColor;
                    existing.SymbolDetails.SmileyBodyColor = attrib.SymbolDetails.SmileyBodyColor;
                    existing.SymbolDetails.SmileyLineColor = attrib.SymbolDetails.SmileyLineColor;
                    existing.SymbolDetails.SmileyMood = attrib.SymbolDetails.SmileyMood;
                    existing.SymbolDetails.Type = attrib.SymbolDetails.Type;
                }
            }

            // Delete
            for (int i = dest.Count - 1; i >= 0; i--)
            {
                if (!source.Any(a => a.Name == dest[i].Name))
                    dest.RemoveAt(i);
            }
        }

        /// <summary>
        /// Uses type casting to update the alteraiton effect's attack attributes should it be of the proper class
        /// </summary>
        private void UpdateAttackAttributeAlterationEffect(IAlterationEffectTemplateViewModel alterationEffect, IList<AttackAttributeTemplateViewModel> sourceAttackAttributes)
        {
            if (alterationEffect is AttackAttributeAuraAlterationEffectTemplateViewModel)
                UpdateAttackAttributeCollection(sourceAttackAttributes, (alterationEffect as AttackAttributeAuraAlterationEffectTemplateViewModel).AttackAttributes);

            else if (alterationEffect is AttackAttributeMeleeAlterationEffectTemplateViewModel)
                UpdateAttackAttributeCollection(sourceAttackAttributes, (alterationEffect as AttackAttributeMeleeAlterationEffectTemplateViewModel).AttackAttributes);

            else if (alterationEffect is AttackAttributePassiveAlterationEffectTemplateViewModel)
                UpdateAttackAttributeCollection(sourceAttackAttributes, (alterationEffect as AttackAttributePassiveAlterationEffectTemplateViewModel).AttackAttributes);

            else if (alterationEffect is AttackAttributeTemporaryAlterationEffectTemplateViewModel)
                UpdateAttackAttributeCollection(sourceAttackAttributes, (alterationEffect as AttackAttributeTemporaryAlterationEffectTemplateViewModel).AttackAttributes);
        }

        /// <summary>
        /// Returns combined list of all alteration effects
        /// </summary>
        private IEnumerable<IAlterationEffectTemplateViewModel> GetAllAlterationEffects(ScenarioConfigurationContainerViewModel configuration)
        {
            var consumableFunc = new Func<ConsumableTemplateViewModel, IEnumerable<IAlterationEffectTemplateViewModel>>(consumable =>
            {
                var list = new List<IEnumerable<IAlterationEffectTemplateViewModel>>()
                {
                    configuration.ConsumableTemplates.Select(x => x.ConsumableAlteration.Effect),
                    configuration.ConsumableTemplates.Select(x => x.ConsumableProjectileAlteration.Effect),
                    configuration.ConsumableTemplates.SelectMany(x => x.LearnedSkill.Skills.Select(z => z.SkillAlteration.Effect))
                };

                return list.SelectMany(x => x);
            });

            var equipmentFunc = new Func<EquipmentTemplateViewModel, IEnumerable<IAlterationEffectTemplateViewModel>>(consumable =>
            {
                var list = new List<IEnumerable<IAlterationEffectTemplateViewModel>>()
                {
                    configuration.EquipmentTemplates.Select(x => x.EquipmentAttackAlteration.Effect),
                    configuration.EquipmentTemplates.Select(x => x.EquipmentCurseAlteration.Effect),
                    configuration.EquipmentTemplates.Select(x => x.EquipmentEquipAlteration.Effect)
                };

                return list.SelectMany(x => x);
            });

            var alterations = new List<IEnumerable<IAlterationEffectTemplateViewModel>>()
            {
                configuration.ConsumableTemplates.SelectMany(x => consumableFunc(x)),
                configuration.EquipmentTemplates.SelectMany(x => equipmentFunc(x)),
                configuration.DoodadTemplates.Select(x => x.AutomaticAlteration.Effect),
                configuration.DoodadTemplates.Select(x => x.InvokedAlteration.Effect),
                configuration.EnemyTemplates.SelectMany(x => x.BehaviorDetails.Behaviors.Select(z => z.EnemyAlteration.Effect)),
                configuration.EnemyTemplates.SelectMany(x => x.StartingConsumables.SelectMany(z => consumableFunc(z.TheTemplate))),
                configuration.EnemyTemplates.SelectMany(x => x.StartingEquipment.SelectMany(z => equipmentFunc(z.TheTemplate))),
                configuration.SkillTemplates.SelectMany(x => x.Skills.Select(z => z.SkillAlteration.Effect)),
                // TODO:CHARACTERCLASS
                //configuration.PlayerTemplate.Skills.SelectMany(x => x.Skills.Select(z => z.SkillAlteration.Effect)),
                //configuration.PlayerTemplate.StartingConsumables.SelectMany(x => consumableFunc(x.TheTemplate)),
                //configuration.PlayerTemplate.StartingEquipment.SelectMany(x => equipmentFunc(x.TheTemplate)),
            };

            return alterations.SelectMany(x => x).Actualize();
        }

        #endregion
    }
}
