using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Processing.Model.Content.Calculator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Static;
using Rogue.NET.Core.Processing.Service.Interface;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Content.Calculator
{
    [Export(typeof(INonPlayerCharacterCalculator))]
    public class NonPlayerCharacterSubProcessor : INonPlayerCharacterCalculator
    {
        readonly IAlterationCalculator _alterationCalculator;
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IModelService _modelService;

        [ImportingConstructor]
        public NonPlayerCharacterSubProcessor(IAlterationCalculator alterationCalculator, IRandomSequenceGenerator randomSequenceGenerator, IModelService modelService)
        {
            _alterationCalculator = alterationCalculator;
            _randomSequenceGenerator = randomSequenceGenerator;
            _modelService = modelService;
        }

        public void ApplyBeginningOfTurn(NonPlayerCharacter character)
        {
            var malignAttackAttributeHit = character.GetMalignAttackAttributeHit();

            // Subtract attack from defender stamina -> then Hp
            var appliedToHp = (malignAttackAttributeHit - character.Stamina).LowLimit(0);

            character.Stamina -= malignAttackAttributeHit;
            character.Hp -= appliedToHp;
        }

        public void ApplyEndOfTurn(NonPlayerCharacter character, Player player, bool actionTaken)
        {
            var staminaRegenerationAlteration = character.Alteration.GetAttribute(CharacterAttribute.StaminaRegen);
            var hpRegenerationAlteration = character.Alteration.GetAttribute(CharacterAttribute.HpRegen);

            // Penalized for action taken - no natural regeneration
            if (actionTaken)
            {
                // First, regenerate stamina
                character.Stamina += staminaRegenerationAlteration;

                // Then, allow Hp regeneration
                if (character.Stamina >= character.StaminaMax)
                    character.Hp += hpRegenerationAlteration;
            }
            else
            {
                // First, regenerate stamina
                character.Stamina += character.GetTotalStaminaRegen();

                // Then, allow Hp regeneration
                if (character.Stamina >= character.StaminaMax)
                    character.Hp += character.GetTotalHpRegen();

                // Override if character has any alterations present
                else if (hpRegenerationAlteration > 0)
                    character.Hp += hpRegenerationAlteration;
            }

            // Increment event times - ignore messages to publish
            character.Alteration.DecrementEventTimes();

            // Calculate Auras Affecting Enemy
            var distance = RogueCalculator.EuclideanDistance(player.Location, character.Location);

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
            character.BehaviorDetails.IncrementBehavior(character, _alterationCalculator, actionTaken, _randomSequenceGenerator.Get());

            character.ApplyLimits();
        }
    }
}
