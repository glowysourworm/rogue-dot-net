﻿using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(IItemGenerator))]
    public class ItemGenerator : IItemGenerator
    {
        private readonly IRandomSequenceGenerator _randomSequenceGenerator;
        private readonly IAttackAttributeGenerator _attackAttributeGenerator;
        private readonly ISkillSetGenerator _skillSetGenerator;
        private readonly ISpellGenerator _spellGenerator;

        [ImportingConstructor]
        public ItemGenerator(IRandomSequenceGenerator randomSequenceGenerator, 
                             IAttackAttributeGenerator attackAttributeGenerator,
                             ISpellGenerator spellGenerator,
                             ISkillSetGenerator skillSetGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;
            _skillSetGenerator = skillSetGenerator;
            _spellGenerator = spellGenerator;
        }

        public Equipment GenerateEquipment(EquipmentTemplate equipmentTemplate)
        {
            if (equipmentTemplate.IsUnique && equipmentTemplate.HasBeenGenerated)
                throw new Exception("Trying to generate a Unique item twice");

            Equipment equipment = new Equipment();
            if (equipmentTemplate.HasEquipSpell)
                equipment.EquipSpell = _spellGenerator.GenerateSpell(equipmentTemplate.EquipSpell);

            if (equipmentTemplate.HasCurseSpell)
                equipment.CurseSpell = _spellGenerator.GenerateSpell(equipmentTemplate.CurseSpell);

            equipment.CharacterColor = equipmentTemplate.SymbolDetails.CharacterColor;
            equipment.CharacterSymbol = equipmentTemplate.SymbolDetails.CharacterSymbol;
            equipment.Icon = equipmentTemplate.SymbolDetails.Icon;
            equipment.SmileyAuraColor = equipmentTemplate.SymbolDetails.SmileyAuraColor;
            equipment.SmileyBodyColor = equipmentTemplate.SymbolDetails.SmileyBodyColor;
            equipment.SmileyLineColor = equipmentTemplate.SymbolDetails.SmileyLineColor;
            equipment.SmileyMood = equipmentTemplate.SymbolDetails.SmileyMood;
            equipment.SymbolType = equipmentTemplate.SymbolDetails.Type;

            equipment.HasEquipSpell = equipmentTemplate.HasEquipSpell;
            equipment.HasCurseSpell = equipmentTemplate.HasCurseSpell;
            equipment.Type = equipmentTemplate.Type;
            equipment.Class = _randomSequenceGenerator.GetRandomValue(equipmentTemplate.Class);
            equipment.IsEquipped = false;
            equipment.IsCursed = equipmentTemplate.IsCursed;
            equipment.RogueName = equipmentTemplate.Name;
            equipment.AmmoName = equipmentTemplate.AmmoTemplate == null ? "" : equipmentTemplate.AmmoTemplate.Name;
            equipment.Weight = equipmentTemplate.Weight;
            equipment.Quality = _randomSequenceGenerator.GetRandomValue(equipmentTemplate.Quality);

            equipment.AttackAttributes = equipmentTemplate.AttackAttributes
                                                          .Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x))
                                                          .ToList();

            equipmentTemplate.HasBeenGenerated = true;
            return equipment;
        }
        public Consumable GenerateConsumable(ConsumableTemplate consumableTemplate)
        {
            if (consumableTemplate.IsUnique && consumableTemplate.HasBeenGenerated)
                throw new Exception("Trying to generate a Unique item twice");

            Consumable consumable = new Consumable();
            consumable.RogueName = consumableTemplate.Name;
            consumable.Spell = _spellGenerator.GenerateSpell(consumableTemplate.SpellTemplate);
            consumable.ProjectileSpell = _spellGenerator.GenerateSpell(consumableTemplate.ProjectileSpellTemplate);
            consumable.LearnedSkill = _skillSetGenerator.GenerateSkillSet(consumableTemplate.LearnedSkill);
            consumable.AmmoSpell = _spellGenerator.GenerateSpell(consumableTemplate.AmmoSpellTemplate);
            consumable.HasLearnedSkillSet = consumableTemplate.HasLearnedSkill;
            consumable.HasProjectileSpell = consumableTemplate.IsProjectile;
            consumable.HasSpell = consumableTemplate.HasSpell;

            consumable.CharacterColor = consumableTemplate.SymbolDetails.CharacterColor;
            consumable.CharacterSymbol = consumableTemplate.SymbolDetails.CharacterSymbol;
            consumable.Icon = consumableTemplate.SymbolDetails.Icon;
            consumable.SmileyAuraColor = consumableTemplate.SymbolDetails.SmileyAuraColor;
            consumable.SmileyBodyColor = consumableTemplate.SymbolDetails.SmileyBodyColor;
            consumable.SmileyLineColor = consumableTemplate.SymbolDetails.SmileyLineColor;
            consumable.SmileyMood = consumableTemplate.SymbolDetails.SmileyMood;
            consumable.SymbolType = consumableTemplate.SymbolDetails.Type;

            consumable.Type = consumableTemplate.Type;
            consumable.SubType = consumableTemplate.SubType;
            consumable.Weight = consumableTemplate.Weight;
            consumable.Uses = _randomSequenceGenerator.GetRandomValue(consumableTemplate.UseCount);
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
