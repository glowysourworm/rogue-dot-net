using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Common.Extension;
using Rogue.NET.ScenarioEditor.ViewModel.Difficulty;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model;

namespace Rogue.NET.ScenarioEditor.Service
{
    [Export(typeof(IScenarioDifficultyCalculationService))]
    public class ScenarioDifficultyCalculationService : IScenarioDifficultyCalculationService
    {
        readonly ICharacterCalculationService _characterCalculationService;

        [ImportingConstructor]
        public ScenarioDifficultyCalculationService(
            ICharacterCalculationService characterCalculationService)
        {
            _characterCalculationService = characterCalculationService;
        }

        // Enemy Attack Power = Enemy Attack - Player Defense
        public IEnumerable<IProjectedQuantityViewModel> CalculateEnemyAttackPower(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets,
            bool usePlayerStrengthAttributeEmphasis,
            bool includeAttackAttributes)
        {
            var playerLow = CreatePlayer(configuration.PlayerTemplate, true);
            var playerHigh = CreatePlayer(configuration.PlayerTemplate, false);

            return new List<IProjectedQuantityViewModel>(
                Enumerable.Range(1, configuration.DungeonTemplate.NumberOfLevels)
                    .Select((levelNumber) =>
                    {
                        playerLow = SimulatePlayer(playerLow, levelNumber, true, true, configuration, includedAssets,
                                                       usePlayerStrengthAttributeEmphasis ? AttributeEmphasis.Strength : AttributeEmphasis.Agility);

                        playerHigh = SimulatePlayer(playerHigh, levelNumber, false, true, configuration, includedAssets,
                                                        usePlayerStrengthAttributeEmphasis ? AttributeEmphasis.Strength : AttributeEmphasis.Agility);

                        // Select enemies whose level range overlaps this level and whose generation rate is
                        // greater than zero.
                        var enemies = configuration.EnemyTemplates
                                                   .Where(x => x.Level.Contains(levelNumber) &&
                                                               x.GenerationRate > 0 &&
                                                               includedAssets.Any(z => z.Id == x.Guid && z.Included));

                        var enemiesLow = enemies.Select(x => CreateEnemy(x, true));
                        var enemiesHigh = enemies.Select(x => CreateEnemy(x, false));

                        // Calculate all attack value ranges
                        var attackLow = enemiesLow.Any() ? enemiesLow.Min(x => x.GetAttack()) : 0;
                        var attackHigh = enemiesHigh.Any() ? enemiesHigh.Max(x => x.GetAttack()) : 0;
                                
                        var high = Math.Max((attackHigh - playerLow.GetDefense()), 0);
                        var low = Math.Max((attackLow - playerHigh.GetDefense()), 0);
                      
                        // Select High, Low, and Average
                        return new ProjectedQuantityViewModel()
                        {
                            High = high,
                            Low = low,
                            Average = (low + high) / 2.0D,
                            Level = levelNumber
                        };
                    }));
        }

        // Enemy HP
        public IEnumerable<IProjectedQuantityViewModel> CalculateEnemyHp(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets)
        {
            return new List<IProjectedQuantityViewModel>(
                Enumerable.Range(1, configuration.DungeonTemplate.NumberOfLevels)
                    .Select((levelNumber) =>
                    {
                        // Select enemies whose level range overlaps this level and whose generation rate is
                        // greater than zero.
                        var enemies = configuration.EnemyTemplates
                                                   .Where(x => x.Level.Contains(levelNumber) &&
                                                               x.GenerationRate > 0 &&
                                                               includedAssets.Any(z => z.Id == x.Guid && z.Included));

                        return new ProjectedQuantityViewModel()
                        {
                            High = enemies.Any() ? enemies.Max(x => x.Hp.High) : 0,
                            Low = enemies.Any() ? enemies.Min(x => x.Hp.Low) : 0,
                            Average = enemies.Any() ? enemies.Average(x => x.Hp.GetAverage()) : 0,
                            Level = levelNumber
                        };
                    }));
        }

