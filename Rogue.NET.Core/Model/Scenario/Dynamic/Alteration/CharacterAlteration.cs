using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Consumable;
using Rogue.NET.Core.Model.Scenario.Alteration.Doodad;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Enemy;
using Rogue.NET.Core.Model.Scenario.Alteration.Equipment;
using Rogue.NET.Core.Model.Scenario.Alteration.Skill;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Alteration.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration
{
    /// <summary>
    /// Complete list of active character alterations - costs and effects. This is serialized along with the model
    /// to maintain dynamic (game-time) state.
    /// </summary>
    [Serializable]
    public class CharacterAlteration
    {
        private static readonly CharacterAlterationTypeValidator _characterAlterationTypeValidator = new CharacterAlterationTypeValidator();

        protected AttackAttributeAuraSourceAlterationCollector AttackAttributeAuraSourceCollector { get; set; }
        protected AttackAttributeAuraTargetAlterationCollector AttackAttributeAuraTargetCollector { get; set; }
        protected AttackAttributePassiveAlterationCollector AttackAttributePassiveCollector { get; set; }
        protected AttackAttributeTemporaryAlterationCollector AttackAttributeTemporaryCollector { get; set; }
        protected AuraSourceAlterationCollector AuraSourceCollector { get; set; }
        protected AuraTargetAlterationCollector AuraTargetCollector { get; set; }
        protected PassiveAlterationCollector PassiveCollector { get; set; }
        protected TemporaryAlterationCollector TemporaryCollector { get; set; }

        // AlterationContainer collectors
        private readonly IAlterationCollector[] _collectors;

        // IAlterationEffect collectors (provides query methods)
        private readonly IAlterationEffectCollector[] _effectCollectors;

        // AttackAttribute collectors
        private readonly IAttackAttributeAlterationCollector[] _attackAttributeCollectors;

        // Temporary IAlterationEffect collectors (provides end-of-turn / remedy methods)
        private readonly ITemporaryAlterationCollector[] _temporaryCollectors;

        /// <summary>
        /// Crates a character alteration
        /// </summary>
        public CharacterAlteration()
        {
            this.AttackAttributeAuraSourceCollector = new AttackAttributeAuraSourceAlterationCollector();
            this.AttackAttributeAuraTargetCollector = new AttackAttributeAuraTargetAlterationCollector();
            this.AttackAttributePassiveCollector = new AttackAttributePassiveAlterationCollector();
            this.AttackAttributeTemporaryCollector = new AttackAttributeTemporaryAlterationCollector();
            this.AuraSourceCollector = new AuraSourceAlterationCollector();
            this.AuraTargetCollector = new AuraTargetAlterationCollector();
            this.PassiveCollector = new PassiveAlterationCollector();
            this.TemporaryCollector = new TemporaryAlterationCollector();

            _collectors = new IAlterationCollector[]
            {
                this.AttackAttributeAuraSourceCollector,
                this.AttackAttributePassiveCollector,
                this.AttackAttributeTemporaryCollector,
                this.AuraSourceCollector,
                this.PassiveCollector,
                this.TemporaryCollector
            };

            _effectCollectors = new IAlterationEffectCollector[]
            {
                this.AttackAttributeAuraSourceCollector,
                this.AttackAttributeAuraTargetCollector,
                this.AttackAttributePassiveCollector,
                this.AttackAttributeTemporaryCollector,
                this.AuraTargetCollector,
                this.AuraSourceCollector,
                this.PassiveCollector,
                this.TemporaryCollector
            };

            _attackAttributeCollectors = new IAttackAttributeAlterationCollector[]
            {
                this.AttackAttributeAuraTargetCollector,
                this.AttackAttributePassiveCollector,
                this.AttackAttributeTemporaryCollector
            };

            _temporaryCollectors = new ITemporaryAlterationCollector[]
            {
                this.AttackAttributeTemporaryCollector,
                this.TemporaryCollector
            };
        }

        #region (public) Query Methods
        /// <summary>
        /// Method to sum all alteration contributions for the specified attribute and return the
        /// result
        /// </summary>
        public double GetAttribute(CharacterAttribute attribute)
        {
            return _effectCollectors.Sum(collector => collector.GetAttributeAggregate(attribute));
        }

        /// <summary>
        /// Returns accumulated character state enums from all active (non-attack attribute) alterations.
        /// </summary>
        public IEnumerable<AlteredCharacterState> GetStates()
        {
            // TODO:ALTERATION Need a flag to designate altered state - OR validation to 
            //                 prevent setting altered state enum. (favoring validation)
            return _effectCollectors.SelectMany(collector => collector.GetAlteredStates());
        }

        /// <summary>
        /// Returns all symbol changes as an enumerable collection
        /// </summary>
        public IEnumerable<SymbolDeltaTemplate> GetSymbolChanges()
        {
            return _effectCollectors.SelectMany(collector => collector.GetSymbolChanges());
        }

        /// <summary>
        /// Returns an aggregate of attack attributes for the specified application and combat types
        /// </summary>
        /// <param name="applicationType">Melee, Temporary, Passive, Aura</param>
        /// <param name="combatType"></param>
        /// <returns></returns>
        public IEnumerable<AttackAttribute> GetAttackAttributes(AlterationAttackAttributeCombatType combatType)
        {
            // TODO:ALTERATION - Use combat type filter

            // Aggregates attack attributes into a list
            var aggregateFunc = new Action<List<AttackAttribute>, AttackAttribute>((aggregator, attackAttribute) =>
            {
                var existingAttribute = aggregator.FirstOrDefault(x => x.RogueName == attackAttribute.RogueName);

                if (existingAttribute == null)
                    aggregator.Add(attackAttribute);

                else
                {
                    existingAttribute.Attack += attackAttribute.Attack;
                    existingAttribute.Resistance += attackAttribute.Resistance;
                    existingAttribute.Weakness += attackAttribute.Weakness;
                }
            });

            var result = _attackAttributeCollectors.Aggregate(new List<AttackAttribute>(), (aggregator, collector) =>
            {
                // Aggregate the collector contributions
                foreach (var attackAttribute in collector.GetAttackAttributes(combatType))                    
                    aggregateFunc(aggregator, attackAttribute);

                return aggregator;
            });

            return result;
        }

        /// <summary>
        /// TODO: REMOVE THIS METHOD. The point of this is to get the first malign attack attribute
        ///       alteration that killed the player. I'd prefer not to have this method here because
        ///       it's too specific for the CharacterAlteration.. maybe an extension method in the
        ///       future.
        /// </summary>
        public string GetKilledBy()
        {
            return _attackAttributeCollectors.SelectMany(x => x.GetEffectNames(AlterationAttackAttributeCombatType.MalignPerStep))
                                             .FirstOrDefault();
        }

        /// <summary>
        /// Method to look through alterations to see if character can see invisible characters
        /// </summary>
        public bool CanSeeInvisible()
        {
            return this.PassiveCollector.CanSeeInvisible() ||
                   this.TemporaryCollector.CanSeeInvisible();
        }

        /// <summary>
        /// Returns a collection of all per-step alteration costs for active alterations (by Alteration name)
        /// </summary>
        public IEnumerable<KeyValuePair<string, AlterationCost>> GetAlterationCosts()
        {
            return _collectors.SelectMany(x => x.GetCosts());
        }

        /// <summary>
        /// Returns a collection of all Alteration effects THAT AFFECT THE CHARACTER by Alteration name. You can include
        /// the source effects that apply to target characters by passing in the includeSourceEffects flag.
        /// </summary>
        public IEnumerable<KeyValuePair<string, IAlterationEffect>> GetAlterationEffects(bool includeSourceEffects = false)
        {
            return _effectCollectors.SelectMany(x => x.GetEffects(includeSourceEffects));
        }

        /// <summary>
        /// Returns aura data for auras that this character is the SOURCE of.
        /// </summary>
        public IEnumerable<Tuple<IAlterationEffect, AuraSourceParameters>> GetAuras()
        {
            return this.AuraSourceCollector
                       .GetAuraEffects()
                       .Union(this.AttackAttributeAuraSourceCollector
                                  .GetAuraEffects())
                       .Actualize();

        }

        /// <summary>
        /// Returns tuple of [AlterationEffectId, AuraSourceParameters] for all aura sources to 
        /// provide information for the UI.
        /// </summary>
        public IEnumerable<Tuple<string, AuraSourceParameters>> GetAuraSourceParameters()
        {
            return this.AuraSourceCollector
                       .GetAuraEffects()
                       .Select(x => new Tuple<string, AuraSourceParameters>(x.Item1.Id, x.Item2))
                       .Union(this.AttackAttributeAuraSourceCollector
                                  .GetAuraEffects()
                                  .Select(x => new Tuple<string, AuraSourceParameters>(x.Item1.Id, x.Item2)))
                       .Actualize();
        }
        #endregion

        #region (public) Apply Methods
        /// <summary>
        /// Applies alteration based on type inspection
        /// </summary>
        public void Apply(Scenario.Alteration.Common.AlterationContainer alteration)
        {
            if (alteration is ConsumableAlteration)
                ApplyAlteration(alteration as ConsumableAlteration);

            else if (alteration is ConsumableProjectileAlteration)
                ApplyAlteration(alteration as ConsumableProjectileAlteration);

            else if (alteration is DoodadAlteration)
                ApplyAlteration(alteration as DoodadAlteration);

            else if (alteration is EnemyAlteration)
                ApplyAlteration(alteration as EnemyAlteration);

            else if (alteration is EquipmentAttackAlteration)
                Apply(alteration as EquipmentAttackAlteration);

            else if (alteration is EquipmentCurseAlteration)
                ApplyAlteration(alteration as EquipmentCurseAlteration);

            else if (alteration is EquipmentEquipAlteration)
                ApplyAlteration(alteration as EquipmentEquipAlteration);

            else if (alteration is SkillAlteration)
                ApplyAlteration(alteration as SkillAlteration);

            else
                throw new Exception("Unhandled Alteration type");
        }

        public IEnumerable<Scenario.Alteration.Common.AlterationContainer> ApplyRemedy(RemedyAlterationEffect remedyEffect) 
        {
            return this.AttackAttributeTemporaryCollector
                       .ApplyRemedy(remedyEffect)
                       .Union(this.TemporaryCollector.ApplyRemedy(remedyEffect))
                       .Actualize();
        }

        /// <summary>
        /// Sets all Aura Effects that act ON THE CHARACTER
        /// </summary>
        public void ApplyTargetAuraEffects(IEnumerable<AttackAttributeAuraAlterationEffect> alterationEffects)
        {
            this.AttackAttributeAuraTargetCollector.Apply(alterationEffects);
        }

        /// <summary>
        /// Sets all Aura Effects that act ON THE CHARACTER
        /// </summary>
        public void ApplyTargetAuraEffects(IEnumerable<AuraAlterationEffect> alterationEffects)
        {
            this.AuraTargetCollector.Apply(alterationEffects);
        }

        /// <summary>
        /// Removes appropriate (passive or aura type) alterations. Essentially, anything that should
        /// be removed or managed by the calling code.
        /// </summary>
        /// <param name="alterationName">This RogueBase.RogueName property from AlterationContainer</param>
        public void Remove(string alterationName)
        {
            // Filter out Attack Attribute Aura Alterations
            this.AttackAttributeAuraSourceCollector.Filter(alterationName);

            // Filter out Aura Alterations
            this.AuraSourceCollector.Filter(alterationName);

            // Filter out Attack Attribute Passive Alterations
            this.AttackAttributePassiveCollector.Filter(alterationName);

            // Filter out Passive Alterations
            this.PassiveCollector.Filter(alterationName);
        }

        public IEnumerable<string> DecrementEventTimes()
        {
            return _temporaryCollectors.SelectMany(x => x.Decrement());
        }
        #endregion

        #region (private) Apply Methods
        /// <summary>
        /// Applies ConsumableAlteration type. Returns false if application fails because it's not stackable.
        /// </summary>
        private void ApplyAlteration(ConsumableAlteration alteration)
        {
            _characterAlterationTypeValidator.Validate(alteration.Effect);

            if (alteration.Effect is AttackAttributeTemporaryAlterationEffect)
                this.AttackAttributeTemporaryCollector.Apply(alteration);

            else if (alteration.Effect is RemedyAlterationEffect)
            {
                this.AttackAttributeTemporaryCollector.ApplyRemedy(alteration.Effect as RemedyAlterationEffect);
                this.TemporaryCollector.ApplyRemedy(alteration.Effect as RemedyAlterationEffect);
            }

            else if (alteration.Effect.GetType() == typeof(TemporaryAlterationEffect))
                this.TemporaryCollector.Apply(alteration);

            else
                throw new Exception("Unhandled IConsumableAlterationEffect implementation type");
        }

        /// <summary>
        /// Applies ConsumableProjectileAlteration type. Returns false if application fails because it's not stackable.
        /// </summary>
        private void ApplyAlteration(ConsumableProjectileAlteration alteration)
        {
            _characterAlterationTypeValidator.Validate(alteration.Effect);

            if (alteration.Effect.GetType() == typeof(AttackAttributeTemporaryAlterationEffect))
                this.AttackAttributeTemporaryCollector.Apply(alteration);

            else if (alteration.Effect.GetType() == typeof(TemporaryAlterationEffect))
                this.TemporaryCollector.Apply(alteration);

            else
                throw new Exception("Unhandled IConsumableProjectileAlterationEffect implementation type");
        }

        /// <summary>
        /// Applies DoodadAlteration type. Returns false if application fails because it's not stackable.
        /// </summary>
        private void ApplyAlteration(DoodadAlteration alteration)
        {
            _characterAlterationTypeValidator.Validate(alteration.Effect);

            if (alteration.Effect.GetType() == typeof(AttackAttributeTemporaryAlterationEffect))
                this.AttackAttributeTemporaryCollector.Apply(alteration);

            else if (alteration.Effect.GetType() == typeof(RemedyAlterationEffect))
            {
                this.AttackAttributeTemporaryCollector.ApplyRemedy(alteration.Effect as RemedyAlterationEffect);
                this.TemporaryCollector.ApplyRemedy(alteration.Effect as RemedyAlterationEffect);
            }

            else if (alteration.Effect.GetType() == typeof(TemporaryAlterationEffect))
                this.TemporaryCollector.Apply(alteration);

            else
                throw new Exception("Unhandled IDoodadAlterationEffect implementation type");
        }

        /// <summary>
        /// Applies EnemyAlteration type. Returns false if application fails because it's not stackable.
        /// </summary>
        private void ApplyAlteration(EnemyAlteration alteration)
        {
            _characterAlterationTypeValidator.Validate(alteration.Effect);

            if (alteration.Effect.GetType() == typeof(AttackAttributeTemporaryAlterationEffect))
                this.AttackAttributeTemporaryCollector.Apply(alteration);

            else if (alteration.Effect.GetType() == typeof(TemporaryAlterationEffect))
                this.TemporaryCollector.Apply(alteration);

            else
                throw new Exception("Unhandled IEnemyAlterationEffect implementation type");
        }

        /// <summary>
        /// Applies EquipmentCurseAlteration type
        /// </summary>
        private void ApplyAlteration(EquipmentCurseAlteration alteration)
        {
            _characterAlterationTypeValidator.Validate(alteration.Effect);

            if (alteration.Effect.GetType() == typeof(AttackAttributeAuraAlterationEffect))
                this.AttackAttributeAuraSourceCollector.Apply(alteration);

            else if (alteration.Effect.GetType() == typeof(AttackAttributePassiveAlterationEffect))
                this.AttackAttributePassiveCollector.Apply(alteration);

            else if (alteration.Effect.GetType() == typeof(AuraAlterationEffect))
                this.AuraSourceCollector.Apply(alteration);

            else if (alteration.Effect.GetType() == typeof(PassiveAlterationEffect))
                this.PassiveCollector.Apply(alteration);

            else
                throw new Exception("Unhandled IEquipmentCurseAlterationEffect implementation type");
        }

        /// <summary>
        /// Applies EquipmentEquipAlteration type
        /// </summary>
        private void ApplyAlteration(EquipmentEquipAlteration alteration)
        {
            _characterAlterationTypeValidator.Validate(alteration.Effect);

            if (alteration.Effect.GetType() == typeof(AttackAttributeAuraAlterationEffect))
                this.AttackAttributeAuraSourceCollector.Apply(alteration);

            else if (alteration.Effect.GetType() == typeof(AttackAttributePassiveAlterationEffect))
                this.AttackAttributePassiveCollector.Apply(alteration);

            else if (alteration.Effect.GetType() == typeof(AuraAlterationEffect))
                this.AuraSourceCollector.Apply(alteration);

            else if (alteration.Effect.GetType() == typeof(PassiveAlterationEffect))
                this.PassiveCollector.Apply(alteration);

            else
                throw new Exception("Unhandled IEquipmentEquipAlterationEffect implementation type");
        }

        /// <summary>
        /// Applies SkillAlteration type
        /// </summary>
        private void ApplyAlteration(SkillAlteration alteration)
        {
            _characterAlterationTypeValidator.Validate(alteration.Effect);

            if (alteration.Effect.GetType() == typeof(AttackAttributeAuraAlterationEffect))
                this.AttackAttributeAuraSourceCollector.Apply(alteration);

            else if (alteration.Effect.GetType() == typeof(AttackAttributePassiveAlterationEffect))
                this.AttackAttributePassiveCollector.Apply(alteration);

            else if (alteration.Effect.GetType() == typeof(AttackAttributeTemporaryAlterationEffect))
                this.AttackAttributeTemporaryCollector.Apply(alteration);

            else if (alteration.Effect.GetType() == typeof(AuraAlterationEffect))
                this.AuraSourceCollector.Apply(alteration);

            else if (alteration.Effect.GetType() == typeof(PassiveAlterationEffect))
                this.PassiveCollector.Apply(alteration);

            else if (alteration.Effect.GetType() == typeof(RemedyAlterationEffect))
            {
                this.AttackAttributeTemporaryCollector.ApplyRemedy(alteration.Effect as RemedyAlterationEffect);
                this.TemporaryCollector.ApplyRemedy(alteration.Effect as RemedyAlterationEffect);
            }

            else if (alteration.Effect.GetType() == typeof(TemporaryAlterationEffect))
                this.TemporaryCollector.Apply(alteration);

            else
                throw new Exception("Unhandled IEquipmentEquipAlterationEffect implementation type");
        }
        #endregion
    }
}
