using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
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
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration
{
    /// <summary>
    /// Complete list of active character alterations - costs and effects. This is serialized along with the model
    /// to maintain dynamic (game-time) state.
    /// </summary>
    [Serializable]
    public class CharacterAlteration_NEW
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

        private readonly IAlterationCollector[] _collectors;
        private readonly IAttackAttributeAlterationCollector[] _attackAttributeCollectors;
        private readonly ITurnBasedAlterationCollector[] _turnBasedCollectors;

        public CharacterAlteration_NEW()
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
                this.PassiveCollector,
                this.TemporaryCollector,
                this.AttackAttributePassiveCollector,
                this.AttackAttributeTemporaryCollector
            };

            _attackAttributeCollectors = new IAttackAttributeAlterationCollector[]
            {
                this.AttackAttributeAuraTargetCollector,
                this.AttackAttributePassiveCollector,
                this.AttackAttributeTemporaryCollector
            };

            _turnBasedCollectors = new ITurnBasedAlterationCollector[]
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
            return _collectors.Sum(collector => collector.GetAttributeAggregate(attribute));
        }

        /// <summary>
        /// Returns accumulated character state enums from all active (non-attack attribute) alterations.
        /// </summary>
        public IEnumerable<AlteredCharacterState> GetStates()
        {
            // TODO:ALTERATION Need a flag to designate altered state - OR validation to 
            //                 prevent setting altered state enum. (favoring validation)
            return _collectors.SelectMany(collector => collector.GetAlteredStates());
        }

        /// <summary>
        /// Returns all symbol changes as an enumerable collection
        /// </summary>
        public IEnumerable<SymbolDeltaTemplate> GetSymbolChanges()
        {
            return _collectors.SelectMany(collector => collector.GetSymbolChanges());
        }

        /// <summary>
        /// Returns an aggregate of attack attributes for the specified application and combat types
        /// </summary>
        /// <param name="applicationType">Melee, Temporary, Passive, Aura</param>
        /// <param name="combatType"></param>
        /// <returns></returns>
        public IEnumerable<AttackAttribute> GetAttackAttributes(
                AlterationAttackAttributeCombatType combatType)
        {
            return _attackAttributeCollectors.Aggregate(new List<AttackAttribute>(), (aggregator, collector) =>
            {
                foreach (var attackAttribute in collector.GetAttackAttributes(combatType))
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
                }

                return aggregator;
            });
        }

        /// <summary>
        /// Method to look through alterations to see if character can see invisible characters
        /// </summary>
        public bool CanSeeInvisible()
        {
            return this.PassiveCollector.CanSeeInvisible() ||
                   this.TemporaryCollector.CanSeeInvisible();
        }

        // TODO:ALTERATION
        ///// <summary>
        /////  Returns total list of active alterations (NOT LIKED; BUT USED FOR UI UPDATING)
        ///// </summary>
        ///// <returns></returns>
        //public virtual IEnumerable<Tuple<AlterationType, AlterationAttackAttributeType, AlterationCost, AlterationEffect>> Get()
        //{
        //    return new List<Tuple<AlterationType, AlterationAttackAttributeType, AlterationCost, AlterationEffect>>();
        //}

        /// <summary>
        /// Returns a collection of all per-step alteration costs for active alterations
        /// </summary>
        public IEnumerable<AlterationCost> GetAlterationCosts()
        {
            return _collectors.SelectMany(x => x.GetCosts())
                              .Union(this.AuraSourceCollector.GetCosts())
                              .Union(this.AttackAttributeAuraSourceCollector.GetCosts())
                              .Actualize();
        }
        #endregion

        #region (public) Apply Methods
        /// <summary>
        /// Applies alteration based on type inspection
        /// </summary>
        public void Apply(AlterationBase alteration)
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
        /// <param name="alterationId">This RogueBase.Id property from AlterationBase</param>
        public void Remove(string alterationId)
        {
            // Filter out Attack Attribute Aura Alterations
            this.AttackAttributeAuraSourceCollector.Filter(alterationId);

            // Filter out Aura Alterations
            this.AuraSourceCollector.Filter(alterationId);

            // Filter out Attack Attribute Passive Alterations
            this.AttackAttributePassiveCollector.Filter(alterationId);

            // Filter out Passive Alterations
            this.PassiveCollector.Filter(alterationId);
        }

        public void DecrementEventTimes()
        {
            _turnBasedCollectors.ForEach(collector => collector.ApplyEndOfTurn());
        }
        #endregion

        #region (private) Apply Methods
        /// <summary>
        /// Applies ConsumableAlteration type. Returns false if application fails because it's not stackable.
        /// </summary>
        private void ApplyAlteration(ConsumableAlteration alteration)
        {
            _characterAlterationTypeValidator.Validate(alteration.Effect);

            if (alteration.Effect.GetType() == typeof(RemedyAlterationEffect))
            {
                var effect = alteration.Effect as RemedyAlterationEffect;

                this.AttackAttributeTemporaryCollector.ApplyRemedy(effect);
                this.TemporaryCollector.ApplyRemedy(effect);
            }

            else if (alteration.Effect.GetType() == typeof(TemporaryAlterationEffect))
                this.TemporaryCollector.Apply(alteration.Id, alteration.Effect as TemporaryAlterationEffect);

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
                this.AttackAttributeTemporaryCollector.Apply(alteration.Id, alteration.Effect as AttackAttributeTemporaryAlterationEffect);

            else if (alteration.Effect.GetType() == typeof(TemporaryAlterationEffect))
                this.TemporaryCollector.Apply(alteration.Id, alteration.Effect as TemporaryAlterationEffect);

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
                this.AttackAttributeTemporaryCollector.Apply(alteration.Id, alteration.Effect as AttackAttributeTemporaryAlterationEffect);

            else if (alteration.Effect.GetType() == typeof(RemedyAlterationEffect))
            {
                var effect = alteration.Effect as RemedyAlterationEffect;

                this.AttackAttributeTemporaryCollector.ApplyRemedy(effect);
                this.TemporaryCollector.ApplyRemedy(effect);
            }

            else if (alteration.Effect.GetType() == typeof(TemporaryAlterationEffect))
                this.TemporaryCollector.Apply(alteration.Id, alteration.Effect as TemporaryAlterationEffect);

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
                this.AttackAttributeTemporaryCollector.Apply(alteration.Id, alteration.Effect as AttackAttributeTemporaryAlterationEffect);

            else if (alteration.Effect.GetType() == typeof(TemporaryAlterationEffect))
                this.TemporaryCollector.Apply(alteration.Id, alteration.Effect as TemporaryAlterationEffect);

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
                this.AttackAttributeAuraSourceCollector.Apply(alteration.Id, alteration.Effect as AttackAttributeAuraAlterationEffect, alteration.AuraParameters);

            else if (alteration.Effect.GetType() == typeof(AttackAttributePassiveAlterationEffect))
                this.AttackAttributePassiveCollector.Apply(alteration.Id, alteration.Effect as AttackAttributePassiveAlterationEffect);

            else if (alteration.Effect.GetType() == typeof(AuraAlterationEffect))
                this.AuraSourceCollector.Apply(alteration.Id, alteration.Effect as AuraAlterationEffect, alteration.AuraParameters);

            else if (alteration.Effect.GetType() == typeof(PassiveAlterationEffect))
                this.PassiveCollector.Apply(alteration.Id, alteration.Effect as PassiveAlterationEffect);

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
                this.AttackAttributeAuraSourceCollector.Apply(alteration.Id, alteration.Effect as AttackAttributeAuraAlterationEffect, alteration.AuraParameters);

            else if (alteration.Effect.GetType() == typeof(AttackAttributePassiveAlterationEffect))
                this.AttackAttributePassiveCollector.Apply(alteration.Id, alteration.Effect as AttackAttributePassiveAlterationEffect);

            else if (alteration.Effect.GetType() == typeof(AuraAlterationEffect))
                this.AuraSourceCollector.Apply(alteration.Id, alteration.Effect as AuraAlterationEffect, alteration.AuraParameters);

            else if (alteration.Effect.GetType() == typeof(PassiveAlterationEffect))
                this.PassiveCollector.Apply(alteration.Id, alteration.Effect as PassiveAlterationEffect);

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
                this.AttackAttributeAuraSourceCollector.Apply(alteration.Id, alteration.Effect as AttackAttributeAuraAlterationEffect, alteration.AuraParameters);

            else if (alteration.Effect.GetType() == typeof(AttackAttributePassiveAlterationEffect))
                this.AttackAttributePassiveCollector.Apply(alteration.Id, alteration.Effect as AttackAttributePassiveAlterationEffect);

            else if (alteration.Effect.GetType() == typeof(AttackAttributeTemporaryAlterationEffect))
                this.AttackAttributeTemporaryCollector.Apply(alteration.Id, alteration.Effect as AttackAttributeTemporaryAlterationEffect);

            else if (alteration.Effect.GetType() == typeof(AuraAlterationEffect))
                this.AuraSourceCollector.Apply(alteration.Id, alteration.Effect as AuraAlterationEffect, alteration.AuraParameters);

            else if (alteration.Effect.GetType() == typeof(PassiveAlterationEffect))
                this.PassiveCollector.Apply(alteration.Id, alteration.Effect as PassiveAlterationEffect);

            else if (alteration.Effect.GetType() == typeof(RemedyAlterationEffect))
            {
                var effect = alteration.Effect as RemedyAlterationEffect;

                this.AttackAttributeTemporaryCollector.ApplyRemedy(effect);
                this.TemporaryCollector.ApplyRemedy(effect);
            }

            else if (alteration.Effect.GetType() == typeof(TemporaryAlterationEffect))
                this.TemporaryCollector.Apply(alteration.Id, alteration.Effect as TemporaryAlterationEffect);

            else
                throw new Exception("Unhandled IEquipmentEquipAlterationEffect implementation type");
        }
        #endregion
    }
}