        // Enemy Strength
        public IEnumerable<IProjectedQuantityViewModel> CalculateEnemyStrength(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets)
        {
            return new List<IProjectedQuantityViewModel>(
                Enumerable.Range(1, configuration.DungeonTemplate.NumberOfLevels)
                    .Select((levelNumber) =>
                    {
                        // Select enemies whose level range overlaps this level and whose generation rate is
                        // greater than zero.
                        var enemies = configuration.EnemyTemplates
                                                   .Where(x => x.Level.Contains(levelNumber) &&
                                                               x.GenerationRate > 0 &&
                                                               includedAssets.Any(z => z.Id == x.Guid && z.Included));

                        return new ProjectedQuantityViewModel()
                        {
                            High = enemies.Any() ? enemies.Max(x => x.Strength.High) : 0,
                            Low = enemies.Any() ? enemies.Min(x => x.Strength.Low) : 0,
                            Average = enemies.Any() ? enemies.Average(x => x.Strength.GetAverage()) : 0,
                            Level = levelNumber
                        };
                    }));
        }

        // Food Availability
        public IEnumerable<IProjectedQuantityViewModel> CalculateFoodAvailability(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets)
        {
            var playerLow = CreatePlayer(configuration.PlayerTemplate, true);
            var playerHigh = CreatePlayer(configuration.PlayerTemplate, false);

            return new List<IProjectedQuantityViewModel>(
                Enumerable.Range(1, configuration.DungeonTemplate.NumberOfLevels)
                    .Select((levelNumber) =>
                    {
                        playerLow = SimulatePlayer(playerLow, levelNumber, true, false, configuration, includedAssets, AttributeEmphasis.Agility);
                        playerHigh = SimulatePlayer(playerHigh, levelNumber, false, false, configuration, includedAssets, AttributeEmphasis.Agility);

                        var high = playerHigh.Consumables
                                            .Values
                                            .Where(x => x.SubType == ConsumableSubType.Food)
                                            .Sum(x => x.Spell.Effect.HungerRange.Low);

                        var low = playerLow.Consumables
                                            .Values
                                            .Where(x => x.SubType == ConsumableSubType.Food)
                                            .Sum(x => x.Spell.Effect.HungerRange.High);

                        return new ProjectedQuantityViewModel()
                        {
                            High = high,
                            Low = low,
                            Average = (low + high) / 2.0D,
                            Level = levelNumber
                        };
                    }));
        }

        // Food Consumption
        public IEnumerable<IProjectedQuantityViewModel> CalculateFoodConsumption(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets)
        {
            var projectedPathLengths = CalculateLayoutPathLength(configuration, includedAssets);

            return new List<IProjectedQuantityViewModel>(
                Enumerable.Range(1, configuration.DungeonTemplate.NumberOfLevels)
                    .Select((levelNumber) =>
                    {
                        var pathLength = projectedPathLengths.First(x => x.Level == levelNumber);

                        return new ProjectedQuantityViewModel()
                        {
                            Average = pathLength.Average * configuration.PlayerTemplate.FoodUsage.GetAverage(),
                            High = pathLength.High * configuration.PlayerTemplate.FoodUsage.High,
                            Low = pathLength.Low * configuration.PlayerTemplate.FoodUsage.Low,
                            Level = levelNumber
                        };
                    }));
        }

