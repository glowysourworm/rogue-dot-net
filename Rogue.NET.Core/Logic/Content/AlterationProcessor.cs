using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
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

        public ScenarioImage CalculateEffectiveSymbol(Enemy enemy)
        {
            var symbol = enemy.Map<Enemy, ScenarioImage>();

            foreach (var symbolTemplate in enemy.Alteration
                                                .GetAlterations()
                                                .Where(x => x.IsSymbolAlteration)
                                                .Select(x => x.SymbolAlteration))
            {
                //Full symbol
                if (symbolTemplate.IsFullSymbolDelta)
                    return symbolTemplate.Map<SymbolDetailsTemplate, ScenarioImage>();

                //Aura
                if (symbolTemplate.IsAuraDelta)
                    symbol.SmileyAuraColor = ColorUtility.Add(symbol.SmileyAuraColor, symbolTemplate.SmileyAuraColor);

                //Body
                if (symbolTemplate.IsBodyDelta)
                    symbol.SmileyBodyColor = ColorUtility.Add(symbol.SmileyBodyColor, symbolTemplate.SmileyBodyColor);

                //Character symbol
                if (symbolTemplate.IsCharacterDelta)
                    symbol.CharacterSymbol = symbolTemplate.CharacterSymbol;

                //Character delta
                if (symbolTemplate.IsColorDelta)
                    symbol.CharacterColor = ColorUtility.Add(symbol.CharacterColor, symbolTemplate.CharacterColor);

                //Image
                if (symbolTemplate.IsImageDelta)
                    symbol.Icon = symbolTemplate.Icon;

                //Line
                if (symbolTemplate.IsLineDelta)
                    symbol.SmileyLineColor = ColorUtility.Add(symbol.SmileyLineColor, symbolTemplate.SmileyLineColor);

                //Mood
                if (symbolTemplate.IsMoodDelta)
                    symbol.SmileyMood = symbolTemplate.SmileyMood;
            }
            return symbol;
        }
        public ScenarioImage CalculateEffectiveSymbol(Player player)
        {
            var symbol = player.Map<Player, ScenarioImage>();

            foreach (var symbolTemplate in player.Alteration
                                                 .GetAlterations()
                                                 .Where(x => x.IsSymbolAlteration)
                                                 .Select(x => x.SymbolAlteration))
            {
                //Full symbol
                if (symbolTemplate.IsFullSymbolDelta)
                    return symbolTemplate.Map<SymbolDetailsTemplate, ScenarioImage>();

                //Aura
                if (symbolTemplate.IsAuraDelta)
                    symbol.SmileyAuraColor = ColorUtility.Add(symbol.SmileyAuraColor, symbolTemplate.SmileyAuraColor);

                //Body
                if (symbolTemplate.IsBodyDelta)
                    symbol.SmileyBodyColor = ColorUtility.Add(symbol.SmileyBodyColor, symbolTemplate.SmileyBodyColor);

                //Character symbol
                if (symbolTemplate.IsCharacterDelta)
                    symbol.CharacterSymbol = symbolTemplate.CharacterSymbol;

                //Character delta
                if (symbolTemplate.IsColorDelta)
                    symbol.CharacterColor = ColorUtility.Add(symbol.CharacterColor, symbolTemplate.CharacterColor);

                //Image
                if (symbolTemplate.IsImageDelta)
                    symbol.Icon = symbolTemplate.Icon;

                //Line
                if (symbolTemplate.IsLineDelta)
                    symbol.SmileyLineColor = ColorUtility.Add(symbol.SmileyLineColor, symbolTemplate.SmileyLineColor);

                //Mood
                if (symbolTemplate.IsMoodDelta)
                    symbol.SmileyMood = symbolTemplate.SmileyMood;
            }
            return symbol;
        }

        public bool CalculateSpellRequiresTarget(Spell spell)
        {
            if (spell.Type == AlterationType.PassiveAura || spell.Type == AlterationType.PassiveSource)
                return false;

            if (spell.Type == AlterationType.PermanentAllTargets
                || spell.Type == AlterationType.PermanentTarget
                || spell.Type == AlterationType.Steal
                || spell.Type == AlterationType.TeleportAllTargets
                || spell.Type == AlterationType.TeleportTarget
                || spell.Type == AlterationType.TemporaryAllTargets
                || spell.Type == AlterationType.TemporaryTarget)
                return true;

            return false;
        }

        public bool CalculateEnemyMeetsAlterationCost(Enemy enemy, AlterationCostTemplate cost)
        {
            return (enemy.AgilityBase - cost.Agility) >= 0 &&
                   (enemy.Hp - cost.Hp) >= 0 &&
                   (enemy.IntelligenceBase - cost.Intelligence) >= 0 &&
                   (enemy.Mp - cost.Mp) >= 0 &&
                   (enemy.StrengthBase - cost.Strength) >= 0;
        }
        public bool CalculatePlayerMeetsAlterationCost(Player player, AlterationCostTemplate cost)
        {
            if (player.AgilityBase - cost.Agility < 0)
            {
                _scenarioMessageService.Publish("Not enough agility left");
                return false;
            }

            if (player.Hp - cost.Hp < 0)
            {
                _scenarioMessageService.Publish("Not enough HP");
                return false;
            }

            if (player.IntelligenceBase - cost.Intelligence < 0)
            {
                _scenarioMessageService.Publish("Not enough intelligence left");
                return false;
            }

            if (player.Mp - cost.Mp < 0)
            {
                _scenarioMessageService.Publish("Not enough MP");
                return false;
            }

            if (player.StrengthBase - cost.Strength < 0)
            {
                _scenarioMessageService.Publish("Not enough strength left");
                return false;
            }

            if (player.AuraRadiusBase - cost.AuraRadius < 0)
            {
                _scenarioMessageService.Publish("Not enough aura left");
                return false;
            }

            if (player.Experience - cost.Experience < 0)
            {
                _scenarioMessageService.Publish("Not enough experience points");
                return false;
            }

            if (player.Hunger + cost.Hunger > 100)
            {
                _scenarioMessageService.Publish("You'll starve! (This is making you Hungry!)");
                return false;
            }

            return true;
        }

        public void ApplyOneTimeAlterationCost(Player player, AlterationCost alterationCost)
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
                player.StrengthBase -= alterationCost.Strength;
            }
            else
                throw new Exception("Per-Step Alteration Costs Must be applied in the CharacterAlteration");
        }
        public void ApplyOneTimeAlterationCost(Enemy enemy, AlterationCost alterationCost)
        {
            if (alterationCost.Type == AlterationCostType.OneTime)
            {
                enemy.AgilityBase -= alterationCost.Agility;
                enemy.AuraRadiusBase -= alterationCost.AuraRadius;
                enemy.Hp -= alterationCost.Hp;
                enemy.IntelligenceBase -= alterationCost.Intelligence;
                enemy.Mp -= alterationCost.Mp;
                enemy.StrengthBase -= alterationCost.Strength;
            }
            else
                throw new Exception("Per-Step Alteration Costs Must be applied in the CharacterAlteration");
        }
        public void ApplyPermanentEffect(Player player, AlterationEffect alterationEffect)
        {
            player.StrengthBase += alterationEffect.Strength;
            player.IntelligenceBase += alterationEffect.Intelligence;
            player.AgilityBase += alterationEffect.Agility;
            player.AuraRadiusBase += alterationEffect.AuraRadius;
            player.FoodUsagePerTurnBase += alterationEffect.FoodUsagePerTurn;

            //Blockable - if negative then block a fraction of the amount (TODO)
            player.Experience += alterationEffect.Experience;
            player.Hunger += alterationEffect.Hunger;
            player.Hp += alterationEffect.Hp;
            player.Mp += alterationEffect.Mp;

            //Apply remedies
            //foreach (var remediedSpellName in alterationEffect.RemediedSpellNames)
            //{
            //    // Alteration applies remedy to remove or modify internal collections
            //    var remediedEffects = player.Alteration.ApplyRemedy(remediedSpellName);

            //    foreach (var effect in remediedEffects)
            //        _scenarioMessageService.Publish(effect.DisplayName + " has been cured!");
            //}
        }
        public void ApplyPermanentEffect(Enemy enemy, AlterationEffect alterationEffect)
        {
            enemy.StrengthBase += alterationEffect.Strength;
            enemy.IntelligenceBase += alterationEffect.Intelligence;
            enemy.AgilityBase += alterationEffect.Agility;
            enemy.AuraRadiusBase += alterationEffect.AuraRadius;
            enemy.Hp += alterationEffect.Hp;
            enemy.Mp += alterationEffect.Mp;
        }

        private bool ProjectPlayerCanSupportAlterationEffect(Player player, AlterationEffect effect)
        {
            if (player.StrengthBase + effect.Strength < 0)
                return false;

            if (player.IntelligenceBase + effect.Intelligence < 0)
                return false;

            if (player.AgilityBase + effect.Agility < 0)
                return false;

            if (player.AuraRadiusBase + effect.AuraRadius < 0)
                return false;

            if (player.GetAttackBase() + effect.Attack < 0)
                return false;

            if (player.GetDefenseBase() + effect.Defense < 0)
                return false;

            if (player.GetMagicBlockBase() + effect.MagicBlockProbability < 0)
                return false;

            //if (p.Dodge + this.DodgeProbability < 0)
            //    return false;

            if (player.Experience + effect.Experience < 0)
                return false;

            if (player.Hunger + effect.Hunger > 100)
                return false;

            if (player.Hp + effect.Hp <= 0)
                return false;

            if (player.Mp + effect.Mp <= 0)
                return false;

            return true;
        }
        private bool ProjectEnemyCanSupportAlterationEffect(Enemy enemy, AlterationEffect effect)
        {
            if (enemy.StrengthBase + effect.Strength < 0)
                return false;

            if (enemy.IntelligenceBase + effect.Intelligence < 0)
                return false;

            if (enemy.AgilityBase + effect.Agility < 0)
                return false;

            if (enemy.AuraRadiusBase + effect.AuraRadius < 0)
                return false;

            if (enemy.Hp + effect.Hp <= 0)
                return false;

            if (enemy.Mp + effect.Mp <= 0)
                return false;

            return true;
        }
    }
}
