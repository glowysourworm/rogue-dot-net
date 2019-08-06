using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    [Serializable]
    public class PassiveAlterationCollector 
                    : IAlterationCollector,
                      IAlterationEffectCollector<PassiveAlterationEffect>
    {
        protected IDictionary<string, PassiveAlterationEffect> Alterations { get; set; }
        protected IDictionary<string, AlterationCost> Costs { get; set; }

        public PassiveAlterationCollector()
        {
            this.Alterations = new Dictionary<string, PassiveAlterationEffect>();
            this.Costs = new Dictionary<string, AlterationCost>();
        }

        public bool Apply(string alterationId, PassiveAlterationEffect alterationEffect, AlterationCost cost = null)
        {
            this.Alterations.Add(alterationId, alterationEffect);

            if (cost != null)
                this.Costs.Add(alterationId, cost);

            return true;
        }

        public void Filter(string alterationId)
        {
            if (this.Alterations.ContainsKey(alterationId))
                this.Alterations.Remove(alterationId);

            if (this.Costs.ContainsKey(alterationId))
                this.Costs.Remove(alterationId);
        }

        public bool CanSeeInvisible()
        {
            return this.Alterations
                       .Values
                       .Any(x => x.CanSeeInvisibleCharacters);
        }

        public IEnumerable<AlterationCost> GetCosts()
        {
            return this.Costs.Values;
        }

        public IEnumerable<SymbolDeltaTemplate> GetSymbolChanges()
        {
            return this.Alterations
                       .Values
                       .Where(x => x.SymbolAlteration.HasSymbolDelta())
                       .Select(x => x.SymbolAlteration)
                       .Actualize();
        }

        public IEnumerable<AlteredCharacterState> GetAlteredStates()
        {
            return new List<AlteredCharacterState>();
        }

        public double GetAttributeAggregate(CharacterAttribute attribute)
        {
            return this.Alterations
                       .Values
                       .Aggregate(0D, (aggregator, effect) =>
                       {
                           switch (attribute)
                           {
                               case CharacterAttribute.Hp:
                                   aggregator += effect.Hp;
                                   break;
                               case CharacterAttribute.Mp:
                                   aggregator += effect.Mp;
                                   break;
                               case CharacterAttribute.Strength:
                                   aggregator += effect.Strength;
                                   break;
                               case CharacterAttribute.Agility:
                                   aggregator += effect.Agility;
                                   break;
                               case CharacterAttribute.Intelligence:
                                   aggregator += effect.Intelligence;
                                   break;
                               case CharacterAttribute.Speed:
                                   aggregator += effect.Speed;
                                   break;
                               case CharacterAttribute.HpRegen:
                                   aggregator += effect.HpPerStep;
                                   break;
                               case CharacterAttribute.MpRegen:
                                   aggregator += effect.MpPerStep;
                                   break;
                               case CharacterAttribute.LightRadius:
                                   aggregator += effect.LightRadius;
                                   break;
                               case CharacterAttribute.Attack:
                                   aggregator += effect.Attack;
                                   break;
                               case CharacterAttribute.Defense:
                                   aggregator += effect.Defense;
                                   break;
                               case CharacterAttribute.Dodge:
                                   aggregator += effect.DodgeProbability;
                                   break;
                               case CharacterAttribute.MagicBlock:
                                   aggregator += effect.MagicBlockProbability;
                                   break;
                               case CharacterAttribute.CriticalHit:
                                   aggregator += effect.CriticalHit;
                                   break;
                               case CharacterAttribute.FoodUsagePerTurn:
                                   aggregator += effect.FoodUsagePerTurn;
                                   break;
                               default:
                                   break;
                           }
                           return aggregator;
                       });
        }
    }
}
