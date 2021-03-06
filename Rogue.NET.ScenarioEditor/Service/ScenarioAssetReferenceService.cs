﻿using Rogue.NET.Common.Extension;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.ScenarioEditor.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioAssetReferenceService))]
    public class ScenarioAssetReferenceService : IScenarioAssetReferenceService
    {
        readonly IScenarioCollectionProvider _scenarioCollectionProvider;

        [ImportingConstructor]
        public ScenarioAssetReferenceService(IScenarioCollectionProvider scenarioCollectionProvider)
        {
            _scenarioCollectionProvider = scenarioCollectionProvider;
        }

        /*
             For building these methods - start with the lowest level asset first (Brush) and
             move up the tree to see what's been affected

             TODO - Forward these affected assets to a notification service
         */

        #region (public) Methods
        public void UpdateAttackAttributes()
        {
            // Alteration Effects - (Update en-mas)
            foreach (var effect in GetAllAlterationEffects())
                UpdateAttackAttributeAlterationEffect(effect, _scenarioCollectionProvider.AttackAttributes);

            // Equipment
            foreach (var equipment in _scenarioCollectionProvider.Equipment)
                UpdateAttackAttributeCollection(_scenarioCollectionProvider.AttackAttributes, equipment.AttackAttributes);

            // Enemies
            foreach (var enemy in _scenarioCollectionProvider.Enemies)
                UpdateAttackAttributeCollection(_scenarioCollectionProvider.AttackAttributes, enemy.AttackAttributes);

            // Friendlies
            foreach (var friendly in _scenarioCollectionProvider.Friendlies)
                UpdateAttackAttributeCollection(_scenarioCollectionProvider.AttackAttributes, friendly.AttackAttributes);

            // Player Template
            foreach (var player in _scenarioCollectionProvider.PlayerClasses)
                UpdateAttackAttributeCollection(_scenarioCollectionProvider.AttackAttributes, player.AttackAttributes);
        }

        public void UpdatePlayerClasses()
        {
            // Nothing to do while player class is stored as a string
        }

        public void UpdateAlteredCharacterStates()
        {
            // Alteration Effects - Update Altered Character States
            foreach (var alterationEffect in GetAllAlterationEffects())
            {
                if (alterationEffect is AttackAttributeTemporaryAlterationEffectTemplateViewModel)
                {
                    var effect = alterationEffect as AttackAttributeTemporaryAlterationEffectTemplateViewModel;

                    effect.AlteredState = MatchByName(_scenarioCollectionProvider.AlteredCharacterStates, effect.AlteredState);

                    if (effect.AlteredState == null)
                        effect.HasAlteredState = false;
                }

                else if (alterationEffect is TemporaryAlterationEffectTemplateViewModel)
                {
                    var effect = alterationEffect as TemporaryAlterationEffectTemplateViewModel;

                    effect.AlteredState = MatchByName(_scenarioCollectionProvider.AlteredCharacterStates, effect.AlteredState);

                    if (effect.AlteredState == null)
                        effect.HasAlteredState = false;
                }
            }
        }

        public void UpdateNonPlayerCharacters()
        {
            // Update Level Branches
            foreach (var branch in _scenarioCollectionProvider.Levels
                                                              .SelectMany(x => x.LevelBranches)
                                                              .Select(x => x.LevelBranch))
            {
                // Enemies
                for (int i = branch.Enemies.Count - 1; i >= 0; i--)
                {
                    if (!_scenarioCollectionProvider.Enemies.Contains(branch.Enemies[i].Asset))
                        branch.Enemies.RemoveAt(i);
                }

                // Friendlies
                for (int i = branch.Friendlies.Count - 1; i >= 0; i--)
                {
                    if (!_scenarioCollectionProvider.Friendlies.Contains(branch.Friendlies[i].Asset))
                        branch.Friendlies.RemoveAt(i);
                }
            }
        }

        public void UpdateItems()
        {
            // Enemies
            foreach (var enemy in _scenarioCollectionProvider.Enemies)
            {
                UpdateStartingConsumablesCollection(_scenarioCollectionProvider.Consumables, enemy.StartingConsumables);
                UpdateStartingEquipmentCollection(_scenarioCollectionProvider.Equipment, enemy.StartingEquipment);
            }

            // Friendlies
            foreach (var friendly in _scenarioCollectionProvider.Friendlies)
            {
                UpdateStartingConsumablesCollection(_scenarioCollectionProvider.Consumables, friendly.StartingConsumables);
                UpdateStartingEquipmentCollection(_scenarioCollectionProvider.Equipment, friendly.StartingEquipment);
            }

            // Player
            foreach (var player in _scenarioCollectionProvider.PlayerClasses)
            {
                UpdateStartingConsumablesCollection(_scenarioCollectionProvider.Consumables, player.StartingConsumables);
                UpdateStartingEquipmentCollection(_scenarioCollectionProvider.Equipment, player.StartingEquipment);
            }

            // Update Level Branches (Asset references are shared - so starting items already set)
            foreach (var branch in _scenarioCollectionProvider.Levels
                                                              .SelectMany(x => x.LevelBranches)
                                                              .Select(x => x.LevelBranch))
            {
                // Consumables
                for (int i = branch.Consumables.Count - 1; i >= 0; i--)
                {
                    if (!_scenarioCollectionProvider.Consumables.Contains(branch.Consumables[i].Asset))
                        branch.Consumables.RemoveAt(i);
                }

                // Equipment
                for (int i = branch.Equipment.Count - 1; i >= 0; i--)
                {
                    if (!_scenarioCollectionProvider.Equipment.Contains(branch.Equipment[i].Asset))
                        branch.Equipment.RemoveAt(i);
                }
            }
        }

        public void UpdateSkillSets()
        {
            // Consumables
            foreach (var consumable in _scenarioCollectionProvider.Consumables)
            {
                consumable.LearnedSkill = MatchByName(_scenarioCollectionProvider.SkillSets, consumable.LearnedSkill);

                if (consumable.LearnedSkill == null)
                    consumable.HasLearnedSkill = false;
            }

            // Player Starting Skills
            foreach (var player in _scenarioCollectionProvider.PlayerClasses)
                UpdateCollection(_scenarioCollectionProvider.SkillSets, player.Skills);
        }

        public void UpdateLayouts()
        {
            // Update Level Branches (Asset references are shared)
            foreach (var branch in _scenarioCollectionProvider.Levels
                                                              .SelectMany(x => x.LevelBranches)
                                                              .Select(x => x.LevelBranch))
            {
                // Layouts
                for (int i = branch.Layouts.Count - 1; i >= 0; i--)
                {
                    if (!_scenarioCollectionProvider.Layouts.Contains(branch.Layouts[i].Asset))
                        branch.Layouts.RemoveAt(i);
                }
            }

            // Update terrain layers
            foreach (var layout in _scenarioCollectionProvider.Layouts)
            {
                for (int i = layout.TerrainLayers.Count - 1; i >= 0; i--)
                {
                    if (!_scenarioCollectionProvider.TerrainLayers.Contains(layout.TerrainLayers[i].TerrainLayer))
                        layout.TerrainLayers.RemoveAt(i);
                }
            }
        }
        public void UpdateDoodads()
        {
            // Update Level Branches (Asset references are shared)
            foreach (var branch in _scenarioCollectionProvider.Levels
                                                              .SelectMany(x => x.LevelBranches)
                                                              .Select(x => x.LevelBranch))
            {
                // Layouts
                for (int i = branch.Doodads.Count - 1; i >= 0; i--)
                {
                    if (!_scenarioCollectionProvider.Doodads.Contains(branch.Doodads[i].Asset))
                        branch.Doodads.RemoveAt(i);
                }
            }
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
            // Update
            foreach (var attrib in source)
            {
                var existing = dest.FirstOrDefault(a => a.Name == attrib.Name);

                if (existing != null)
                {
                    existing.SymbolDetails.BackgroundColor = attrib.SymbolDetails.BackgroundColor;
                    existing.SymbolDetails.SmileyBodyColor = attrib.SymbolDetails.SmileyBodyColor;
                    existing.SymbolDetails.SmileyLineColor = attrib.SymbolDetails.SmileyLineColor;
                    existing.SymbolDetails.SmileyExpression = attrib.SymbolDetails.SmileyExpression;
                    existing.SymbolDetails.SymbolHue = attrib.SymbolDetails.SymbolHue;
                    existing.SymbolDetails.SymbolLightness = attrib.SymbolDetails.SymbolLightness;
                    existing.SymbolDetails.SymbolSaturation = attrib.SymbolDetails.SymbolSaturation;
                    existing.SymbolDetails.SymbolType = attrib.SymbolDetails.SymbolType;
                    existing.SymbolDetails.SymbolClampColor = attrib.SymbolDetails.SymbolClampColor;
                    existing.SymbolDetails.SymbolEffectType = attrib.SymbolDetails.SymbolEffectType;
                    existing.SymbolDetails.SymbolPath = attrib.SymbolDetails.SymbolPath;
                    existing.SymbolDetails.SymbolSize = attrib.SymbolDetails.SymbolSize;
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

            else if (alterationEffect is CreateTemporaryCharacterAlterationEffectTemplateViewModel)
            {
                if ((alterationEffect as CreateTemporaryCharacterAlterationEffectTemplateViewModel).TemporaryCharacter != null)
                    UpdateAttackAttributeCollection(sourceAttackAttributes, (alterationEffect as CreateTemporaryCharacterAlterationEffectTemplateViewModel).TemporaryCharacter.AttackAttributes);
            }
        }

        /// <summary>
        /// Returns combined list of all alteration effects
        /// </summary>
        private IEnumerable<IAlterationEffectTemplateViewModel> GetAllAlterationEffects()
        {
            var consumableFunc = new Func<ConsumableTemplateViewModel, IEnumerable<IAlterationEffectTemplateViewModel>>(consumable =>
            {
                var list = new List<IEnumerable<IAlterationEffectTemplateViewModel>>()
                {
                    _scenarioCollectionProvider.Consumables.Select(x => x.ConsumableAlteration.Effect),
                    _scenarioCollectionProvider.Consumables.Select(x => x.ConsumableProjectileAlteration.Effect),
                    _scenarioCollectionProvider.Consumables.SelectMany(x => x.LearnedSkill.Skills.Select(z => z.SkillAlteration.Effect))
                };

                return list.SelectMany(x => x);
            });

            var equipmentFunc = new Func<EquipmentTemplateViewModel, IEnumerable<IAlterationEffectTemplateViewModel>>(consumable =>
            {
                var list = new List<IEnumerable<IAlterationEffectTemplateViewModel>>()
                {
                    _scenarioCollectionProvider.Equipment.Select(x => x.EquipmentAttackAlteration.Effect),
                    _scenarioCollectionProvider.Equipment.Select(x => x.EquipmentCurseAlteration.Effect),
                    _scenarioCollectionProvider.Equipment.Select(x => x.EquipmentEquipAlteration.Effect)
                };

                return list.SelectMany(x => x);
            });

            var alterations = new List<IEnumerable<IAlterationEffectTemplateViewModel>>()
            {
                _scenarioCollectionProvider.Consumables.SelectMany(x => consumableFunc(x)),
                _scenarioCollectionProvider.Equipment.SelectMany(x => equipmentFunc(x)),
                _scenarioCollectionProvider.Doodads.Select(x => x.AutomaticAlteration.Effect),
                _scenarioCollectionProvider.Doodads.Select(x => x.InvokedAlteration.Effect),
                _scenarioCollectionProvider.Enemies.SelectMany(x => x.BehaviorDetails.Behaviors.Select(z => z.Alteration.Effect)),
                _scenarioCollectionProvider.Enemies.SelectMany(x => x.StartingConsumables.SelectMany(z => consumableFunc(z.TheTemplate))),
                _scenarioCollectionProvider.Enemies.SelectMany(x => x.StartingEquipment.SelectMany(z => equipmentFunc(z.TheTemplate))),
                _scenarioCollectionProvider.Friendlies.SelectMany(x => x.BehaviorDetails.Behaviors.Select(z => z.Alteration.Effect)),
                _scenarioCollectionProvider.Friendlies.SelectMany(x => x.StartingConsumables.SelectMany(z => consumableFunc(z.TheTemplate))),
                _scenarioCollectionProvider.Friendlies.SelectMany(x => x.StartingEquipment.SelectMany(z => equipmentFunc(z.TheTemplate))),
                _scenarioCollectionProvider.SkillSets.SelectMany(x => x.Skills.Select(z => z.SkillAlteration.Effect)),
                _scenarioCollectionProvider.PlayerClasses.SelectMany(q => q.Skills).SelectMany(x => x.Skills.Select(z => z.SkillAlteration.Effect)),
                _scenarioCollectionProvider.PlayerClasses.SelectMany(q => q.StartingConsumables).SelectMany(x => consumableFunc(x.TheTemplate)),
                _scenarioCollectionProvider.PlayerClasses.SelectMany(q => q.StartingEquipment).SelectMany(x => equipmentFunc(x.TheTemplate)),
            };

            return alterations.SelectMany(x => x).Actualize();
        }

        #endregion
    }
}
