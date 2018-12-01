using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
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
        /// Primary method to process an update to the contents. This should be called after
        /// a turn has been processed.
        /// </summary>
        void UpdateContents();

        /// <summary>
        /// Primary method to update visible cells that are now in view of the Player
        /// </summary>
        void UpdateVisibleLocations();

        /// <summary>
        /// Statefully maintained collection of locations visible to the Player 
        /// </summary>
        IEnumerable<CellPoint> GetVisibleLocations();

        /// <summary>
        /// Statefully maintained collection of locations in the Player's line of sight
        /// </summary>
        IEnumerable<CellPoint> GetLineOfSightLocations();

        /// <summary>
        /// Statefully maintained collection of locations that player has explored
        /// </summary>
        IEnumerable<CellPoint> GetExploredLocations();

        /// <summary>
        /// Statefully maintained collection of locations that are revealed
        /// </summary>
        /// <returns></returns>
        IEnumerable<CellPoint> GetRevealedLocations();

        /// <summary>
        /// Statefully maintained collection of level contents visible to the Player
        /// </summary>
        IEnumerable<ScenarioObject> GetVisibleContents();

        /// <summary>
        /// Statefully maintained collection of enemies visible to the Player
        /// </summary>
        IEnumerable<Enemy> GetVisibleEnemies();

        /// <summary>
        /// Statefully maintained targeted enemy collection
        /// </summary>
        IEnumerable<Enemy> GetTargetedEnemies();

        /// <summary>
        /// Returns enemy to have slain the Player
        /// </summary>
        Enemy GetFinalEnemy();

        /// <summary>
        /// Sets enemy to have slain the Player
        /// </summary>
        void SetFinalEnemy(Enemy enemy);

        /// <summary>
        /// Sets enemy targeted
        /// </summary>
        void SetTargetedEnemy(Enemy enemy);

        /// <summary>
        /// Clears targeted enemy list
        /// </summary>
        void ClearTargetedEnemies();

        /// <summary>
        /// Method to get display name for Scenario object.
        /// </summary>
        /// <returns>Display name if the object is Identified</returns>
        string GetDisplayName(string rogueName);
    }
}
