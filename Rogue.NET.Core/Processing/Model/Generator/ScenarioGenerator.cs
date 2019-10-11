using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration;

using System.ComponentModel.Composition;
using System.Linq;

using Rogue.NET.Core.Model.Scenario.Content.Skill.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Common.Extension;
using System;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [Export(typeof(IScenarioGenerator))]
    public class ScenarioGenerator : IScenarioGenerator
    {
        readonly IRogueEventAggregator _eventAggregator;
        readonly ILayoutGenerator _layoutGenerator;
        readonly IContentGenerator _contentGenerator;
        readonly ICharacterGenerator _characterGenerator;
        readonly IAttackAttributeGenerator _attackAttributeGenerator;
        readonly IScenarioMetaDataGenerator _scenarioMetaDataGenerator;
        readonly ISymbolDetailsGenerator _symbolDetailsGenerator;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        [ImportingConstructor]
        public ScenarioGenerator(
            IRogueEventAggregator eventAggregator,
            ILayoutGenerator layoutGenerator,
            IContentGenerator contentGenerator,
            ICharacterGenerator characterGenerator,
            IAttackAttributeGenerator attackAttributeGenerator,
            IScenarioMetaDataGenerator scenarioMetaDataGenerator,
            ISymbolDetailsGenerator symbolDetailsGenerator,
            IRandomSequenceGenerator randomSequenceGenerator)
        {
            _eventAggregator = eventAggregator;
            _layoutGenerator = layoutGenerator;
            _contentGenerator = contentGenerator;
            _characterGenerator = characterGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;
            _scenarioMetaDataGenerator = scenarioMetaDataGenerator;
            _symbolDetailsGenerator = symbolDetailsGenerator;
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public ScenarioContainer CreateScenario(ScenarioConfigurationContainer configuration, string characterClassName, int seed, bool survivorMode)
        {
            ScenarioContainer scenario = new ScenarioContainer();

            // Reseed the Random number generator
            _randomSequenceGenerator.Reseed(seed);

            // Randomize symbols
            RandomizeSymbols(configuration);

            // Scenario Generation Procedure
            //
            // 1) Find and select level branches with:
            //      A) Any Objective (SINGLE AND ONLY) (VALIDATED WITH EDITOR)
            //      B) Any other branch
            //
            // 2) Draw layouts for each branch
            //
            // 3) Generate layouts
            //
            // 4) Generate -> Map Assets -> (Levels Finished)
            //
            // 5) Generate Player
            //
            // 6) Load Rogue-Encyclopedia (Meta-data)
            //

            // Select path through the scenario (level branch sequence)
            var levelBranches = configuration.ScenarioDesign
                                             .LevelDesigns
                                             .Select(x => _randomSequenceGenerator.GetWeightedRandom(x.LevelBranches, z => z.GenerationWeight))
                                             .Select(x => x.LevelBranch)
                                             .Actualize();

            // Select layouts for each branch
            var levelLayouts = levelBranches.Select(x => _randomSequenceGenerator.GetWeightedRandom(x.Layouts, z => z.GenerationWeight))
                                            .Actualize();

            // *** Generate Layouts
            var levels = _layoutGenerator.CreateLayouts(levelBranches.Select(x => x.Name), levelLayouts);

            // Create dictionary of Level -> LayoutTemplate
            var layoutDictionary = levels.Zip(levelLayouts, (level, layout) => new { Level = level, Layout = layout })
                                         .ToDictionary(x => x.Level, x => x.Layout);

            // Create dictionary of Level -> LevelBranch
            var branchDictionary = levels.Zip(levelBranches, (level, branch) => new { Level = level, Branch = branch })
                                         .ToDictionary(x => x.Level, x => x.Branch);

            // *** Generate Contents
            scenario.LoadedLevels = _contentGenerator.CreateContents(levels, branchDictionary, layoutDictionary, survivorMode)
                                                     .ToList();

            // Generate Attack Attributes
            scenario.AttackAttributes = configuration.AttackAttributes
                                                     .Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x))
                                                     .ToDictionary(x => x.RogueName, x => x);

            // Generate Player
            scenario.Player = _characterGenerator
                                .GeneratePlayer(configuration.PlayerTemplates
                                                             .First(x => x.Name == characterClassName));

            //Load Encyclopedia Rogue-Tanica (Consumables)
            foreach (var template in configuration.ConsumableTemplates)
                scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Equipment)
            foreach (var template in configuration.EquipmentTemplates)
                scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Enemies)
            foreach (var template in configuration.EnemyTemplates)
                scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Friendlies)
            foreach (var template in configuration.FriendlyTemplates)
                scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Doodads)
            foreach (var template in configuration.DoodadTemplates)
                scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Skill Sets)
            foreach (var template in configuration.SkillTemplates)
                scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Normal Doodads)
            scenario.ScenarioEncyclopedia.Add(ModelConstants.DoodadSavePointRogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.SavePoint));
            scenario.ScenarioEncyclopedia.Add(ModelConstants.DoodadStairsDownRogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.StairsDown));
            scenario.ScenarioEncyclopedia.Add(ModelConstants.DoodadStairsUpRogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.StairsUp));
            scenario.ScenarioEncyclopedia.Add(ModelConstants.DoodadTeleporterARogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.Teleport1));
            scenario.ScenarioEncyclopedia.Add(ModelConstants.DoodadTeleporterBRogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.Teleport2));
            scenario.ScenarioEncyclopedia.Add(ModelConstants.DoodadTeleporterRandomRogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.TeleportRandom));

            //Identify player skills / equipment / consumables / and character classes
            foreach (var skillSet in scenario.Player.SkillSets)
            {
                scenario.ScenarioEncyclopedia[skillSet.RogueName].IsIdentified = true;

                // Also setup skill AreRequirementsMet flag based on player level for skills that have zero cost
                foreach (var skill in skillSet.Skills)
                    skill.AreRequirementsMet = skill.AreRequirementsMet(scenario.Player);
            }

            foreach (var equipment in scenario.Player.Equipment.Values)
            {
                scenario.ScenarioEncyclopedia[equipment.RogueName].IsIdentified = true;
                equipment.IsIdentified = true;
            }

            foreach (var consumable in scenario.Player.Consumables.Values)
            {
                scenario.ScenarioEncyclopedia[consumable.RogueName].IsIdentified = true;
                consumable.IsIdentified = true;
            }

            // Load Encyclopedia Rogue-Tanica (Temporary Characters) (A little more to do here to get all alterations)
            foreach (var skillSet in configuration.SkillTemplates)
            {
                foreach (var skill in skillSet.Skills)
                {
                    if (skill.SkillAlteration.Effect is CreateTemporaryCharacterAlterationEffectTemplate)
                    {
                        // Add the Temporary Character Template Meta-data
                        var template = (skill.SkillAlteration.Effect as CreateTemporaryCharacterAlterationEffectTemplate).TemporaryCharacter;

                        // Create Scenario Meta Data
                        var metaData = _scenarioMetaDataGenerator.CreateScenarioMetaData(template);

                        // Identify Temporary Characters involved with player skill set
                        if (scenario.Player.SkillSets.Any(x => x.RogueName == skillSet.Name))
                            metaData.IsIdentified = true;

                        scenario.ScenarioEncyclopedia.Add(template.Name, metaData);
                    }
                }
            }
            foreach (var enemy in configuration.EnemyTemplates)
            {
                // Search for Create Temporary Character Alteration Effect
                foreach (var behavior in enemy.BehaviorDetails.Behaviors.Where(x => x.AttackType == CharacterAttackType.Alteration))
                {
                    if (behavior.Alteration.Effect is CreateTemporaryCharacterAlterationEffectTemplate)
                    {
                        // Add the Temporary Character Template Meta-data
                        var template = (behavior.Alteration.Effect as CreateTemporaryCharacterAlterationEffectTemplate).TemporaryCharacter;
                        scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));
                    }
                }
            }
            foreach (var doodad in configuration.DoodadTemplates)
            {
                // Search for Create Temporary Character Alteration Effect
                if (doodad.IsAutomatic)
                {
                    if (doodad.AutomaticAlteration.Effect is CreateTemporaryCharacterAlterationEffectTemplate)
                    {
                        // Add the Temporary Character Template Meta-data
                        var template = (doodad.AutomaticAlteration.Effect as CreateTemporaryCharacterAlterationEffectTemplate).TemporaryCharacter;
                        scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));
                    }
                }
                if (doodad.IsInvoked)
                {
                    if (doodad.InvokedAlteration.Effect is CreateTemporaryCharacterAlterationEffectTemplate)
                    {
                        // Add the Temporary Character Template Meta-data
                        var template = (doodad.InvokedAlteration.Effect as CreateTemporaryCharacterAlterationEffectTemplate).TemporaryCharacter;
                        scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));
                    }
                }
            }
            foreach (var consumable in configuration.ConsumableTemplates)
            {
                // Search for Create Temporary Character Alteration Effect
                if (consumable.HasAlteration)
                {
                    if (consumable.ConsumableAlteration.Effect is CreateTemporaryCharacterAlterationEffectTemplate)
                    {
                        // Add the Temporary Character Template Meta-data
                        var template = (consumable.ConsumableAlteration.Effect as CreateTemporaryCharacterAlterationEffectTemplate).TemporaryCharacter;
                        scenario.ScenarioEncyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));
                    }
                }
            }

            return scenario;
        }

        /// <summary>
        /// Modifies symbols for consumable / equipment / doodads to randomize them using the configuration symbol pools
        /// </summary>
        private void RandomizeSymbols(ScenarioConfigurationContainer configuration)
        {
            // Procedure
            //
            // Symbol Pool Category:  Defines a category to pool random symbols for any asset with that category.
            // Symbol Pool:           Defines a pool of EXTRA symbols to ADD TO the asset's category.
            // Randomize:             Allows asset's symbol to be used to randomize; and randomizes THAT symbol.
            //
            // 1) Find symbols on assets of the symbol pool category (Consumables, Doodads, Equipment ONLY)
            // 2) Add symbol pool symbols to these as the total pool to draw from
            // 3) Draw a random symbol
            // 4) Map its properties onto the asset's symbol details
            // 5) Store the symbol reference so as not to re-use it.
            //

            var usedSymbols = new Dictionary<string, List<SymbolDetailsTemplate>>();

            var mapSymbolFunction = new Action<SymbolDetailsTemplate, 
                                               IEnumerable<SymbolDetailsTemplate>, 
                                               SymbolPoolItemTemplate>((symbolDetails, assetCategorySymbols, symbolPool) =>
            {
                // Get previously used symbols
                if (usedSymbols.ContainsKey(symbolPool.SymbolPoolCategory))
                {
                    // Symbol Pool[Symbol Category] + Asset Symbols[Symbol Category]
                    var allSymbols = symbolPool.Symbols.Union(assetCategorySymbols);

                    // Calculate remaining symbols
                    var remainingSymbols = allSymbols.Except(usedSymbols[symbolPool.SymbolPoolCategory]);

                    // Map symbol by property
                    var randomSymbol = _randomSequenceGenerator.GetRandomElement(remainingSymbols);

                    randomSymbol.MapOnto(symbolDetails);

                    usedSymbols[symbolPool.SymbolPoolCategory].Add(randomSymbol);
                }
                else
                {
                    // Symbol Pool[Symbol Category] + Asset Symbols[Symbol Category]
                    var allSymbols = symbolPool.Symbols.Union(assetCategorySymbols);

                    // Map symbol by property
                    var randomSymbol = _randomSequenceGenerator.GetRandomElement(allSymbols);

                    randomSymbol.MapOnto(symbolDetails);

                    usedSymbols.Add(symbolPool.SymbolPoolCategory, new List<SymbolDetailsTemplate>() { randomSymbol });
                }
            });

            // Calculate random symbols for all user-set randomizable symbols
            //
            var randomizableSymbols = configuration.ConsumableTemplates
                                                   .Cast<DungeonObjectTemplate>()
                                                   .Union(configuration.EquipmentTemplates)
                                                   .Union(configuration.DoodadTemplates)
                                                   .Where(x => x.SymbolDetails.Randomize)
                                                   .Select(x => x.SymbolDetails)
                                                   .Actualize();

            // NOTE*** ALL RANDOMIZED SYMBOLS MUST HAVE A VALID CATEGORY (See Scenario Configuration Validator)
            foreach (var symbolDetails in randomizableSymbols)
            {
                // Get symbol pool for this category
                var categorySymbolPool = configuration.SymbolPool.FirstOrDefault(x => x.SymbolPoolCategory == symbolDetails.SymbolPoolCategory);

                if (categorySymbolPool == null)
                    throw new Exception("Symbol Pool Not Created for one of the symbol categories");

                // Get all asset symbols for this category
                var assetCategorySymbols = randomizableSymbols.Where(x => x.SymbolPoolCategory == symbolDetails.SymbolPoolCategory);

                // Map a random symbol's properties onto this one
                mapSymbolFunction(symbolDetails, assetCategorySymbols, categorySymbolPool);
            }
        }
    }
}
