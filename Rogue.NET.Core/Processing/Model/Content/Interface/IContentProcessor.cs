using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using System;

namespace Rogue.NET.Core.Processing.Model.Content.Interface
{
    public interface IContentProcessor
    {
        /// <summary>
        /// Figures out what enemies to provide for processing. Fires events for
        /// each enemy involved.
        /// </summary>
        void CalculateCharacterReactions();

        /// <summary>
        /// Processes an enemy reaction. Fires events for animations.
        /// </summary>
        void ProcessCharacterReaction(NonPlayerCharacter character);

        void DropPlayerItem(string itemId);
        void StepOnItem(Character character, ItemBase item);
        void StepOnDoodad(Character character, DoodadBase doodad);
        bool Equip(string equipId);
        void CharacterDeath(NonPlayerCharacter character);
    }
}
