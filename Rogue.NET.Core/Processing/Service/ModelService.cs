using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Design;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Service.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Rogue.NET.Core.Processing.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IModelService))]
    public class ModelService : IModelService
    {
        static readonly string INFINITY = Encoding.UTF8.GetString(new byte[] { 0xE2, 0x88, 0x9E });

        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly ISymbolDetailsGenerator _symbolDetailsGenerator;

        // Stored configuration for the scenario
        ScenarioConfigurationContainer _configuration;

        // Enemy to have slain the player
        string _killedBy;

        [ImportingConstructor]
        public ModelService(
                IRandomSequenceGenerator randomSequenceGenerator,
                ISymbolDetailsGenerator symbolDetailsGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _symbolDetailsGenerator = symbolDetailsGenerator;
        }

        public void Load(
            Player player,
            PlayerStartLocation startLocation,
            Level level,
            double zoomFactor,
            IEnumerable<ScenarioObject> injectedContents,
            ScenarioEncyclopedia encyclopedia,
            ScenarioConfigurationContainer configuration)
        {
            _configuration = configuration;

            this.Level = level;
            this.Player = player;
            this.ZoomFactor = zoomFactor;
            this.ScenarioEncyclopedia = encyclopedia;

            GridLocation location;

            // Calculate player start location
            switch (startLocation)
            {
                case PlayerStartLocation.SavePoint:
                    if (level.HasSavePoint())
                        location = level.Content[level.GetSavePoint()];
                    else
                        location = level.Content[level.GetStairsUp()];
                    break;
                case PlayerStartLocation.StairsUp:
                    location = level.Content[level.GetStairsUp()];
                    break;
                case PlayerStartLocation.StairsDown:
                    location = level.Content[level.GetStairsDown()];
                    break;
                case PlayerStartLocation.Random:
                    location = level.Grid.GetNonOccupiedLocation(LayoutGrid.LayoutLayer.Placement, _randomSequenceGenerator, new GridLocation[] { });
                    break;
                default:
                    throw new Exception("Unhandled player start location");
            }

            // Load the level
            level.Load(player, location, injectedContents);

            UpdateVisibility();
        }

        public IEnumerable<ScenarioObject> Unload()
        {
            // Run Level Unload Process (Removes Temporary Characters / Returns Extractable Content)
            IEnumerable<ScenarioObject> extractedContent = this.Level.Unload();

            _configuration = null;

            this.Level = null;
            this.Player = null;
            this.ScenarioEncyclopedia = null;

            return extractedContent;
        }

        public bool IsLoaded { get { return this.Level != null; } }

        public Level Level { get; private set; }

        public Player Player { get; private set; }

        public GridLocation PlayerLocation { get { return this.Level.Content[this.Player]; } }

        public double ZoomFactor { get; set; }

        public ScenarioEncyclopedia ScenarioEncyclopedia { get; private set; }

        public GridLocation GetContentLocation(ScenarioObject scenarioObject)
        {
            return this.Level.Content[scenarioObject];
        }

        public IEnumerable<EnemyGenerationTemplate> GetEnemyTemplates()
        {
            // TODO: STORE AS PARAMETERS; OR A SEPARATE LOOKUP.. JUST CLEAN THIS UP!
            return _configuration.ScenarioDesign
                                 .LevelDesigns
                                 .SelectMany(design => design.LevelBranches)
                                 .First(branch => branch.Name == this.Level.Parameters.LevelBranchName)
                                 .LevelBranch
                                 .Enemies
                                 .Actualize();
        }

        public LayoutTemplate GetLayoutTemplate()
        {
            // TODO: STORE AS PARAMETERS; OR A SEPARATE LOOKUP.. JUST CLEAN THIS UP!
            return _configuration.LayoutTemplates.First(layout => layout.Name == this.Level.Parameters.LayoutName);
        }

        public void GetPlayerAdvancementParameters(ref double hpPerPoint, ref double staminaPerPoint,
                                                   ref double strengthPerPoint, ref double agilityPerPoint,
                                                   ref double intelligencePerPoint, ref int skillPointsPerPoint)
        {
            hpPerPoint = _configuration.ScenarioDesign.HpPerCharacterPoint;
            staminaPerPoint = _configuration.ScenarioDesign.StaminaPerCharacterPoint;
            strengthPerPoint = _configuration.ScenarioDesign.StrengthPerCharacterPoint;
            agilityPerPoint = _configuration.ScenarioDesign.AgilityPerCharacterPoint;
            intelligencePerPoint = _configuration.ScenarioDesign.IntelligencePerCharacterPoint;
            skillPointsPerPoint = _configuration.ScenarioDesign.SkillPointsPerCharacterPoint;
        }

        public string GetScenarioName()
        {
            return _configuration.ScenarioDesign.Name;
        }
        public string GetScenarioDescription()
        {
            return _configuration.ScenarioDesign.ObjectiveDescription;
        }

        public int GetNumberOfLevels()
        {
            return _configuration.ScenarioDesign.LevelDesigns.Count;
        }
        public string GetDisplayName(ScenarioObject scenarioObject)
        {
            // TODO - HANDLE PROPER NOUNS (Example:  Player (assumed proper noun), Enemy that is Unique

            var noun = scenarioObject.RogueName;

            // Player not in the Scenario Encyclopedia
            if (scenarioObject is Player)
                return noun;

            var identified = this.ScenarioEncyclopedia[scenarioObject.RogueName].IsIdentified;

            return identified ?
                        noun :
                        ModelConstants.UnIdentifiedDisplayName;
        }

        public string GetDisplayName(ScenarioImage scenarioImage)
        {
            // TODO - HANDLE PROPER NOUNS (Example:  Player (assumed proper noun), Enemy that is Unique

            var noun = scenarioImage.RogueName;

            // Player not in the Scenario Encyclopedia
            if (scenarioImage is Player)
                return noun;

            var identified = this.ScenarioEncyclopedia[scenarioImage.RogueName].IsIdentified;

            return identified ?
                        noun :
                        ModelConstants.UnIdentifiedDisplayName;
        }

        public string GetKilledBy()
        {
            return _killedBy;
        }

        public void SetKilledBy(string killedBy)
        {
            _killedBy = killedBy;
        }

        public void UpdateVisibility()
        {
            // Calculate effective lighting for each cell
            this.Level.CalculateEffectiveLighting();

            // Apply blanket update for layout visibiltiy
            foreach (var character in this.Level.Content.NonPlayerCharacters)
                this.Level.Movement.Update(character, this.Level.Content[character]);

            this.Level.Movement.Update(this.Player, this.Level.Content[this.Player]);

            // TODO: COMPONENTIZE THIS NICELY

            // Calculate visible contents
            var visibleLocations = this.Level.Movement.GetVisibleLocations();
            var visibleContent = this.Level.Content.GetManyAt<ScenarioObject>(visibleLocations);

            // Update Memorized Contents
            this.Level.UpdateMemorizedContent(visibleLocations);

            // Visible content has to be updated for the IsExplored / IsRevealed flags
            foreach (var scenarioObject in visibleContent)
            {
                // Set this based on whether the cell is physically visible. Once the cell is seen
                // the IsRevealed flag gets reset. Also, the IsDetected flag gets reset. 
                scenarioObject.IsRevealed = false;
                scenarioObject.IsDetectedAlignment = false;
                scenarioObject.IsDetectedCategory = false;
            }
        }
    }
}
