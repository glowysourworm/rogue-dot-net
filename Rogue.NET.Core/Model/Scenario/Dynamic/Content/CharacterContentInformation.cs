using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Dynamic.Content.Interface;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

using CharacterBase = Rogue.NET.Core.Model.Scenario.Character.Character;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Content
{
    public class CharacterContentInformation : ICharacterContentInformation
    {
        readonly ICharacterLayoutInformation _characterLayoutInformation;

        Dictionary<CharacterBase, List<ScenarioObject>> _lineOfSightContent;
        Dictionary<CharacterBase, List<ScenarioObject>> _visibleContent;

        /// <summary>
        /// Constructor for the CharacterContentInformation should be called once per
        /// level and updated on each turn.
        /// </summary>
        public CharacterContentInformation(ICharacterLayoutInformation characterLayoutInformation)
        {
            _characterLayoutInformation = characterLayoutInformation;

            _lineOfSightContent = new Dictionary<CharacterBase, List<ScenarioObject>>();
            _visibleContent = new Dictionary<CharacterBase, List<ScenarioObject>>();

            // TODO:ALTERATION Check for "topology changes" to the level (opening doors)
        }

        public IEnumerable<ScenarioObject> GetVisibleContents(CharacterBase character)
        {
            return _visibleContent[character];
        }

        public IEnumerable<CharacterBase> GetVisibleCharacters(CharacterBase character)
        {
            return _visibleContent[character]
                    .Where(x => x is CharacterBase)
                    .Cast<CharacterBase>()
                    .Actualize();
        }

        public void ApplyUpdate(Level level, Player player)
        {
            _lineOfSightContent.Clear();
            _visibleContent.Clear();

            // Player
            _lineOfSightContent.Add(player, new List<ScenarioObject>());
            _visibleContent.Add(player, new List<ScenarioObject>());

            // Player -> Contents -> Line of Sight
            foreach (var location in _characterLayoutInformation.GetLineOfSightLocations(player))
            {
                _lineOfSightContent[player].AddRange(level.GetManyAt<ScenarioObject>(location));
            }

            // Player -> Contents -> Visible
            foreach (var location in _characterLayoutInformation.GetVisibleLocations(player))
            {
                // Fetch content for this location
                var content = level.GetManyAt<ScenarioObject>(location);

                // Visible content has to be updated for the IsExplored / IsRevealed flags
                foreach (var scenarioObject in content)
                {
                    scenarioObject.IsExplored = true;

                    // Set this based on whether the cell is physically visible. Once the cell is seen
                    // the IsRevealed flag gets reset. Also, the IsDetected flag gets reset. 
                    scenarioObject.IsRevealed = false;
                    scenarioObject.IsDetectedAlignment = false;
                    scenarioObject.IsDetectedCategory = false;
                }

                _visibleContent[player].AddRange(level.GetManyAt<ScenarioObject>(location));
            }

            // Character -> Contents
            foreach (var character in level.NonPlayerCharacters)
            {
                // Character Entry
                _lineOfSightContent.Add(character, new List<ScenarioObject>());
                _visibleContent.Add(character, new List<ScenarioObject>());

                foreach (var location in _characterLayoutInformation.GetLineOfSightLocations(character))
                {
                    _lineOfSightContent[character].AddRange(level.GetManyAt<ScenarioObject>(location));

                    // Add Player if applicable
                    if (player.Location == location)
                        _lineOfSightContent[character].Add(player);
                }
                foreach (var location in _characterLayoutInformation.GetVisibleLocations(character))
                {
                    _visibleContent[character].AddRange(level.GetManyAt<ScenarioObject>(location));

                    // Add Player if applicable
                    if (player.Location == location)
                        _visibleContent[character].Add(player);
                }

                // Be sure not to add the actual character (maybe clean this up)
                if (_lineOfSightContent[character].Contains(character))
                    _lineOfSightContent[character].Remove(character);

                if (_visibleContent[character].Contains(character))
                    _visibleContent[character].Remove(character);
            }
        }
    }
}