        // Player Attack Power = Player Attack - Enemy Defense
        public IEnumerable<IProjectedQuantityViewModel> CalculatePlayerAttackPower(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets,
            bool usePlayerStrengthAttributeEmphasis,
            bool includeAttackAttributes)
        {
            var playerLow = CreatePlayer(configuration.PlayerTemplate, true);
            var playerHigh = CreatePlayer(configuration.PlayerTemplate, false);

            return new List<IProjectedQuantityViewModel>(
                Enumerable.Range(1, configuration.DungeonTemplate.NumberOfLevels)
                    .Select((levelNumber) =>
                    {
                        playerLow = SimulatePlayer(playerLow, levelNumber, true, true, configuration, includedAssets,
                                                       usePlayerStrengthAttributeEmphasis ? AttributeEmphasis.Strength : AttributeEmphasis.Agility);

                        playerHigh = SimulatePlayer(playerHigh, levelNumber, false, true, configuration, includedAssets,
                                                        usePlayerStrengthAttributeEmphasis ? AttributeEmphasis.Strength : AttributeEmphasis.Agility);

                        // Select enemies whose level range overlaps this level and whose generation rate is
                        // greater than zero.
                        var enemiesLow = configuration.EnemyTemplates
                                                      .Where(x => x.Level.Contains(levelNumber) &&
                                                                  x.GenerationRate > 0 &&
                                                                  includedAssets.Any(z => z.Id == x.Guid && z.Included))
                                                      .Select(x => CreateEnemy(x, true));

                        var enemiesHigh = configuration.EnemyTemplates
                                                       .Where(x => x.Level.Contains(levelNumber) &&
                                                                   x.GenerationRate > 0 &&
                                                                   includedAssets.Any(z => z.Id == x.Guid && z.Included))
                                                       .Select(x => CreateEnemy(x, false));

                        // Calculate all base defense value ranges
                        var defenseLow = enemiesLow.Any() ? enemiesLow.Min(x => x.GetDefense()) : 0D;
                        var defenseHigh = enemiesHigh.Any() ? enemiesHigh.Max(x => x.GetDefense()) : 0D;

                        // Calculate Player attack power
                        var high = Math.Max((playerHigh.GetAttack() - defenseLow), 0);
                        var low = Math.Max((playerLow.GetAttack() - defenseHigh), 0);
                        
                        return new ProjectedQuantityViewModel()
                        {
                            High = high,
                            Low = low,
                            Average = (low + high) / 2.0D,
                            Level = levelNumber
                        };
                    }));
        }

        // Player Experience
        public IEnumerable<IProjectedQuantityViewModel> CalculatePlayerExperience(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets)
        {
            var playerLow = CreatePlayer(configuration.PlayerTemplate, true);
            var playerHigh = CreatePlayer(configuration.PlayerTemplate, false);

            return new List<IProjectedQuantityViewModel>(
              Enumerable.Range(1, configuration.DungeonTemplate.NumberOfLevels)
                        .Select((levelNumber) =>
                        {
                            playerLow = SimulatePlayer(playerLow, levelNumber, true, true, configuration, includedAssets,AttributeEmphasis.Agility);
                            playerHigh = SimulatePlayer(playerHigh, levelNumber, false, true, configuration, includedAssets, AttributeEmphasis.Agility);

                            return new ProjectedQuantityViewModel()
                            {
                                High = playerHigh.Experience,
                                Low = playerLow.Experience,
                                Average = (playerLow.Experience + playerHigh.Experience) / 2.0D,
                                Level = levelNumber
                            };
                        }));
        }

        // Player HP
        public IEnumerable<IProjectedQuantityViewModel> CalculatePlayerHp(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets,
            bool usePlayerStrengthAttributeEmphasis)
        {
            var playerLow = CreatePlayer(configuration.PlayerTemplate, true);
            var playerHigh = CreatePlayer(configuration.PlayerTemplate, false);

            return new List<IProjectedQuantityViewModel>(
              Enumerable.Range(1, configuration.DungeonTemplate.NumberOfLevels)
                        .Select((levelNumber) =>
                        {
                            playerLow = SimulatePlayer(playerLow, levelNumber, true, true, configuration, includedAssets, AttributeEmphasis.Agility);
                            playerHigh = SimulatePlayer(playerHigh, levelNumber, false, true, configuration, includedAssets, AttributeEmphasis.Agility);

                            return new ProjectedQuantityViewModel()
                            {
                                High = playerHigh.Hp,
                                Low = playerLow.Hp,
                                Average = (playerLow.Hp + playerHigh.Hp) / 2.0D,
                                Level = levelNumber
                            };
                        }));
        }

