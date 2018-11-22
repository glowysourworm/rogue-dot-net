using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;

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
        public void UpdateAlterations(ScenarioConfigurationContainerViewModel configuration)
        {
            var collection = configuration.MagicSpells;

            // Skill Sets
            foreach (var skillSet in configuration.SkillTemplates)
                UpdateCollection(collection, skillSet.Spells);

            // Doodads
            foreach (var doodad in configuration.DoodadTemplates)
            {
                doodad.AutomaticMagicSpellTemplate = MatchByName(collection, doodad.AutomaticMagicSpellTemplate);
                doodad.InvokedMagicSpellTemplate = MatchByName(collection, doodad.InvokedMagicSpellTemplate);
            }

            // Equipment
            foreach (var equipment in configuration.EquipmentTemplates)
            {
                equipment.AmmoTemplate.AmmoSpellTemplate = MatchByName(collection, equipment.AmmoTemplate.AmmoSpellTemplate);
                equipment.CurseSpell = MatchByName(collection, equipment.CurseSpell);
                equipment.EquipSpell = MatchByName(collection, equipment.EquipSpell);
            }

            // Consumables
            foreach (var consumable in configuration.ConsumableTemplates)
            {
                consumable.AmmoSpellTemplate = MatchByName(collection, consumable.AmmoSpellTemplate);
                consumable.ProjectileSpellTemplate = MatchByName(collection, consumable.ProjectileSpellTemplate);
                consumable.SpellTemplate = MatchByName(collection, consumable.SpellTemplate);
            }
        }

        public void UpdateAnimations(ScenarioConfigurationContainerViewModel configuration)
        {
            foreach (var alteration in configuration.MagicSpells)
                UpdateCollection(configuration.AnimationTemplates, alteration.Animations);
        }

        public void UpdateAttackAttributes(ScenarioConfigurationContainerViewModel configuration)
        {
            // Alterations
            foreach (var spell in configuration.MagicSpells)
                UpdateAttackAttributeCollection(configuration.AttackAttributes, spell.Effect.AttackAttributes);

            // Equipment
            foreach (var equipment in configuration.EquipmentTemplates)
                UpdateAttackAttributeCollection(configuration.AttackAttributes, equipment.AttackAttributes);

            // Enemies
            foreach (var enemy in configuration.EnemyTemplates)
                UpdateAttackAttributeCollection(configuration.AttackAttributes, enemy.AttackAttributes);
        }

        public void UpdateBrushes(ScenarioConfigurationContainerViewModel configuration)
        {
            foreach (var animation in configuration.AnimationTemplates)
            {
                animation.FillTemplate = MatchByName(configuration.BrushTemplates, animation.FillTemplate);
                animation.StrokeTemplate = MatchByName(configuration.BrushTemplates, animation.StrokeTemplate);
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

            // Player
            UpdateStartingConsumablesCollection(configuration.ConsumableTemplates, configuration.PlayerTemplate.StartingConsumables);
            UpdateStartingEquipmentCollection(configuration.EquipmentTemplates, configuration.PlayerTemplate.StartingEquipment);
        }

        public void UpdateSkillSets(ScenarioConfigurationContainerViewModel configuration)
        {
            // Consumables
            foreach (var consumable in configuration.ConsumableTemplates)
                consumable.LearnedSkill = MatchByName(configuration.SkillTemplates, consumable.LearnedSkill);

            UpdateCollection(configuration.SkillTemplates, configuration.PlayerTemplate.Skills);
        }
        #endregion

        #region (private) Collection Methods

        private T MatchByName<T>(IList<T> source, T dest) where T : TemplateViewModel
        {
            return source.FirstOrDefault(x => x.Name == dest.Name);
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
        private void UpdateAttackAttributeCollection(IList<DungeonObjectTemplateViewModel> source, IList<AttackAttributeTemplateViewModel> dest)
        {
            // Create
            foreach (var attrib in source)
            {
                if (!dest.Any(a => a.Name == attrib.Name))
                    dest.Add(new AttackAttributeTemplateViewModel()
                    {
                        Name = attrib.Name,
                        SymbolDetails = attrib.SymbolDetails
                    });

                // Update
                else
                {
                    var existing = dest.First(a => a.Name == attrib.Name);
                    existing.SymbolDetails.Icon = attrib.SymbolDetails.Icon;
                }
            }

            // Delete
            for (int i = dest.Count - 1; i >= 0; i--)
            {
                if (!source.Any(a => a.Name == dest[i].Name))
                    dest.RemoveAt(i);
            }
        }

        #endregion

        /*

        #region TODO REMOVE THESE!!!
        private void ResolveConfigurationReferences()
        {
            // References to spells should be held through serialization. I don't know how to do this using the 
            // .NET serializer; but it's not that much code to ensure they're consistent... 
            

            // animation brushes
            foreach (var animation in _config.AnimationTemplates)
            {
                if (animation.FillTemplate != null && animation.FillTemplate.Name != "New Template")
                    animation.FillTemplate = _config.BrushTemplates.First(b => b.Name == animation.FillTemplate.Name);

                if (animation.StrokeTemplate != null && animation.StrokeTemplate.Name != "New Template")
                    animation.StrokeTemplate = _config.PenTemplates.First(b => b.Name == animation.StrokeTemplate.Name);
            }

            // spells
            foreach (var spell in _config.MagicSpells)
            {
                for (int i = 0; i < spell.Animations.Count; i++)
                {
                    var animation = spell.Animations[i];
                    if (animation != null)
                        animation = _config.AnimationTemplates.First(a => a.Name == animation.Name);

                    SyncAttackAttributes(_config.AttackAttributes, spell.Effect.AttackAttributes.Cast<DungeonObjectTemplateViewModel>().ToList());
                    SyncAttackAttributes(_config.AttackAttributes, spell.AuraEffect.AttackAttributes.Cast<DungeonObjectTemplateViewModel>().ToList());
                }
            }

            // skill set spells
            foreach (var skillSet in _config.SkillTemplates)
            {
                for (int i = 0; i < skillSet.Spells.Count; i++)
                {
                    var spell = skillSet.Spells[i];
                    if (spell != null)
                        spell = _config.MagicSpells.First(s => s.Name == spell.Name);
                }
            }

            // consumable spells and skill sets
            foreach (var consumable in _config.ConsumableTemplates)
            {
                if (consumable.AmmoSpellTemplate != null && consumable.SubType == ConsumableSubType.Ammo)
                    consumable.AmmoSpellTemplate = _config.MagicSpells.First(s => s.Name == consumable.AmmoSpellTemplate.Name);

                if (consumable.LearnedSkill != null && consumable.HasLearnedSkill)
                    consumable.LearnedSkill = _config.SkillTemplates.First(s => s.Name == consumable.LearnedSkill.Name);

                if (consumable.ProjectileSpellTemplate != null && consumable.IsProjectile)
                    consumable.ProjectileSpellTemplate = _config.MagicSpells.First(s => s.Name == consumable.ProjectileSpellTemplate.Name);

                if (consumable.SpellTemplate != null && consumable.HasSpell)
                    consumable.SpellTemplate = _config.MagicSpells.First(s => s.Name == consumable.SpellTemplate.Name);
            }

            // equipment spells
            foreach (var equipment in _config.EquipmentTemplates)
            {
                if (equipment.AmmoTemplate != null && equipment.Type == EquipmentType.RangeWeapon)
                    equipment.AmmoTemplate = _config.ConsumableTemplates.First(s => s.Name == equipment.AmmoTemplate.Name);

                if (equipment.AttackSpell != null && equipment.HasAttackSpell)
                    equipment.AttackSpell = _config.MagicSpells.First(s => s.Name == equipment.AttackSpell.Name);

                if (equipment.EquipSpell != null && equipment.HasEquipSpell)
                    equipment.EquipSpell = _config.MagicSpells.First(s => s.Name == equipment.EquipSpell.Name);

                if (equipment.CurseSpell != null && equipment.HasCurseSpell)
                    equipment.CurseSpell = _config.MagicSpells.First(s => s.Name == equipment.CurseSpell.Name);

                SyncAttackAttributes(_config.AttackAttributes, equipment.AttackAttributes.Cast<DungeonObjectTemplateViewModel>().ToList());
            }

            // doodad spells
            foreach (var doodad in _config.DoodadTemplates)
            {
                if (doodad.AutomaticMagicSpellTemplate != null && doodad.IsAutomatic)
                    doodad.AutomaticMagicSpellTemplate = _config.MagicSpells.First(s => s.Name == doodad.AutomaticMagicSpellTemplate.Name);

                if (doodad.InvokedMagicSpellTemplate != null && doodad.IsInvoked)
                    doodad.InvokedMagicSpellTemplate = _config.MagicSpells.First(s => s.Name == doodad.InvokedMagicSpellTemplate.Name);
            }

            // player consumables
            for (int i = 0; i < _config.PlayerTemplate.StartingConsumables.Count; i++)
            {
                var consumable = _config.PlayerTemplate.StartingConsumables[i];
                if (consumable != null)
                    consumable.TheTemplate = _config.ConsumableTemplates.First(c => c.Name == consumable.TheTemplate.Name);
            }

            // player equipment
            for (int i = 0; i < _config.PlayerTemplate.StartingEquipment.Count; i++)
            {
                var equipment = _config.PlayerTemplate.StartingEquipment[i];
                if (equipment != null)
                    equipment.TheTemplate = _config.EquipmentTemplates.First(c => c.Name == equipment.TheTemplate.Name);
            }

            // player skills
            for (int i = 0; i < _config.PlayerTemplate.Skills.Count; i++)
            {
                var skill = _config.PlayerTemplate.Skills[i];
                if (skill != null)
                    _config.PlayerTemplate.Skills[i] = _config.SkillTemplates.First(s => s.Name == skill.Name);
            }

            // Enemies
            foreach (var enemy in _config.EnemyTemplates)
            {
                if (enemy.BehaviorDetails.PrimaryBehavior.EnemySpell != null &&
                    enemy.BehaviorDetails.PrimaryBehavior.AttackType == CharacterAttackType.Skill)
                    enemy.BehaviorDetails.PrimaryBehavior.EnemySpell = _config.MagicSpells.First(s => s.Name == enemy.BehaviorDetails.PrimaryBehavior.EnemySpell.Name);

                if (enemy.BehaviorDetails.SecondaryBehavior.EnemySpell != null &&
                    enemy.BehaviorDetails.SecondaryBehavior.AttackType == CharacterAttackType.Skill)
                    enemy.BehaviorDetails.SecondaryBehavior.EnemySpell = _config.MagicSpells.First(s => s.Name == enemy.BehaviorDetails.SecondaryBehavior.EnemySpell.Name);

                // enemy consumables
                for (int i = 0; i < enemy.StartingConsumables.Count; i++)
                {
                    var consumable = enemy.StartingConsumables[i];
                    if (consumable != null)
                        consumable.TheTemplate = _config.ConsumableTemplates.First(c => c.Name == consumable.TheTemplate.Name);
                }

                // enemy equipment
                for (int i = 0; i < enemy.StartingEquipment.Count; i++)
                {
                    var equipment = enemy.StartingEquipment[i];
                    if (equipment != null)
                        equipment.TheTemplate = _config.EquipmentTemplates.First(c => c.Name == equipment.TheTemplate.Name);
                }

                // enemy attack attributes
                SyncAttackAttributes(_config.AttackAttributes, enemy.AttackAttributes.Cast<DungeonObjectTemplateViewModel>().ToList());
            }
        }

        private void SyncAttackAttributes(IList<DungeonObjectTemplateViewModel> source, IList<DungeonObjectTemplateViewModel> dest)
        {
            // Create
            foreach (var attrib in source)
            {
                if (!dest.Any(a => a.Name == attrib.Name))
                    dest.Add(attrib);

                // Update
                else
                {
                    var existing = dest.First(a => a.Name == attrib.Name);
                    existing.SymbolDetails.Icon = attrib.SymbolDetails.Icon;
                }
            }

            // Delete
            for (int i = dest.Count - 1; i >= 0; i--)
            {
                if (!source.Any(a => a.Name == dest[i].Name))
                    dest.RemoveAt(i);
            }
        }
        #endregion
    */
    }
}
