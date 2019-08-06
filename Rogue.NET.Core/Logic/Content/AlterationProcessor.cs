using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.Scenario.Content.Skill.Extension;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioMessage;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Core.Utility;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Logic.Content
{
    [Export(typeof(IAlterationProcessor))]
    public class AlterationProcessor : IAlterationProcessor
    {
        readonly IScenarioMessageService _scenarioMessageService;

        [ImportingConstructor]
        public AlterationProcessor(IScenarioMessageService scenarioMessageService)
        {
            _scenarioMessageService = scenarioMessageService;
        }

        public ScenarioImage CalculateEffectiveSymbol(Character character)
        {
            var symbol = new ScenarioImage();

            // Map properties onto the symbol
            character.Update(symbol);

            bool firstAlteration = true;

            foreach (var symbolTemplate in character.Alteration
                                                 .GetSymbolAlteringEffects()
                                                 .Select(x => x.SymbolAlteration))
            {
                //Full symbol
                if (symbolTemplate.IsFullSymbolDelta)
                    return new ScenarioImage()
                    {
                        CharacterColor = symbolTemplate.CharacterColor,
                        CharacterSymbol = symbolTemplate.SmileyAuraColor,
                        Icon = symbolTemplate.Icon,
                        SmileyAuraColor = symbolTemplate.SmileyAuraColor,
                        SmileyBodyColor = symbolTemplate.SmileyBodyColor,
                        SmileyLineColor = symbolTemplate.SmileyLineColor,
                        SmileyMood = symbolTemplate.SmileyMood,
                        SymbolType = symbolTemplate.Type
                    };

                //Aura
                if (symbolTemplate.IsAuraDelta)
                    symbol.SmileyAuraColor = firstAlteration ?
                                                symbolTemplate.SmileyAuraColor :
                                                ColorUtility.Add(symbol.SmileyAuraColor, symbolTemplate.SmileyAuraColor);

                //Body
                if (symbolTemplate.IsBodyDelta)
                    symbol.SmileyBodyColor = firstAlteration ?
                                                symbolTemplate.SmileyBodyColor :
                                                ColorUtility.Add(symbol.SmileyBodyColor, symbolTemplate.SmileyBodyColor);

                //Character symbol
                if (symbolTemplate.IsCharacterDelta)
                    symbol.CharacterSymbol = symbolTemplate.CharacterSymbol;

                //Character delta
                if (symbolTemplate.IsColorDelta)
                    symbol.CharacterColor = firstAlteration ?
                                                symbolTemplate.CharacterColor :
                                                ColorUtility.Add(symbol.CharacterColor, symbolTemplate.CharacterColor);

                //Image
                if (symbolTemplate.IsImageDelta)
                    symbol.Icon = symbolTemplate.Icon;

                //Line
                if (symbolTemplate.IsLineDelta)
                    symbol.SmileyLineColor = firstAlteration ?
                                                symbolTemplate.SmileyLineColor :
                                                ColorUtility.Add(symbol.SmileyLineColor, symbolTemplate.SmileyLineColor);

                //Mood
                if (symbolTemplate.IsMoodDelta)
                    symbol.SmileyMood = symbolTemplate.SmileyMood;

                firstAlteration = false;
            }
            return symbol;
        }

        public bool CalculateSpellRequiresTarget(Spell spell)
        {
            if (spell.IsPassive())
                return false;

            if (spell.Type == AlterationType.PermanentAllTargets
                || spell.Type == AlterationType.PermanentTarget
                || spell.Type == AlterationType.Steal
                || spell.Type == AlterationType.TeleportAllTargets
                || spell.Type == AlterationType.TeleportTarget
                || spell.Type == AlterationType.TemporaryAllTargets
                || spell.Type == AlterationType.TemporaryTarget)
                return true;

            if (spell.Type == AlterationType.AttackAttribute &&
               (spell.AttackAttributeType == AlterationAttackAttributeType.TemporaryFriendlyTarget ||
                spell.AttackAttributeType == AlterationAttackAttributeType.TemporaryMalignTarget ||
                spell.AttackAttributeType == AlterationAttackAttributeType.MeleeTarget))
                return true;

            return false;
        }

        public bool CalculateEnemyMeetsAlterationCost(Enemy enemy, AlterationCostTemplate cost)
        {
            return (enemy.AgilityBase - cost.Agility) >= 0 &&
                   (enemy.SpeedBase - cost.Speed) >= ModelConstants.MinSpeed &&
                   (enemy.Hp - cost.Hp) >= 0 &&
                   (enemy.IntelligenceBase - cost.Intelligence) >= 0 &&
                   (enemy.Mp - cost.Mp) >= 0 &&
                   (enemy.StrengthBase - cost.Strength) >= 0;
        }
        public bool CalculatePlayerMeetsAlterationCost(Player player, AlterationCostTemplate cost)
        {
            if (player.AgilityBase - cost.Agility < 0)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough Agility");
                return false;
            }

            if (player.SpeedBase - cost.Speed < ModelConstants.MinSpeed)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough Speed");
                return false;
            }

            if (player.Hp - cost.Hp < 0)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough HP");
                return false;
            }

            if (player.IntelligenceBase - cost.Intelligence < 0)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough Intelligence");
                return false;
            }

            if (player.Mp - cost.Mp < 0)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough MP");
                return false;
            }

            if (player.StrengthBase - cost.Strength < 0)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough Strength");
                return false;
            }

            if (player.AuraRadiusBase - cost.AuraRadius < 0)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Will go Blind!");
                return false;
            }

            if (player.Experience - cost.Experience < 0)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough Experience");
                return false;
            }

            if (player.Hunger + cost.Hunger > 100)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "You'll starve! (This is making you Hungry!)");
                return false;
            }

            return true;
        }

        public void ApplyOneTimeAlterationCost(Character character, AlterationCost alterationCost)
        {
            if (character is Player)
                ApplyOneTimeAlterationCost(character as Player, alterationCost);
            else
                ApplyOneTimeAlterationCost(character as Enemy, alterationCost);
        }
        public void ApplyPermanentEffect(Character character, AlterationEffect alterationEffect)
        {
            if (character is Player)
                ApplyPermanentEffect(character as Player, alterationEffect);
            else
                ApplyPermanentEffect(character as Enemy, alterationEffect);
        }
        public void ApplyRemedy(Character character, AlterationEffect alterationEffect)
        {
            if (character is Player)
                ApplyRemedy(character as Player, alterationEffect);
            else
                ApplyRemedy(character as Enemy, alterationEffect);
        }

        protected void ApplyOneTimeAlterationCost(Player player, AlterationCost alterationCost)
        {
            if (alterationCost.Type == AlterationCostType.OneTime)
            {
                player.AgilityBase -= alterationCost.Agility;
                player.AuraRadiusBase -= alterationCost.AuraRadius;
                player.Experience -= alterationCost.Experience;
                player.FoodUsagePerTurnBase += alterationCost.FoodUsagePerTurn;
                player.Hp -= alterationCost.Hp;
                player.Hunger += alterationCost.Hunger;
                player.IntelligenceBase -= alterationCost.Intelligence;
                player.Mp -= alterationCost.Mp;
                player.SpeedBase -= alterationCost.Speed;
                player.StrengthBase -= alterationCost.Strength;
            }
            else
                throw new Exception("Per-Step Alteration Costs Must be applied in the CharacterAlteration");
        }
        protected void ApplyOneTimeAlterationCost(Enemy enemy, AlterationCost alterationCost)
        {
            if (alterationCost.Type == AlterationCostType.OneTime)
            {
                enemy.AgilityBase -= alterationCost.Agility;
                enemy.AuraRadiusBase -= alterationCost.AuraRadius;
                enemy.SpeedBase -= alterationCost.Speed;
                enemy.Hp -= alterationCost.Hp;
                enemy.IntelligenceBase -= alterationCost.Intelligence;
                enemy.Mp -= alterationCost.Mp;
                enemy.StrengthBase -= alterationCost.Strength;
            }
            else
                throw new Exception("Per-Step Alteration Costs Must be applied in the CharacterAlteration");
        }
        protected void ApplyPermanentEffect(Player player, AlterationEffect alterationEffect)
        {
            player.StrengthBase += alterationEffect.Strength;
            player.IntelligenceBase += alterationEffect.Intelligence;
            player.AgilityBase += alterationEffect.Agility;
            player.SpeedBase += alterationEffect.Speed;
            player.AuraRadiusBase += alterationEffect.AuraRadius;
            player.FoodUsagePerTurnBase += alterationEffect.FoodUsagePerTurn;
            player.Experience += alterationEffect.Experience;
            player.Hunger += alterationEffect.Hunger;
            player.Hp += alterationEffect.Hp;
            player.Mp += alterationEffect.Mp;

            // Publish Messages

            // Strength
            if (alterationEffect.Strength > 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Strength has changed by " + alterationEffect.Strength.ToString("F2"));

            else if (alterationEffect.Strength < 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " Strength has changed by " + alterationEffect.Strength.ToString("F2"));

            // Intelligence
            if (alterationEffect.Intelligence > 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Intelligence has changed by " + alterationEffect.Intelligence.ToString("F2"));

            else if (alterationEffect.Intelligence < 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " Intelligence has changed by " + alterationEffect.Intelligence.ToString("F2"));

            // Agility
            if (alterationEffect.Agility > 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Agility has changed by " + alterationEffect.Agility.ToString("F2"));

            else if (alterationEffect.Agility < 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " Agility has changed by " + alterationEffect.Agility.ToString("F2"));

            // Speed
            if (alterationEffect.Speed > 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Speed has changed by " + alterationEffect.Speed.ToString("F2"));

            else if (alterationEffect.Speed < 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " Speed has changed by " + alterationEffect.Speed.ToString("F2"));

            // AuraRadius
            if (alterationEffect.AuraRadius > 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Light Radius has changed by " + alterationEffect.AuraRadius.ToString("F2"));

            else if (alterationEffect.AuraRadius < 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " Light Radius has changed by " + alterationEffect.AuraRadius.ToString("F2"));

            // FoodUsagePerTurn
            if (alterationEffect.FoodUsagePerTurn > 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " Food Usage has changed by " + alterationEffect.FoodUsagePerTurn.ToString("F3"));

            else if (alterationEffect.FoodUsagePerTurn < 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Food Usage has changed by " + alterationEffect.FoodUsagePerTurn.ToString("F3"));

            // Experience
            if (alterationEffect.Experience > 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Experience has changed by " + alterationEffect.Experience.ToString("N0"));

            else if (alterationEffect.Experience < 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " Experience has changed by " + alterationEffect.Experience.ToString("N0"));

            // Hunger
            if (alterationEffect.Hunger > 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " Hunger has changed by " + alterationEffect.Hunger.ToString("N0"));

            else if (alterationEffect.Hunger < 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Hunger has changed by " + alterationEffect.Hunger.ToString("N0"));

            // Hp
            if (alterationEffect.Hp > 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Hp has changed by " + alterationEffect.Hp.ToString("F2"));

            else if (alterationEffect.Hp < 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " Hp has changed by " + alterationEffect.Hp.ToString("F2"));

            // Mp
            if (alterationEffect.Mp > 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Mp has changed by " + alterationEffect.Mp.ToString("N0"));

            else if (alterationEffect.Mp < 0)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " Mp has changed by " + alterationEffect.Mp.ToString("N0"));
        }
        protected void ApplyPermanentEffect(Enemy enemy, AlterationEffect alterationEffect)
        {
            enemy.StrengthBase += alterationEffect.Strength;
            enemy.IntelligenceBase += alterationEffect.Intelligence;
            enemy.AgilityBase += alterationEffect.Agility;
            enemy.SpeedBase += alterationEffect.Speed;
            enemy.AuraRadiusBase += alterationEffect.AuraRadius;
            enemy.Hp += alterationEffect.Hp;
            enemy.Mp += alterationEffect.Mp;
        }
        protected void ApplyRemedy(Player player, AlterationEffect alterationEffect)
        {
            // Alteration applies remedy to remove or modify internal collections
            var remediedEffects = player.Alteration.ApplyRemedy(alterationEffect.RemediedStateName);

            foreach (var effect in remediedEffects)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Good, effect.DisplayName + " has been cured!");
        }
        protected void ApplyRemedy(Enemy enemy, AlterationEffect alterationEffect)
        {
            // Alteration applies remedy to remove or modify internal collections
            var remediedEffects = enemy.Alteration.ApplyRemedy(alterationEffect.RemediedStateName);

            foreach (var effect in remediedEffects)
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, enemy.RogueName + " has cured " + effect.DisplayName);
        }
    }
}