        // Player Strength
        public IEnumerable<IProjectedQuantityViewModel> CalculatePlayerStrength(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets,
            bool usePlayerStrengthAttributeEmphasis)
        {
            var playerLow = CreatePlayer(configuration.PlayerTemplate, true);
            var playerHigh = CreatePlayer(configuration.PlayerTemplate, false);

            return new List<IProjectedQuantityViewModel>(
              Enumerable.Range(1, configuration.DungeonTemplate.NumberOfLevels)
                        .Select((levelNumber) =>
                        {
                            playerLow = SimulatePlayer(playerLow, levelNumber, true, true, configuration, includedAssets, AttributeEmphasis.Agility);
                            playerHigh = SimulatePlayer(playerHigh, levelNumber, false, true, configuration, includedAssets, AttributeEmphasis.Agility);

                            return new ProjectedQuantityViewModel()
                            {
                                High = playerHigh.StrengthBase,
                                Low = playerLow.StrengthBase,
                                Average = (playerLow.StrengthBase + playerHigh.StrengthBase) / 2.0D,
                                Level = levelNumber
                            };
                        }));
        }

        // Player Level
        public IEnumerable<IProjectedQuantityViewModel> CalculatePlayerLevel(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets)
        {
            var playerLow = CreatePlayer(configuration.PlayerTemplate, true);
            var playerHigh = CreatePlayer(configuration.PlayerTemplate, false);

            return new List<IProjectedQuantityViewModel>(
              Enumerable.Range(1, configuration.DungeonTemplate.NumberOfLevels)
                        .Select((levelNumber) =>
                        {
                            playerLow = SimulatePlayer(playerLow, levelNumber, true, true, configuration, includedAssets, AttributeEmphasis.Agility);
                            playerHigh = SimulatePlayer(playerHigh, levelNumber, false, true, configuration, includedAssets, AttributeEmphasis.Agility);

                            return new ProjectedQuantityViewModel()
                            {
                                High = playerHigh.Level,
                                Low = playerLow.Level,
                                Average = (playerLow.Level + playerHigh.Level) / 2.0D,
                                Level = levelNumber
                            };
                        }));
        }
        
        // Player Hunger
        public IEnumerable<IProjectedQuantityViewModel> CalculateHungerCurve(
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets)
        {
            var playerLow = CreatePlayer(configuration.PlayerTemplate, true);
            var playerHigh = CreatePlayer(configuration.PlayerTemplate, false);

            return new List<IProjectedQuantityViewModel>(
              Enumerable.Range(1, configuration.DungeonTemplate.NumberOfLevels)
                        .Select((levelNumber) =>
                        {
                            playerLow = SimulatePlayer(playerLow, levelNumber, true, true, configuration, includedAssets, AttributeEmphasis.Agility);
                            playerHigh = SimulatePlayer(playerHigh, levelNumber, false, true, configuration, includedAssets, AttributeEmphasis.Agility);

                            return new ProjectedQuantityViewModel()
                            {
                                High = playerHigh.Hunger,
                                Low = playerLow.Hunger,
                                Average = (playerLow.Hunger + playerHigh.Hunger) / 2.0D,
                                Level = levelNumber
                            };
                        }));
        }

