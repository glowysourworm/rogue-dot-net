﻿using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
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

            foreach (var symbolDelta in character.Alteration
                                                 .GetSymbolChanges())
            {
                //Full symbol
                if (symbolDelta.IsFullSymbolDelta)
                    return new ScenarioImage()
                    {
                        CharacterColor = symbolDelta.CharacterColor,
                        CharacterSymbol = symbolDelta.SmileyAuraColor,
                        Icon = symbolDelta.Icon,
                        SmileyAuraColor = symbolDelta.SmileyAuraColor,
                        SmileyBodyColor = symbolDelta.SmileyBodyColor,
                        SmileyLineColor = symbolDelta.SmileyLineColor,
                        SmileyMood = symbolDelta.SmileyMood,
                        SymbolType = symbolDelta.Type
                    };

                //Aura
                if (symbolDelta.IsAuraDelta)
                    symbol.SmileyAuraColor = firstAlteration ?
                                                symbolDelta.SmileyAuraColor :
                                                ColorUtility.Add(symbol.SmileyAuraColor, symbolDelta.SmileyAuraColor);

                //Body
                if (symbolDelta.IsBodyDelta)
                    symbol.SmileyBodyColor = firstAlteration ?
                                                symbolDelta.SmileyBodyColor :
                                                ColorUtility.Add(symbol.SmileyBodyColor, symbolDelta.SmileyBodyColor);

                //Character symbol
                if (symbolDelta.IsCharacterDelta)
                    symbol.CharacterSymbol = symbolDelta.CharacterSymbol;

                //Character delta
                if (symbolDelta.IsColorDelta)
                    symbol.CharacterColor = firstAlteration ?
                                                symbolDelta.CharacterColor :
                                                ColorUtility.Add(symbol.CharacterColor, symbolDelta.CharacterColor);

                //Image
                if (symbolDelta.IsImageDelta)
                    symbol.Icon = symbolDelta.Icon;

                //Line
                if (symbolDelta.IsLineDelta)
                    symbol.SmileyLineColor = firstAlteration ?
                                                symbolDelta.SmileyLineColor :
                                                ColorUtility.Add(symbol.SmileyLineColor, symbolDelta.SmileyLineColor);

                //Mood
                if (symbolDelta.IsMoodDelta)
                    symbol.SmileyMood = symbolDelta.SmileyMood;

                firstAlteration = false;
            }
            return symbol;
        }

        public bool CalculateMeetsAlterationCost(Character character, AlterationCost cost)
        {
            var isPlayer = character is Player;

            if (character.AgilityBase - cost.Agility < 0)
            {
                if (isPlayer)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough Agility");

                return false;
            }

            if (character.SpeedBase - cost.Speed < ModelConstants.MinSpeed)
            {
                if (isPlayer)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough Speed");

                return false;
            }

            if (character.Hp - cost.Hp < 0)
            {
                if (isPlayer)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough HP");

                return false;
            }

            if (character.IntelligenceBase - cost.Intelligence < 0)
            {
                if (isPlayer)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough Intelligence");

                return false;
            }

            if (character.Mp - cost.Mp < 0)
            {
                if (isPlayer)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough MP");

                return false;
            }

            if (character.StrengthBase - cost.Strength < 0)
            {
                if (isPlayer)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough Strength");

                return false;
            }

            if (character.LightRadiusBase - cost.LightRadius < 0)
            {
                if (isPlayer)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Will go Blind!");

                return false;
            }

            // Player-Only
            if (isPlayer)
            {
                var player = character as Player;

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
            }

            return true;
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

            if (player.LightRadiusBase - cost.AuraRadius < 0)
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
        public void ApplyPermanentEffect(Character character, PermanentAlterationEffect alterationEffect)
        {
            character.StrengthBase += alterationEffect.Strength;
            character.IntelligenceBase += alterationEffect.Intelligence;
            character.AgilityBase += alterationEffect.Agility;
            character.SpeedBase += alterationEffect.Speed;
            character.LightRadiusBase += alterationEffect.LightRadius;
            character.Hp += alterationEffect.Hp;
            character.Mp += alterationEffect.Mp;

            // Player Specific - (Also Publish Messages)
            if (character is Player)
            {
                var player = character as Player;

                player.Experience += alterationEffect.Experience;
                player.Hunger += alterationEffect.Hunger;

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

                // LightRadius
                if (alterationEffect.LightRadius > 0)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Light Radius has changed by " + alterationEffect.LightRadius.ToString("F2"));

                else if (alterationEffect.LightRadius < 0)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " Light Radius has changed by " + alterationEffect.LightRadius.ToString("F2"));

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
        }
        public void ApplyRemedy(Character character, RemedyAlterationEffect alterationEffect)
        {
            // Alteration applies remedy to remove or modify internal collections
            var remediedEffects = character.Alteration.ApplyRemedy(alterationEffect);

            // Publish Messages
            foreach (var effect in remediedEffects)
            {
                _scenarioMessageService.Publish(
                    ScenarioMessagePriority.Good,
                    "{0} has been cured!",
                    alterationEffect.RogueName);
            }
        }

        protected void ApplyOneTimeAlterationCost(Player player, AlterationCost alterationCost)
        {
            player.AgilityBase -= alterationCost.Agility;
            player.LightRadiusBase -= alterationCost.LightRadius;
            player.Experience -= alterationCost.Experience;
            player.FoodUsagePerTurnBase += alterationCost.FoodUsagePerTurn;
            player.Hp -= alterationCost.Hp;
            player.Hunger += alterationCost.Hunger;
            player.IntelligenceBase -= alterationCost.Intelligence;
            player.Mp -= alterationCost.Mp;
            player.SpeedBase -= alterationCost.Speed;
            player.StrengthBase -= alterationCost.Strength;
        }
        protected void ApplyOneTimeAlterationCost(Enemy enemy, AlterationCost alterationCost)
        {
            enemy.AgilityBase -= alterationCost.Agility;
            enemy.LightRadiusBase -= alterationCost.LightRadius;
            enemy.SpeedBase -= alterationCost.Speed;
            enemy.Hp -= alterationCost.Hp;
            enemy.IntelligenceBase -= alterationCost.Intelligence;
            enemy.Mp -= alterationCost.Mp;
            enemy.StrengthBase -= alterationCost.Strength;
        }
    }
}
