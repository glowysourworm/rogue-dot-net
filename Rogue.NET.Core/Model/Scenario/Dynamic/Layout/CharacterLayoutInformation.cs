using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Algorithm.Interface;

using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Layout
{
    public class CharacterLayoutInformation : ICharacterLayoutInformation
    {
        readonly Level _level;
        readonly IVisibilityCalculator _visibilityCalculator;

        Dictionary<CharacterBase, IEnumerable<DistanceLocation>> _visibleDict;
        Dictionary<CharacterBase, IEnumerable<DistanceLocation>> _lineOfSightDict;
        Dictionary<CharacterBase, Dictionary<string, AuraInformation>> _auraDict;
        Dictionary<CharacterBase, Dictionary<CharacterBase, GridLocation>> _characterPathDict;

        List<GridLocation> _exploredLocations;
        List<GridLocation> _revealedLocations;

        #region Nested Classes
        protected class AuraInformation
        {
            public string Color { get; set; }
            public IEnumerable<DistanceLocation> AffectedLocations { get; set; }
        }
        #endregion

        /// <summary>
        /// Constructor for the CharacterLayoutInformation should be called once per
        /// level and updated on each turn.
        /// </summary>
        public CharacterLayoutInformation(Level level, IVisibilityCalculator visibilityCalculator)
        {
            _level = level;
            _visibilityCalculator = visibilityCalculator;

            _visibleDict = new Dictionary<CharacterBase, IEnumerable<DistanceLocation>>();
            _lineOfSightDict = new Dictionary<CharacterBase, IEnumerable<DistanceLocation>>();
            _auraDict = new Dictionary<CharacterBase, Dictionary<string, AuraInformation>>();
            _characterPathDict = new Dictionary<CharacterBase, Dictionary<CharacterBase, GridLocation>>();

            _exploredLocations = new List<GridLocation>();
            _revealedLocations = new List<GridLocation>();
        }

        public void ApplyUpdate(IEnumerable<CharacterBase> characters)
        {
            _visibleDict.Clear();
            _lineOfSightDict.Clear();
            _auraDict.Clear();

            _exploredLocations.Clear();
            _revealedLocations.Clear();

            // Create Line-of-Sight to draw other data from
            foreach (var character in characters)
            {
                // Fetch character location from the level
                var characterLocation = _level.Content[character];

                // Calculate visible locations
                var visibleLocations = _visibilityCalculator.CalculateVisibility(_level.Grid, characterLocation);

                // TODO:TERRAIN - RE-CALCULATE VISIBLE / LINE-OF-SIGHT WITH NEW VISION PARAMETER
                _visibleDict.Add(character, visibleLocations);

                // TODO:TERRAIN - RE-CALCULATE VISIBLE / LINE-OF-SIGHT WITH NEW VISION PARAMETER
                //_lineOfSightDict.Add(character, lineOfSightLocations);
                _lineOfSightDict.Add(character, visibleLocations);

                // Player - Calculate Revealed / Explored locations
                //
                // TODO: Consider better design because this does work on the
                //       LevelGrid and it's supposed to just be a calculation.
                if (character is Player)
                {
                    // Visible Cells -> Explored / No Longer Revealed
                    foreach (var location in visibleLocations.Select(x => x.Location))
                    {
                        var cell = _level.Grid[location];

                        cell.IsExplored = true;
                        cell.IsRevealed = false;
                    }
                }

                // Instantiate the aura collection to empty
                _auraDict.Add(character, new Dictionary<string, AuraInformation>());

                // Auras
                foreach (var aura in character.Alteration.GetAuras())
                {
                    _auraDict[character].Add(aura.Item1.Id, new AuraInformation()
                    {
                        AffectedLocations = _lineOfSightDict[character].Where(x =>
                        {
                            // NOTE*** Using Euclidean distance to calculate auras
                            return x.EuclideanDistance <= aura.Item2.AuraRange;
                        }),
                        Color = aura.Item2.AuraColor
                    });
                }
            }

            _exploredLocations = _level.Grid
                                       .FullMap
                                       .GetLocations()
                                       .Where(x => _level.Grid[x].IsExplored)
                                       .ToList();

            _revealedLocations = _level.Grid
                                       .FullMap
                                       .GetLocations()
                                       .Where(x => _level.Grid[x].IsRevealed)
                                       .ToList();
        }

        public void CalculateCharacterPaths()
        {
            // CLEAR OUT OLD PATH DICTIONARY
            _characterPathDict.Clear();

            // Calculate player and enemy aligned characters
            var playerFaction = _level.Content
                                     .Characters
                                     .Where(character => character is Player ||
                                                       ((character as NonPlayerCharacter).AlignmentType == CharacterAlignmentType.PlayerAligned))
                                     .Actualize();

            var enemyFaction = _level.Content
                                    .Characters
                                    .Where(character => !(character is Player) &&
                                                        ((character as NonPlayerCharacter).AlignmentType == CharacterAlignmentType.EnemyAligned))
                                    .Actualize();

            // Run a Dijkstra scan - using the CHEAPEST option (must run once per goal.. so choose the smaller collection as goals)
            //
            var smallerFaction = playerFaction.Count() <= enemyFaction.Count() ? playerFaction : enemyFaction;
            var largerFaction = playerFaction.Count() > enemyFaction.Count() ? playerFaction : enemyFaction;

            foreach (var character in smallerFaction)
            {
                // Add REVERSE entry for this character
                _characterPathDict.Add(character, new Dictionary<CharacterBase, GridLocation>());

                var dijkstraGrid = new DijkstraPathFinder(_level,
                                                          _level.Content[character],
                                                          largerFaction.Select(character => _level.Content[character]).Actualize(), false);

                // Get next path location for each character in the larger faction
                foreach (var otherCharacter in largerFaction)
                {
                    if (otherCharacter is Player)
                        continue;

                    if (!_characterPathDict.ContainsKey(otherCharacter))
                        _characterPathDict.Add(otherCharacter, new Dictionary<CharacterBase, GridLocation>());

                    // Add an entry for this GOAL
                    _characterPathDict[otherCharacter].Add(character, dijkstraGrid.GetNextPathLocation(_level.Content[otherCharacter]));

                    // Add reverse entry for this SOURCE
                    _characterPathDict[character].Add(otherCharacter, dijkstraGrid.GetReverseNextPathLocation(_level.Content[otherCharacter]));
                }
            }
        }

        public GridLocation GetNextPathLocation(CharacterBase character)
        {
            return _characterPathDict[character].MinBy(element => Metric.RoguianDistance(element.Value, _level.Content[character])).Value;
        }

        public IEnumerable<GridLocation> GetLineOfSightLocations(CharacterBase character)
        {
            return _lineOfSightDict[character].Select(x => x.Location).Actualize();
        }

        public IEnumerable<GridLocation> GetVisibleLocations(CharacterBase character)
        {
            return _visibleDict[character].Select(x => x.Location).Actualize();
        }

        public IEnumerable<GridLocation> GetAuraAffectedLocations(CharacterBase character, string alterationEffectId)
        {
            return _auraDict[character][alterationEffectId].AffectedLocations.Select(x => x.Location);
        }

        public IEnumerable<GridLocation> GetExploredLocations()
        {
            return _exploredLocations;
        }

        public IEnumerable<GridLocation> GetRevealedLocations()
        {
            return _revealedLocations;
        }
    }
}
