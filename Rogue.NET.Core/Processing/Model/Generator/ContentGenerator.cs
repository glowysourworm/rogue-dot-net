using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
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
using Rogue.NET.Core.Processing.Model.Generator.Layout.Component;

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

        public Level CreateContents(Level level,
                                    LevelBranchTemplate branchTemplate,
                                    LayoutGenerationTemplate layoutTemplate,
                                    ScenarioEncyclopedia encyclopedia,
                                    Graph<Region<GridLocation>> transporterGraph,
                                    bool lastLevel,
                                    bool survivorMode)
        {
            // NOTE*** ADD MAPPED CONTENT FIRST - BUT MUST IGNORE DURING THE MAPPING PHASE. THIS INCLUDES
            //         ANY NORMAL DOODADS

            // Remove transport points from walkable cells; and randomize the collection
            //
            var transporterDict = transporterGraph.Vertices
                                                  .Select(vertex => vertex.Reference)
                                                  .Distinct()
                                                  .ToDictionary(region => region, region => _randomSequenceGenerator.GetRandomElement(region.Locations));

            var openLocations = level.Grid.GetWalkableCells()
                                          .Except(transporterDict.Values.Select(location => level.Grid[location.Column, location.Row]))
                                          .Select(cell => cell.Location)
                                          .Actualize();

            var randomizedLocations = _randomSequenceGenerator.Randomize(openLocations);
            var randomizedLocationQueue = new Queue<GridLocation>(randomizedLocations);

            // Must have for each level (Except the last one) (MAPPED)
            if (!lastLevel)
            {
                var stairsDown = _doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadStairsDownRogueName, DoodadNormalType.StairsDown);
                stairsDown.Location = randomizedLocationQueue.Dequeue();
                level.AddStairsDown(stairsDown);
            }

            // Stairs up - every level has one - (MAPPED)
            var stairsUp = _doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadStairsUpRogueName, DoodadNormalType.StairsUp);
            stairsUp.Location = randomizedLocationQueue.Dequeue();
            level.AddStairsUp(stairsUp);

            // Add teleporter level content - (MAPPED)
            if (layoutTemplate.Asset.ConnectionType == LayoutConnectionType.ConnectionPoints)
                AddTransporters(level, transporterDict, transporterGraph);

            // Every level has a save point if not in survivor mode - (MAPPED)
            if (!survivorMode)
            {
                var savePoint = _doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadSavePointRogueName, DoodadNormalType.SavePoint);
                savePoint.Location = randomizedLocationQueue.Dequeue();
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
            MapLevel(level, randomizedLocationQueue);

            return level;
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
        private void AddTransporters(Level level, IDictionary<Region<GridLocation>, GridLocation> transporterDict,  Graph<Region<GridLocation>> transportGraph)
        {
            var transporters = new List<DoodadNormal>();
            var usedRegions = new List<Region<GridLocation>>();

            // Create the transporters
            foreach (var element in transporterDict)
            {
                // Create the transporter for this location
                var transporter = _doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadTransporterRogueName, DoodadNormalType.Transporter);

                // Set transporter location
                transporter.Location = element.Value;

                // Add to the list for lookup
                transporters.Add(transporter);

                // Add it to the level content
                level.AddContent(transporter);
            }

            // Link them together
            foreach (var element in transporterDict)
            {
                // Get vertices for this region
                var vertices = transportGraph.Find(element.Key);

                // Find all connecting edges
                var connectingEdges = vertices.SelectMany(vertex => transportGraph[vertex]);

                // Locate connected regions and add to the transporter list
                var connectingLocations = connectingEdges.Select(edge =>
                {
                    if (edge.Point1.Reference == element.Key)
                        return transporterDict[edge.Point2.Reference];

                    else
                        return transporterDict[edge.Point1.Reference];
                });

                // Set all connection id's for each transporter
                var primaryTransporter = transporters.First(x => x.Location.Equals(element.Value));

                // Get other transporters by connecting location
                var otherTransporters = transporters.Where(x => connectingLocations.Contains(x.Location));

                // Set connecting id's
                foreach (var transporterId in otherTransporters.Select(x => x.Id))
                    primaryTransporter.TransportGroupIds.Add(transporterId);

                // ALSO SET PRIMARY ID
                primaryTransporter.TransportGroupIds.Add(primaryTransporter.Id);
            }
        }
        private void MapLevel(Level level, Queue<GridLocation> randomizedLocationQueue)
        {
            var levelContents = level.GetContents();

            // Map Level Contents:  Set locations for each ScenarioObject. Removal of these can be
            // done based on the total length of the levelContents array - which will be altered during
            // the loop interally to the Level.
            for (int i = levelContents.Length - 1; i >= 0 && randomizedLocationQueue.Any(); i--)
            {
                // Already Placed
                if (levelContents[i] is DoodadNormal)
                    continue;

                var location = randomizedLocationQueue.Dequeue();

                // Entire grid is occupied
                if (location == null)
                    level.RemoveContent(levelContents[i]);
                else
                    levelContents[i].Location = location;
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
