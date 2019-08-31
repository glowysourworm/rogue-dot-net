using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Service.Interface;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Logic.Algorithm.Interface;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Extension;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout.Interface;
using Rogue.NET.Core.Model.Scenario.Dynamic.Content.Interface;
using Rogue.NET.Core.Model.Scenario.Dynamic.Content;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout;

namespace Rogue.NET.Core.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IModelService))]
    public class ModelService : IModelService
    {
        readonly IRayTracer _rayTracer;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IAttackAttributeGenerator _attackAttributeGenerator;

        // Dynamic (non-serialized) data about line-of-sight / visible line-of-sight / aura line-of-sight
        // per character (These must be re-created each level)
        ICharacterLayoutInformation _characterLayoutInformation;
        ICharacterContentInformation _characterContentInformation;

        // Collection of targeted enemies
        IList<Enemy> _targetedEnemies;

        // Enemy to have slain the player
        string _killedBy;

        [ImportingConstructor]
        public ModelService(
                IRayTracer rayTracer, 
                IRandomSequenceGenerator randomSequenceGenerator, 
                IAttackAttributeGenerator attackAttributeGenerator)
        {
            _rayTracer = rayTracer;
            _randomSequenceGenerator = randomSequenceGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;

            _targetedEnemies = new List<Enemy>();
        }

        public void Load(
            Player player, 
            PlayerStartLocation startLocation,
            Level level, 
            IDictionary<string, ScenarioMetaData> encyclopedia, 
            ScenarioConfigurationContainer configuration)
        {
            this.Level = level;
            this.Player = player;
            this.ScenarioEncyclopedia = encyclopedia;
            this.ScenarioConfiguration = configuration;
            this.CharacterClasses = configuration.PlayerTemplates.Select(x => new ScenarioImage()
            {
                // TODO: Set the Character Class to PlayerTemplate.Class
                RogueName = x.Name,
                CharacterColor = x.SymbolDetails.CharacterColor,
                CharacterSymbol = x.SymbolDetails.CharacterSymbol,
                DisplayIcon = x.SymbolDetails.DisplayIcon,
                Icon = x.SymbolDetails.Icon,
                SmileyLightRadiusColor = x.SymbolDetails.SmileyAuraColor,
                SymbolType = x.SymbolDetails.Type,
                SmileyBodyColor = x.SymbolDetails.SmileyBodyColor,
                SmileyLineColor = x.SymbolDetails.SmileyLineColor,
                SmileyExpression = x.SymbolDetails.SmileyExpression

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

            _characterLayoutInformation = new CharacterLayoutInformation(this.Level.Grid, _rayTracer);
            _characterContentInformation = new CharacterContentInformation(_characterLayoutInformation);

            UpdateVisibility();
        }

        public void Unload()
        {
            if (this.Level != null)
                this.Level.Dispose();

            _characterContentInformation = null;
            _characterLayoutInformation = null;

            this.Level = null;
            this.Player = null;
            this.ScenarioConfiguration = null;
            this.ScenarioEncyclopedia = null;
            this.CharacterClasses = null;

            _targetedEnemies = new List<Enemy>();
        }

        public Level Level { get; private set; }

        public Player Player { get; private set; }

        public IDictionary<string, ScenarioMetaData> ScenarioEncyclopedia { get; private set; }

        public ScenarioConfigurationContainer ScenarioConfiguration { get; private set; }

        public IEnumerable<ScenarioImage> CharacterClasses { get; private set; }

        public IEnumerable<AttackAttribute> AttackAttributes
        {
            get
            {
                return this.ScenarioConfiguration
                      .AttackAttributes
                      .Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x))
                      .Actualize();
            }
        }

        public string GetDisplayName(ScenarioObject scenarioObject)
        {
            // TODO - HANDLE PROPER NOUNS (Example:  Player (assumed proper noun), Enemy that is Unique

            if (scenarioObject is Player)
                return scenarioObject.RogueName;

            return this.ScenarioEncyclopedia[scenarioObject.RogueName].IsIdentified ? 
                            scenarioObject.RogueName : 
                            ModelConstants.UnIdentifiedDisplayName;
        }

        public string GetDisplayName(ScenarioImage scenarioImage)
        {
            if (scenarioImage is Player)
                return scenarioImage.RogueName;

            return this.ScenarioEncyclopedia[scenarioImage.RogueName].IsIdentified ?
                            scenarioImage.RogueName :
                            ModelConstants.UnIdentifiedDisplayName;
        }

        public IEnumerable<Enemy> GetTargetedEnemies()
        {
            return _targetedEnemies;
        }
        public void ClearTargetedEnemies()
        {
            _targetedEnemies.Clear();
        }
        public string GetKilledBy()
        {
            return _killedBy;
        }
        public void SetKilledBy(string killedBy)
        {
            _killedBy = killedBy;
        }

        public bool IsVisibleTo(Character sourceCharacter, ScenarioObject scenarioObject)
        {
            return _characterContentInformation.GetVisibleContents(sourceCharacter)
                                               .Contains(scenarioObject);
        }

        public IEnumerable<GridLocation> GetVisibleLocations(Character character)
        {
            return _characterLayoutInformation.GetVisibleLocations(character);
        }

        public IEnumerable<GridLocation> GetLineOfSightLocations(Character character)
        {
            return _characterLayoutInformation.GetLineOfSightLocations(character);
        }

        public IEnumerable<GridLocation> GetAuraLocations(Character character, string alterationEffectId)
        {
            return _characterLayoutInformation.GetAuraAffectedLocations(character, alterationEffectId);
        }

        public IEnumerable<Character> GetVisibleCharacters(Character character)
        {
            return _characterContentInformation.GetVisibleCharacters(character);
        }
        public IEnumerable<GridLocation> GetExploredLocations()
        {
            return _characterLayoutInformation.GetExploredLocations();
        }
        public IEnumerable<GridLocation> GetRevealedLocations()
        {
            return _characterLayoutInformation.GetRevealedLocations();
        }

        public void SetTargetedEnemy(Enemy enemy)
        {
            if (!_targetedEnemies.Contains(enemy))
                _targetedEnemies.Add(enemy);
        }
        public void UpdateVisibility()
        {
            // Apply blanket update for layout visibiltiy
            _characterLayoutInformation
                .ApplyUpdate(this.Level
                                 .Enemies
                                 .Cast<Character>()
                                 .Union(new Character[] { this.Player }));

            // Apply blanket update for contents
            _characterContentInformation.ApplyUpdate(this.Level, this.Player);
        }
    }
}
