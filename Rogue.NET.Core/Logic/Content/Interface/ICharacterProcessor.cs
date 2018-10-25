using Rogue.NET.Core.Model.Scenario.Character;

namespace Rogue.NET.Core.Logic.Content.Interface
{
    public interface ICharacterProcessor
    {
        /// <summary>
        /// Returns effective HP regeneration - set to false to calculate for malign effects only
        /// </summary>
        double GetHpRegen(Character character, bool regenerate);
        double GetMagicBlockBase(Character character);
        double GetDodgeBase(Character character);
        double GetHaulMax(Character character);
        double GetMpRegen(Character character);
        double GetStrength(Character character);
        double GetAgility(Character character);
        double GetIntelligence(Character character);
        double GetAuraRadius(Character character);
        double GetMagicBlock(Character character);
        double GetDodge(Character character);
        double GetHaul(Character character);
    }
}
