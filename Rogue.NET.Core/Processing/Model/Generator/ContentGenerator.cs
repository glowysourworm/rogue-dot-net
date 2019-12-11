using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Design;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
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
                ScenarioEncyclopedia encyclopedia,
                IDictionary<Level, LevelBranchTemplate> selectedBranches,
                IDictionary<Level, LayoutGenerationTemplate> selectedLayouts,
                bool survivorMode)
        {
            var result = new List<Level>();

            for (int i = 0; i < levels.Count(); i++)
            {
                // Get Level to map contents INTO
                var level = levels.ElementAt(i);

                // Get the level branch template from the scenario design
                var branchTemplate = selectedBranches[level];

                // Get the layout template that was selected from this branch
                var layoutTemplate = selectedLayouts[level];

                // Generate level contents from the template
                GenerateLevelContent(level,
                                     branchTemplate,
                                     layoutTemplate,
                                     encyclopedia,
                                     i == levels.Count() - 1,
                                     survivorMode);

                result.Add(level);
            }

            return result;
        }

        private void GenerateLevelContent(Level level, LevelBranchTemplate branchTemplate, LayoutGenerationTemplate layoutTemplate, ScenarioEncyclopedia encyclopedia, bool lastLevel, bool survivorMode)
        {
            // Create lists to know what cells are free
            var rooms = level.Grid.RoomMap.GetRegions().ToList();
            var freeCells = level.Grid.GetCells().Where(cell => !cell.IsWall).Select(x => x.Location).ToList();
            var freeRoomCells = rooms.ToDictionary(room => room, room => room.Cells.Where(cell => !level.Grid[cell.Column, cell.Row].IsWall).ToList());

            // NOTE*** ADD MAPPED CONTENT FIRST - BUT MUST IGNORE DURING THE MAPPING PHASE. THIS INCLUDES
            //         ANY NORMAL DOODADS

            // Must have for each level (Except the last one) (MAPPED)
            if (!lastLevel)
            {
                var stairsDown = _doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadStairsDownRogueName, DoodadNormalType.StairsDown);
                stairsDown.Location = GetRandomCell(false, null, freeCells, freeRoomCells);
                level.AddStairsDown(stairsDown);
            }

            // Stairs up - every level has one - (MAPPED)
            var stairsUp = _doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadStairsUpRogueName, DoodadNormalType.StairsUp);
            stairsUp.Location = GetRandomCell(false, null, freeCells, freeRoomCells);
            level.AddStairsUp(stairsUp);

            // TODO:TERRAIN - Use pre-mapped mandatory cells

            // Add teleporter level content - (MAPPED)
            if (level.Layout.Asset.ConnectionType == LayoutConnectionType.Teleporter)
                AddTeleporterLevelContent(level, freeCells, freeRoomCells);

            // Every level has a save point if not in survivor mode - (MAPPED)
            if (!survivorMode)
            {
                var savePoint = _doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadSavePointRogueName, DoodadNormalType.SavePoint);
                savePoint.Location = GetRandomCell(false, null, freeCells, freeRoomCells);
                level.AddSavePoint(savePoint);
            }

#if DEBUG_MINIMUM_CONTENT

            // Don't generate contents

#else
            // Applies to all levels - (UNMAPPED)
            GenerateEnemies(level, branchTemplate, encyclopedia);
            GenerateItems(level, branchTemplate);
            GenerateDoodads(level, branchTemplate);
#endif
            MapLevel(level, freeCells, freeRoomCells);

#if DEBUG_MINIMUM_CONTENT 

            // Don't generate party room contents

#else
            // Create party room if there's a room to use and the rate is greater than U[0,1] - (MAPPED)
            if ((layoutTemplate.PartyRoomGenerationRate > _randomSequenceGenerator.Get()) && freeRoomCells.Any())
                AddPartyRoomContent(level, branchTemplate, encyclopedia, freeCells, freeRoomCells);
