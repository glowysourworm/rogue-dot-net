using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Dynamic.Content;
using Rogue.NET.Core.Model.Scenario.Dynamic.Content.Interface;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Design;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm.Interface;
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

        readonly IVisibilityCalculator _visibilityCalculator;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly ISymbolDetailsGenerator _symbolDetailsGenerator;

        // Stored configuration for the scenario
        ScenarioConfigurationContainer _configuration;

        // Dynamic (non-serialized) data about line-of-sight / visible line-of-sight / aura line-of-sight
        // per character (These must be re-created each level)
        ICharacterLayoutInformation _characterLayoutInformation;
        ICharacterContentInformation _characterContentInformation;

        // Stateful sub-component to provide layout calculations using the loaded level
        IModelLayoutService _modelLayoutService;

        // Enemy to have slain the player
        string _killedBy;

        public ICharacterLayoutInformation CharacterLayoutInformation { get { return _characterLayoutInformation; } }
        public ICharacterContentInformation CharacterContentInformation { get { return _characterContentInformation; } }
        public IModelLayoutService LayoutService { get { return _modelLayoutService; } }

        [ImportingConstructor]
        public ModelService(
                IVisibilityCalculator visibilityCalculator,
                IRandomSequenceGenerator randomSequenceGenerator,
                ISymbolDetailsGenerator symbolDetailsGenerator)
        {
            _visibilityCalculator = visibilityCalculator;
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

            level.Load(player, injectedContents);

            // Have to provide locations for the injected contents
            foreach (var content in injectedContents)
            {
                // TODO: This doesn't currently have any specification other than 
                //       type:
                //      
                //       Friendly:  starts at Player location
                if (content is Friendly)
                {
                    // Set Friendly Location
                    content.Location = player.Location;

                    // Add Friendly to Level
                    level.AddContent(content);
                }

                else
                    throw new Exception("Unhandled injected content type");
            }

            _characterLayoutInformation = new CharacterLayoutInformation(this.Level.Grid, _visibilityCalculator);
            _characterContentInformation = new CharacterContentInformation(_characterLayoutInformation);
            _modelLayoutService = new ModelLayoutService(level, player, _randomSequenceGenerator);

            // Calculate player start location
            switch (startLocation)
            {
                case PlayerStartLocation.SavePoint:
                    if (level.HasSavePoint())
                        player.Location = level.GetSavePoint().Location;
                    else
                        player.Location = level.GetStairsUp().Location;
                    break;
                case PlayerStartLocation.StairsUp:
                    player.Location = level.GetStairsUp().Location;
                    break;
                case PlayerStartLocation.StairsDown:
                    player.Location = level.GetStairsDown().Location;
                    break;
                case PlayerStartLocation.Random:
                    player.Location = _modelLayoutService.GetRandomLocation(true);
                    break;
            }

            UpdateVisibility();
        }

        public IEnumerable<ScenarioObject> Unload()
        {
            // Run Level Unload Process (Removes Temporary Characters / Returns Extractable Content)
            IEnumerable<ScenarioObject> extractedContent = this.Level.Unload();

            _configuration = null;

            _characterContentInformation = null;
            _characterLayoutInformation = null;
            _modelLayoutService = null;

            this.Level = null;
            this.Player = null;
            this.ScenarioEncyclopedia = null;

            return extractedContent;
        }

        public bool IsLoaded { get { return this.Level != null; } }

        public Level Level { get; private set; }

        public Player Player { get; private set; }

        public double ZoomFactor { get; set; }

        public ScenarioEncyclopedia ScenarioEncyclopedia { get; private set; }

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
            _modelLayoutService.CalculateEffectiveLighting();

            // Apply blanket update for layout visibiltiy
            _characterLayoutInformation
                .ApplyUpdate(this.Level
                                 .NonPlayerCharacters
                                 .Cast<Character>()
                                 .Union(new Character[] { this.Player }));

            // Apply blanket update for contents
            _characterContentInformation.ApplyUpdate(this.Level, this.Player);
        }
    }
}
