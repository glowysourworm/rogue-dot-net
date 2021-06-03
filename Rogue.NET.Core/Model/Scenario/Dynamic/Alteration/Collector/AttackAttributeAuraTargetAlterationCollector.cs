using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    /// <summary>
    /// Collects alterations that are effecting the character
    /// </summary>
    [Serializable]
    public class AttackAttributeAuraTargetAlterationCollector 
               : IAlterationEffectCollector,
                 IAttackAttributeAlterationCollector
    {
        protected IList<AttackAttributeAuraAlterationEffect> TargetEffects { get; set; }

        public AttackAttributeAuraTargetAlterationCollector()
        {
            this.TargetEffects = new List<AttackAttributeAuraAlterationEffect>();
        }

        /// <summary>
        /// Clears / re-applies all specified effects that act ON THE CHARACTER. This should
        /// be done at the end of each turn.
        /// </summary>
        public void Apply(IEnumerable<AttackAttributeAuraAlterationEffect> alterationEffects)
        {
            this.TargetEffects.Clear();
            foreach (var effect in alterationEffects)
                this.TargetEffects.Add(effect);
        }

        public IEnumerable<KeyValuePair<string, IAlterationEffect>> GetEffects(bool includeSourceEffects = false)
        {
            return this.TargetEffects
                       .ToSimpleDictionary(x => x.RogueName, x => (IAlterationEffect)x);
        }

        public IEnumerable<SymbolEffectTemplate> GetSymbolChanges()
        {
            return this.TargetEffects
                       .Where(x => x.SymbolAlteration.HasSymbolChange())
                       .Select(x => x.SymbolAlteration)
                       .Actualize();
        }

        public IEnumerable<AlteredCharacterState> GetAlteredStates()
        {
            // Attack Attribute Auras don't support altered states
            return new List<AlteredCharacterState>();
        }

        public double GetAttributeAggregate(CharacterAttribute attribute)
        {
            return 0D;
        }

        public IEnumerable<AttackAttribute> GetAttackAttributes(AlterationAttackAttributeCombatType combatType)
        {
            return this.TargetEffects
                        .Where(x => x.CombatType == combatType)
                        .Aggregate(new List<AttackAttribute>(), (aggregator, effect) =>
                        {
                            foreach (var attackAttribute in effect.AttackAttributes)
                            {
                                var existingAttackAttribute = aggregator.FirstOrDefault(x => x.RogueName == attackAttribute.RogueName);

                                if (existingAttackAttribute == null)
                                    aggregator.Add(attackAttribute);

                                else
                                {
                                    existingAttackAttribute.Attack += attackAttribute.Attack;
                                    existingAttackAttribute.Resistance += attackAttribute.Resistance;
                                    existingAttackAttribute.Weakness += attackAttribute.Weakness;
                                }
                            }

                            return aggregator;
                        });
        }

        public IEnumerable<string> GetEffectNames(AlterationAttackAttributeCombatType combatType)
        {
            // TODO:ALTERATION - Have to apply the AlterationContainer.RogueName to all the
            //                   IAlterationEffect instances.
            return this.TargetEffects
                       .Where(x => x.CombatType == combatType)
                       .Select(x => x.RogueName)
                       .Actualize();
        }
    }
}
