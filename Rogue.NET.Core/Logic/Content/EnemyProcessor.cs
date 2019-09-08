using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Service.Interface;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Logic.Content
{
    [Export(typeof(IEnemyProcessor))]
    public class EnemyProcessor : IEnemyProcessor
    {
        readonly IAlterationProcessor _alterationProcessor;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IModelService _modelService;

        [ImportingConstructor]
        public EnemyProcessor(IAlterationProcessor alterationProcessor, IRandomSequenceGenerator randomSequenceGenerator, IModelService modelService)
        {
            _alterationProcessor = alterationProcessor;
            _randomSequenceGenerator = randomSequenceGenerator;
            _modelService = modelService;
        }

        public void ApplyBeginningOfTurn(Enemy enemy)
        {
            enemy.Hp -= enemy.GetMalignAttackAttributeHit(_modelService.AttackAttributes);
        }

        public void ApplyEndOfTurn(Enemy enemy, Player player, bool actionTaken)
        {
            enemy.Hp += actionTaken ? 0 : enemy.GetHpRegen();
            enemy.Mp += enemy.GetMpRegen();

            // Increment event times - ignore messages to publish
            enemy.Alteration.DecrementEventTimes();

            // Calculate Auras Affecting Enemy
            var distance = Calculator.EuclideanDistance(player.Location, enemy.Location);

            // Get Player Active Auras (where Enemy is in an affected cell)
            var playerAuras = player.Alteration
                                    .GetAuras()
                                    .Where(x => _modelService.CharacterLayoutInformation
                                                             .GetAuraAffectedLocations(player, x.Item1.Id)
                                                             .Contains(enemy.Location))
                                    .Select(x => x.Item1)
                                    .GroupBy(x => x.GetType());

            // TODO: Figure out a nicer way to clear these; but they are only cleared out when
            //       new ones are applied.. so have to clear them this way for now.
            enemy.Alteration.ApplyTargetAuraEffects(new AttackAttributeAuraAlterationEffect[] { });
            enemy.Alteration.ApplyTargetAuraEffects(new AuraAlterationEffect[] { });

            // Set Effect to Enemies
            foreach (var auraGroup in playerAuras)
            {
                if (auraGroup.Key == typeof(AttackAttributeAuraAlterationEffect))
                    enemy.Alteration.ApplyTargetAuraEffects(auraGroup.Cast<AttackAttributeAuraAlterationEffect>());

                else if (auraGroup.Key == typeof(AuraAlterationEffect))
                    enemy.Alteration.ApplyTargetAuraEffects(auraGroup.Cast<AuraAlterationEffect>());

                else
                    throw new Exception("Unknwon Aura Alteration Effect Type");
            }

            // Increment Behavior Turn Counter / Select next behavior
            enemy.BehaviorDetails.IncrementBehavior(enemy, _alterationProcessor, actionTaken, _randomSequenceGenerator.Get());

            enemy.ApplyLimits();
        }
    }
}
