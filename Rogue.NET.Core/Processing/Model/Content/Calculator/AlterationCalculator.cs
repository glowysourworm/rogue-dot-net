using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using Rogue.NET.Core.Processing.Model.Content.Calculator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Content.Calculator
{
    [Export(typeof(IAlterationCalculator))]
    public class AlterationSubProcessor : IAlterationCalculator
    {
        readonly IModelService _modelService;
        readonly IScenarioMessageService _scenarioMessageService;
        readonly ISymbolDetailsGenerator _symbolDetailsGenerator;

        [ImportingConstructor]
        public AlterationSubProcessor(
                IModelService modelService,
                IScenarioMessageService scenarioMessageService,
                ISymbolDetailsGenerator symbolDetailsGenerator)
        {
            _modelService = modelService;
            _scenarioMessageService = scenarioMessageService;
            _symbolDetailsGenerator = symbolDetailsGenerator;
        }

        public ScenarioImage CalculateEffectiveSymbol(CharacterBase character)
        {
            // Copy image properties onto new instance
            var symbol = ScenarioImage.Copy(character);

            bool firstAlteration = true;

            // TODO:SYMBOL
            //foreach (var symbolChange in character.Alteration
            //                                      .GetSymbolChanges())
            //{
            //    //Full symbol
            //    if (symbolChange.IsFullSymbolChange)
            //    {
            //        // Map details onto symbol
            //        _symbolDetailsGenerator.MapSymbolDetails(symbolChange.FullSymbolChangeDetails, symbol);

            //        // Return symbol
            //        return symbol;
            //    }

            //    //Body
            //    if (symbolChange.IsSmileyBodyColorChange)
            //        symbol.SmileyBodyColor = firstAlteration ?
            //                                    symbolChange.SmileyBodyColor :
            //                                    ColorOperations.Add(symbol.SmileyBodyColor, symbolChange.SmileyBodyColor);

            //    //Line
            //    if (symbolChange.IsSmileyLineColorChange)
            //        symbol.SmileyLineColor = firstAlteration ?
            //                                    symbolChange.SmileyLineColor :
            //                                    ColorOperations.Add(symbol.SmileyLineColor, symbolChange.SmileyLineColor);

            //    //Expression
            //    if (symbolChange.IsSmileyExpressionChange)
            //        symbol.SmileyExpression = symbolChange.SmileyExpression;

            //    //Character symbol
            //    if (symbolChange.IsCharacterSymbolChange)
            //    {
            //        symbol.CharacterSymbol = symbolChange.CharacterSymbol;
            //        symbol.CharacterSymbolCategory = symbolChange.CharacterSymbolCategory;
            //    }

            //    //Character delta
            //    if (symbolChange.IsCharacterColorChange)
            //        symbol.CharacterColor = firstAlteration ?
            //                                    symbolChange.CharacterColor :
            //                                    ColorOperations.Add(symbol.CharacterColor, symbolChange.CharacterColor);

            //    firstAlteration = false;
            //}
            return symbol;
        }

        public bool CalculateMeetsAlterationCost(CharacterBase character, AlterationCost cost)
        {
            var isPlayer = character is Player;

            if (character.Health - cost.Health < 0)
            {
                if (isPlayer)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough HP");

                return false;
            }

            if (character.Stamina - cost.Stamina < 0)
            {
                if (isPlayer)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough Stamina");

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
        public bool CalculateCharacterMeetsAlterationCost(CharacterBase character, AlterationCostTemplate cost)
        {
            return (character.Health - cost.Health) >= 0 &&
                   (character.Stamina - cost.Stamina) >= 0;
        }
        public bool CalculatePlayerMeetsAlterationCost(Player player, AlterationCostTemplate cost)
        {
            if (player.Health - cost.Health < 0)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough Health");
                return false;
            }

            if (player.Stamina - cost.Stamina < 0)
            {
                _scenarioMessageService.Publish(ScenarioMessagePriority.Normal, "Not enough Stamina");
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

        public void ApplyOneTimeAlterationCost(CharacterBase character, AlterationCost alterationCost)
        {
            if (character is Player)
                ApplyOneTimeAlterationCost(character as Player, alterationCost);
            else
                ApplyOneTimeAlterationCost(character as Enemy, alterationCost);
        }
        public void ApplyPermanentEffect(CharacterBase character, PermanentAlterationEffect alterationEffect)
        {
            character.StrengthBase += alterationEffect.Strength;
            character.IntelligenceBase += alterationEffect.Intelligence;
            character.AgilityBase += alterationEffect.Agility;
            character.SpeedBase += alterationEffect.Speed;
            character.VisionBase += alterationEffect.Vision;
            character.Health += alterationEffect.Health;
            character.Stamina += alterationEffect.Stamina;

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
                if (alterationEffect.Vision > 0)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Vision has changed by " + alterationEffect.Vision.ToString("F2"));

                else if (alterationEffect.Vision < 0)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " Vision has changed by " + alterationEffect.Vision.ToString("F2"));

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
                if (alterationEffect.Health > 0)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Hp has changed by " + alterationEffect.Health.ToString("F2"));

                else if (alterationEffect.Health < 0)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " Hp has changed by " + alterationEffect.Health.ToString("F2"));

                // Mp
                if (alterationEffect.Stamina > 0)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Good, player.RogueName + " Stamina has changed by " + alterationEffect.Stamina.ToString("N0"));

                else if (alterationEffect.Stamina < 0)
                    _scenarioMessageService.Publish(ScenarioMessagePriority.Bad, player.RogueName + " Stamina has changed by " + alterationEffect.Stamina.ToString("N0"));
            }
        }
        public void ApplyRemedy(CharacterBase character, RemedyAlterationEffect alterationEffect)
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
        public void ApplyEquipmentDamageEffect(CharacterBase affectedCharacter, EquipmentDamageAlterationEffect effect, Equipment item)
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

        public void ApplyDrainMeleeEffect(CharacterBase actor, CharacterBase affectedCharacter, DrainMeleeAlterationEffect effect)
        {
            actor.Health += effect.Health;
            affectedCharacter.Health -= effect.Health;

            actor.Stamina += effect.Stamina;
            affectedCharacter.Stamina -= effect.Stamina;

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
            if (effect.Health > 0)
            {
                _scenarioMessageService
                    .Publish(ScenarioMessagePriority.Normal,
                             "{0} has drained {1} Health from {2}",
                             _modelService.GetDisplayName(actor),
                             effect.Health.ToString("F1"),
                             _modelService.GetDisplayName(affectedCharacter));
            }

            if (effect.Stamina > 0)
            {
                _scenarioMessageService
                    .Publish(ScenarioMessagePriority.Normal,
                             "{0} has drained {1} Stamina from {2}",
                             _modelService.GetDisplayName(actor),
                             effect.Stamina.ToString("F1"),
                             _modelService.GetDisplayName(affectedCharacter));
            }
        }

        protected void ApplyOneTimeAlterationCost(Player player, AlterationCost alterationCost)
        {
            player.Experience -= alterationCost.Experience;
            player.Health -= alterationCost.Health;
            player.Hunger += alterationCost.Hunger;
            player.Stamina -= alterationCost.Stamina;
        }
        protected void ApplyOneTimeAlterationCost(NonPlayerCharacter character, AlterationCost alterationCost)
        {
            character.Health -= alterationCost.Health;
            character.Stamina -= alterationCost.Stamina;
        }
    }
}
