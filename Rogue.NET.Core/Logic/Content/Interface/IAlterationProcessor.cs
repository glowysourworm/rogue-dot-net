﻿using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;

namespace Rogue.NET.Core.Logic.Content.Interface
{
    public interface IAlterationProcessor
    {
        ScenarioImage CalculateEffectiveSymbol(Character character);

        bool CalculateSpellRequiresTarget(Spell spell);
        bool CalculateEnemyMeetsAlterationCost(Enemy enemy, AlterationCostTemplate cost);
        bool CalculatePlayerMeetsAlterationCost(Player player, AlterationCostTemplate cost);

        void ApplyOneTimeAlterationCost(Character character, AlterationCost alterationCost);
        void ApplyPermanentEffect(Character character, AlterationEffect alterationEffect);
        void ApplyRemedy(Character character, AlterationEffect alterationEffect);
    }
}
