using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout.Interface;
using Rogue.NET.Core.Processing.Model.Algorithm.Interface;
using System.Collections.Generic;
using System.Linq;

using CharacterBase = Rogue.NET.Core.Model.Scenario.Character.Character;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Layout
{
    public class CharacterLayoutInformation : ICharacterLayoutInformation
    {
        readonly Level _level;
        readonly IVisibilityCalculator _visibilityCalculator;

        Dictionary<CharacterBase, IEnumerable<DistanceLocation>> _visibleDict;
        Dictionary<CharacterBase, IEnumerable<DistanceLocation>> _lineOfSightDict;
        Dictionary<CharacterBase, Dictionary<string, AuraInformation>> _auraDict;

        List<GridLocation> _exploredLocations;
        List<GridLocation> _revealedLocations;

        #region Nested Class
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
                var characterLocation = _level.GetLocation(character);

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

        public IEnumerable<GridLocation> GetLineOfSightLocations(CharacterBase character)
        {
            return _lineOfSightDict[character].Select(x => x.Location);
        }

        public IEnumerable<GridLocation> GetVisibleLocations(CharacterBase character)
        {
            return _visibleDict[character].Select(x => x.Location);
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
