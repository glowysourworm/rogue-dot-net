using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(ICharacterGenerator))]
    public class CharacterGenerator : ICharacterGenerator
    {
        private readonly IRandomSequenceGenerator _randomSequenceGenerator;
        private readonly IAttackAttributeGenerator _attackAttributeGenerator;
        private readonly ISkillSetGenerator _skillSetGenerator;
        private readonly IBehaviorGenerator _behaviorGenerator;
        private readonly IItemGenerator _itemGenerator;

        [ImportingConstructor]
        public CharacterGenerator(
            IRandomSequenceGenerator randomSequenceGenerator, 
            IAttackAttributeGenerator attackAttributeGenerator,
            ISkillSetGenerator skillSetGenerator,
            IBehaviorGenerator behaviorGenerator,
            IItemGenerator itemGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;
            _skillSetGenerator = skillSetGenerator;
            _behaviorGenerator = behaviorGenerator;
            _itemGenerator = itemGenerator;
        }

        public Player GeneratePlayer(PlayerTemplate playerTemplate)
        {
            var player = new Player();
            player.FoodUsagePerTurnBase = _randomSequenceGenerator.GetRandomValue(playerTemplate.FoodUsage);
            player.IntelligenceBase = _randomSequenceGenerator.GetRandomValue(playerTemplate.Intelligence);
            player.StrengthBase = _randomSequenceGenerator.GetRandomValue(playerTemplate.Strength);
            player.AgilityBase = _randomSequenceGenerator.GetRandomValue(playerTemplate.Agility);
            player.HpMax = _randomSequenceGenerator.GetRandomValue(playerTemplate.Hp);
            player.MpMax = _randomSequenceGenerator.GetRandomValue(playerTemplate.Mp);
            player.Hp = player.HpMax;
            player.Mp = player.MpMax;
            player.AuraRadiusBase = playerTemplate.AuraRadius;

            player.Icon = playerTemplate.SymbolDetails.Icon;
            player.CharacterSymbol = playerTemplate.SymbolDetails.CharacterSymbol;
            player.CharacterColor = playerTemplate.SymbolDetails.CharacterColor;
            player.SmileyMood = playerTemplate.SymbolDetails.SmileyMood;
            player.SmileyAuraColor = playerTemplate.SymbolDetails.SmileyAuraColor;
            player.SmileyBodyColor = playerTemplate.SymbolDetails.SmileyBodyColor;
            player.SmileyLineColor = playerTemplate.SymbolDetails.SmileyLineColor;

            // Replace Agility for speed calculation
            //player.SpeedBase = //TODO

            player.Class = playerTemplate.Class;
            player.Experience = 0;
            player.Hunger = 0;
            player.Level = 0;

            //Starting Consumables
            foreach (ProbabilityConsumableTemplate template in playerTemplate.StartingConsumables)
            {
                var consumableTemplate = (ConsumableTemplate)template.TheTemplate;
                if (consumableTemplate.IsUnique && consumableTemplate.HasBeenGenerated)
                    continue;

                var generationNumber = _randomSequenceGenerator.CalculateGenerationNumber(template.GenerationProbability);

                for (int i = 0; i < generationNumber; i++)
                {
                    var consumable = _itemGenerator.GenerateConsumable(consumableTemplate);
                    player.Consumables.Add(consumable.Id, consumable);
                }
            }

            //Starting Equipment
            foreach (ProbabilityEquipmentTemplate template in playerTemplate.StartingEquipment)
            {
                var equipmentTemplate = (EquipmentTemplate)template.TheTemplate;
                if (equipmentTemplate.IsUnique && equipmentTemplate.HasBeenGenerated)
                    continue;

                int generationNumber = _randomSequenceGenerator.CalculateGenerationNumber(template.GenerationProbability);
                for (int i = 0; i < generationNumber; i++)
                {
                    var equipment = _itemGenerator.GenerateEquipment(equipmentTemplate);
                    equipment.IsEquipped = template.EquipOnStartup;
                    player.Equipment.Add(equipment.Id, equipment);
                }
            }

            //Starting Skills
            player.SkillSets = playerTemplate.Skills
                                          .Select(x =>
                                          {
                                              var skillSet = _skillSetGenerator.GenerateSkillSet(x);
                                              skillSet.IsLearned = skillSet.LevelLearned <= player.Level;

                                              return skillSet;
                                          })
                                          .ToList();

            return player;
        }
        public Enemy GenerateEnemy(EnemyTemplate enemyTemplate)
        {
            if (enemyTemplate.IsUnique && enemyTemplate.HasBeenGenerated)
                throw new Exception("Trying to generate unique enemy twice");

            if (enemyTemplate.IsObjectiveItem && enemyTemplate.HasBeenGenerated)
                throw new Exception("Trying to generate objective enemy twice");

            var enemy = new Enemy();
            enemy.CreatureClass = enemyTemplate.CreatureClass.Name;
            enemy.AgilityBase = _randomSequenceGenerator.GetRandomValue(enemyTemplate.Agility);
            enemy.ExperienceGiven = _randomSequenceGenerator.GetRandomValue(enemyTemplate.ExperienceGiven);
            enemy.Hp = _randomSequenceGenerator.GetRandomValue(enemyTemplate.Hp);
            enemy.Mp = _randomSequenceGenerator.GetRandomValue(enemyTemplate.Mp);
            enemy.MpMax = enemy.Mp;
            enemy.HpMax = enemy.Hp;
            enemy.RogueName = enemyTemplate.Name;
            enemy.StrengthBase = _randomSequenceGenerator.GetRandomValue(enemyTemplate.Strength);
            enemy.IntelligenceBase = _randomSequenceGenerator.GetRandomValue(enemyTemplate.Intelligence);
            enemy.Icon = enemyTemplate.SymbolDetails.Icon;
            enemy.CharacterSymbol = enemyTemplate.SymbolDetails.CharacterSymbol;
            enemy.CharacterColor = enemyTemplate.SymbolDetails.CharacterColor;
            enemy.SmileyMood = enemyTemplate.SymbolDetails.SmileyMood;
            enemy.SmileyAuraColor = enemyTemplate.SymbolDetails.SmileyAuraColor;
            enemy.SmileyBodyColor = enemyTemplate.SymbolDetails.SmileyBodyColor;
            enemy.SmileyLineColor = enemyTemplate.SymbolDetails.SmileyLineColor;

            enemy.BehaviorDetails = new BehaviorDetails();
            enemy.BehaviorDetails.PrimaryBehavior = _behaviorGenerator.GenerateBehavior(enemyTemplate.BehaviorDetails.PrimaryBehavior);
            enemy.BehaviorDetails.SecondaryBehavior = _behaviorGenerator.GenerateBehavior(enemyTemplate.BehaviorDetails.SecondaryBehavior);
            enemy.BehaviorDetails.SecondaryProbability = enemyTemplate.BehaviorDetails.SecondaryProbability;
            enemy.BehaviorDetails.SecondaryReason = enemyTemplate.BehaviorDetails.SecondaryReason;

            enemy.AttackAttributes = enemyTemplate.AttackAttributes.Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x))
                                                   .ToDictionary(x => x.RogueName);

            //Starting Consumables
            foreach (var consumableTemplate in enemyTemplate.StartingConsumables)
            {
                if (_randomSequenceGenerator.Get() > consumableTemplate.GenerationProbability)
                    continue;

                var template = (ConsumableTemplate)consumableTemplate.TheTemplate;
                if (template.IsUnique && template.HasBeenGenerated)
                    continue;

                var consumable = _itemGenerator.GenerateConsumable(template);

                enemy.Consumables.Add(consumable.Id, consumable);
            }

            //Starting Equipment
            foreach (var equipmentTemplate in enemyTemplate.StartingEquipment)
            {
                if (_randomSequenceGenerator.Get() > equipmentTemplate.GenerationProbability)
                    continue;

                var template = (EquipmentTemplate)equipmentTemplate.TheTemplate;

                if (template.IsUnique && template.HasBeenGenerated)
                    continue;

                var equipment = _itemGenerator.GenerateEquipment(template);

                enemy.Equipment.Add(equipment.Id, equipment);
            }

            enemyTemplate.HasBeenGenerated = true;
            return enemy;
        }
    }
}
