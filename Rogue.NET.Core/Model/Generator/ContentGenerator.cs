using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System;
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
            IEnumerable<Religion> religions,
            IEnumerable<AttackAttribute> scenarioAttributes,
            bool survivorMode)
        {
            var levelNumber = 1;
            var result = new List<Level>();

            for (int i=0;i<levels.Count();i++)
                result.Add(GenerateLevelContent(levels.ElementAt(i), configurationContainer, religions, scenarioAttributes, levelNumber++, survivorMode));

            return result;
        }

        private Level GenerateLevelContent(Level level, ScenarioConfigurationContainer configurationContainer, IEnumerable<Religion> religions, IEnumerable<AttackAttribute> scenarioAttributes, int levelNumber, bool survivorMode)
        {
            // Create lists to know what cells are free
            var rooms = level.Grid.Rooms.ToList();
            var freeCells = level.Grid.GetCells().Select(x => x.Location).ToList();
            var freeRoomCells = level.Grid.Rooms.ToDictionary(room => room, room => room.Cells.ToList());

            // NOTE*** ADD MAPPED CONTENT FIRST - BUT MUST IGNORE DURING THE MAPPING PHASE. THIS INCLUDES
            //         ANY NORMAL DOODADS

            //must have for each level (Except the last one) (MAPPED)
            if (levelNumber != configurationContainer.DungeonTemplate.NumberOfLevels)
            {
                var stairsDown = new DoodadNormal(DoodadNormalType.StairsDown, ModelConstants.DoodadStairsDownRogueName, "");
                stairsDown.Location = GetRandomCell(false, null, freeCells, freeRoomCells);
                level.AddStairsDown(stairsDown);
            }

            //Stairs up - every level has one - (MAPPED)
            var stairsUp = new DoodadNormal(DoodadNormalType.StairsUp, ModelConstants.DoodadStairsUpRogueName, "");
            stairsUp.Location = GetRandomCell(false, null, freeCells, freeRoomCells);
            level.AddStairsUp(stairsUp);

            // Add teleporter level content - (MAPPED)
            if ((level.Type == LayoutType.ConnectedRectangularRooms ||
                 level.Type == LayoutType.ConnectedCellularAutomata) &&
                level.ConnectionType == LayoutConnectionType.Teleporter)
                AddTeleporterLevelContent(level, configurationContainer, levelNumber, freeCells, freeRoomCells);

            if ((level.Type == LayoutType.ConnectedRectangularRooms ||
                 level.Type == LayoutType.ConnectedCellularAutomata) &&
                level.ConnectionType == LayoutConnectionType.TeleporterRandom)
                AddTeleportRandomLevelContent(level, configurationContainer, levelNumber, freeCells, freeRoomCells);

            // Every level has a save point if not in survivor mode - (MAPPED)
            if (!survivorMode)
            {
                var savePoint = new DoodadNormal(DoodadNormalType.SavePoint, ModelConstants.DoodadSavePointRogueName, "");
                savePoint.Location = GetRandomCell(false, null, freeCells, freeRoomCells);
                level.AddSavePoint(savePoint);
            }

            // Applies to all levels - (UNMAPPED)
            GenerateEnemies(level, configurationContainer, religions, scenarioAttributes, levelNumber);
            GenerateItems(level, configurationContainer, levelNumber, religions);
            GenerateDoodads(level, configurationContainer, levelNumber, religions);

            MapLevel(level, configurationContainer, religions, scenarioAttributes, levelNumber, freeCells, freeRoomCells);

            return level;
        }
        private void GenerateDoodads(Level level, ScenarioConfigurationContainer configurationContainer, int levelNumber, IEnumerable<Religion> religions)
        {
            foreach (var doodadTemplate in configurationContainer.DoodadTemplates)
            {
                if (!doodadTemplate.Level.Contains(levelNumber))
                    continue;

                if (doodadTemplate.IsUnique && doodadTemplate.HasBeenGenerated)
                    continue;

                int number = _randomSequenceGenerator.CalculateGenerationNumber(doodadTemplate.GenerationRate);

                // If objective item (and hasn't been generated) - create at least one
                for (int i = 0; (i < number || (doodadTemplate.IsObjectiveItem && !doodadTemplate.HasBeenGenerated))
                                            && !(doodadTemplate.IsUnique && doodadTemplate.HasBeenGenerated); i++)
                {
                    level.AddContent(_doodadGenerator.GenerateDoodad(doodadTemplate, religions));
                }
            }
        }
        private void GenerateEnemies(Level level, ScenarioConfigurationContainer configurationContainer, IEnumerable<Religion> religions, IEnumerable<AttackAttribute> scenarioAttributes, int levelNumber)
        {
            foreach (var enemyTemplate in configurationContainer.EnemyTemplates)
            {
                if (!enemyTemplate.Level.Contains(levelNumber))
                    continue;

                if (enemyTemplate.IsUnique && enemyTemplate.HasBeenGenerated)
                    continue;

                int number = _randomSequenceGenerator.CalculateGenerationNumber(enemyTemplate.GenerationRate);

                for (int i = 0; (i < number || (enemyTemplate.IsObjectiveItem && !enemyTemplate.HasBeenGenerated))
                                            && !(enemyTemplate.IsUnique && enemyTemplate.HasBeenGenerated); i++)
                {
                    level.AddContent(_characterGenerator.GenerateEnemy(enemyTemplate, religions, scenarioAttributes));
                }
            }
        }
        private void GenerateItems(Level level, ScenarioConfigurationContainer configurationContainer, int levelNumber, IEnumerable<Religion> religions)
        {
            // Equipment for the level
            foreach (var template in configurationContainer.EquipmentTemplates)
            {
                if (!template.Level.Contains(levelNumber))
                    continue;

                if (template.IsUnique && template.HasBeenGenerated)
                    continue;

                int number = _randomSequenceGenerator.CalculateGenerationNumber(template.GenerationRate);

                for (int i = 0; (i < number || (template.IsObjectiveItem && !template.HasBeenGenerated))
                                            && !(template.IsUnique && template.HasBeenGenerated); i++)
                {
                    level.AddContent(_itemGenerator.GenerateEquipment(template, religions));
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

                    for (int i = 0; (i < number || (template.IsObjectiveItem && !template.HasBeenGenerated))
                                                && !(template.IsUnique && template.HasBeenGenerated); i++)
                    {
                        level.AddContent(_itemGenerator.GenerateConsumable(template, religions));
                    }
                }
            }
        }

        private void AddTeleporterLevelContent(Level level, ScenarioConfigurationContainer configurationContainer, int levelNumber, IList<CellPoint> freeCells, Dictionary<Room, List<CellPoint>> freeRoomCells)
        {
            var rooms = level.Grid.Rooms.ToList();

            // Connect rooms with teleporters sequentially to make sure can reach all rooms
            for (int i = 0; i < rooms.Count - 1; i++)
            {
                var teleport1 = new DoodadNormal(DoodadNormalType.Teleport1, ModelConstants.DoodadTeleporterARogueName, "");
                var teleport2 = new DoodadNormal(DoodadNormalType.Teleport2, ModelConstants.DoodadTeleporterBRogueName, teleport1.Id);

                var location1 = GetRandomCell(true, rooms.ElementAt(i), freeCells, freeRoomCells);
                var location2 = GetRandomCell(true, rooms.ElementAt(i + 1), freeCells, freeRoomCells);

                if (location1 == CellPoint.Empty)
                    throw new Exception("Trying to place teleporter but ran out of room!");

                if (location2 == CellPoint.Empty)
                    throw new Exception("Trying to place teleporter but ran out of room!");

                teleport1.Location = location1;
                teleport2.Location = location2;

                teleport1.PairId = teleport2.Id;

                level.AddContent(teleport1);
                level.AddContent(teleport2);
            }

            var lastRoomTeleport = new DoodadNormal(DoodadNormalType.Teleport1, ModelConstants.DoodadTeleporterARogueName, "");
            var firstRoomTeleport = new DoodadNormal(DoodadNormalType.Teleport2, ModelConstants.DoodadTeleporterBRogueName, lastRoomTeleport.Id);

            var lastRoomLocation = GetRandomCell(true, rooms.ElementAt(rooms.Count - 1), freeCells, freeRoomCells);
            var firstRoomLocation = GetRandomCell(true, rooms.ElementAt(0), freeCells, freeRoomCells);

            if (lastRoomLocation == CellPoint.Empty)
                throw new Exception("Trying to place teleporter but ran out of room!");

            if (firstRoomLocation == CellPoint.Empty)
                throw new Exception("Trying to place teleporter but ran out of room!");

            lastRoomTeleport.Location = lastRoomLocation;
            firstRoomTeleport.Location = firstRoomLocation;

            lastRoomTeleport.PairId = firstRoomTeleport.Id;

            level.AddContent(lastRoomTeleport);
            level.AddContent(firstRoomTeleport);

            //Add some extra ones (one per room)
            for (int i = 0; i < rooms.Count; i++)
            {
                var extraTeleport1 = new DoodadNormal(DoodadNormalType.Teleport1, ModelConstants.DoodadTeleporterARogueName, "");
                var extraTeleport2 = new DoodadNormal(DoodadNormalType.Teleport2, ModelConstants.DoodadTeleporterBRogueName, extraTeleport1.Id);

                var extraLocation1 = GetRandomCell(false, null, freeCells, freeRoomCells);
                var extraLocation2 = GetRandomCell(false, null, freeCells, freeRoomCells);

                if (extraLocation1 == CellPoint.Empty)
                    throw new Exception("Trying to place teleporter but ran out of room!");

                if (extraLocation2 == CellPoint.Empty)
                    throw new Exception("Trying to place teleporter but ran out of room!");

                // Pick 2 random rooms
                extraTeleport1.Location = extraLocation1;
                extraTeleport2.Location = extraLocation2;

                extraTeleport1.PairId = extraTeleport2.Id;

                level.AddContent(extraTeleport1);
                level.AddContent(extraTeleport2);
            }
        }
        private void AddTeleportRandomLevelContent(Level level, ScenarioConfigurationContainer configurationContainer, int levelNumber, IList<CellPoint> freeCells, Dictionary<Room, List<CellPoint>> freeRoomCells)
        {
            var rooms = level.Grid.Rooms.ToList();

            // Random teleporters send character to random point in the level. Add
            // one per room
            for (int i = 0; i < rooms.Count; i++)
            {
                var doodad = new DoodadNormal(DoodadNormalType.TeleportRandom, ModelConstants.DoodadTeleporterRandomRogueName, "");
                var location = GetRandomCell(true, rooms[i], freeCells, freeRoomCells);

                if (location == CellPoint.Empty)
                    throw new Exception("Trying to place teleporter but ran out of room!");

                doodad.Location = location;
                level.AddContent(doodad);
            }
        }
        private void AddPartyRoomContent(Level level, ScenarioConfigurationContainer configurationContainer, IEnumerable<Religion> religions, IEnumerable<AttackAttribute> scenarioAttributes, int levelNumber, IList<CellPoint> freeCells, Dictionary<Room, List<CellPoint>> freeRoomCells)
        {
            var rooms = level.Grid.Rooms.ToList();
            var partyRoom = rooms.PickRandom(_randomSequenceGenerator.Get());

            // Party room equipment - generate for each 
            foreach (var template in configurationContainer.EquipmentTemplates)
            {
                if (!template.Level.Contains(levelNumber))
                    continue;

                if (template.IsUnique && template.HasBeenGenerated)
                    continue;

                int number = _randomSequenceGenerator.CalculateGenerationNumber(template.GenerationRate);

                for (int i = 0; (i < number || (template.IsObjectiveItem && !template.HasBeenGenerated))
                                            && !(template.IsUnique && template.HasBeenGenerated); i++)
                {
                    var location = GetRandomCell(true, partyRoom, freeCells, freeRoomCells);

                    if (location != CellPoint.Empty)
                    {
                        var equipment = _itemGenerator.GenerateEquipment(template, religions);
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

                    for (int i = 0; (i < number || (template.IsObjectiveItem && !template.HasBeenGenerated))
                                                && !(template.IsUnique && template.HasBeenGenerated); i++)
                    {
                        var location = GetRandomCell(true, partyRoom, freeCells, freeRoomCells);

                        if (location != CellPoint.Empty)
                        {
                            var consumable = _itemGenerator.GenerateConsumable(template, religions);
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

                for (int i = 0; (i < number || (enemyTemplate.IsObjectiveItem && !enemyTemplate.HasBeenGenerated))
                                            && !(enemyTemplate.IsUnique && enemyTemplate.HasBeenGenerated); i++)
                {
                    var location = GetRandomCell(true, partyRoom, freeCells, freeRoomCells);
                    if (location != CellPoint.Empty)
                    { 
                        var enemy = _characterGenerator.GenerateEnemy(enemyTemplate, religions, scenarioAttributes);
                        enemy.Location = location;
                        level.AddContent(enemy);
                    }
                }
            }
        }
        private void MapLevel(Level level, ScenarioConfigurationContainer configurationContainer, IEnumerable<Religion> religions, IEnumerable<AttackAttribute> scenarioAttributes, int levelNumber, IList<CellPoint> freeCells, Dictionary<Room, List<CellPoint>> freeRoomCells)
        {
            var levelContents = level.GetContents();

            // Map Level Contents:  Set locations for each ScenarioObject. Removal of these can be
            // done based on the total length of the levelContents array - which will be altered during
            // the loop interally to the Level.
            for (int i= levelContents.Length - 1; i >= 0; i--)
            {
                // Already Placed
                if (levelContents[i] is DoodadNormal)
                    continue;

                var location = GetRandomCell(false, null, freeCells, freeRoomCells);

                // Entire grid is occupied
                if (location == CellPoint.Empty)
                    level.RemoveContent(levelContents[i]);
                else
                    levelContents[i].Location = location;
            }

            // Create party room if there's a room to use and the rate is greater than U[0,1]
            if ((configurationContainer.DungeonTemplate.PartyRoomGenerationRate > _randomSequenceGenerator.Get()) &&
                (level.Type == LayoutType.ConnectedRectangularRooms))
                AddPartyRoomContent(level, configurationContainer, religions, scenarioAttributes, levelNumber, freeCells, freeRoomCells);
        }

        private CellPoint GetRandomCell(bool inRoom, Room room, IList<CellPoint> freeCells, Dictionary<Room, List<CellPoint>> freeRoomCells)
        {
            // Check overall collection of cells for remaining locations
            if (freeCells.Count == 0)
                return CellPoint.Empty;

            if (inRoom)
            {
                var roomCells = freeRoomCells[room];

                if (roomCells.Count == 0)
                    return CellPoint.Empty;

                var randomIndex = _randomSequenceGenerator.Get(0, roomCells.Count);

                var location = roomCells[randomIndex];

                // Update both collections
                roomCells.RemoveAt(randomIndex);

                if (freeCells.Contains(location))
                    freeCells.Remove(location);

                return location;
            }
            else
            {
                var randomIndex = _randomSequenceGenerator.Get(0, freeCells.Count);

                var location = freeCells[randomIndex];

                // Update both collections
                freeCells.RemoveAt(randomIndex);

                var containingRoom = freeRoomCells.Values.FirstOrDefault(x => x.Contains(location));

                if (containingRoom != null)
                    containingRoom.Remove(location);

                return location;
            }
        }
    }
}
