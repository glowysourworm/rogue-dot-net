﻿namespace Rogue.NET.Core.Model.Enums
{
    public enum LevelMessageType
    {
        Level,
        PlayerAdvancement
    }
    public enum LevelTemporaryEventType
    {
        RoamingLightSource
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
        /// To permit spells to use random level placement (and change of level)
        /// </summary>
        Random,
    }

    public enum ActionType
    {
        LevelAction,
        ViewAction
    }

    public enum ViewActionType
    {
        None,
        ShowPlayerSubpanelEquipment,
        ShowPlayerSubpanelConsumables,
        ShowPlayerSubpanelSkills,
        ShowPlayerSubpanelStats,
        ShowPlayerSubpanelAlterations,
        ShowPlayerSubpanelReligion
    }
}
