using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Behavior;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [Export(typeof(ICharacterGenerator))]
    public class CharacterGenerator : ICharacterGenerator
    {
        private readonly IRandomSequenceGenerator _randomSequenceGenerator;
        private readonly ISymbolDetailsGenerator _symbolDetailsGenerator;
        private readonly IAttackAttributeGenerator _attackAttributeGenerator;
        private readonly ISkillSetGenerator _skillSetGenerator;
        private readonly IBehaviorGenerator _behaviorGenerator;
        private readonly IItemGenerator _itemGenerator;
        private readonly IAnimationGenerator _animationGenerator;
        private readonly IAlterationGenerator _alterationGenerator;

        [ImportingConstructor]
        public CharacterGenerator(
            IRandomSequenceGenerator randomSequenceGenerator, 
            ISymbolDetailsGenerator symbolDetailsGenerator,
            IAttackAttributeGenerator attackAttributeGenerator,
            ISkillSetGenerator skillSetGenerator,
            IBehaviorGenerator behaviorGenerator,
            IItemGenerator itemGenerator,
            IAnimationGenerator animationGenerator,
            IAlterationGenerator alterationGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _symbolDetailsGenerator = symbolDetailsGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;
            _skillSetGenerator = skillSetGenerator;
            _behaviorGenerator = behaviorGenerator;
            _itemGenerator = itemGenerator;
            _animationGenerator = animationGenerator;
            _alterationGenerator = alterationGenerator;
        }

        public Player GeneratePlayer(PlayerTemplate playerTemplate)
        {
            var player = new Player();

            SetCharacterProperties(player, playerTemplate);

            player.FoodUsagePerTurnBase = _randomSequenceGenerator.GetRandomValue(playerTemplate.FoodUsage);
            player.Class = playerTemplate.Name; // TODO: Have to move character class name to PlayerTemplate.Class
            player.Experience = 0;
            player.Hunger = 0;
            player.Level = 0;

            player.AttackAttributes = playerTemplate.AttackAttributes.Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x))
                                                                     .ToDictionary(x => x.RogueName);

            //Starting Skills
            player.SkillSets = playerTemplate.Skills
                                             .Select(x => _skillSetGenerator.GenerateSkillSet(x))
                                             .ToList();

            return player;
        }
        public Enemy GenerateEnemy(EnemyTemplate enemyTemplate)
        {
            if (enemyTemplate.IsUnique && enemyTemplate.HasBeenGenerated)
                throw new Exception("Trying to generate unique enemy twice");

            var enemy = new Enemy();

            enemy.ExperienceGiven = _randomSequenceGenerator.GetRandomValue(enemyTemplate.ExperienceGiven);

            SetCharacterProperties(enemy, enemyTemplate);
            SetNonPlayerCharacterProperties(enemy, enemyTemplate);

            enemyTemplate.HasBeenGenerated = true;
            return enemy;
        }
        public Friendly GenerateFriendly(FriendlyTemplate template)
        {
            if (template.IsUnique && template.HasBeenGenerated)
                throw new Exception("Trying to generate unique friendly twice");

            var friendly = new Friendly();

            friendly.AlignmentType = CharacterAlignmentType.PlayerAligned;
            friendly.InPlayerParty = false;

            SetCharacterProperties(friendly, template);
            SetNonPlayerCharacterProperties(friendly, template);

            template.HasBeenGenerated = true;
            return friendly;
        }
        public TemporaryCharacter GenerateTemporaryCharacter(TemporaryCharacterTemplate template)
        {
            // Temporary Characters Shouldn't be treated as Assets
            //
            //if (template.IsUnique && template.HasBeenGenerated)
            //    throw new Exception("Trying to generate unique temporary character twice");

            var temporaryCharacter = new TemporaryCharacter();

            temporaryCharacter.AlignmentType = template.AlignmentType;
            temporaryCharacter.LifetimeCounter = _randomSequenceGenerator.GetRandomValue(template.LifetimeCounter);

            SetCharacterProperties(temporaryCharacter, template);
            SetNonPlayerCharacterProperties(temporaryCharacter, template);

            template.HasBeenGenerated = true;
            return temporaryCharacter;
        }

        protected void SetCharacterProperties(Character character, CharacterTemplate template)
        {
            character.RogueName = template.Name;

            character.Hp = _randomSequenceGenerator.GetRandomValue(template.Hp);
            character.Mp = _randomSequenceGenerator.GetRandomValue(template.Mp);
            character.HpRegenBase = _randomSequenceGenerator.GetRandomValue(template.HpRegen);
            character.MpRegenBase = _randomSequenceGenerator.GetRandomValue(template.MpRegen);
            character.MpMax = character.Mp;
            character.HpMax = character.Hp;
            character.AgilityBase = _randomSequenceGenerator.GetRandomValue(template.Agility);
            character.StrengthBase = _randomSequenceGenerator.GetRandomValue(template.Strength);
            character.IntelligenceBase = _randomSequenceGenerator.GetRandomValue(template.Intelligence);
            character.SpeedBase = _randomSequenceGenerator.GetRandomValue(template.Speed);
            character.LightRadiusBase = _randomSequenceGenerator.GetRandomValue(template.LightRadius);

            // Map Symbol Details
            _symbolDetailsGenerator.MapSymbolDetails(template.SymbolDetails, character);

            // Attack Attributes
            character.AttackAttributes = template.AttackAttributes.Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x))
                                                                  .ToDictionary(x => x.RogueName);

            //Starting Consumables
            foreach (var consumableTemplate in template.StartingConsumables)
            {
                // MUST GENERATE OBJECTIVE ITEMS
                if (_randomSequenceGenerator.Get() < consumableTemplate.GenerationProbability ||
                    consumableTemplate.TheTemplate.IsObjectiveItem)
                {

                    var theTemplate = consumableTemplate.TheTemplate;

                    if (theTemplate.IsUnique && theTemplate.HasBeenGenerated)
                        continue;

                    var consumable = _itemGenerator.GenerateConsumable(theTemplate);

                    character.Consumables.Add(consumable.Id, consumable);
                }
            }

            //Starting Equipment
            foreach (var equipmentTemplate in template.StartingEquipment)
            {
                // MUST GENERATE OBJECTIVE ITEMS
                if (_randomSequenceGenerator.Get() < equipmentTemplate.GenerationProbability ||
                    equipmentTemplate.TheTemplate.IsObjectiveItem)
                {

                    var theTemplate = (EquipmentTemplate)equipmentTemplate.TheTemplate;

                    if (theTemplate.IsUnique && theTemplate.HasBeenGenerated)
                        continue;

                    var equipment = _itemGenerator.GenerateEquipment(theTemplate);

                    // Equip on Startup
                    if (equipmentTemplate.EquipOnStartup)
                    {
                        // Set Equipped
                        equipment.IsEquipped = true;

                        // Set any Alterations
                        if (equipment.HasEquipAlteration)
                            character.Alteration.Apply(_alterationGenerator.GenerateAlteration(equipment.EquipAlteration));

                        if (equipment.HasCurseAlteration)
                            character.Alteration.Apply(_alterationGenerator.GenerateAlteration(equipment.CurseAlteration));
                    }

                    character.Equipment.Add(equipment.Id, equipment);
                }
            }
        }
        protected void SetNonPlayerCharacterProperties(NonPlayerCharacter character, NonPlayerCharacterTemplate template)
        {
            character.BehaviorDetails = new BehaviorDetails();

            // Create Behavior "State Machine"
            foreach (var behaviorTemplate in template.BehaviorDetails.Behaviors)
                character.BehaviorDetails.Behaviors.Add(_behaviorGenerator.GenerateBehavior(behaviorTemplate));

            character.BehaviorDetails.CanOpenDoors = template.BehaviorDetails.CanOpenDoors;
            character.BehaviorDetails.RandomizerTurnCount = template.BehaviorDetails.RandomizerTurnCount;
            character.BehaviorDetails.UseRandomizer = template.BehaviorDetails.UseRandomizer;

            character.DeathAnimation = _animationGenerator.GenerateAnimation(template.DeathAnimation);
        }
    }
}
