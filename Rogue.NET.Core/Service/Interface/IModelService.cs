using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Service.Interface
{
    /// <summary>
    /// This is the primary (singleton) model acccess component. It's purpose is to serve data
    /// to the Logic components for processing. All stateful data should be stored here.
    /// </summary>
    public interface IModelService
    {
        void Load(
                Player player,
                PlayerStartLocation startLocation,
                Level level,
                IDictionary<string, ScenarioMetaData> encyclopedia,
                ScenarioConfigurationContainer configuration);

        void Unload();

        /// <summary>
        /// Currently loaded Scenario Level
        /// </summary>
        Level Level { get; }

        /// <summary>
        /// Player (single) instance for the Scenario
        /// </summary>
        Player Player { get; }

        /// <summary>
        /// "Encyclopedia" Rogue-Tanica. Contains all the meta-data for the Scenario objects.
        /// </summary>
        IDictionary<string, ScenarioMetaData> ScenarioEncyclopedia { get; }

        /// <summary>
        /// Configuration for the scenario
        /// </summary>
        ScenarioConfigurationContainer ScenarioConfiguration { get; }

        /// <summary>
        /// Character Classes for the scenario
        /// </summary>
        IEnumerable<ScenarioImage> CharacterClasses { get; }

        /// <summary>
        /// Primary method to update visible cells that are now in view of the Player - along with
        /// contents
        /// </summary>
        void UpdateVisibility();

        /// <summary>
        /// Statefully maintained collection of locations visible to a Character
        /// </summary>
        IEnumerable<GridLocation> GetVisibleLocations(Character character);

        /// <summary>
        /// Statefully maintained collection of locations in the Player's line of sight
        /// </summary>
        IEnumerable<GridLocation> GetLineOfSightLocations(Character character);

        /// <summary>
        /// Statefully maintained collection of auras per character
        /// </summary>
        IEnumerable<GridLocation> GetAuraLocations(Character character, string alterationEffectId);

        /// <summary>
        /// Statefully maintained collection of locations that player has explored
        /// </summary>
        IEnumerable<GridLocation> GetExploredLocations();

        /// <summary>
        /// Statefully maintained collection of locations that are revealed
        /// </summary>
        /// <returns></returns>
        IEnumerable<GridLocation> GetRevealedLocations();

        /// <summary>
        /// Statefully maintained collection of level contents visible to a Character. THIS DOES NOT INCLUDE
        /// INVISIBILITY ALTERATION EFFECTS
        /// </summary>
        bool IsVisibleTo(Character sourceCharacter, ScenarioObject scenarioObject);

        /// <summary>
        /// Statefully maintained collection of enemies visible to a Character
        /// </summary>
        IEnumerable<Character> GetVisibleCharacters(Character character);

        /// <summary>
        /// Statefully maintained targeted enemy collection
        /// </summary>
        IEnumerable<Enemy> GetTargetedEnemies();

        /// <summary>
        /// Returns enemy to have slain the Player
        /// </summary>
        string GetKilledBy();

        /// <summary>
        /// Sets enemy or alteration to have slain the Player
        /// </summary>
        void SetKilledBy(string killedBy);

        /// <summary>
        /// Sets enemy targeted
        /// </summary>
        void SetTargetedEnemy(Enemy enemy);

        /// <summary>
        /// Clears targeted enemy list
        /// </summary>
        void ClearTargetedEnemies();

        /// <summary>
        /// Method to get display name for Scenario Object
        /// </summary>
        /// <param name="scenarioObject"></param>
        /// <returns>Display name is the object is Identified</returns>
        string GetDisplayName(ScenarioObject scenarioObject);

        /// <summary>
        /// Method to get display name for Scenario Image
        /// </summary>
        /// <returns>Display name is the object is Identified</returns>
        string GetDisplayName(ScenarioImage scenarioImage);

        /// <summary>
        /// Returns Empty (Attack = 0, Resistence = 0) Attack Attribute collection copied (Cloned) from
        /// the configuration. This contains definitions for all attack attributes.
        /// </summary>
        IEnumerable<AttackAttribute> AttackAttributes { get; }
    }
}
