using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;

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

        public void UpdateAlteredCharacterStates(ScenarioConfigurationContainerViewModel configuration)
        {
            // Alterations
            foreach (var spell in configuration.MagicSpells)
            {
                // Fix for initializing existing scenarios
                spell.Effect.AlteredState = spell.Effect.AlteredState ?? new AlteredCharacterStateTemplateViewModel();
                spell.AuraEffect.AlteredState = spell.AuraEffect.AlteredState ?? new AlteredCharacterStateTemplateViewModel();

                // NOTE*** Create a new default altered state (Normal) for non-matching (dangling) altered states. These
                //         should not interfere with operation because references aren't kept strongly (there's allowance for
                //         dangling references - so long as the underlying state is "Normal" so it doesn't interfere with 
                //         character operation).
                spell.Effect.AlteredState = MatchByName(configuration.AlteredCharacterStates, spell.Effect.AlteredState) ??
                                            new AlteredCharacterStateTemplateViewModel();

                spell.AuraEffect.AlteredState = MatchByName(configuration.AlteredCharacterStates, spell.AuraEffect.AlteredState) ??
                                            new AlteredCharacterStateTemplateViewModel();
            }
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
    }
}
