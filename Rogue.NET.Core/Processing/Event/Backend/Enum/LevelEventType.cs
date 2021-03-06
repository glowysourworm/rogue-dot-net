﻿namespace Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum
{
    public enum LevelEventType
    {
        /// <summary>
        /// Occurs when all content in the level needs to be updated
        /// </summary>
        ContentAll,

        /// <summary>
        /// Occurs when visible content has changed (includes setting explored boolean)
        /// </summary>
        ContentVisible,

        /// <summary>
        /// Occurs when content is revealed
        /// </summary>
        ContentReveal,

        /// <summary>
        /// Occurs when content is removed from level
        /// </summary>
        ContentRemove,

        /// <summary>
        /// Occurs when content location is changed
        /// </summary>
        ContentMove,

        /// <summary>
        /// Occurs when content has updated - possibly a state change
        /// </summary>
        ContentUpdate,

        /// <summary>
        /// Occurs when content is changed
        /// </summary>
        ContentAdd,

        /// <summary>
        /// Occurs when the layout has changed (LevelGrid)
        /// </summary>
        LayoutAll,

        /// <summary>
        /// Occurs when the region surrounding the player changes (visible locations and explored locations)
        /// </summary>
        LayoutVisible,

        /// <summary>
        /// Occurs when the layout is fully revealed
        /// </summary>
        LayoutReveal,

        /// <summary>
        /// Occurs when the layout has changed because the door topology has changed (includes searched doors)
        /// </summary>
        LayoutTopology,

        /// <summary>
        /// Occurs when the player location needs to be updated
        /// </summary>
        PlayerLocation,

        /// <summary>
        /// Occurs when player has removed consumable from inventory
        /// </summary>
        PlayerConsumableRemove,

        /// <summary>
        /// Occurs when player has added consumable from inventory
        /// </summary>
        PlayerConsumableAddOrUpdate,

        /// <summary>
        /// Occurs when player has removed equipment from inventory
        /// </summary>
        PlayerEquipmentRemove,

        /// <summary>
        /// Occurs when player has added equipment to inventory
        /// </summary>
        PlayerEquipmentAddOrUpdate,

        /// <summary>
        /// Occurs when player learns new skill set from a consumable item. This should not require the skill be sent - so
        /// will require a full refresh from the model for the player skill sets.
        /// </summary>
        PlayerSkillSetAdd,

        /// <summary>
        /// Occurs when player changes property of any skill set
        /// </summary>
        PlayerSkillSetRefresh,

        /// <summary>
        /// Occurs when player stats change
        /// </summary>
        PlayerStats,

        /// <summary>
        /// Signals an update for all player Inventory items, Skill sets, stats, and location
        /// </summary>
        PlayerAll,

        /// <summary>
        /// Occurs when an item, enemy, or doodad has been identified
        /// </summary>
        EncyclopediaIdentify,

        /// <summary>
        /// Occurs when a curse has been identified
        /// </summary>
        EncyclopediaCurseIdentify
    }
}
