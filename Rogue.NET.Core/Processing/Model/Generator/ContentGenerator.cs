using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
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
                                    bool lastLevel,
                                    bool survivorMode)
        {
            // Remove transport points from walkable cells; and randomize the collection
            //
            var transporterDict = level.Grid
                                       .RoomMap
                                       .Regions
                                       .ToDictionary(region => region, region => _randomSequenceGenerator.GetRandomElement(region.Locations));

            var mandatoryNormalDoodads = new List<DoodadNormal>();

            // Must have for each level (Except the last one)
            if (!lastLevel)
                mandatoryNormalDoodads.Add(_doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadStairsDownRogueName, DoodadNormalType.StairsDown));

            // Stairs up - every level has one
            mandatoryNormalDoodads.Add(_doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadStairsUpRogueName, DoodadNormalType.StairsUp));

            // Every level has a save point if not in survivor mode
            if (!survivorMode)
                mandatoryNormalDoodads.Add(_doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadSavePointRogueName, DoodadNormalType.SavePoint));

            // ADD MANDATORY NORMAL DOODADS "DISTANTLY"
            if (!level.AddContentGroup(_randomSequenceGenerator, mandatoryNormalDoodads, ContentGroupPlacementType.RandomlyDistant, transporterDict.Values))
                throw new Exception("Unable to place mandatory normal doodads ContentGenerator");

            // Add teleporter level content
            if (layoutTemplate.Asset.ConnectionType == LayoutConnectionType.ConnectionPoints)
                AddTransporters(level, transporterDict);

#if DEBUG_MINIMUM_CONTENT

            // Don't generate contents

#else
            // Applies to all levels - (UNMAPPED)
            GenerateEnemies(level, branchTemplate, encyclopedia);
            GenerateItems(level, branchTemplate);
            GenerateDoodads(level, branchTemplate);
#endif
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
                    level.AddContentRandom(_randomSequenceGenerator, doodad, ContentRandomPlacementType.Random, new GridLocation[] { });
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
                    level.AddContentRandom(_randomSequenceGenerator, enemy, ContentRandomPlacementType.Random, new GridLocation[] { });
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
                    level.AddContentRandom(_randomSequenceGenerator, equipment, ContentRandomPlacementType.Random, new GridLocation[] { });
            }

            // Make a configured number of random draws from the consumable templates for this branch
            for (int i = 0; i < consumableGenerationNumber; i++)
            {
                // Run generation method
                var consumable = GenerateConsumable(branchTemplate);

                if (consumable != null)
                    level.AddContentRandom(_randomSequenceGenerator, consumable, ContentRandomPlacementType.Random, new GridLocation[] { });
            }
        }
        private void AddTransporters(Level level, IDictionary<Region<GridLocation>, GridLocation> transporterDict)
        {
            var transporters = new List<DoodadNormal>();
            var usedRegions = new List<Region<GridLocation>>();

            // Create the transporters
            foreach (var element in transporterDict)
            {
                // Create the transporter for this location
                var transporter = _doodadGenerator.GenerateNormalDoodad(ModelConstants.DoodadTransporterRogueName, DoodadNormalType.Transporter);

                // Add to the list for lookup
                transporters.Add(transporter);

                // Add it to the level content
                level.AddContent(transporter, element.Value);
            }

            // Link them together
            foreach (var element in transporterDict)
            {
                // Find all connecting edges
                var connectedRegions = level.Grid.ConnectionMap.Connections(element.Key.Id);

                // Locate connected regions and add to the transporter list
                var connectingLocations = connectedRegions.Select(region => transporterDict[region]);

                // Set all connection id's for each transporter
                var primaryTransporter = transporters.First(x => level.Content[x].Equals(element.Value));

                // Get other transporters by connecting location
                var otherTransporters = transporters.Where(x => connectingLocations.Contains(level.Content[x]));

                // Set connecting id's
                foreach (var transporterId in otherTransporters.Select(x => x.Id))
                    primaryTransporter.TransportGroupIds.Add(transporterId);

                // ALSO SET PRIMARY ID
                primaryTransporter.TransportGroupIds.Add(primaryTransporter.Id);
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
