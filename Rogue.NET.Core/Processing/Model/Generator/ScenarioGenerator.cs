using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Abstract;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Skill.Extension;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Service.Interface;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioGenerator))]
    public class ScenarioGenerator : IScenarioGenerator
    {
        readonly ILayoutGenerator _layoutGenerator;
        readonly IContentGenerator _contentGenerator;
        readonly ICharacterGenerator _characterGenerator;
        readonly IAttackAttributeGenerator _attackAttributeGenerator;
        readonly IScenarioMetaDataGenerator _scenarioMetaDataGenerator;
        readonly ISymbolDetailsGenerator _symbolDetailsGenerator;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IScenarioResourceService _scenarioResourceService;

        [ImportingConstructor]
        public ScenarioGenerator(
            ILayoutGenerator layoutGenerator,
            IContentGenerator contentGenerator,
            ICharacterGenerator characterGenerator,
            IAttackAttributeGenerator attackAttributeGenerator,
            IScenarioMetaDataGenerator scenarioMetaDataGenerator,
            ISymbolDetailsGenerator symbolDetailsGenerator,
            IRandomSequenceGenerator randomSequenceGenerator,
            IScenarioResourceService scenarioResourceService)
        {
            _layoutGenerator = layoutGenerator;
            _contentGenerator = contentGenerator;
            _characterGenerator = characterGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;
            _scenarioMetaDataGenerator = scenarioMetaDataGenerator;
            _symbolDetailsGenerator = symbolDetailsGenerator;
            _randomSequenceGenerator = randomSequenceGenerator;
            _scenarioResourceService = scenarioResourceService;
        }

        public ScenarioContainer CreateScenario(ScenarioConfigurationContainer configuration, string rogueName, string characterClassName, int seed, bool survivorMode)
        {
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

            // Create static scenario data for generation
            var characterClasses = CreateCharacterClasses(configuration);
            var alterationCategories = CreateAlterationCategories(configuration);
            var encyclopediaDict = CreateEncyclopediaData(configuration);
            var encyclopedia = new ScenarioEncyclopedia(encyclopediaDict, characterClasses, alterationCategories);

            // Generate Player
            var player = _characterGenerator.GeneratePlayer(configuration.PlayerTemplates.First(x => x.Name == characterClassName), encyclopedia);

            // Set Player Name
            player.RogueName = rogueName;

            // Select path through the scenario (level branch sequence)
            var levelBranches = configuration.ScenarioDesign
                                             .LevelDesigns
                                             .Select(x => _randomSequenceGenerator.GetWeightedRandom(x.LevelBranches, z => z.GenerationWeight))
                                             .Select(x => x.LevelBranch)
                                             .ToList();

            // NOTE***  SOME SHARED THREAD DATA. Need to spawn threads for levels that have NON-SHARED data in parallel.
            //          Shared assets are ALL THE PUBLIC PROPERTIES in the ScenarioGeneratorThreadData.
            //
            //          So, select several with distinct pointers to run in parallel. Repeat until complted.
            //
            //          Content generation WRITES to properties - so is NOT THREAD SAFE.

            var useLevelCompression = true;
            // TODO: SUPPORT MULTI-THREADED RANDOM SEQUENCES
            var maxThreadCount = 1;         
            var runningThreads = new List<Thread>();
            var threads = new SimpleDictionary<ScenarioGeneratorThreadData, Thread>();
            var completedThreadData = new ConcurrentDictionary<string, ScenarioGeneratorThreadData>();

            // NOTE*** IRandomSequenceGenerator is shared - but has a multi-threading policy that 
            //         helps guarantee repeatability.
            //
            for (int index = 0; index < levelBranches.Count; index++)
            {
                // Create worker thread method
                var thread = new Thread(new ParameterizedThreadStart((threadData) =>
                {
                    var scenarioThreadData = (ScenarioGeneratorThreadData)threadData;

                    // Generate the level grid
                    var levelGrid = _layoutGenerator.CreateLayout(scenarioThreadData.Layout);

                    // Create a new level object
                    var level = new Level(scenarioThreadData.Layout, scenarioThreadData.LevelBranch, levelGrid, scenarioThreadData.LevelNumber);

                    // Create temp file for the level
                    var fileName = _scenarioResourceService.SaveToTempFile(level, useLevelCompression);

                    completedThreadData.TryAdd(fileName, scenarioThreadData);
                }));

                // Set up thread start data (MAKE A COPY TO DECOUPLE)
                var branchCopy = ObjectExtension.Clone(levelBranches[index]);

                // Get weighted random layout template (MAKE A COPY TO DECOUPLE)
                var generationTemplateCopy = ObjectExtension.Clone(_randomSequenceGenerator.GetWeightedRandom(branchCopy.Layouts, template => template.GenerationWeight));

                // TODO: Figure out proper setting
                thread.Priority = ThreadPriority.Highest;
                // thread.IsBackground = true;

                // COPY SHARED ASSETS TO DECOUPLE
                threads.Add(new ScenarioGeneratorThreadData()
                {
                    LevelNumber = index + 1,
                    HasStartedGeneration = false,
                    Layout = generationTemplateCopy.Asset,
                    LayoutGeneration = generationTemplateCopy,
                    LevelBranch = branchCopy

                }, thread);
            }

            // Run until all threads have ceased
            while (threads.Count(thread => !thread.Key.HasStartedGeneration) > 0 ||
                   runningThreads.Count > 0)
            {
                // Check running thread states
                for (int index = runningThreads.Count - 1; index >= 0; index--)
                {
                    var thread = runningThreads[index];

                    if (thread.ThreadState == ThreadState.Stopped)
                    {
                        runningThreads.RemoveAt(index);

                        // Perhaps don't need to join here (?)
                        thread.Join();

                        // GARBAGE COLLECTOR
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                    }
                }

                // Spawn new threads if there are any left that haven't started (AND)
                // All the running threads have ceased.
                //
                while (runningThreads.Count < maxThreadCount &&
                       threads.Count(thread => !thread.Key.HasStartedGeneration) > 0)
                {
                    // Need to join on ALL threads that haven't been started yet
                    var nextKey = threads.Keys
                                         .First(x => !x.HasStartedGeneration);

                    var thread = threads[nextKey];

                    // Add to running threads
                    runningThreads.Add(thread);

                    // Mark the key as generated
                    nextKey.HasStartedGeneration = true;

                    // Start layout generation
                    thread.Start(nextKey);
                }

                Thread.Sleep(100);
            }

            // Use IScenarioResourceService to complete serialization -> Delete temp file for the level
            var scenarioFileEntry = _scenarioResourceService.CreateScenarioEntry(rogueName, configuration.ScenarioDesign.Name, seed);

            Level firstLevel = null;

            // Run content creation on main thread
            foreach (var levelDataPair in completedThreadData.OrderBy(x => x.Value.LevelNumber))
            {
                var levelFileName = levelDataPair.Key;
                var levelData = levelDataPair.Value;

                // Load level from temp data storage
                var level = _scenarioResourceService.LoadFromTempFile<Level>(levelFileName, useLevelCompression);
                var lastLevel = (levelData.LevelNumber == levelBranches.Count);

                // Complete level content
                var completedLevel = _contentGenerator.CreateContents(level,
                                                                      levelData.LevelBranch,
                                                                      levelData.LayoutGeneration,
                                                                      encyclopedia,
                                                                      lastLevel,
                                                                      survivorMode);

                // SERIALIZING BY HAND - SEE ScenarioContainer METHODS
                scenarioFileEntry.AddOrUpdate("Level " + level.Parameters.Number.ToString(), completedLevel);

                // Delete Temp File
                _scenarioResourceService.DeleteTempFile(levelFileName);

                // Save first level
                if (levelData.LevelNumber == 1)
                    firstLevel = completedLevel;
            }

            // PRIMARY SCENARIO CONTAINER
            var scenario = new ScenarioContainer(configuration, encyclopedia, player, firstLevel, seed, survivorMode);

            // Identify player skills / equipment / consumables / and character classes
            foreach (var skillSet in scenario.Player.SkillSets)
            {
                scenario.Encyclopedia[skillSet.RogueName].IsIdentified = true;

                // Also setup skill AreRequirementsMet flag based on player level for skills that have zero cost
                foreach (var skill in skillSet.Skills)
                    skill.AreRequirementsMet = skill.AreRequirementsMet(scenario.Player);
            }

            foreach (var equipment in scenario.Player.Equipment.Values)
            {
                scenario.Encyclopedia[equipment.RogueName].IsIdentified = true;
                equipment.IsIdentified = true;
            }

            foreach (var consumable in scenario.Player.Consumables.Values)
            {
                scenario.Encyclopedia[consumable.RogueName].IsIdentified = true;
                consumable.IsIdentified = true;
            }

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
                    }
                }
            }

            // Finally, save rest of scenario parametrs - SEE ScenarioContainer !!! (CurrentLevel is omitted)
            scenarioFileEntry.AddOrUpdate("Detail", scenario.Detail);
            scenarioFileEntry.AddOrUpdate("Player", scenario.Player);
            scenarioFileEntry.AddOrUpdate("Configuration", scenario.Configuration);
            scenarioFileEntry.AddOrUpdate("Encyclopedia", scenario.Encyclopedia);
            scenarioFileEntry.AddOrUpdate("Statistics", scenario.Statistics);

            return scenario;
        }

        /// <summary>
        /// Creates Scenario MetaData and INITIALIZES IsIdentified FLAGS FOR SCENARIO
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private SimpleDictionary<string, ScenarioMetaData> CreateEncyclopediaData(ScenarioConfigurationContainer configuration)
        {
            SimpleDictionary<string, ScenarioMetaData> encyclopedia = new SimpleDictionary<string, ScenarioMetaData>();

            //Load Encyclopedia Rogue-Tanica (Consumables)
            foreach (var template in configuration.ConsumableTemplates)
                encyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Equipment)
            foreach (var template in configuration.EquipmentTemplates)
                encyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Enemies)
            foreach (var template in configuration.EnemyTemplates)
                encyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Friendlies)
            foreach (var template in configuration.FriendlyTemplates)
                encyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Doodads)
            foreach (var template in configuration.DoodadTemplates)
                encyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Skill Sets)
            foreach (var template in configuration.SkillTemplates)
                encyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));

            //Load Encyclopedia Rogue-Tanica (Normal Doodads)
            encyclopedia.Add(ModelConstants.DoodadSavePointRogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.SavePoint));
            encyclopedia.Add(ModelConstants.DoodadStairsDownRogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.StairsDown));
            encyclopedia.Add(ModelConstants.DoodadStairsUpRogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.StairsUp));
            encyclopedia.Add(ModelConstants.DoodadTransporterRogueName, _scenarioMetaDataGenerator.CreateScenarioMetaData(DoodadNormalType.Transporter));

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

                        encyclopedia.Add(template.Name, metaData);
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
                        encyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));
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
                        encyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));
                    }
                }
                if (doodad.IsInvoked)
                {
                    if (doodad.InvokedAlteration.Effect is CreateTemporaryCharacterAlterationEffectTemplate)
                    {
                        // Add the Temporary Character Template Meta-data
                        var template = (doodad.InvokedAlteration.Effect as CreateTemporaryCharacterAlterationEffectTemplate).TemporaryCharacter;
                        encyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));
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
                        encyclopedia.Add(template.Name, _scenarioMetaDataGenerator.CreateScenarioMetaData(template));
                    }
                }
            }

            return encyclopedia;
        }

        private IEnumerable<ScenarioImage> CreateCharacterClasses(ScenarioConfigurationContainer configuration)
        {
            return configuration.PlayerTemplates.Select(x =>
            {
                var result = new ScenarioImage();

                result.RogueName = x.Name;

                _symbolDetailsGenerator.MapSymbolDetails(x.SymbolDetails, result);

                return result;

            }).Actualize();
        }

        private IEnumerable<AlterationCategory> CreateAlterationCategories(ScenarioConfigurationContainer configuration)
        {
            return configuration.AlterationCategories.Select(x =>
            {
                var result = new AlterationCategory();

                result.RogueName = x.Name;
                result.AlignmentType = x.AlignmentType;

                _symbolDetailsGenerator.MapSymbolDetails(x.SymbolDetails, result);

                return result;
            }).Actualize();
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

            var usedSymbols = new SimpleDictionary<string, List<SymbolDetailsTemplate>>();

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
