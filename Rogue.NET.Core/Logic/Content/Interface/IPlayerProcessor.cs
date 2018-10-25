using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Item;

namespace Rogue.NET.Core.Logic.Content.Interface
{
    public interface IPlayerProcessor
    {
        // Calculate state
        double GetAttackBase(Player player);
        double GetDefenseBase(Player player);
        double GetAttack(Player player);
        double GetDefense(Player player);
        double GetFoodUsagePerTurn(Player player);
        double GetCriticalHitProbability(Player player);

        // Calculate Level Gains / Experience
        double CalculateExperienceNext(Player player);
        void CalculateLevelGains(Player player);

        // Query Equipment Inventory
        Equipment GetEquippedType(Player player, EquipmentType type);
        int GetNumberEquipped(Player player, EquipmentType type);
        int GetNumberOfFreeHands(Player player);

        // Process Turn for Player
        void ApplyEndOfTurn(Player player, bool regenerate);
        void ProcessSkillLearning(Player player);
        void ApplyLimits(Player player);
    }
}
