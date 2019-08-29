using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(IItemGenerator))]
    public class ItemGenerator : IItemGenerator
    {
        private readonly IRandomSequenceGenerator _randomSequenceGenerator;
        private readonly IAttackAttributeGenerator _attackAttributeGenerator;
        private readonly IAnimationGenerator _animationGenerator;
        private readonly ISkillSetGenerator _skillSetGenerator;       

        [ImportingConstructor]
        public ItemGenerator(IRandomSequenceGenerator randomSequenceGenerator, 
                             IAttackAttributeGenerator attackAttributeGenerator,
                             IAnimationGenerator animationGenerator,
                             ISkillSetGenerator skillSetGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;
            _animationGenerator = animationGenerator;
            _skillSetGenerator = skillSetGenerator;
        }

        public Equipment GenerateEquipment(EquipmentTemplate equipmentTemplate)
        {
            if (equipmentTemplate.IsUnique && equipmentTemplate.HasBeenGenerated)
                throw new Exception("Trying to generate a Unique item twice");

            Equipment equipment = new Equipment();
            if (equipmentTemplate.HasEquipAlteration)
                equipment.EquipAlteration = equipmentTemplate.EquipmentEquipAlteration;

            if (equipmentTemplate.HasCurseAlteration)
                equipment.CurseAlteration = equipmentTemplate.EquipmentCurseAlteration;

            if (equipmentTemplate.HasAttackAlteration)
                equipment.AttackAlteration = equipmentTemplate.EquipmentAttackAlteration;

            equipment.CharacterColor = equipmentTemplate.SymbolDetails.CharacterColor;
            equipment.CharacterSymbol = equipmentTemplate.SymbolDetails.CharacterSymbol;
            equipment.Icon = equipmentTemplate.SymbolDetails.Icon;
            equipment.SmileyLightRadiusColor = equipmentTemplate.SymbolDetails.SmileyAuraColor;
            equipment.SmileyBodyColor = equipmentTemplate.SymbolDetails.SmileyBodyColor;
            equipment.SmileyLineColor = equipmentTemplate.SymbolDetails.SmileyLineColor;
            equipment.SmileyMood = equipmentTemplate.SymbolDetails.SmileyMood;
            equipment.SymbolType = equipmentTemplate.SymbolDetails.Type;

            equipment.HasEquipAlteration = equipmentTemplate.HasEquipAlteration;
            equipment.HasCurseAlteration = equipmentTemplate.HasCurseAlteration;
            equipment.HasAttackAlteration = equipmentTemplate.HasAttackAlteration;
            equipment.Type = equipmentTemplate.Type;
            equipment.CombatType = equipmentTemplate.CombatType;
            equipment.Class = _randomSequenceGenerator.GetRandomValue(equipmentTemplate.Class);
            equipment.IsEquipped = false;
            equipment.IsCursed = equipmentTemplate.IsCursed;
            equipment.RogueName = equipmentTemplate.Name;
            equipment.AmmoName = equipmentTemplate.AmmoTemplate == null ? "" : equipmentTemplate.AmmoTemplate.Name;
            equipment.Weight = equipmentTemplate.Weight;
            equipment.LevelRequired = equipmentTemplate.LevelRequired;
            equipment.Quality = _randomSequenceGenerator.GetRandomValue(equipmentTemplate.Quality);

            equipment.AttackAttributes = equipmentTemplate.AttackAttributes
                                                          .Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x))
                                                          .ToList();

            equipment.HasCharacterClassRequirement = equipmentTemplate.HasCharacterClassRequirement;

            // TODO:CHARACTERCLASS
            // Character Class Requirement
            //if (equipmentTemplate.HasCharacterClassRequirement)
            //    equipment.CharacterClass = characterClasses.First(x => x.RogueName == equipmentTemplate.CharacterClass.Name);

            equipmentTemplate.HasBeenGenerated = true;
            return equipment;
        }
        public Consumable GenerateConsumable(ConsumableTemplate consumableTemplate)
        {
            if (consumableTemplate.IsUnique && consumableTemplate.HasBeenGenerated)
                throw new Exception("Trying to generate a Unique item twice");

            Consumable consumable = new Consumable();
            consumable.RogueName = consumableTemplate.Name;
            consumable.Alteration = consumableTemplate.ConsumableAlteration;
            consumable.ProjectileAlteration = consumableTemplate.ConsumableProjectileAlteration;
            consumable.LearnedSkill = _skillSetGenerator.GenerateSkillSet(consumableTemplate.LearnedSkill);
            consumable.AmmoAnimationGroup = _animationGenerator.GenerateAnimationGroup(consumableTemplate.AmmoAnimationGroup);
            consumable.HasLearnedSkillSet = consumableTemplate.HasLearnedSkill;
            consumable.HasAlteration = consumableTemplate.HasAlteration;
            consumable.HasProjectileAlteration = consumableTemplate.HasProjectileAlteration;

            consumable.CharacterColor = consumableTemplate.SymbolDetails.CharacterColor;
            consumable.CharacterSymbol = consumableTemplate.SymbolDetails.CharacterSymbol;
            consumable.Icon = consumableTemplate.SymbolDetails.Icon;
            consumable.SmileyLightRadiusColor = consumableTemplate.SymbolDetails.SmileyAuraColor;
            consumable.SmileyBodyColor = consumableTemplate.SymbolDetails.SmileyBodyColor;
            consumable.SmileyLineColor = consumableTemplate.SymbolDetails.SmileyLineColor;
            consumable.SmileyMood = consumableTemplate.SymbolDetails.SmileyMood;
            consumable.SymbolType = consumableTemplate.SymbolDetails.Type;

            consumable.Type = consumableTemplate.Type;
            consumable.SubType = consumableTemplate.SubType;
            consumable.Weight = consumableTemplate.Weight;
            consumable.LevelRequired = consumableTemplate.LevelRequired;
            consumable.Uses = _randomSequenceGenerator.GetRandomValue(consumableTemplate.UseCount);
            consumable.IdentifyOnUse = consumableTemplate.IdentifyOnUse;
            consumable.NoteMessage = consumableTemplate.NoteMessage;

            consumable.HasCharacterClassRequirement = consumableTemplate.HasCharacterClassRequirement;

            // TODO:CHARACTERCLASS
            // Character Class Affiliation Requirement
            //if (consumableTemplate.HasCharacterClassRequirement)
            //    consumable.CharacterClass = characterClasses.First(x => x.RogueName == consumableTemplate.CharacterClass.Name);

            consumableTemplate.HasBeenGenerated = true;

            return consumable;
        }


        public Consumable GenerateProbabilityConsumable(ProbabilityConsumableTemplate probabilityTemplate)
        {
            int num = _randomSequenceGenerator.CalculateGenerationNumber(probabilityTemplate.GenerationProbability);

            return (num > 0) ? GenerateConsumable((ConsumableTemplate)probabilityTemplate.TheTemplate) : null;
        }

        public Equipment GenerateProbabilityEquipment(ProbabilityEquipmentTemplate probabilityTemplate, bool equipOnStartup = false)
        {
            int num = _randomSequenceGenerator.CalculateGenerationNumber(probabilityTemplate.GenerationProbability);

            var equipment = (num > 0) ? GenerateEquipment((EquipmentTemplate)probabilityTemplate.TheTemplate) : null;

            if (equipment != null)
                equipment.IsEquipped = probabilityTemplate.EquipOnStartup && equipOnStartup;

            return equipment;
        }
    }
}
