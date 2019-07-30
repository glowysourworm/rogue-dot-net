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

        public Equipment GenerateEquipment(EquipmentTemplate equipmentTemplate, IEnumerable<CharacterClass> religions)
        {
            if (equipmentTemplate.IsUnique && equipmentTemplate.HasBeenGenerated)
                throw new Exception("Trying to generate a Unique item twice");

            Equipment equipment = new Equipment();
            if (equipmentTemplate.HasEquipSpell)
                equipment.EquipSpell = _spellGenerator.GenerateSpell(equipmentTemplate.EquipSpell, religions);

            if (equipmentTemplate.HasCurseSpell)
                equipment.CurseSpell = _spellGenerator.GenerateSpell(equipmentTemplate.CurseSpell, religions);

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
            equipment.LevelRequired = equipmentTemplate.LevelRequired;
            equipment.Quality = _randomSequenceGenerator.GetRandomValue(equipmentTemplate.Quality);

            equipment.AttackAttributes = equipmentTemplate.AttackAttributes
                                                          .Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x))
                                                          .ToList();
            // TODO:RELIGION
            // equipment.HasReligionRequirement = equipmentTemplate.HasReligionRequirement;

            // TODO:RELIGION
            // Religious Affiliation Requirement
            //if (equipment.HasReligionRequirement)
            //    equipment.Religion = religions.First(religion => religion.RogueName == equipmentTemplate.Religion.Name);

            equipmentTemplate.HasBeenGenerated = true;
            return equipment;
        }
        public Consumable GenerateConsumable(ConsumableTemplate consumableTemplate, IEnumerable<CharacterClass> religions)
        {
            if (consumableTemplate.IsUnique && consumableTemplate.HasBeenGenerated)
                throw new Exception("Trying to generate a Unique item twice");

            Consumable consumable = new Consumable();
            consumable.RogueName = consumableTemplate.Name;
            consumable.Spell = _spellGenerator.GenerateSpell(consumableTemplate.SpellTemplate, religions);
            consumable.ProjectileSpell = _spellGenerator.GenerateSpell(consumableTemplate.ProjectileSpellTemplate, religions);
            consumable.LearnedSkill = _skillSetGenerator.GenerateSkillSet(consumableTemplate.LearnedSkill, religions);
            consumable.AmmoSpell = _spellGenerator.GenerateSpell(consumableTemplate.AmmoSpellTemplate, religions);
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
            consumable.LevelRequired = consumableTemplate.LevelRequired;
            consumable.Uses = _randomSequenceGenerator.GetRandomValue(consumableTemplate.UseCount);
            consumable.IdentifyOnUse = consumableTemplate.IdentifyOnUse;
            consumable.NoteMessage = consumableTemplate.NoteMessage;

            // TODO:RELIGION
            //consumable.HasReligionRequirement = consumableTemplate.HasReligionRequirement;

            // TODO:RELIGION
            // Religious Affiliation Requirement
            //if (consumable.HasReligionRequirement)
            //    consumable.Religion = religions.First(religion => religion.RogueName == consumableTemplate.Religion.Name);

            consumableTemplate.HasBeenGenerated = true;

            return consumable;
        }


        public Consumable GenerateProbabilityConsumable(ProbabilityConsumableTemplate probabilityTemplate, IEnumerable<CharacterClass> religions)
        {
            int num = _randomSequenceGenerator.CalculateGenerationNumber(probabilityTemplate.GenerationProbability);

            return (num > 0) ? GenerateConsumable((ConsumableTemplate)probabilityTemplate.TheTemplate, religions) : null;
        }

        public Equipment GenerateProbabilityEquipment(ProbabilityEquipmentTemplate probabilityTemplate, IEnumerable<CharacterClass> religions, bool equipOnStartup = false)
        {
            int num = _randomSequenceGenerator.CalculateGenerationNumber(probabilityTemplate.GenerationProbability);

            var equipment = (num > 0) ? GenerateEquipment((EquipmentTemplate)probabilityTemplate.TheTemplate, religions) : null;

            if (equipment != null)
                equipment.IsEquipped = probabilityTemplate.EquipOnStartup && equipOnStartup;

            return equipment;
        }
    }
}
