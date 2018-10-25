using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;

namespace Rogue.NET.Core.Logic.Interface
{
    public interface IContentEngine
    {
        void PlacePlayer(PlayerStartLocation location);
        void StepOnItem(Character character, ItemBase item);
        void StepOnDoodad(Character character, DoodadBase doodad);
        bool Equip(string equipId);
        void DropPlayerItem(string itemId);
        void EnemyDeath(Enemy enemy);
        void ProcessEnemyReaction(Enemy enemy);
        void ApplyEndOfTurn();
    }
}