#endif
        }
        private void GenerateDoodads(Level level, LevelBranchTemplate branchTemplate)
        {
            var generationNumber = _randomSequenceGenerator.GetRandomValue(branchTemplate.DoodadGenerationRange);

            // Make a configured number of random draws from the doodad templates for this branch
            for (int i = 0; i < generationNumber; i++)
            {
                // Run generation method
                var doodad = GenerateDoodad(branchTemplate);

                if (doodad != null)
                    level.AddContent(doodad);
            }
        }
        private void GenerateEnemies(Level level, LevelBranchTemplate branchTemplate, ScenarioEncyclopedia encyclopedia)
        {
            var generationNumber = _randomSequenceGenerator.GetRandomValue(branchTemplate.EnemyGenerationRange);

            // Make a configured number of random draws from the enemy templates for this branch
            for (int i = 0; i < generationNumber; i++)
            {
                // Run generation method
                var enemy = GenerateEnemy(branchTemplate, encyclopedia);

                if (enemy != null)
                    level.AddContent(enemy);
            }
        }
        private void GenerateItems(Level level, LevelBranchTemplate branchTemplate)
        {
            var equipmentGenerationNumber = _randomSequenceGenerator.GetRandomValue(branchTemplate.EquipmentGenerationRange);
            var consumableGenerationNumber = _randomSequenceGenerator.GetRandomValue(branchTemplate.ConsumableGenerationRange);

            // Make a configured number of random draws from the equipment templates for this branch
            for (int i = 0; i < equipmentGenerationNumber; i++)
            {
                // Run generation method
                var equipment = GenerateEquipment(branchTemplate);

                if (equipment != null)
                    level.AddContent(equipment);
            }

            // Make a configured number of random draws from the consumable templates for this branch
            for (int i = 0; i < consumableGenerationNumber; i++)
            {
                // Run generation method
                var consumable = GenerateConsumable(branchTemplate);

                if (consumable != null)
                    level.AddContent(consumable);
            }
        }
        private void AddTeleporterLevelContent(Level level, IList<GridLocation> freeCells, Dictionary<Region, List<GridLocation>> freeRoomCells)
        {
            var rooms = level.Grid.RoomMap.GetRegions().ToList();

            // Connect rooms with teleporters sequentially to make sure can reach all rooms
            for (int i = 0; i < rooms.Count - 1; i++)
            {
                var teleport1 = _doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadTeleporterARogueName, DoodadNormalType.Teleport1);
                var teleport2 = _doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadTeleporterBRogueName, DoodadNormalType.Teleport2);

                var location1 = GetRandomCell(true, rooms.ElementAt(i), freeCells, freeRoomCells);
                var location2 = GetRandomCell(true, rooms.ElementAt(i + 1), freeCells, freeRoomCells);

                if (location1 == null)
                    throw new Exception("Trying to place teleporter but ran out of room!");

                if (location2 == null)
                    throw new Exception("Trying to place teleporter but ran out of room!");

                teleport1.Location = location1;
                teleport2.Location = location2;

                teleport1.PairId = teleport2.Id;
                teleport2.PairId = teleport1.Id;

                level.AddContent(teleport1);
                level.AddContent(teleport2);
            }

            var lastRoomTeleport = _doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadTeleporterARogueName, DoodadNormalType.Teleport1);
            var firstRoomTeleport = _doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadTeleporterBRogueName, DoodadNormalType.Teleport2);

            var lastRoomLocation = GetRandomCell(true, rooms.ElementAt(rooms.Count - 1), freeCells, freeRoomCells);
            var firstRoomLocation = GetRandomCell(true, rooms.ElementAt(0), freeCells, freeRoomCells);

            if (lastRoomLocation == null)
                throw new Exception("Trying to place teleporter but ran out of room!");

            if (firstRoomLocation == null)
                throw new Exception("Trying to place teleporter but ran out of room!");

            lastRoomTeleport.Location = lastRoomLocation;
            firstRoomTeleport.Location = firstRoomLocation;

            // Pair the teleporters
            firstRoomTeleport.PairId = lastRoomTeleport.Id;
            lastRoomTeleport.PairId = firstRoomTeleport.Id;

            level.AddContent(lastRoomTeleport);
            level.AddContent(firstRoomTeleport);

            //Add some extra ones (one per room)
            for (int i = 0; i < rooms.Count; i++)
            {
                var extraLocation1 = GetRandomCell(false, null, freeCells, freeRoomCells);
                var extraLocation2 = GetRandomCell(false, null, freeCells, freeRoomCells);

                // If we ran out of room then just return since these are extra
                if (extraLocation1 == null ||
                    extraLocation2 == null)
                    return;

                var extraTeleport1 = _doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadTeleporterARogueName, DoodadNormalType.Teleport1);
                var extraTeleport2 = _doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadTeleporterBRogueName, DoodadNormalType.Teleport2);

                extraTeleport1.Location = extraLocation1;
                extraTeleport2.Location = extraLocation2;

                // Pair the teleporters
                extraTeleport1.PairId = extraTeleport2.Id;
                extraTeleport2.PairId = extraTeleport1.Id;

                level.AddContent(extraTeleport1);
                level.AddContent(extraTeleport2);
            }
        }
        private void AddTeleportRandomLevelContent(Level level, IList<GridLocation> freeCells, Dictionary<Region, List<GridLocation>> freeRoomCells)
        {
            var rooms = level.Grid.RoomMap.GetRegions().ToList();

            // Random teleporters send character to random point in the level. Add
            // one per room
            for (int i = 0; i < rooms.Count; i++)
            {
                var doodad = _doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadTeleporterRandomRogueName, DoodadNormalType.TeleportRandom);
                var location = GetRandomCell(true, rooms[i], freeCells, freeRoomCells);

                if (location == null)
                    throw new Exception("Trying to place teleporter but ran out of room!");

                doodad.Location = location;
                level.AddContent(doodad);
            }
        }
        private void AddPartyRoomContent(Level level, LevelBranchTemplate branchTemplate, ScenarioEncyclopedia encyclopedia, IList<GridLocation> freeCells, Dictionary<Region, List<GridLocation>> freeRoomCells)
        {
            // *** Party Room Procedure (TODO: Parameterize this at some point)
            //
            //     1) Choose Room at random
            //     2) Draw random number of Enemies between [1, 5] -> Place Enemies
            //     3) Draw random number of Consumables between [1, 3] -> Place Consumables
            //     4) Draw random number of Equipment between [1, 2] -> Place Equipment
            //     5) Repeat steps 2-4 until either:
            //          A) Reach total asset number between [15, 30]
            //       OR B) Run out of room
            //

            var partyRoom = _randomSequenceGenerator.GetRandomElement(level.Grid.RoomMap.GetRegions());

            var totalAssetNumber = _randomSequenceGenerator.Get(15, 30);
            var enemyNumber = _randomSequenceGenerator.Get(1, 5);
            var consumableNumber = _randomSequenceGenerator.Get(1, 3);
            var equipmentNumber = _randomSequenceGenerator.Get(1, 2);

            var generatedAssets = 0;
            var availableLocation = true;

            // Generate -> Map assets while there's available room and the limit hasn't been exceeded
            while ((generatedAssets < totalAssetNumber) && availableLocation)
            {
                // Enemies
                for (int i = 0; (i < enemyNumber) &&
                                (generatedAssets < totalAssetNumber) &&
                                 availableLocation; i++)
                {
                    // If Asset generation fails - exit loop
                    var enemy = GenerateEnemy(branchTemplate, encyclopedia);

                    if (enemy == null)
                        break;

                    // Draw random cell from remaining locations
                    var location = GetRandomCell(true, partyRoom, freeCells, freeRoomCells);

                    if (location != null)
                    {
                        // Map the enemy - add to level content
                        enemy.Location = location;
                        level.AddContent(enemy);
                        generatedAssets++;
                    }
                    else
                        availableLocation = false;
                }

                // Consumables
                for (int i = 0; (i < consumableNumber) &&
                                (generatedAssets < totalAssetNumber) &&
                                 availableLocation; i++)
                {
                    // If Asset generation fails - exit loop
                    var consumable = GenerateConsumable(branchTemplate);

                    if (consumable == null)
                        break;

                    // Draw random cell from remaining locations
                    var location = GetRandomCell(true, partyRoom, freeCells, freeRoomCells);

                    if (location != null)
                    {
                        // Map the enemy - add to level content
                        consumable.Location = location;
                        level.AddContent(consumable);
                        generatedAssets++;
                    }
                    else
                        availableLocation = false;
                }

                // Equipment
                for (int i = 0; (i < equipmentNumber) &&
                                (generatedAssets < totalAssetNumber) &&
                                availableLocation; i++)
                {
                    // If Asset generation fails - exit loop
                    var equipment = GenerateEquipment(branchTemplate);

                    if (equipment == null)
                        break;

                    // Draw random cell from remaining locations
                    var location = GetRandomCell(true, partyRoom, freeCells, freeRoomCells);

                    if (location != null)
                    {
                        // Map the enemy - add to level content
                        equipment.Location = location;
                        level.AddContent(equipment);
                        generatedAssets++;
                    }
                    else
                        availableLocation = false;
                }
            }
        }
        private void MapLevel(Level level, IList<GridLocation> freeCells, Dictionary<Region, List<GridLocation>> freeRoomCells)
        {
            var levelContents = level.GetContents();

            // Map Level Contents:  Set locations for each ScenarioObject. Removal of these can be
            // done based on the total length of the levelContents array - which will be altered during
            // the loop interally to the Level.
            for (int i = levelContents.Length - 1; i >= 0; i--)
            {
                // Already Placed
                if (levelContents[i] is DoodadNormal)
                    continue;

                var location = GetRandomCell(false, null, freeCells, freeRoomCells);

                // Entire grid is occupied
                if (location == null)
                    level.RemoveContent(levelContents[i]);
                else
                    levelContents[i].Location = location;
            }
        }
        private GridLocation GetRandomCell(bool inRoom, Region room, IList<GridLocation> freeCells, Dictionary<Region, List<GridLocation>> freeRoomCells)
        {
            // Check overall collection of cells for remaining locations
            if (freeCells.Count == 0)
                return null;

            if (inRoom)
            {
                var roomCells = freeRoomCells[room];

                if (roomCells.Count == 0)
                    return null;

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

        private DoodadMagic GenerateDoodad(LevelBranchTemplate branchTemplate)
        {
            // Get doodads that pass generation requirements
            //
            //  1) IsObjective -> MUST GENERATE
            //  2) IsUnique -> Generate ONCE
            //

            var doodadsViable = branchTemplate.Doodads.Any(x => x.Asset.IsObjectiveItem &&
                                                                  !x.Asset.HasBeenGenerated) ?
                                branchTemplate.Doodads.Where(x => x.Asset.IsObjectiveItem &&
                                                                  !x.Asset.HasBeenGenerated) :
                                branchTemplate.Doodads.Where(x => !(x.Asset.IsUnique && x.Asset.HasBeenGenerated));

            // If no viable doodads, then return
            if (doodadsViable.None())
                return null;

            // Get weighted random draw from the branch template's doodad collection (SHARED REFERENCES TO PRIMARY ASSETS)
            var doodadTemplate = _randomSequenceGenerator.GetWeightedRandom(doodadsViable, x => x.GenerationWeight);

            // Generate doodad
            return _doodadGenerator.GenerateMagicDoodad(doodadTemplate.Asset);
        }
        private Enemy GenerateEnemy(LevelBranchTemplate branchTemplate, ScenarioEncyclopedia encyclopedia)
        {
            // Get enemies that pass generation requirements
            //
            //  1) IsObjective -> MUST GENERATE
            //  2) IsUnique -> Generate ONCE
            //

            var enemiesViable = branchTemplate.Enemies.Any(x => GetMustGenerate(x.Asset)) ?
                                branchTemplate.Enemies.Where(x => GetMustGenerate(x.Asset)) :
                                branchTemplate.Enemies.Where(x => !(x.Asset.IsUnique && x.Asset.HasBeenGenerated));

            // If no viable enemies, then return
            if (enemiesViable.None())
                return null;

            // Get weighted random draw from the branch template's enemy collection (SHARED REFERENCES TO PRIMARY ASSETS)
            var enemyTemplate = _randomSequenceGenerator.GetWeightedRandom(enemiesViable, x => x.GenerationWeight);

            // Generate enemy
            return _characterGenerator.GenerateEnemy(enemyTemplate.Asset, encyclopedia);
        }
        private Equipment GenerateEquipment(LevelBranchTemplate branchTemplate)
        {
            // Get equipment that pass generation requirements
            //
            //  1) IsObjective -> MUST GENERATE
            //  2) IsUnique -> Generate ONCE
            //

            var equipmentViable = branchTemplate.Equipment.Any(x => x.Asset.IsObjectiveItem &&
                                                                  !x.Asset.HasBeenGenerated) ?
                                    branchTemplate.Equipment.Where(x => x.Asset.IsObjectiveItem &&
                                                                      !x.Asset.HasBeenGenerated) :
                                    branchTemplate.Equipment.Where(x => !(x.Asset.IsUnique && x.Asset.HasBeenGenerated));

            // If no viable enemies, then return
            if (equipmentViable.None())
                return null;

            // Get weighted random draw from the branch template's equipment collection (SHARED REFERENCES TO PRIMARY ASSETS)
            var equipmentTemplate = _randomSequenceGenerator.GetWeightedRandom(equipmentViable, x => x.GenerationWeight);

            // Generate equipment
            return _itemGenerator.GenerateEquipment(equipmentTemplate.Asset);
        }
        private Consumable GenerateConsumable(LevelBranchTemplate branchTemplate)
        {
            // Get consumable that pass generation requirements
            //
            //  1) IsObjective -> MUST GENERATE
            //  2) IsUnique -> Generate ONCE
            //

            var consumablesViable = branchTemplate.Consumables.Any(x => x.Asset.IsObjectiveItem &&
                                                                   !x.Asset.HasBeenGenerated) ?
                                    branchTemplate.Consumables.Where(x => x.Asset.IsObjectiveItem &&
                                                                         !x.Asset.HasBeenGenerated) :
                                    branchTemplate.Consumables.Where(x => !(x.Asset.IsUnique && x.Asset.HasBeenGenerated));

            // If no viable enemies, then return
            if (consumablesViable.None())
                return null;

            // Get weighted random draw from the branch template's consumable collection (SHARED REFERENCES TO PRIMARY ASSETS)
            var consumableTemplate = _randomSequenceGenerator.GetWeightedRandom(consumablesViable, x => x.GenerationWeight);

            // Generate consumable
            return _itemGenerator.GenerateConsumable(consumableTemplate.Asset);
        }
        private bool GetMustGenerate(CharacterTemplate character)
        {
            if (character.IsObjectiveItem && !character.HasBeenGenerated)
                return true;

            // Is Objective -> Has been generated -> BUT DIDN'T GENERATE OBJECTIVE CONSUMABLE
            if (character.IsObjectiveItem &&
                character.HasBeenGenerated &&
                character.StartingConsumables.Any(x => x.TheTemplate.IsObjectiveItem && !x.TheTemplate.HasBeenGenerated))
                throw new Exception("Generated character DID NOT GENERATE OBJECTIVE ITEM");

            // Is Objective -> Has been generated -> BUT DIDN'T GENERATE OBJECTIVE EQUIPMENT
            if (character.IsObjectiveItem &&
                character.HasBeenGenerated &&
                character.StartingEquipment.Any(x => x.TheTemplate.IsObjectiveItem && !x.TheTemplate.HasBeenGenerated))
                throw new Exception("Generated character DID NOT GENERATE OBJECTIVE ITEM");

            if (character.StartingConsumables.Any(x => x.TheTemplate.IsObjectiveItem && !x.TheTemplate.HasBeenGenerated))
                return true;

            if (character.StartingEquipment.Any(x => x.TheTemplate.IsObjectiveItem && !x.TheTemplate.HasBeenGenerated))
                return true;

            return false;
        }
    }
}
