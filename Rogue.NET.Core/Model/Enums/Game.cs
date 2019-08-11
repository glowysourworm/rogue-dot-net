namespace Rogue.NET.Core.Model.Enums
{
    public enum LevelMessageType
    {
        Level,
        PlayerAdvancement
    }
    public enum PlayerStartLocation
    {
        /// <summary>
        /// On opening a saved game - player will be placed at the save point
        /// </summary>
        SavePoint,
        
        /// <summary>
        /// On player death (Non-Hardcore) for the 1st level - continuing at the stairs up is permitted.
        /// </summary>
        StairsUp,

        /// <summary>
        /// On player returing to the previous level
        /// </summary>
        StairsDown,

        /// <summary>
        /// Starts player at random available location
        /// </summary>
        Random,
    }

    public enum ViewActionType
    {
        None,
        ShowPlayerSubpanelEquipment,
        ShowPlayerSubpanelConsumables,
        ShowPlayerSubpanelSkills,
        ShowPlayerSubpanelStats,
        ShowPlayerSubpanelAlterations
    }
}
