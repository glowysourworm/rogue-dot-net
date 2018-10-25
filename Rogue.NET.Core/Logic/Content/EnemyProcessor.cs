using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Character;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Logic.Content
{
    [Export(typeof(IEnemyProcessor))]
    public class EnemyProcessor : IEnemyProcessor
    {
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        [ImportingConstructor]
        public EnemyProcessor(IRandomSequenceGenerator randomSequenceGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public void ApplyEndOfTurn(Enemy enemy)
        {
            //this.MalignAuraEffects.Clear();
            //this.MalignAuraEffects.AddRange(passiveAuraEffects);

            //playerAdvancementEventArgs = null;
            //LevelMessageEventArgs[] msgs = new LevelMessageEventArgs[] { };

            //if (regenerate)
            //{
            //    this.Hp += this.GetHpRegen(regenerate);
            //    this.Mp += this.GetMpRegen();
            //}

            //for (int i = this.ActiveTemporaryEffects.Count - 1; i >= 0; i--)
            //{
            //    //Check temporary event
            //    this.ActiveTemporaryEffects[i].EventTime--;
            //    if (this.ActiveTemporaryEffects[i].EventTime < 0)
            //    {
            //        this.ActiveTemporaryEffects.RemoveAt(i);
            //        continue;
            //    }
            //}

            // Affects from player to enemy from auras
            //for (int i = level.Enemies.Count() - 1; i >= 0; i--)
            //{
            //    // TODO
            //    ////distance between enemy and player
            //    //double dist = Helper.RoguianDistance(this.Player.Location, enemy.Location);

            //    ////Add enemy auras to list where player is within aura effect range
            //    //passiveEnemyAuraEffects.AddRange(enemy.GetActiveAuras().Where(z => z.EffectRange >= dist));

            //    ////Process tick - pass in active player auras where the enemy is within effect radius
            //    //PlayerAdvancementEventArgs enemyAdvancementEventArgs = null;
            //    //LevelMessageEventArgs[] msgArgs = enemy.OnDungeonTick(this.Random, this.Player.GetActiveAuras().Where(z => z.EffectRange >= dist), true, out enemyAdvancementEventArgs);
            //    //foreach (LevelMessageEventArgs a in msgArgs)
            //    //    PublishScenarioMessage(a.Message);
            //}


            ////Attack attribute temporary effects
            //for (int i = this.AttackAttributeTemporaryFriendlyEffects.Count - 1; i >= 0; i--)
            //{
            //    this.AttackAttributeTemporaryFriendlyEffects[i].EventTime--;
            //    if (this.AttackAttributeTemporaryFriendlyEffects[i].EventTime < 0)
            //    {
            //        this.AttackAttributeTemporaryFriendlyEffects.RemoveAt(i);
            //    }
            //}
            //for (int i = this.AttackAttributeTemporaryMalignEffects.Count - 1; i >= 0; i--)
            //{
            //    this.AttackAttributeTemporaryMalignEffects[i].EventTime--;
            //    if (this.AttackAttributeTemporaryMalignEffects[i].EventTime < 0)
            //    {
            //        this.AttackAttributeTemporaryMalignEffects.RemoveAt(i);
            //    }
            //}

            //ApplyBehaviorRules(enemy);
            //ApplyLimits();

            //return msgs;
        }

        private void ApplyBehaviorRules(Enemy enemy)
        {
            switch (enemy.BehaviorDetails.SecondaryReason)
            {
                case SecondaryBehaviorInvokeReason.SecondaryNotInvoked:
                    enemy.BehaviorDetails.IsSecondaryBehavior = false;
                    break;
                case SecondaryBehaviorInvokeReason.PrimaryInvoked:
                    enemy.BehaviorDetails.IsSecondaryBehavior = true;
                    break;
                case SecondaryBehaviorInvokeReason.HpLow: // Hp is less than 10%
                    if ((enemy.Hp / enemy.HpMax) < 0.1)
                        enemy.BehaviorDetails.IsSecondaryBehavior = true;
                    break;
                case SecondaryBehaviorInvokeReason.Random:
                    if (_randomSequenceGenerator.Get() < enemy.BehaviorDetails.SecondaryProbability)
                        enemy.BehaviorDetails.IsSecondaryBehavior = !enemy.BehaviorDetails.IsSecondaryBehavior;
                    break;
                default:
                    break;
            }
        }

        protected void ApplyLimits(Enemy enemy)
        {
            if (enemy.Mp < 0)
                enemy.Mp = 0;

            if (enemy.Hp > enemy.HpMax)
                enemy.Hp = enemy.HpMax;

            if (enemy.Mp > enemy.MpMax)
                enemy.Mp = enemy.MpMax;

            if (enemy.SpeedBase < ModelConstants.MIN_SPEED)
                enemy.SpeedBase = ModelConstants.MIN_SPEED;

            if (enemy.StrengthBase < 0)
                enemy.StrengthBase = 0;

            if (enemy.AgilityBase < 0)
                enemy.AgilityBase = 0;

            if (enemy.IntelligenceBase < 0)
                enemy.IntelligenceBase = 0;
        }
    }
}
