using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Processing.Model.Content.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Static;
using Rogue.NET.Core.Processing.Service.Interface;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Content
{
    [Export(typeof(INonPlayerCharacterProcessor))]
    public class NonPlayerCharacterProcessor : INonPlayerCharacterProcessor
    {
        readonly IAlterationProcessor _alterationProcessor;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IModelService _modelService;

        [ImportingConstructor]
        public NonPlayerCharacterProcessor(IAlterationProcessor alterationProcessor, IRandomSequenceGenerator randomSequenceGenerator, IModelService modelService)
        {
            _alterationProcessor = alterationProcessor;
            _randomSequenceGenerator = randomSequenceGenerator;
            _modelService = modelService;
        }

        public void ApplyBeginningOfTurn(NonPlayerCharacter character)
        {
            character.Hp -= character.GetMalignAttackAttributeHit(_modelService.AttackAttributes);
        }

        public void ApplyEndOfTurn(NonPlayerCharacter character, Player player, bool actionTaken)
        {
            character.Hp += actionTaken ? 0 : character.GetHpRegen();
            character.Mp += character.GetMpRegen();

            // Increment event times - ignore messages to publish
            character.Alteration.DecrementEventTimes();

            // Calculate Auras Affecting Enemy
            var distance = Calculator.EuclideanDistance(player.Location, character.Location);

            // TODO:FRIENDLY - Check for alteration target type for affected characters

            // Get Player Active Auras (where Enemy is in an affected cell)
            var playerAuras = player.Alteration
                                    .GetAuras()
                                    .Where(x => _modelService.CharacterLayoutInformation
                                                             .GetAuraAffectedLocations(player, x.Item1.Id)
                                                             .Contains(character.Location))
                                    .Select(x => x.Item1)
                                    .GroupBy(x => x.GetType());

            // TODO: Figure out a nicer way to clear these; but they are only cleared out when
            //       new ones are applied.. so have to clear them this way for now.
            character.Alteration.ApplyTargetAuraEffects(new AttackAttributeAuraAlterationEffect[] { });
            character.Alteration.ApplyTargetAuraEffects(new AuraAlterationEffect[] { });

            // Set Effect to Characters
            foreach (var auraGroup in playerAuras)
            {
                if (auraGroup.Key == typeof(AttackAttributeAuraAlterationEffect))
                    character.Alteration.ApplyTargetAuraEffects(auraGroup.Cast<AttackAttributeAuraAlterationEffect>());

                else if (auraGroup.Key == typeof(AuraAlterationEffect))
                    character.Alteration.ApplyTargetAuraEffects(auraGroup.Cast<AuraAlterationEffect>());

                else
                    throw new Exception("Unknwon Aura Alteration Effect Type");
            }

            // Increment Behavior Turn Counter / Select next behavior
            character.BehaviorDetails.IncrementBehavior(character, _alterationProcessor, actionTaken, _randomSequenceGenerator.Get());

            character.ApplyLimits();
        }
    }
}
