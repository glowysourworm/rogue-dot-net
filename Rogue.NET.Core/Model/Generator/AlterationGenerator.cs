﻿using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(IAlterationGenerator))]
    public class AlterationGenerator : IAlterationGenerator
    {
        private readonly IRandomSequenceGenerator _randomSequenceGenerator;
        private readonly IAttackAttributeGenerator _attackAttributeGenerator;

        [ImportingConstructor]
        public AlterationGenerator(
            IRandomSequenceGenerator randomSequenceGenerator, 
            IAttackAttributeGenerator attackAttributeGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;
        }

        public AlterationContainer GenerateAlteration(Spell spell)
        {
            AlterationCost alterationCost = new AlterationCost();
            AlterationEffect alterationEffect = GenerateAlterationEffect(spell.RogueName, spell.DisplayName, spell.EffectRange, spell.Effect);
            AlterationEffect auraEffect = GenerateAlterationEffect(spell.RogueName, spell.DisplayName, spell.EffectRange, spell.AuraEffect);

            alterationCost.Type = spell.Cost.Type;
            alterationCost.Agility = spell.Cost.Agility;
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
            alterationContainer.BlockType = spell.BlockType;
            alterationContainer.Type = spell.Type;
            alterationContainer.IsStackable = spell.IsStackable;
            alterationContainer.CreateMonsterEnemy = spell.CreateMonsterEnemyName;

            return alterationContainer;
        }
        public AlterationEffect GenerateAlterationEffect(string spellName, string spellDisplayName, double effectRange, AlterationEffectTemplate alterationEffectTemplate)
        {
            AlterationEffect alterationEffect = new AlterationEffect();

            alterationEffect.Agility = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.AgilityRange);
            alterationEffect.Attack = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.AttackRange);
            alterationEffect.AuraRadius = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.AuraRadiusRange);
            alterationEffect.Defense = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.DefenseRange);
            alterationEffect.DodgeProbability = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.DodgeProbabilityRange);
            alterationEffect.Experience = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.ExperienceRange);
            alterationEffect.EventTime = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.EventTime);
            alterationEffect.FoodUsagePerTurn = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.FoodUsagePerTurnRange);
            alterationEffect.Hp = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.HpRange);
            alterationEffect.HpPerStep = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.HpPerStepRange);
            alterationEffect.Hunger = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.HungerRange);
            alterationEffect.Intelligence = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.IntelligenceRange);
            alterationEffect.IsSymbolAlteration = alterationEffectTemplate.IsSymbolAlteration;
            alterationEffect.MagicBlockProbability = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.MagicBlockProbabilityRange);
            alterationEffect.Mp = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.MpRange);
            alterationEffect.MpPerStep = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.MpPerStepRange);
            alterationEffect.PostEffectString = alterationEffectTemplate.PostEffectText;
            alterationEffect.RogueName = alterationEffectTemplate.Name;
            alterationEffect.State = alterationEffectTemplate.StateType;
            alterationEffect.Strength = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.StrengthRange);
            alterationEffect.SymbolAlteration = alterationEffectTemplate.SymbolAlteration;
            alterationEffect.RogueName = spellName;
            alterationEffect.DisplayName = spellDisplayName;
            alterationEffect.CriticalHit = _randomSequenceGenerator.GetRandomValue(alterationEffectTemplate.CriticalHit);

            //Store list of remedied spells
            alterationEffect.RemediedSpellNames = alterationEffectTemplate.RemediedSpells.Select(z => z.Name).ToList();

            //Attack Attributes
            alterationEffect.AttackAttributes = alterationEffectTemplate.AttackAttributes
                                                      .Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x))
                                                      .ToList();

            //Copied to aura effect so that it can detach from the spell
            alterationEffect.EffectRange = effectRange;

            return alterationEffect;
        }
    }
}