        // Layout Path Length
        public IEnumerable<IProjectedQuantityViewModel> CalculateLayoutPathLength(
                ScenarioConfigurationContainerViewModel configuration,
                IEnumerable<IDifficultyAssetViewModel> includedAssets)
        {
            return new List<IProjectedQuantityViewModel>(
                Enumerable.Range(1, configuration.DungeonTemplate.NumberOfLevels)
                    .Select((levelNumber) =>
                    {
                        // Get layout templates whose level range includes this level
                        var pathLengths = configuration.DungeonTemplate
                                                           .LayoutTemplates
                                                           .Where(x => x.Level.Contains(levelNumber))
                                                           .Select(template =>
                                                           {
                                                               switch (template.Type)
                                                               {
                                                                   default:
                                                                   case LayoutType.Normal:
                                                                   case LayoutType.Teleport:
                                                                   case LayoutType.TeleportRandom:
                                                                   case LayoutType.Hall:
                                                                   case LayoutType.BigRoom:

                                                                       // Measure  = # of traversals * length of traversal for
                                                                       //            a single pass only * 4;
                                                                       return template.RoomDivCellHeight * template.NumberRoomRows *
                                                                              template.NumberRoomCols * 4;
                                                                   case LayoutType.Maze:

                                                                       // Measure = Made up :) 
                                                                       return 100 * template.NumberRoomCols * template.RoomDivCellWidth *
                                                                                    template.NumberRoomRows * template.RoomDivCellHeight;
                                                               }
                                                           });



                        return new ProjectedQuantityViewModel()
                        {
                            Average = pathLengths.Average(),
                            High = pathLengths.Max(),
                            Low = pathLengths.Min(),
                            Level = levelNumber
                        };
                    }));
        }

