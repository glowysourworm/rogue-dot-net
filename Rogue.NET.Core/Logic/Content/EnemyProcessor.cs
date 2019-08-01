using Rogue.NET.Core.Logic.Content.Interface;
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

            // Increment Behavior Turn Counter / Select next behavior
            enemy.BehaviorDetails.IncrementBehavior(enemy, _alterationProcessor, actionTaken, _randomSequenceGenerator.Get());

            enemy.ApplyLimits();
        }
    }
}
