using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Dynamic.Content.Interface;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Service.Interface
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
                IEnumerable<ScenarioObject> previousLevelContent,
                IDictionary<string, ScenarioMetaData> encyclopedia,
                ScenarioConfigurationContainer configuration);

        /// <summary>
        /// Unloads content of current Level and associated objects. Returns extracted level content to be 
        /// moved with the Player to the next level.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ScenarioObject> Unload();

        /// <summary>
        /// Gets a value saying whether or not the IModelService is loaded (Load(...) has beed called)
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// Currently loaded Scenario Level
        /// </summary>
        Level Level { get; }

        /// <summary>
        /// Player (single) instance for the Scenario
        /// </summary>
        Player Player { get; }

        /// <summary>
        /// Exposes layout information per character [ visible locations, line-of-sight, explored, revealed ]
        /// </summary>
        ICharacterLayoutInformation CharacterLayoutInformation { get; }

        /// <summary>
        /// Exposes content information per character [ visible content, revealed content, line-of-sight content ]
        /// </summary>
        ICharacterContentInformation CharacterContentInformation { get; }

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
        /// Returns enemy to have slain the Player
        /// </summary>
        string GetKilledBy();

        /// <summary>
        /// Sets enemy or alteration to have slain the Player
        /// </summary>
        void SetKilledBy(string killedBy);

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
