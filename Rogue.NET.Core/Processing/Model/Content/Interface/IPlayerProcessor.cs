using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Item;

namespace Rogue.NET.Core.Processing.Model.Content.Interface
{
    public interface IPlayerProcessor
    {
        // Calculate Level Gains / Experience
        double CalculateExperienceNext(Player player);
        void CalculateLevelGains(Player player);
        void CalculateEnemyDeathGains(Player player, Enemy slainEnemy);

        // Query Equipment Inventory
        Equipment GetEquippedType(Player player, EquipmentType type);
        int GetNumberEquipped(Player player, EquipmentType type);
        int GetNumberOfFreeHands(Player player);

        void DeActivateSkills(Player player);

        // Process Turn for Player
        void ApplyEndOfTurn(Player player, bool regenerate, out bool playerAdvancement);
    }
}
