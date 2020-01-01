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
        ScenarioImage CalculateEffectiveSymbol(CharacterBase character);

        bool CalculateMeetsAlterationCost(CharacterBase character, AlterationCost cost);
        bool CalculateCharacterMeetsAlterationCost(CharacterBase character, AlterationCostTemplate cost);
        bool CalculatePlayerMeetsAlterationCost(Player player, AlterationCostTemplate cost);

        void ApplyOneTimeAlterationCost(CharacterBase character, AlterationCost alterationCost);
        void ApplyPermanentEffect(CharacterBase character, PermanentAlterationEffect alterationEffect);
        void ApplyRemedy(CharacterBase character, RemedyAlterationEffect alterationEffect);

        void ApplyEquipmentEnhanceEffect(Player player, EquipmentEnhanceAlterationEffect effect, Equipment item);
        void ApplyEquipmentDamageEffect(CharacterBase affectedCharacter, EquipmentDamageAlterationEffect effect, Equipment item);

        void ApplyDrainMeleeEffect(CharacterBase actor, CharacterBase affectedCharacter, DrainMeleeAlterationEffect effect);
    }
}
