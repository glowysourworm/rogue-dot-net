﻿using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Service.Interface;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Logic.Content
{
    [Export(typeof(IEnemyProcessor))]
    public class EnemyProcessor : IEnemyProcessor
    {
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IModelService _modelService;

        [ImportingConstructor]
        public EnemyProcessor(IRandomSequenceGenerator randomSequenceGenerator, IModelService modelService)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _modelService = modelService;
        }

        public void ApplyBeginningOfTurn(Enemy enemy)
        {
            enemy.Hp -= enemy.GetMalignAttackAttributeHit(_modelService.GetAttackAttributes());
        }

        public void ApplyEndOfTurn(Enemy enemy, Player player, bool actionTaken)
        {
            enemy.Hp += actionTaken ? 0 : enemy.GetHpRegen();
            enemy.Mp += enemy.GetMpRegen();

            // Increment event times - ignore messages to publish
            enemy.Alteration.DecrementEventTimes();

            // Calculate Auras Affecting Enemy
            var distance = Calculator.RoguianDistance(player.Location, enemy.Location);

            // Get Player Active Auras
            var playerAuraEffects = player.Alteration.GetActiveAuras();

            // Set Effect to Enemies in range
            enemy.Alteration.SetAuraEffects(playerAuraEffects.Where(x => x.EffectRange >= distance));

            ApplyBehaviorRules(enemy, actionTaken);
            enemy.ApplyLimits();
        }

        private void ApplyBehaviorRules(Enemy enemy, bool actionTaken)
        {
            switch (enemy.BehaviorDetails.SecondaryReason)
            {
                case SecondaryBehaviorInvokeReason.SecondaryNotInvoked:
                    enemy.BehaviorDetails.IsSecondaryBehavior = false;
                    break;
                case SecondaryBehaviorInvokeReason.PrimaryInvoked:
                    enemy.BehaviorDetails.IsSecondaryBehavior = enemy.BehaviorDetails.IsSecondaryBehavior || actionTaken;
                    break;
                case SecondaryBehaviorInvokeReason.HpLow: // Hp is less than 10%
                    if ((enemy.Hp / enemy.HpMax) < ModelConstants.HpLowFraction)
                        enemy.BehaviorDetails.IsSecondaryBehavior = true;
                    break;
                case SecondaryBehaviorInvokeReason.Random:
                    enemy.BehaviorDetails.IsSecondaryBehavior = (_randomSequenceGenerator.Get() < enemy.BehaviorDetails.SecondaryProbability);
                    break;
                default:
                    break;
            }
        }
    }
}
