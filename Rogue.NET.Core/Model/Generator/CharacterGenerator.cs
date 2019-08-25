using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;
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
        private readonly IAnimationGenerator _animationGenerator;
        private readonly IAlterationGenerator _alterationGenerator;

        [ImportingConstructor]
        public CharacterGenerator(
            IRandomSequenceGenerator randomSequenceGenerator, 
            IAttackAttributeGenerator attackAttributeGenerator,
            ISkillSetGenerator skillSetGenerator,
            IBehaviorGenerator behaviorGenerator,
            IItemGenerator itemGenerator,
            IAnimationGenerator animationGenerator,
            IAlterationGenerator alterationGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;
            _skillSetGenerator = skillSetGenerator;
            _behaviorGenerator = behaviorGenerator;
            _itemGenerator = itemGenerator;
            _animationGenerator = animationGenerator;
            _alterationGenerator = alterationGenerator;
        }

        public Player GeneratePlayer(PlayerTemplate playerTemplate, string characterClassName, IEnumerable<CharacterClass> characterClasses, IEnumerable<AttackAttribute> scenarioAttributes)
        {
            var player = new Player();
            player.FoodUsagePerTurnBase = _randomSequenceGenerator.GetRandomValue(playerTemplate.FoodUsage);
            player.IntelligenceBase = _randomSequenceGenerator.GetRandomValue(playerTemplate.Intelligence);
            player.StrengthBase = _randomSequenceGenerator.GetRandomValue(playerTemplate.Strength);
            player.AgilityBase = _randomSequenceGenerator.GetRandomValue(playerTemplate.Agility);
            player.SpeedBase = _randomSequenceGenerator.GetRandomValue(playerTemplate.Speed);
            player.LightRadiusBase = _randomSequenceGenerator.GetRandomValue(playerTemplate.LightRadius);
            player.HpMax = _randomSequenceGenerator.GetRandomValue(playerTemplate.Hp);
            player.MpMax = _randomSequenceGenerator.GetRandomValue(playerTemplate.Mp);
            player.Hp = player.HpMax;
            player.Mp = player.MpMax;

            player.Icon = playerTemplate.SymbolDetails.Icon;
            player.CharacterSymbol = playerTemplate.SymbolDetails.CharacterSymbol;
            player.CharacterColor = playerTemplate.SymbolDetails.CharacterColor;
            player.SmileyMood = playerTemplate.SymbolDetails.SmileyMood;
            player.SmileyLightRadiusColor = playerTemplate.SymbolDetails.SmileyAuraColor;
            player.SmileyBodyColor = playerTemplate.SymbolDetails.SmileyBodyColor;
            player.SmileyLineColor = playerTemplate.SymbolDetails.SmileyLineColor;
            player.SymbolType = playerTemplate.SymbolDetails.Type;

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
                    var consumable = _itemGenerator.GenerateConsumable(consumableTemplate, characterClasses);
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
                    // Generate the Equipment
                    var equipment = _itemGenerator.GenerateEquipment(equipmentTemplate, characterClasses);

                    // Set Equipped
                    equipment.IsEquipped = template.EquipOnStartup;

                    if (equipment.IsEquipped)
                    {
                        if (equipment.HasEquipAlteration)
                            player.Alteration.Apply(_alterationGenerator.GenerateAlteration(equipment.EquipAlteration));

                        if (equipment.HasCurseAlteration)
                            player.Alteration.Apply(_alterationGenerator.GenerateAlteration(equipment.CurseAlteration));
                    }


                    player.Equipment.Add(equipment.Id, equipment);
                }
            }

            //Starting Skills
            player.SkillSets = playerTemplate.Skills
                                             .Select(x => _skillSetGenerator.GenerateSkillSet(x, characterClasses))
                                             .ToList();

            // Character Class
            if (!string.IsNullOrEmpty(characterClassName))
            {
                var characterClass = characterClasses.First(x => x.RogueName == characterClassName);

                // Start with chosen character class
                player.Alteration = new CharacterAlteration(characterClass, scenarioAttributes);
            }

            return player;
        }
        public Enemy GenerateEnemy(EnemyTemplate enemyTemplate, IEnumerable<CharacterClass> characterClasses, IEnumerable<AttackAttribute> scenarioAttributes)
        {
            if (enemyTemplate.IsUnique && enemyTemplate.HasBeenGenerated)
                throw new Exception("Trying to generate unique enemy twice");

            var enemy = new Enemy();
            enemy.AgilityBase = _randomSequenceGenerator.GetRandomValue(enemyTemplate.Agility);
            enemy.ExperienceGiven = _randomSequenceGenerator.GetRandomValue(enemyTemplate.ExperienceGiven);
            enemy.Hp = _randomSequenceGenerator.GetRandomValue(enemyTemplate.Hp);
            enemy.Mp = _randomSequenceGenerator.GetRandomValue(enemyTemplate.Mp);
            enemy.MpMax = enemy.Mp;
            enemy.HpMax = enemy.Hp;
            enemy.RogueName = enemyTemplate.Name;
            enemy.StrengthBase = _randomSequenceGenerator.GetRandomValue(enemyTemplate.Strength);
            enemy.IntelligenceBase = _randomSequenceGenerator.GetRandomValue(enemyTemplate.Intelligence);
            enemy.SpeedBase = _randomSequenceGenerator.GetRandomValue(enemyTemplate.Speed);
            enemy.LightRadiusBase = _randomSequenceGenerator.GetRandomValue(enemyTemplate.LightRadius);
            enemy.Icon = enemyTemplate.SymbolDetails.Icon;
            enemy.CharacterSymbol = enemyTemplate.SymbolDetails.CharacterSymbol;
            enemy.CharacterColor = enemyTemplate.SymbolDetails.CharacterColor;
            enemy.SmileyMood = enemyTemplate.SymbolDetails.SmileyMood;
            enemy.SmileyLightRadiusColor = enemyTemplate.SymbolDetails.SmileyAuraColor;
            enemy.SmileyBodyColor = enemyTemplate.SymbolDetails.SmileyBodyColor;
            enemy.SmileyLineColor = enemyTemplate.SymbolDetails.SmileyLineColor;
            enemy.SymbolType = enemyTemplate.SymbolDetails.Type;

            // Permanent Invisibility Flag
            enemy.IsInvisible = enemyTemplate.IsInvisible;

            enemy.BehaviorDetails = new BehaviorDetails();

            // Create Behavior "State Machine"
            foreach (var behaviorTemplate in enemyTemplate.BehaviorDetails.Behaviors)
                enemy.BehaviorDetails.Behaviors.Add(_behaviorGenerator.GenerateBehavior(behaviorTemplate));

            enemy.BehaviorDetails.CanOpenDoors = enemyTemplate.BehaviorDetails.CanOpenDoors;
            enemy.BehaviorDetails.CounterAttackProbability = enemyTemplate.BehaviorDetails.CounterAttackProbability;
            enemy.BehaviorDetails.CriticalRatio = enemyTemplate.BehaviorDetails.CriticalRatio;
            enemy.BehaviorDetails.DisengageRadius = enemyTemplate.BehaviorDetails.DisengageRadius;
            enemy.BehaviorDetails.EngageRadius = enemyTemplate.BehaviorDetails.EngageRadius;
            enemy.BehaviorDetails.RandomizerTurnCount = enemyTemplate.BehaviorDetails.RandomizerTurnCount;
            enemy.BehaviorDetails.UseRandomizer = enemyTemplate.BehaviorDetails.UseRandomizer;

            enemy.AttackAttributes = enemyTemplate.AttackAttributes.Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x))
                                                   .ToDictionary(x => x.RogueName);

            enemy.DeathAnimation = _animationGenerator.GenerateAnimationGroup(enemyTemplate.DeathAnimationGroup);

            //Starting Consumables
            foreach (var consumableTemplate in enemyTemplate.StartingConsumables)
            {
                if (_randomSequenceGenerator.Get() > consumableTemplate.GenerationProbability)
                    continue;

                var template = (ConsumableTemplate)consumableTemplate.TheTemplate;
                if (template.IsUnique && template.HasBeenGenerated)
                    continue;

                var consumable = _itemGenerator.GenerateConsumable(template, characterClasses);

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

                var equipment = _itemGenerator.GenerateEquipment(template, characterClasses);

                // Equip on Startup
                if (equipmentTemplate.EquipOnStartup)
                {
                    // Set Equipped
                    equipment.IsEquipped = true;

                    // Set any Alterations
                    if (equipment.HasEquipAlteration)
                        enemy.Alteration.Apply(_alterationGenerator.GenerateAlteration(equipment.EquipAlteration));

                    if (equipment.HasCurseAlteration)
                        enemy.Alteration.Apply(_alterationGenerator.GenerateAlteration(equipment.CurseAlteration));
                }

                enemy.Equipment.Add(equipment.Id, equipment);
            }

            enemyTemplate.HasBeenGenerated = true;
            return enemy;
        }
    }
}
