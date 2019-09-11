using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using Rogue.NET.Core.Processing.Model.Content.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Utility;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Content
{
    [Export(typeof(IAlterationProcessor))]
    public class AlterationProcessor : IAlterationProcessor
    {
        readonly IModelService _modelService;
        readonly ILayoutEngine _layoutEngine;
        readonly IScenarioMessageService _scenarioMessageService;

        [ImportingConstructor]
        public AlterationProcessor(
                IModelService modelService,
                ILayoutEngine layoutEngine,
                IScenarioMessageService scenarioMessageService)
        {
            _modelService = modelService;
            _layoutEngine = layoutEngine;
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
                        SmileyLightRadiusColor = symbolDelta.SmileyAuraColor,
                        SmileyBodyColor = symbolDelta.SmileyBodyColor,
                        SmileyLineColor = symbolDelta.SmileyLineColor,
                        SmileyExpression = symbolDelta.SmileyExpression,
                        SymbolType = symbolDelta.Type
                    };

                //Aura
                if (symbolDelta.IsAuraDelta)
                    symbol.SmileyLightRadiusColor = firstAlteration ?
                                                symbolDelta.SmileyAuraColor :
                                                ColorUtility.Add(symbol.SmileyLightRadiusColor, symbolDelta.SmileyAuraColor);

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

                //Expression
                if (symbolDelta.IsExpressionDelta)
                    symbol.SmileyExpression = symbolDelta.SmileyExpression;

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
        public bool CalculateCharacterMeetsAlterationCost(Character character, AlterationCostTemplate cost)
        {
            return (character.AgilityBase - cost.Agility) >= 0 &&
                   (character.SpeedBase - cost.Speed) >= ModelConstants.MinSpeed &&
                   (character.Hp - cost.Hp) >= 0 &&
                   (character.IntelligenceBase - cost.Intelligence) >= 0 &&
                   (character.Mp - cost.Mp) >= 0 &&
                   (character.StrengthBase - cost.Strength) >= 0;
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
        public void ApplyEquipmentEnhanceEffect(Player player, EquipmentEnhanceAlterationEffect effect, Equipment item)
        {
            switch (effect.Type)
            {
                case AlterationModifyEquipmentType.ArmorClass:
                case AlterationModifyEquipmentType.WeaponClass:
                    {
                        // Change class of item
                        item.Class += effect.ClassChange;

                        // Publish message
                        _scenarioMessageService
                            .Publish(ScenarioMessagePriority.Good,
                                     "Your {0} glows gently...",
                                     _modelService.GetDisplayName(item));
                    }
                    break;
                case AlterationModifyEquipmentType.ArmorImbue:
                case AlterationModifyEquipmentType.WeaponImbue:
                    {
                        foreach (var attackAttribute in effect.AttackAttributes
                                                              .Where(x => x.Attack != 0 ||
                                                                          x.Resistance != 0 ||
                                                                          x.Weakness != 0))
                        {
                            var equipmentAttackAttribute = item.AttackAttributes.First(x => x.RogueName == attackAttribute.RogueName);

                            equipmentAttackAttribute.Attack += attackAttribute.Attack;
                            equipmentAttackAttribute.Resistance += attackAttribute.Resistance;
                            equipmentAttackAttribute.Weakness -= attackAttribute.Weakness;

                            // Make sure to clip weakness value to zero
                            equipmentAttackAttribute.Weakness = equipmentAttackAttribute.Weakness.LowLimit(0);
                        }

                        // Publish message
                        _scenarioMessageService
                            .Publish(ScenarioMessagePriority.Normal,
                                     "Your {0} shines with a brilliant radiance!",
                                     _modelService.GetDisplayName(item));
                    }
                    break;
                case AlterationModifyEquipmentType.ArmorQuality:
                case AlterationModifyEquipmentType.WeaponQuality:
                    {
                        // Change class of item
                        item.Quality += effect.QualityChange;

                        // Publish message
                        _scenarioMessageService
                            .Publish(ScenarioMessagePriority.Good,
                                     "Your {0} has improved",
                                     _modelService.GetDisplayName(item));
                    }
                    break;
                default:
                    throw new Exception("Unhandled Alteration Modify Equipment Type");
            }
        }
        public void ApplyEquipmentDamageEffect(Character affectedCharacter, EquipmentDamageAlterationEffect effect, Equipment item)
        {
            switch (effect.Type)
            {
                case AlterationModifyEquipmentType.ArmorClass:
                case AlterationModifyEquipmentType.WeaponClass:
                    {
                        // Change class of item
                        item.Class -= effect.ClassChange;

                        // Clip value of class to zero
                        item.Class = item.Class.LowLimit(0);

                        // Publish message
                        if (affectedCharacter is Player)
                        {
                            _scenarioMessageService
                                .Publish(ScenarioMessagePriority.Bad,
                                         "Your {0} darkens...!",
                                         _modelService.GetDisplayName(item));
                        }
                        else
                        {
                            _scenarioMessageService
                                .Publish(ScenarioMessagePriority.Normal,
                                         "You dis-enchanted {0}'s {1}", 
                                         _modelService.GetDisplayName(affectedCharacter),
                                         _modelService.GetDisplayName(item));
                        }
                    }
                    break;
                case AlterationModifyEquipmentType.ArmorImbue:
                case AlterationModifyEquipmentType.WeaponImbue:
                    {
                        foreach (var attackAttribute in effect.AttackAttributes
                                                              .Where(x => x.Attack != 0 ||
                                                                          x.Resistance != 0 ||
                                                                          x.Weakness != 0))
                        {
                            var equipmentAttackAttribute = item.AttackAttributes.First(x => x.RogueName == attackAttribute.RogueName);

                            equipmentAttackAttribute.Attack -= attackAttribute.Attack;
                            equipmentAttackAttribute.Resistance -= attackAttribute.Resistance;
                            equipmentAttackAttribute.Weakness += attackAttribute.Weakness;

                            // Clip Values to Zero
                            equipmentAttackAttribute.Attack = equipmentAttackAttribute.Attack.LowLimit(0);
                            equipmentAttackAttribute.Resistance = equipmentAttackAttribute.Resistance.LowLimit(0);
                        }

                        // Publish message
                        if (affectedCharacter is Player)
                        {
                            _scenarioMessageService
                                .Publish(ScenarioMessagePriority.Bad,
                                         "Your {0} looses its radiance",
                                         _modelService.GetDisplayName(item));
                        }
                        else
                        {
                            _scenarioMessageService
                                .Publish(ScenarioMessagePriority.Normal,
                                         "{0}'s {1} looses its radiance",
                                         _modelService.GetDisplayName(affectedCharacter),
                                         _modelService.GetDisplayName(item));
                        }
                    }
                    break;
                case AlterationModifyEquipmentType.ArmorQuality:
                case AlterationModifyEquipmentType.WeaponQuality:
                    {
                        // Change quality of item
                        item.Quality -= effect.QualityChange;

                        // Clip value to zero
                        item.Quality = item.Quality.LowLimit(0);

                        // Publish message
                        if (affectedCharacter is Player)
                        {
                            _scenarioMessageService
                                .Publish(ScenarioMessagePriority.Bad,
                                         "Your {0} is damaged!",
                                         _modelService.GetDisplayName(item));
                        }
                        else
                        {
                            _scenarioMessageService
                                .Publish(ScenarioMessagePriority.Normal,
                                         "{0}'s {1} is damaged",
                                         _modelService.GetDisplayName(affectedCharacter),
                                         _modelService.GetDisplayName(item));
                        }
                    }
                    break;
                default:
                    throw new Exception("Unhandled Alteration Modify Equipment Type");
            }
        }

        public void ApplyDrainMeleeEffect(Character actor, Character affectedCharacter, DrainMeleeAlterationEffect effect)
        {
            actor.Hp += effect.Hp;
            affectedCharacter.Hp -= effect.Hp;

            actor.Mp += effect.Mp;
            affectedCharacter.Mp -= effect.Mp;

            // TODO: Clean up ApplyLimits extension to a singe method
            if (actor is Player)
                (actor as Player).ApplyLimits();

            else if (actor is Enemy)
                (actor as Enemy).ApplyLimits();

            if (affectedCharacter is Player)
                (affectedCharacter as Player).ApplyLimits();

            else if (affectedCharacter is Enemy)
                (affectedCharacter as Enemy).ApplyLimits();

            // TODO: Clean up this message
            if (effect.Hp > 0)
            {
                _scenarioMessageService
                    .Publish(ScenarioMessagePriority.Normal,
                             "{0} has drained {1} Hp from {2}",
                             _modelService.GetDisplayName(actor),
                             effect.Hp.ToString("F1"),
                             _modelService.GetDisplayName(affectedCharacter));
            }

            if (effect.Mp > 0)
            {
                _scenarioMessageService
                    .Publish(ScenarioMessagePriority.Normal,
                             "{0} has drained {1} Mp from {2}",
                             _modelService.GetDisplayName(actor),
                             effect.Mp.ToString("F1"),
                             _modelService.GetDisplayName(affectedCharacter));
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
