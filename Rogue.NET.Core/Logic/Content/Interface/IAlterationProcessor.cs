using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;

namespace Rogue.NET.Core.Logic.Content.Interface
{
    public interface IAlterationProcessor
    {
        ScenarioImage CalculateEffectiveSymbol(Enemy enemy);
        ScenarioImage CalculateEffectiveSymbol(Player player);

        bool CalculateSpellRequiresTarget(Spell spell);
        bool CalculateEnemyMeetsAlterationCost(Enemy enemy, AlterationCostTemplate cost);
        bool CalculatePlayerMeetsAlterationCost(Player player, AlterationCostTemplate cost);

        void ApplyAlterationCost(Player player, AlterationCost alterationCost);
        void ApplyAlterationCost(Enemy enemy, AlterationCost alterationCost);
        void ApplyPermanentEffect(Player player, AlterationEffect alterationEffect);
        void ApplyPermanentEffect(Enemy enemy, AlterationEffect alterationEffect);
    }
}
