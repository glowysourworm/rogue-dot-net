using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Design;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;

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
                double zoomFactor,
                IEnumerable<ScenarioObject> previousLevelContent,
                ScenarioEncyclopedia encyclopedia,
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
        /// Currently loaded Scenario Level TODO: REMOVE THIS EXPOSURE OF THE LEVEL OBJECT
        /// </summary>
        Level Level { get; }

        /// <summary>
        /// Player (single) instance for the Scenario
        /// </summary>
        Player Player { get; }

        /// <summary>
        /// Returns the location of the player
        /// </summary>
        GridLocation PlayerLocation { get; }

        /// <summary>
        /// Exposes layout information per character [ visible locations, line-of-sight, explored, revealed ]
        /// </summary>
        ICharacterLayoutInformation CharacterLayoutInformation { get; }

        /// <summary>
        /// Exposes layout service for performing layout calculations based on the loaded level
        /// </summary>
        IModelLayoutService LayoutService { get; }

        /// <summary>
        /// "Encyclopedia" Rogue-Tanica. Contains all the meta-data for the Scenario objects, alteration categories, and character classes
        /// </summary>
        ScenarioEncyclopedia ScenarioEncyclopedia { get; }

        /// <summary>
        /// Returns the location of the specified content object
        /// </summary>
        public GridLocation GetLocation(ScenarioObject scenarioObject);

        /// <summary>
        /// Gets the layout template for this level
        /// </summary>
        LayoutTemplate GetLayoutTemplate();

        /// <summary>
        /// Returns Enemy templates from the scenario configuration for the loaded level
        /// </summary>
        IEnumerable<EnemyGenerationTemplate> GetEnemyTemplates();

        /// <summary>
        /// Method to populate player advancement parameters
        /// </summary>
        void GetPlayerAdvancementParameters(ref double hpPerPoint, ref double staminaPerPoint,
                                            ref double strengthPerPoint, ref double agilityPerPoint, ref double intelligencePerPoint,
                                            ref int skillPointsPerPoint);

        /// <summary>
        /// Returns the name of the the scenario
        /// </summary>
        /// <returns></returns>
        string GetScenarioName();

        /// <summary>
        /// Returns the objective description for the scenario
        /// </summary>
        /// <returns></returns>
        string GetScenarioDescription();

        /// <summary>
        /// Returns number of levels in the scenario
        /// </summary>
        int GetNumberOfLevels();

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
        /// Gets / sets the zoom factor for the scenario
        /// </summary>
        double ZoomFactor { get; set; }
    }
}
