using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Extension;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout.Interface;
using Rogue.NET.Core.Model.Scenario.Dynamic.Content.Interface;
using Rogue.NET.Core.Model.Scenario.Dynamic.Content;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout;
using System.Text;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Processing.Model.Algorithm.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using Rogue.NET.Core.Model.ScenarioConfiguration.Design;

namespace Rogue.NET.Core.Processing.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IModelService))]
    public class ModelService : IModelService
    {
        static readonly string INFINITY = Encoding.UTF8.GetString(new byte[] { 0xE2, 0x88, 0x9E });

        readonly IRayTracer _rayTracer;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly ISymbolDetailsGenerator _symbolDetailsGenerator;
        readonly IAttackAttributeGenerator _attackAttributeGenerator;

        // Stored configuration for the scenario
        ScenarioConfigurationContainer _configuration;

        // Dynamic (non-serialized) data about line-of-sight / visible line-of-sight / aura line-of-sight
        // per character (These must be re-created each level)
        ICharacterLayoutInformation _characterLayoutInformation;
        ICharacterContentInformation _characterContentInformation;

        // UI Parameters
        double _zoomFactor;

        // Enemy to have slain the player
        string _killedBy;

        public ICharacterLayoutInformation CharacterLayoutInformation { get { return _characterLayoutInformation; } }
        public ICharacterContentInformation CharacterContentInformation { get { return _characterContentInformation; } }

        [ImportingConstructor]
        public ModelService(
                IRayTracer rayTracer, 
                IRandomSequenceGenerator randomSequenceGenerator, 
                ISymbolDetailsGenerator symbolDetailsGenerator,
                IAttackAttributeGenerator attackAttributeGenerator)
        {
            _rayTracer = rayTracer;
            _randomSequenceGenerator = randomSequenceGenerator;
            _symbolDetailsGenerator = symbolDetailsGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;
        }

        public void Load(
            Player player, 
            PlayerStartLocation startLocation,
            Level level, 
            double zoomFactor,
            IEnumerable<ScenarioObject> injectedContents,
            IDictionary<string, ScenarioMetaData> encyclopedia, 
            ScenarioConfigurationContainer configuration)
        {
            _configuration = configuration;

            this.Level = level;
            this.Player = player;
            this.ZoomFactor = zoomFactor;
            this.ScenarioEncyclopedia = encyclopedia;
            this.CharacterClasses = configuration.PlayerTemplates.Select(x =>
            {
                var result = new ScenarioImage();

                result.RogueName = x.Name;

                _symbolDetailsGenerator.MapSymbolDetails(x.SymbolDetails, result);

                return result;

            }).Actualize();

            switch (startLocation)
            {
                case PlayerStartLocation.SavePoint:
                    if (level.HasSavePoint)
                        player.Location = level.SavePoint.Location;
                    else
                        player.Location = level.StairsUp.Location;
                    break;
                case PlayerStartLocation.StairsUp:
                    player.Location = level.StairsUp.Location;
                    break;
                case PlayerStartLocation.StairsDown:
                    player.Location = level.StairsDown.Location;
                    break;
                case PlayerStartLocation.Random:
                    player.Location = level.GetRandomLocation(true, _randomSequenceGenerator);
                    break;
            }

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

            _characterLayoutInformation = new CharacterLayoutInformation(this.Level.Grid, _rayTracer);
            _characterContentInformation = new CharacterContentInformation(_characterLayoutInformation);

            UpdateVisibility();
        }

        public IEnumerable<ScenarioObject> Unload()
        {
            // Run Level Unload Process (Removes Temporary Characters / Returns Extractable Content)
            IEnumerable<ScenarioObject> extractedContent = this.Level.Unload();

            _configuration = null;

            _characterContentInformation = null;
            _characterLayoutInformation = null;

            this.Level = null;
            this.Player = null;            
            this.ScenarioEncyclopedia = null;
            this.CharacterClasses = null;

            return extractedContent;
        }

        public bool IsLoaded { get { return this.Level != null; } }

        public Level Level { get; private set; }

        public Player Player { get; private set; }

        public double ZoomFactor { get; set; }

        public IDictionary<string, ScenarioMetaData> ScenarioEncyclopedia { get; private set; }

        public IEnumerable<ScenarioImage> CharacterClasses { get; private set; }

        public IEnumerable<AttackAttribute> AttackAttributes
        {
            get
            {
                return _configuration
                      .AttackAttributes
                      .Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x))
                      .Actualize();
            }
        }

        public LevelBranchTemplate GetLevelBranch()
        {
            // TODO: Change this to use Guid to identify branch
            return _configuration.ScenarioDesign
                                 .LevelDesigns
                                 .SelectMany(x => x.LevelBranches)
                                 .First(x => x.LevelBranch.Name == this.Level.LevelBranchName)
                                 .LevelBranch;
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
