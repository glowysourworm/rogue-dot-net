using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IItemGenerator))]
    public class ItemGenerator : IItemGenerator
    {
        private readonly IRandomSequenceGenerator _randomSequenceGenerator;
        private readonly ISymbolDetailsGenerator _symbolDetailsGenerator;
        private readonly IAttackAttributeGenerator _attackAttributeGenerator;
        private readonly IAnimationGenerator _animationGenerator;
        private readonly ISkillSetGenerator _skillSetGenerator;       

        [ImportingConstructor]
        public ItemGenerator(IRandomSequenceGenerator randomSequenceGenerator, 
                             ISymbolDetailsGenerator symbolDetailsGenerator,
                             IAttackAttributeGenerator attackAttributeGenerator,
                             IAnimationGenerator animationGenerator,
                             ISkillSetGenerator skillSetGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _symbolDetailsGenerator = symbolDetailsGenerator;
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

            // Map Symbol Details
            _symbolDetailsGenerator.MapSymbolDetails(equipmentTemplate.SymbolDetails, equipment);

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
            equipment.ThrowQuality = _randomSequenceGenerator.GetRandomValue(equipmentTemplate.ThrowQuality);

            equipment.AttackAttributes = equipmentTemplate.AttackAttributes
                                                          .Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x))
                                                          .ToList();

            equipment.HasCharacterClassRequirement = equipmentTemplate.HasCharacterClassRequirement;

            // Character Class Requirement
            if (equipmentTemplate.HasCharacterClassRequirement)
                equipment.CharacterClass = equipmentTemplate.CharacterClass;

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
            consumable.HasLearnedSkillSet = consumableTemplate.HasLearnedSkill;
            consumable.HasAlteration = consumableTemplate.HasAlteration;
            consumable.HasProjectileAlteration = consumableTemplate.HasProjectileAlteration;

            // Map Symbol Details
            _symbolDetailsGenerator.MapSymbolDetails(consumableTemplate.SymbolDetails, consumable);

            consumable.Type = consumableTemplate.Type;
            consumable.SubType = consumableTemplate.SubType;
            consumable.Weight = consumableTemplate.Weight;
            consumable.LevelRequired = consumableTemplate.LevelRequired;
            consumable.Uses = _randomSequenceGenerator.GetRandomValue(consumableTemplate.UseCount);
            consumable.IdentifyOnUse = consumableTemplate.IdentifyOnUse;
            consumable.NoteMessage = consumableTemplate.NoteMessage;

            consumable.HasCharacterClassRequirement = consumableTemplate.HasCharacterClassRequirement;

            // Character Class
            if (consumableTemplate.HasCharacterClassRequirement)
                consumable.CharacterClass = consumableTemplate.CharacterClass;

            consumableTemplate.HasBeenGenerated = true;

            return consumable;
        }


        public Consumable GenerateProbabilityConsumable(ProbabilityConsumableTemplate probabilityTemplate)
        {
            return (_randomSequenceGenerator.Get() < probabilityTemplate.GenerationProbability) ? 
                        GenerateConsumable((ConsumableTemplate)probabilityTemplate.TheTemplate) : null;
        }

        public Equipment GenerateProbabilityEquipment(ProbabilityEquipmentTemplate probabilityTemplate, bool equipOnStartup = false)
        {
            var equipment = (_randomSequenceGenerator.Get() < probabilityTemplate.GenerationProbability) ? 
                                GenerateEquipment((EquipmentTemplate)probabilityTemplate.TheTemplate) : null;

            if (equipment != null)
                equipment.IsEquipped = probabilityTemplate.EquipOnStartup && equipOnStartup;

            return equipment;
        }
    }
}
