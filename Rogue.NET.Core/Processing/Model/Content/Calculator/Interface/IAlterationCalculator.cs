using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;

namespace Rogue.NET.Core.Processing.Model.Content.Calculator.Interface
{
    public interface IAlterationCalculator
    {
        ScenarioImage CalculateEffectiveSymbol(Character character);

        bool CalculateMeetsAlterationCost(Character character, AlterationCost cost);
        bool CalculateCharacterMeetsAlterationCost(Character character, AlterationCostTemplate cost);
        bool CalculatePlayerMeetsAlterationCost(Player player, AlterationCostTemplate cost);

        void ApplyOneTimeAlterationCost(Character character, AlterationCost alterationCost);
        void ApplyPermanentEffect(Character character, PermanentAlterationEffect alterationEffect);
        void ApplyRemedy(Character character, RemedyAlterationEffect alterationEffect);

        void ApplyEquipmentEnhanceEffect(Player player, EquipmentEnhanceAlterationEffect effect, Equipment item);
        void ApplyEquipmentDamageEffect(Character affectedCharacter, EquipmentDamageAlterationEffect effect, Equipment item);

        void ApplyDrainMeleeEffect(Character actor, Character affectedCharacter, DrainMeleeAlterationEffect effect);
    }
}
