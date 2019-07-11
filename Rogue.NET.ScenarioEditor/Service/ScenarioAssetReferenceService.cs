using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;

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
            {
                // Skill
                foreach (var skill in skillSet.Skills)
                    skill.Alteration = MatchByName(configuration.MagicSpells, skill.Alteration);
            }

            // Doodads
            foreach (var doodad in configuration.DoodadTemplates)
            {
                doodad.AutomaticMagicSpellTemplate = MatchByName(collection, doodad.AutomaticMagicSpellTemplate);
                doodad.InvokedMagicSpellTemplate = MatchByName(collection, doodad.InvokedMagicSpellTemplate);

                if (doodad.AutomaticMagicSpellTemplate == null)
                    doodad.IsAutomatic = false;

                if (doodad.InvokedMagicSpellTemplate == null)
                    doodad.IsInvoked = false;
            }

            // Equipment
            foreach (var equipment in configuration.EquipmentTemplates)
            {
                equipment.AmmoTemplate.AmmoSpellTemplate = MatchByName(collection, equipment.AmmoTemplate.AmmoSpellTemplate);
                equipment.CurseSpell = MatchByName(collection, equipment.CurseSpell);
                equipment.EquipSpell = MatchByName(collection, equipment.EquipSpell);

                if (equipment.CurseSpell == null)
                    equipment.HasCurseSpell = false;

                if (equipment.EquipSpell == null)
                    equipment.HasEquipSpell = false;
            }

            // Consumables
            foreach (var consumable in configuration.ConsumableTemplates)
            {
                consumable.AmmoSpellTemplate = MatchByName(collection, consumable.AmmoSpellTemplate);
                consumable.ProjectileSpellTemplate = MatchByName(collection, consumable.ProjectileSpellTemplate);
                consumable.SpellTemplate = MatchByName(collection, consumable.SpellTemplate);

                if (consumable.ProjectileSpellTemplate == null)
                    consumable.IsProjectile = false;

                if (consumable.SpellTemplate == null)
                    consumable.HasSpell = false;
            }
        }

        public void UpdateAnimations(ScenarioConfigurationContainerViewModel configuration)
        {
            foreach (var alteration in configuration.MagicSpells)
                UpdateCollection(configuration.AnimationTemplates, alteration.Animations);

            foreach (var enemy in configuration.EnemyTemplates)
                UpdateCollection(configuration.AnimationTemplates, enemy.DeathAnimations);
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

            // Religions
            foreach (var religion in configuration.Religions)
                UpdateAttackAttributeCollection(configuration.AttackAttributes, religion.BonusAttackAttributes);
        }

        public void UpdateReligions(ScenarioConfigurationContainerViewModel configuration)
        {
            // Alterations
            foreach (var alteration in configuration.MagicSpells
                                                    .Where(x => x.Type == AlterationType.OtherMagicEffect &&
                                                                x.OtherEffectType == AlterationMagicEffectType.IncreaseReligiousAffiliation))
            {
                alteration.ReligiousAffiliationReligion = MatchByName(configuration.Religions, alteration.ReligiousAffiliationReligion);
            }

            // Consumables
            foreach (var consumable in configuration.ConsumableTemplates.Where(x => x.HasReligionRequirement))
            {
                consumable.Religion = MatchByName(configuration.Religions, consumable.Religion);

                if (consumable.Religion == null)
                    consumable.HasReligionRequirement= false;
            }

            // Equipment
            foreach (var equipment in configuration.EquipmentTemplates.Where(x => x.HasReligionRequirement))
            {
                equipment.Religion = MatchByName(configuration.Religions, equipment.Religion);

                if (equipment.Religion == null)
                    equipment.HasReligionRequirement = false;
            }

            // Doodads
            foreach (var doodad in configuration.DoodadTemplates.Where(x => x.HasReligionRequirement))
            {
                doodad.Religion = MatchByName(configuration.Religions, doodad.Religion);

                if (doodad.Religion == null)
                    doodad.HasReligionRequirement = false;
            }

            // Enemies
            foreach (var enemy in configuration.EnemyTemplates.Where(x => x.HasReligion))
            {
                enemy.Religion = MatchByName(configuration.Religions, enemy.Religion);

                if (enemy.Religion == null)
                    enemy.HasReligion = false;
            }
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
            {
                consumable.LearnedSkill = MatchByName(configuration.SkillTemplates, consumable.LearnedSkill);

                if (consumable.LearnedSkill == null)
                    consumable.HasLearnedSkill = false;
            }

            // Religion Starting Skill Set
            foreach (var religion in configuration.Religions)
            {
                if (religion.HasBonusSkillSet)
                {
                    var skillSet = configuration.SkillTemplates.FirstOrDefault(x => x.Name == religion.Name);

                    if (skillSet != null)
                        religion.BonusSkillSetName = skillSet.Name;
                    else
                        religion.HasBonusSkillSet = false;
                }
            }

            // Player Starting Skills
            UpdateCollection(configuration.SkillTemplates, configuration.PlayerTemplate.Skills);
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

        #endregion
    }
}