        /// <summary>
        /// Simulates player working their way through the specified level. Use player = null for creating a new one.
        /// </summary>
        private Player SimulatePlayer(
            Player player,
            int level,
            bool simulateLow, // false for high
            bool simulateEating,
            ScenarioConfigurationContainerViewModel configuration,
            IEnumerable<IDifficultyAssetViewModel> includedAssets,
            AttributeEmphasis attributeEmphasis)
        {
            // Create a new player to use for the simulation
            if (player == null)
                player = CreatePlayer(configuration.PlayerTemplate, simulateLow);

            // Calculate the projected path length for this level
            var projectedPathLengths = CalculateLayoutPathLength(configuration, includedAssets);
            var projectedPathLength = projectedPathLengths.First(x => x.Level == level);

            // Simulate a player's traversal the level
                       
            // Select content that WILL be generated (And are included assets)
            var equipment = configuration.EquipmentTemplates
                                        .Where(x => x.Level.Contains(level) &&
                                                    includedAssets.Any(z => z.Id == x.Guid && z.Included));

            var consumables = configuration.ConsumableTemplates
                                            .Where(x => x.Level.Contains(level) &&
                                                        includedAssets.Any(z => z.Id == x.Guid && z.Included));

            var enemies = configuration.EnemyTemplates
                                        .Where(x => x.Level.Contains(level) &&
                                                    includedAssets.Any(z => z.Id == x.Guid && z.Included));

            // Calculate a LOW and HIGH generation number and clone items for player
            var generatedEnemies = simulateLow ? enemies.SelectMany(x => x.TransformMany(z => z, (int)x.GenerationRate))
                                                : enemies.SelectMany(x => x.TransformMany(z => z, (int)x.GenerationRate + (x.GenerationRate % 1 > 0 ? 1 : 0)));

            var generatedEnemyEquipment = simulateLow ? generatedEnemies.SelectMany(x => x.StartingEquipment.Where(z => z.GenerationProbability == 1).Select(z => z.TheTemplate))
                                                      : generatedEnemies.SelectMany(x => x.StartingEquipment.Where(z => z.GenerationProbability > 0).Select(z => z.TheTemplate));

            var generatedEnemyConsumables = simulateLow ? generatedEnemies.SelectMany(x => x.StartingConsumables.Where(z => z.GenerationProbability == 1).Select(z => z.TheTemplate))
                                                        : generatedEnemies.SelectMany(x => x.StartingConsumables.Where(z => z.GenerationProbability > 0).Select(z => z.TheTemplate));

            // Calculate total item collections
            var generatedEquipment = simulateLow ? equipment.SelectMany(x => x.TransformMany(z => z, (int)x.GenerationRate))
                                                 : equipment.SelectMany(x => x.TransformMany(z => z, (int)x.GenerationRate + (x.GenerationRate % 1 > 0 ? 1 : 0)));

            var generatedConsumables = simulateLow ? consumables.SelectMany(x => x.TransformMany(z => z, (int)x.GenerationRate))
                                                    : consumables.SelectMany(x => x.TransformMany(z => z, (int)x.GenerationRate + (x.GenerationRate % 1 > 0 ? 1 : 0)));

            var totalEquipment = generatedEquipment.Union(generatedEnemyEquipment);
            var totalConsumables = generatedConsumables.Union(generatedEnemyConsumables);

            // Simulate player advancement assuming they'll retrieve all items and slay all enemies. 

            // 0) Simulate player walking through the level
            player.Hunger += (simulateLow ? projectedPathLength.Low : projectedPathLength.High) * player.FoodUsagePerTurnBase;

            // 1) Simulate slaying all enemies to collect experience
            player.Experience += generatedEnemies.Sum(x => simulateLow ? x.ExperienceGiven.Low : x.ExperienceGiven.High);

            // 2) Simulate level gains for the player
            while (player.Experience >= PlayerCalculator.CalculateExperienceNext(player.Level))
            {
                var simulatedRandom = simulateLow ? 0 : 1.0D;

                player.Hp += PlayerCalculator.CalculateHpGain(simulatedRandom * player.StrengthBase);
                player.Mp += PlayerCalculator.CalculateMpGain(simulatedRandom * player.IntelligenceBase);

                player.StrengthBase += PlayerCalculator.CalculateStrengthGain(simulatedRandom, attributeEmphasis == AttributeEmphasis.Strength);
                player.IntelligenceBase += PlayerCalculator.CalculateIntelligenceGain(simulatedRandom, attributeEmphasis == AttributeEmphasis.Intelligence);
                player.AgilityBase += PlayerCalculator.CalculateAgilityGain(simulatedRandom, attributeEmphasis == AttributeEmphasis.Agility);

                player.Level++;
            }

            // 3) Add items to player inventory (IGNORING HAUL).
            totalConsumables.ForEach(x =>
            {
                var theConsumable = CreateConsumable(x);

                player.Consumables.Add(theConsumable.Id, theConsumable);
            });
            totalEquipment.ForEach(x =>
            {
                var theEquipment = CreateEquipment(x, simulateLow, false);

                player.Equipment.Add(theEquipment.Id, theEquipment);
            });

            // 4) Equip best set of player items (Order by descending attack value -> then by "weapons first")
            player.Equipment.ForEach(x => x.Value.IsEquipped = false);
            player.Equipment
                .Where(x => x.Value.LevelRequired <= player.Level)
                .OrderByDescending(x => x.Value.Class * x.Value.Quality)
                .OrderBy(x => (int)x.Value.Type)
                .ForEach(x =>
            {
                switch (x.Value.Type)
                {
                    case EquipmentType.Armor:
                    case EquipmentType.Shoulder:
                    case EquipmentType.Belt:
                    case EquipmentType.Helmet:
                    case EquipmentType.Amulet:
                    case EquipmentType.Boots:
                    case EquipmentType.Gauntlets:
                    case EquipmentType.Orb:
                        x.Value.IsEquipped = !player.Equipment.Any(z => z.Value.Type == x.Value.Type &&
                                                                        z.Value.IsEquipped);
                        break;
                    case EquipmentType.OneHandedMeleeWeapon:
                    case EquipmentType.Shield:
                        x.Value.IsEquipped = (player.Equipment.Count(z => (z.Value.Type == EquipmentType.OneHandedMeleeWeapon ||
                                                                            z.Value.Type == EquipmentType.Shield) &&
                                                                            z.Value.IsEquipped) +
                                            player.Equipment.Count(z => (z.Value.Type == EquipmentType.TwoHandedMeleeWeapon ||
                                                                            z.Value.Type == EquipmentType.RangeWeapon) &&
                                                                            z.Value.IsEquipped) * 2) < 2;
                        break;
                    case EquipmentType.TwoHandedMeleeWeapon:
                    case EquipmentType.RangeWeapon:
                        x.Value.IsEquipped = player.Equipment.Count(z => (z.Value.Type == EquipmentType.OneHandedMeleeWeapon ||
                                                                        z.Value.Type == EquipmentType.Shield ||
                                                                        z.Value.Type == EquipmentType.TwoHandedMeleeWeapon ||
                                                                        z.Value.Type == EquipmentType.RangeWeapon) &&
                                                                        z.Value.IsEquipped) == 0;
                        break;
                    default:
                        break;
                }
            });

            // 5) Feed player until Hunger drops below 100
            while (player.Consumables.Values.Any(x => x.SubType == ConsumableSubType.Food &&
                                                      x.LevelRequired <= player.Level &&
                                                      x.HasSpell &&
                                                      x.Spell.Type == AlterationType.PermanentSource &&
                                                      x.Spell.Effect != null &&                                                      
                                                      x.Spell.Effect.HungerRange.Low < 0 &&
                                                      x.Spell.Effect.HungerRange.High < 0) &&
                                                      player.Hunger > 100 &&
                                                      simulateEating)
            {
                // Order by increasing hunger to avoid consumables with hunger > 0
                var foodItem = player.Consumables
                                     .Values
                                     .OrderBy(x => x.Spell.Effect.HungerRange.Low)
                                     .First(x => x.SubType == ConsumableSubType.Food);

                // Simulate using the food item
                player.Hunger -= simulateLow ? foodItem.Spell.Effect.HungerRange.High :
                                               foodItem.Spell.Effect.HungerRange.Low;

                // Remove or decrement use counter
                switch (foodItem.Type)
                {
                    case ConsumableType.OneUse:
                        player.Consumables.Remove(foodItem.Id);
                        break;
                    case ConsumableType.MultipleUses:
                        foodItem.Uses--;
                        break;
                    case ConsumableType.UnlimitedUses:
                        break;
                }
            }

            return player;
        }

