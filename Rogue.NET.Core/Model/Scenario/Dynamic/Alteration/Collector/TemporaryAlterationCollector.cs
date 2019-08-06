using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    [Serializable]
    public class TemporaryAlterationCollector 
                    : IAlterationCollector,
                      IAlterationEffectCollector<TemporaryAlterationEffect>, 
                      ITurnBasedAlterationCollector
    {
        protected IDictionary<string, TemporaryAlterationEffect> Alterations { get; set; }

        public TemporaryAlterationCollector()
        {
            this.Alterations = new Dictionary<string, TemporaryAlterationEffect>();
        }

        public bool Apply(string alterationId, TemporaryAlterationEffect effect, AlterationCost cost = null)
        {
            if (effect.IsStackable &&
                this.Alterations.Any(x => x.Value.RogueName == effect.RogueName))
                return false;

            this.Alterations.Add(alterationId, effect);

            return true;
        }

        public void ApplyRemedy(RemedyAlterationEffect effect)
        {
            this.Alterations.Filter(x => x.Value.AlteredState.RogueName == effect.RemediedState.RogueName);
        }

        public void Filter(string alterationId)
        {
            this.Alterations.Remove(alterationId);
        }

        public bool CanSeeInvisible()
        {
            return this.Alterations
                       .Values
                       .Any(x => x.CanSeeInvisibleCharacters);
        }

        public IEnumerable<AlteredCharacterState> GetAlteredStates()
        {
            return this.Alterations
                       .Where(x => x.Value.AlteredState.BaseType != Enums.CharacterStateType.Normal)
                       .Select(x => x.Value.AlteredState)
                       .Actualize();
        }
        public void ApplyEndOfTurn()
        {
            for (int i=this.Alterations.Count - 1;i>=0;i--)
            {
                var item = this.Alterations.ElementAt(i);

                item.Value.EventTime--;

                if (item.Value.EventTime == 0)
                    this.Alterations.Remove(item.Key);
            }
        }

        public IEnumerable<AlterationCost> GetCosts()
        {
            return new List<AlterationCost>();
        }

        public IEnumerable<SymbolDeltaTemplate> GetSymbolChanges()
        {
            return this.Alterations
                       .Values
                       .Where(x => x.SymbolAlteration.HasSymbolDelta())
                       .Select(x => x.SymbolAlteration)
                       .Actualize();
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
