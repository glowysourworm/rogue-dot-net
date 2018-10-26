using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using System;

namespace Rogue.NET.Core.Logic.Interface
{
    public interface IContentEngine
    {
        /// <summary>
        /// Event that signals an enemy reaction to a player action
        /// </summary>
        event EventHandler<Enemy> EnemyReactionEvent;

        /// <summary>
        /// Figures out what enemies to provide for processing. Fires events for
        /// each enemy involved.
        /// </summary>
        void CalculateEnemyReactions();

        /// <summary>
        /// Processes an enemy reaction. Fires events for animations.
        /// </summary>
        void ProcessEnemyReaction(Enemy enemy);

        /// <summary>
        /// Applies end of turn for level contents. This does not include any characters
        /// </summary>
        void ApplyEndOfTurn();

        void PlacePlayer(PlayerStartLocation location);
        void DropPlayerItem(string itemId);
        void StepOnItem(Character character, ItemBase item);
        void StepOnDoodad(Character character, DoodadBase doodad);
        bool Equip(string equipId);
        void EnemyDeath(Enemy enemy);
    }
}
