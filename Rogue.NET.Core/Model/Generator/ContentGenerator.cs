using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(IContentGenerator))]
    public class ContentGenerator : IContentGenerator
    {
        private readonly IRandomSequenceGenerator _randomSequenceGenerator;
        private readonly ICharacterGenerator _characterGenerator;
        private readonly IDoodadGenerator _doodadGenerator;
        private readonly IItemGenerator _itemGenerator;

        [ImportingConstructor]
        public ContentGenerator(
            IRandomSequenceGenerator randomSequenceGenerator,
            ICharacterGenerator characterGenerator,
            IDoodadGenerator doodadGenerator,
            IItemGenerator itemGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _characterGenerator = characterGenerator;
            _doodadGenerator = doodadGenerator;
            _itemGenerator = itemGenerator;
        }

        public IEnumerable<Level> CreateContents(
            IEnumerable<Level> levels, 
            ScenarioConfigurationContainer configurationContainer, 
            bool survivorMode)
        {
            var levelNumber = 1;
            var result = new List<Level>();

            for (int i=0;i<levels.Count();i++)
                result.Add(GenerateLevelContent(levels.ElementAt(i), configurationContainer, levelNumber++, survivorMode));

            return result;
        }

        private Level GenerateLevelContent(Level level, ScenarioConfigurationContainer configurationContainer, int levelNumber, bool survivorMode)
        {
            // Create lists to know what cells are free
            var rooms = level.Grid.GetRooms().ToList();
            var freeCells = level.Grid.GetCells().Select(x => x.Location).ToList();
            var freeRoomCells = freeCells.GroupBy(x => rooms.FindIndex(r => r.Contains(x)))
                                         .ToDictionary(x => x.Key, y => y.ToList());


            //must have for each level (Except the last one)
            if (levelNumber != configurationContainer.DungeonTemplate.NumberOfLevels)
            {
                var stairsDown = new DoodadNormal(DoodadNormalType.StairsDown, "Stairs Down", "");
                stairsDown.Location = GetRandomCell(freeCells);
                level.AddStairsDown(stairsDown);
            }

            //Stairs up - every level has one
            var stairsUp = new DoodadNormal(DoodadNormalType.StairsUp, "Stairs Up", "");
            stairsUp.Location = GetRandomCell(freeCells);
            level.AddStairsUp(stairsUp);

            // Applies to all levels
            GenerateEnemies(level, configurationContainer, levelNumber);
            GenerateItems(level, configurationContainer, levelNumber);
            GenerateDoodads(level, configurationContainer, levelNumber);

            // Add teleporter level content
            switch (level.Type)
            {
                case LayoutType.Teleport:
                    AddTeleporterLevelContent(level, configurationContainer, levelNumber, freeCells, freeRoomCells);
                    break;
                case LayoutType.TeleportRandom:
                    AddTeleportRandomLevelContent(level, configurationContainer, levelNumber, freeCells, freeRoomCells);
                    break;
            }

            //Every level has a save point if not in survivor mode
            if (!survivorMode)
            {
                var savePoint = new DoodadNormal(DoodadNormalType.SavePoint, "Save Point", "");
                savePoint.Location = GetRandomCell(freeCells);
                level.AddSavePoint(savePoint);
            }

            MapLevel(level, configurationContainer, levelNumber, freeCells, freeRoomCells);

            return level;
        }
        private void GenerateDoodads(Level level, ScenarioConfigurationContainer configurationContainer, int levelNumber)
        {
            foreach (var doodadTemplate in configurationContainer.DoodadTemplates)
            {
                if (!doodadTemplate.Level.Contains(levelNumber))
                    continue;

                if (doodadTemplate.IsUnique && doodadTemplate.HasBeenGenerated)
                    continue;

                int number = _randomSequenceGenerator.CalculateGenerationNumber(doodadTemplate.GenerationRate);

                // If objective item (and hasn't been generated) - create at least one
                for (var i = 0; i < number || (doodadTemplate.IsObjectiveItem && !doodadTemplate.HasBeenGenerated && i == 0); i++)
                {
                    level.AddContent(_doodadGenerator.GenerateDoodad(doodadTemplate));
                }
            }
        }
        private void GenerateEnemies(Level level, ScenarioConfigurationContainer configurationContainer, int levelNumber)
        {
            foreach (var enemyTemplate in configurationContainer.EnemyTemplates)
            {
                if (!enemyTemplate.Level.Contains(levelNumber))
                    continue;

                if (enemyTemplate.IsUnique && enemyTemplate.HasBeenGenerated)
                    continue;

                int number = _randomSequenceGenerator.CalculateGenerationNumber(enemyTemplate.GenerationRate);

                for (int i = 0; i < number || (enemyTemplate.IsObjectiveItem && !enemyTemplate.HasBeenGenerated && i == 0); i++)
                {
                    level.AddContent(_characterGenerator.GenerateEnemy(enemyTemplate));
                }
            }
        }
        private void GenerateItems(Level level, ScenarioConfigurationContainer configurationContainer, int levelNumber)
        {
            // Equipment for the level
            foreach (var template in configurationContainer.EquipmentTemplates)
            {
                if (!template.Level.Contains(levelNumber))
                    continue;

                if (template.IsUnique && template.HasBeenGenerated)
                    continue;

                int number = _randomSequenceGenerator.CalculateGenerationNumber(template.GenerationRate);

                for (int i = 0; i < number || (template.IsObjectiveItem && !template.HasBeenGenerated && i == 0); i++)
                {
                    level.AddContent(_itemGenerator.GenerateEquipment(template));
                }
            }

            // Consumables for the level
            foreach (var template in configurationContainer.ConsumableTemplates)
            {
                if (template.IsUnique && template.HasBeenGenerated)
                    continue;

                if (template.Level.Contains(levelNumber))
                {
                    int number = _randomSequenceGenerator.CalculateGenerationNumber(template.GenerationRate);

                    for (int i = 0; i < number || (template.IsObjectiveItem && !template.HasBeenGenerated && i == 0); i++)
                    {
                        level.AddContent(_itemGenerator.GenerateConsumable(template));
                    }
                }
            }
        }

        private void AddTeleporterLevelContent(Level level, ScenarioConfigurationContainer configurationContainer, int levelNumber, IList<CellPoint> freeCells, Dictionary<int, List<CellPoint>> freeRoomCells)
        {
            var rooms = level.Grid.GetRooms();

            // Connect rooms with teleporters sequentially to make sure can reach all rooms
            for (int i = 0; i < rooms.Length - 1; i++)
            {
                var teleport1 = new DoodadNormal(DoodadNormalType.Teleport1, "Teleport 1", "");
                var teleport2 = new DoodadNormal(DoodadNormalType.Teleport2, "Teleport 2", teleport1.Id);

                teleport1.Location = GetRandomCellInRoom(i, freeCells, freeRoomCells);
                teleport2.Location = GetRandomCellInRoom(i + 1, freeCells, freeRoomCells);

                teleport1.PairId = teleport2.Id;

                level.AddContent(teleport1);
                level.AddContent(teleport2);
            }

            var lastRoomTeleport = new DoodadNormal(DoodadNormalType.Teleport1, "Teleport 1", "");
            var firstRoomTeleport = new DoodadNormal(DoodadNormalType.Teleport2, "Teleport 2", lastRoomTeleport.Id);

            lastRoomTeleport.Location = GetRandomCellInRoom(rooms.Length - 1, freeCells, freeRoomCells);
            firstRoomTeleport.Location = GetRandomCellInRoom(0, freeCells, freeRoomCells);

            lastRoomTeleport.PairId = firstRoomTeleport.Id;

            level.AddContent(lastRoomTeleport);
            level.AddContent(firstRoomTeleport);

            //Add some extra ones (one per room)
            for (int i = 0; i < rooms.Length; i++)
            {
                var extraTeleport1 = new DoodadNormal(DoodadNormalType.Teleport1, "Teleport 1", "");
                var extraTeleport2 = new DoodadNormal(DoodadNormalType.Teleport2, "Teleport 2", extraTeleport1.Id);

                // Pick 2 random rooms
                extraTeleport1.Location = GetRandomCellInRoom(_randomSequenceGenerator.Get(0, rooms.Length), freeCells, freeRoomCells);
                extraTeleport2.Location = GetRandomCellInRoom(_randomSequenceGenerator.Get(0, rooms.Length), freeCells, freeRoomCells);

                extraTeleport1.PairId = extraTeleport2.Id;

                level.AddContent(extraTeleport1);
                level.AddContent(extraTeleport2);
            }
        }
        private void AddTeleportRandomLevelContent(Level level, ScenarioConfigurationContainer configurationContainer, int levelNumber, IList<CellPoint> freeCells, Dictionary<int, List<CellPoint>> freeRoomCells)
        {
            var rooms = level.Grid.GetRooms();

            // Random teleporters send character to random point in the level. Add
            // one per room
            for (int i = 0; i < rooms.Length; i++)
            {
                var doodad = new DoodadNormal(DoodadNormalType.TeleportRandom, "Random Teleporter", "");
                doodad.Location = GetRandomCellInRoom(i, freeCells, freeRoomCells);
                level.AddContent(doodad);
            }
        }
        private void AddPartyRoomContent(Level level, ScenarioConfigurationContainer configurationContainer, int levelNumber, IList<CellPoint> freeCells, Dictionary<int, List<CellPoint>> freeRoomCells)
        {
            var rooms = level.Grid.GetRooms();
            var partyRoomIndex = _randomSequenceGenerator.Get(0, rooms.Length);

            // Party room equipment - generate for each 
            foreach (var template in configurationContainer.EquipmentTemplates)
            {
                if (!template.Level.Contains(levelNumber))
                    continue;

                if (template.IsUnique && template.HasBeenGenerated)
                    continue;

                int number = _randomSequenceGenerator.CalculateGenerationNumber(template.GenerationRate);

                for (int i = 0; i < number || (template.IsObjectiveItem && !template.HasBeenGenerated && i == 0); i++)
                {
                    var location = GetRandomCellInRoom(partyRoomIndex, freeCells, freeRoomCells);

                    if (location != CellPoint.Empty)
                    {
                        var equipment = _itemGenerator.GenerateEquipment(template);
                        equipment.Location = location;
                        level.AddContent(equipment);
                    }
                }
            }

            // Party room consumables
            foreach (var template in configurationContainer.ConsumableTemplates)
            {
                if (template.IsUnique && template.HasBeenGenerated)
                    continue;

                if (template.Level.Contains(levelNumber))
                {
                    int number = _randomSequenceGenerator.CalculateGenerationNumber(template.GenerationRate);

                    for (int i = 0; i < number || (template.IsObjectiveItem && !template.HasBeenGenerated && i == 0); i++)
                    {
                        var location = GetRandomCellInRoom(partyRoomIndex, freeCells, freeRoomCells);

                        if (location != CellPoint.Empty)
                        {
                            var consumable = _itemGenerator.GenerateConsumable(template);
                            consumable.Location = location;
                            level.AddContent(consumable);
                        }
                    }
                }
            }
            // Party room enemies
            foreach (var enemyTemplate in configurationContainer.EnemyTemplates.Where(z => z.Level.Contains(levelNumber)))
            {
                if (enemyTemplate.IsUnique && enemyTemplate.HasBeenGenerated)
                    continue;

                int number = _randomSequenceGenerator.CalculateGenerationNumber(enemyTemplate.GenerationRate);

                for (int i = 0; i < number || (enemyTemplate.IsObjectiveItem && !enemyTemplate.HasBeenGenerated && i == 0); i++)
                {
                    var location = GetRandomCellInRoom(partyRoomIndex, freeCells, freeRoomCells);
                    if (location != CellPoint.Empty)
                    { 
                        var enemy = _characterGenerator.GenerateEnemy(enemyTemplate);
                        enemy.Location = location;
                        level.AddContent(enemy);
                    }
                }
            }
        }
        private void MapLevel(Level level, ScenarioConfigurationContainer configurationContainer, int levelNumber, IList<CellPoint> freeCells, Dictionary<int, List<CellPoint>> freeRoomCells)
        {
            var levelContents = level.GetContents();

            // Map Level Contents:  Set locations for each ScenarioObject. Removal of these can be
            // done based on the total length of the levelContents array - which will be altered during
            // the loop interally to the Level.
            for (int i= levelContents.Length - 1; i >= 0; i--)
            {
                var location = GetRandomCell(freeCells);

                // Entire grid is occupied
                if (location == CellPoint.Empty)
                    level.RemoveContent(levelContents[i]);

                // Already placed
                else if (levelContents[i] is DoodadNormal &&
                        (levelContents[i] as DoodadNormal).NormalType == DoodadNormalType.TeleportRandom)
                    continue;

                else
                    levelContents[i].Location = location;
            }

            // Create party room if there's a room to use and the rate is greater than U[0,1]

            if ((configurationContainer.DungeonTemplate.PartyRoomGenerationRate > _randomSequenceGenerator.Get()) &&
                (level.Grid.GetRoomGridHeight() * level.Grid.GetRoomGridWidth() > 0))
                AddPartyRoomContent(level, configurationContainer, levelNumber, freeCells, freeRoomCells);
        }

        private CellPoint GetRandomCell(IList<CellPoint> freeCells)
        {
            if (freeCells.Count == 0)
                return CellPoint.Empty;

            var randomIndex = _randomSequenceGenerator.Get(0, freeCells.Count);

            var nextCellPoint = freeCells[randomIndex];

            freeCells.RemoveAt(randomIndex);

            return nextCellPoint;
        }
        private CellPoint GetRandomCellInRoom(int roomIndex, IList<CellPoint> freeCells, Dictionary<int, List<CellPoint>> freeRoomCells)
        {
            var location = GetRandomCell(freeRoomCells[roomIndex]);

            if (location == CellPoint.Empty)
                return CellPoint.Empty;

            freeRoomCells[roomIndex].Remove(location);

            return location;
        }
    }
}