        #region Cloning Methods
        private Player CreatePlayer(PlayerTemplateViewModel template, bool simulateLow)
        {
            var player = new Player();

            player.StrengthBase = simulateLow ? template.Strength.Low : template.Strength.High;
            player.AgilityBase = simulateLow ? template.Agility.Low : template.Agility.High;
            player.IntelligenceBase = simulateLow ? template.Intelligence.Low : template.Intelligence.High;
            player.Hp = simulateLow ? template.Hp.Low : template.Hp.High;
            player.Mp = simulateLow ? template.Mp.Low : template.Mp.High;

            return player;
        }
        private Consumable CreateConsumable(ConsumableTemplateViewModel template)
        {
            // COPYING ONLY FOOD RELATED PARAMETERS
            var consumable = new Consumable();
            consumable.Type = template.Type;
            consumable.SubType = template.SubType;
            consumable.LevelRequired = template.LevelRequired;

            if (consumable.HasSpell)
            {
                consumable.Spell.Effect.HungerRange.Low = template.SpellTemplate.Effect.HungerRange.Low;
                consumable.Spell.Effect.HungerRange.High = template.SpellTemplate.Effect.HungerRange.High;
            }

            return consumable;
        }
        private Equipment CreateEquipment(EquipmentTemplateViewModel template, bool simulateLow, bool equip)
        {
            // COPYING ONLY PARAMETERS USED IN BASIC COMBAT
            var equipment = new Equipment();

            equipment.IsEquipped = equip;
            equipment.Class = simulateLow ? template.Class.Low : template.Class.High;
            equipment.Quality = simulateLow ? template.Quality.Low : template.Quality.High;
            equipment.LevelRequired = template.LevelRequired;
            equipment.Type = template.Type;

            // Adding on Attack Attributes
            equipment.AttackAttributes = template.AttackAttributes.Select(x => new AttackAttribute()
            {
                RogueName = x.Name,
                Attack = simulateLow ? x.Attack.Low : x.Attack.High,
                Resistance = simulateLow ? x.Resistance.Low : x.Resistance.High,

            }).ToList();

            return equipment;
        }
        private Enemy CreateEnemy(EnemyTemplateViewModel template, bool simulateLow)
        {
            return new Enemy()
            {
                RogueName = template.Name,
                AgilityBase = simulateLow ? template.Agility.Low : template.Agility.High,
                StrengthBase = simulateLow ? template.Strength.Low : template.Strength.High,
                IntelligenceBase = simulateLow ? template.Intelligence.Low : template.Intelligence.High,
                Equipment = template.StartingEquipment
                                    .Where(x => (simulateLow ? x.GenerationProbability >= 1 : x.GenerationProbability > 0))
                                    .Select(x => CreateEquipment(x.TheTemplate, simulateLow, x.EquipOnStartup))
                                    .ToDictionary(x => x.Id),
                Consumables = template.StartingConsumables
                                    .Where(x => (simulateLow ? x.GenerationProbability >= 1 : x.GenerationProbability > 0))
                                    .Select(x => CreateConsumable(x.TheTemplate))
                                    .ToDictionary(x => x.Id),
                AttackAttributes = template.AttackAttributes
                                           .Select(x => new AttackAttribute()
                                           {
                                               RogueName = x.Name,
                                               Attack = simulateLow ? x.Attack.Low : x.Attack.High,
                                               Resistance = simulateLow ? x.Resistance.Low : x.Resistance.High,
                                               AppliesToIntelligenceBasedCombat = x.AppliesToIntelligenceBasedCombat,
                                               AppliesToStrengthBasedCombat = x.AppliesToStrengthBasedCombat,
                                               ScaledByIntelligence = x.ScaledByIntelligence,
                                               ScaledByStrength = x.ScaledByStrength
                                           })
                                           .ToDictionary(x => x.RogueName)
            };
        }
        private IEnumerable<AttackAttributeTemplate> CreateAttackAttributes(ScenarioConfigurationContainerViewModel configuration)
        {
            return configuration.AttackAttributes.Select(x => new AttackAttributeTemplate()
            {
                Name = x.Name,
                Attack = new Range<double>(0,0),
                Resistance = new Range<double>(0,0),
                AppliesToIntelligenceBasedCombat = x.AppliesToIntelligenceBasedCombat,
                AppliesToStrengthBasedCombat = x.AppliesToStrengthBasedCombat,
                ScaledByStrength = x.ScaledByStrength,
                ScaledByIntelligence = x.ScaledByIntelligence
            });
        }
        #endregion

        #region IEqualityComparer Sub-Classes
        public class EquipmentTemplateViewModelComparer : IEqualityComparer<EquipmentTemplateViewModel>
        {
            public bool Equals(EquipmentTemplateViewModel x, EquipmentTemplateViewModel y)
            {
                return x.Guid.Equals(y.Guid);
            }

            public int GetHashCode(EquipmentTemplateViewModel obj)
            {
                return obj.Guid.GetHashCode();
            }
        }
        public class ConsumableTemplateViewModelComparer : IEqualityComparer<ConsumableTemplateViewModel>
        {
            public bool Equals(ConsumableTemplateViewModel x, ConsumableTemplateViewModel y)
            {
                return x.Guid.Equals(y.Guid);
            }

            public int GetHashCode(ConsumableTemplateViewModel obj)
            {
                return obj.Guid.GetHashCode();
            }
        }
        #endregion
    }
}
