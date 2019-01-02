using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(IAlterationGenerator))]
    public class AlterationGenerator : IAlterationGenerator
    {
        private readonly IRandomSequenceGenerator _randomSequenceGenerator;
        private readonly IAttackAttributeGenerator _attackAttributeGenerator;
        private readonly IAlteredStateGenerator _alteredStateGenerator;

        [ImportingConstructor]
        public AlterationGenerator(
            IRandomSequenceGenerator randomSequenceGenerator, 
            IAttackAttributeGenerator attackAttributeGenerator,
            IAlteredStateGenerator alteredStateGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;
            _alteredStateGenerator = alteredStateGenerator;
        }

        public AlterationContainer GenerateAlteration(Spell spell, double characterIntelligence)
        {
            AlterationCost alterationCost = new AlterationCost();
            AlterationEffect alterationEffect = GenerateAlterationEffect(spell.RogueName, spell.DisplayName, spell.EffectRange, spell.Effect, characterIntelligence, false);
            AlterationEffect auraEffect = GenerateAlterationEffect(spell.RogueName, spell.DisplayName, spell.EffectRange, spell.AuraEffect, characterIntelligence, false);

            alterationCost.Type = spell.Cost.Type;
            alterationCost.Agility = spell.Cost.Agility;
            alterationCost.Speed = spell.Cost.Speed;
            alterationCost.AuraRadius = spell.Cost.AuraRadius;
            alterationCost.Experience = spell.Cost.Experience;
            alterationCost.FoodUsagePerTurn = spell.Cost.FoodUsagePerTurn;
            alterationCost.Hp = spell.Cost.Hp;
            alterationCost.Hunger = spell.Cost.Hunger;
            alterationCost.Intelligence = spell.Cost.Intelligence;
            alterationCost.Mp = spell.Cost.Mp;
            alterationCost.Strength = spell.Cost.Strength;

            AlterationContainer alterationContainer = new AlterationContainer();
            alterationContainer.Cost = alterationCost;
            alterationContainer.Effect = alterationEffect;
            alterationContainer.AuraEffect = auraEffect;
            alterationContainer.RogueName = spell.RogueName;
            alterationContainer.EffectRange = spell.EffectRange;
            alterationContainer.OtherEffectType = spell.OtherEffectType;
            alterationContainer.AttackAttributeType = spell.AttackAttributeType;
            alterationContainer.Type = spell.Type;
            alterationContainer.BlockType = spell.BlockType;
            alterationContainer.GeneratingSpellId = spell.Id;
            alterationContainer.GeneratingSpellName = spell.RogueName;
            alterationContainer.IsStackable = spell.IsStackable;
            alterationContainer.CreateMonsterEnemy = spell.CreateMonsterEnemyName;

            return alterationContainer;
        }
        public AlterationEffect GenerateAlterationEffect(
                string spellName, 
                string spellDisplayName, 
                double effectRange, 
                AlterationEffectTemplate alterationEffectTemplate, 
                double characterIntelligence,
                bool scaleByIntelligence)
        {
            AlterationEffect alterationEffect = new AlterationEffect();

            var scale = scaleByIntelligence ? (1 + characterIntelligence) : 1;

            // Scaled Parameters
            alterationEffect.Agility = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.AgilityRange);
            alterationEffect.Attack = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.AttackRange);
            alterationEffect.AuraRadius = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.AuraRadiusRange);
            alterationEffect.Defense = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.DefenseRange);
            alterationEffect.DodgeProbability = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.DodgeProbabilityRange);
            alterationEffect.Experience = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.ExperienceRange);
            alterationEffect.FoodUsagePerTurn = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.FoodUsagePerTurnRange);
            alterationEffect.Hp = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.HpRange);
            alterationEffect.HpPerStep = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.HpPerStepRange);
            alterationEffect.Hunger = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.HungerRange);
            alterationEffect.Intelligence = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.IntelligenceRange);
            alterationEffect.MagicBlockProbability = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.MagicBlockProbabilityRange);
            alterationEffect.Mp = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.MpRange);
            alterationEffect.MpPerStep = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.MpPerStepRange);
            alterationEffect.Speed = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.SpeedRange);
            alterationEffect.Strength = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.StrengthRange);
            alterationEffect.CriticalHit = scale * _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.CriticalHit);

            // Scaled - Attack Attributes
            alterationEffect.AttackAttributes = alterationEffectTemplate.AttackAttributes
                                                      .Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x))
                                                      .Select(x =>
                                                      {
                                                          x.Attack = scale * x.Attack;
                                                          x.Resistance = scale * x.Resistance;

                                                          return x;
                                                      })
                                                      .ToList();

            // Non-Scaled Parameters
            alterationEffect.EventTime = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.EventTime);

            // Non-Scaled - Copied to aura effect so that it can detach from the spell
            alterationEffect.EffectRange = effectRange;

            alterationEffect.IsSymbolAlteration = alterationEffectTemplate.IsSymbolAlteration;
            alterationEffect.RogueName = alterationEffectTemplate.Name;
            alterationEffect.State = _alteredStateGenerator.GenerateAlteredState(alterationEffectTemplate.AlteredState);
            alterationEffect.SymbolAlteration = alterationEffectTemplate.SymbolAlteration;
            alterationEffect.RogueName = spellName;
            alterationEffect.DisplayName = spellDisplayName;
            alterationEffect.CanSeeInvisibleCharacters = alterationEffectTemplate.CanSeeInvisibleCharacters;

            //Store remedied state name
            alterationEffect.RemediedStateName = alterationEffectTemplate.RemediedState.Name;

            return alterationEffect;
        }
    }
}
