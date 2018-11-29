using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Dynamic
{
    /// <summary>
    /// Complete list of active character alterations - costs and effects. This is serialized along with the model
    /// to maintain dynamic (game-time) state.
    /// </summary>
    [Serializable]
    public class CharacterAlteration //: ISerializable
    {
        #region (protected) Nested Dictionary Key Sub-Class
        [Serializable]
        protected class SpellReference
        {
            public string SpellId { get; set; }
            public string SpellRogueName { get; set; }

            public SpellReference() { }
            public SpellReference(AlterationContainer container)
            {
                this.SpellId = container.GeneratingSpellId;
                this.SpellRogueName = container.GeneratingSpellName;
            }

            public override int GetHashCode()
            {
                return this.SpellId.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                if (obj is SpellReference)
                {
                    return (obj as SpellReference).SpellId == this.SpellId;
                }
                return false;
            }
        }
        #endregion

        #region (Passives / Per Step Costs) Managed via AlterationContainer.Id
        /// <summary>
        /// Keeps a list of alteration costs for the character - applied on turn. The key is the
        /// Spell.Id that is it's parent
        /// </summary>
        protected IDictionary<SpellReference, AlterationCost> PerStepAlterationCosts { get; set; }

        /// <summary>
        /// Active passive effects on the character. The key is the Id for the Spell.Id
        /// </summary>
        protected IDictionary<SpellReference, AlterationEffect> ActivePassiveEffects { get; set; }

        /// <summary>
        /// List that holds set of passive attack attribute alterations - THESE ARE APPLIED WITH 
        /// AN EQUIPMENT ITEM. The key is the Id for the Spell
        /// </summary>
        protected IDictionary<SpellReference, AlterationEffect> AttackAttributePassiveEffects { get; set; }
        #endregion

        #region Aura Effects - Managed by Container! (SET ON ENEMY END OF TURN)
        /// <summary>
        /// List of all active aura effects that act ON THE CHARACTER (THIS MUST BE CALCUATED
        /// BY A CONTAINER). THIS SHOULD NOT AFFECT PLAYER BECAUSE AURAS AREN'T SUPPORTED FOR ENEMIES.
        /// </summary>
        protected IList<AlterationEffect> ActiveAuraEffects { get; set; }
        #endregion

        #region Temporary Effects - Managed via Event Time; but contain keys to Spell.Id
        /// <summary>
        /// Active temporary effects on the character
        /// </summary>
        protected IDictionary<SpellReference, AlterationEffect> ActiveTemporaryEffects { get; set; }

        /// <summary>
        /// List of temporary friendly effects using attack attributes only (Means that defense
        /// is calculated per turn of melee for this character)
        /// </summary>
        protected IDictionary<SpellReference, AlterationEffect> AttackAttributeTemporaryFriendlyEffects { get; set; }

        /// <summary>
        /// List of temporary malign effects using attack attributes only (Means that an attack is
        /// calculated per step on the character for the specified attack attribute(s))
        /// </summary>
        protected IDictionary<SpellReference, AlterationEffect> AttackAttributeTemporaryMalignEffects { get; set; }
        #endregion

        public CharacterAlteration()
        {
            this.ActivePassiveEffects = new Dictionary<SpellReference, AlterationEffect>();
            this.AttackAttributePassiveEffects = new Dictionary<SpellReference, AlterationEffect>();
            this.PerStepAlterationCosts = new Dictionary<SpellReference, AlterationCost>();

            this.ActiveAuraEffects = new List<AlterationEffect>();

            this.ActiveTemporaryEffects = new Dictionary<SpellReference, AlterationEffect>();
            this.AttackAttributeTemporaryFriendlyEffects = new Dictionary<SpellReference, AlterationEffect>();
            this.AttackAttributeTemporaryMalignEffects = new Dictionary<SpellReference, AlterationEffect>();
        }

        #region (public) Methods
        /// <summary>
        /// Method to get all magic (non-attack attribute) effects acting on the character
        /// </summary>
        public IEnumerable<AlterationEffect> GetAlterations()
        {
            return this.ActiveTemporaryEffects.Values
                       .Union(this.ActivePassiveEffects.Values)
                       .Union(this.ActiveAuraEffects);
        }
        /// <summary>
        /// Returns all AlterationEffects that have a potential to modify the character symbol
        /// </summary>
        public IEnumerable<AlterationEffect> GetSymbolAlteringEffects()
        {
            return this.ActiveTemporaryEffects.Values
                       .Union(this.ActivePassiveEffects.Values)
                       .Union(this.ActiveAuraEffects)
                       .Union(this.AttackAttributePassiveEffects.Values)
                       .Union(this.AttackAttributeTemporaryFriendlyEffects.Values)
                       .Union(this.AttackAttributeTemporaryMalignEffects.Values)
                       .Where(x => x.IsSymbolAlteration);
        }
        public IEnumerable<AlterationEffect> GetTemporaryAttackAttributeAlterations(bool friendly)
        {
            return friendly ? this.AttackAttributeTemporaryFriendlyEffects.Values : this.AttackAttributeTemporaryMalignEffects.Values;
        }
        public IEnumerable<AlterationEffect> GetPassiveAttackAttributeAlterations()
        {
            return this.AttackAttributePassiveEffects.Values;
        }

        /// <summary>
        /// Gets list of spell Id's for applied effects that COULD BE factored into IsStackable calculation. (These 
        /// represent spells that have been applied to create effects. These are used as keys for the containing dictionaries)
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetStackableSpellIds()
        {
            // These collections should be taken into account when calculating IsStackable
            return this.ActivePassiveEffects
                  .Union(this.AttackAttributePassiveEffects)
                  .Union(this.ActiveTemporaryEffects)
                  .Union(this.AttackAttributeTemporaryFriendlyEffects)
                  .Union(this.AttackAttributeTemporaryMalignEffects)
                  .Select(x => x.Key.SpellId)
                  .ToList();
        }

        /// <summary>
        /// Sets all Aura Effects that act ON THE CHARACTER (ENEMY ONLY!!!)
        /// </summary>
        /// <param name="auraEffects">Effectcs that are generated by the Player</param>
        public void SetAuraEffects(IEnumerable<AlterationEffect> auraEffects)
        {
            this.ActiveAuraEffects.Clear();

            foreach (var effect in auraEffects)
                this.ActiveAuraEffects.Add(effect);
        }

        /// <summary>
        /// Returns accumulated character state enums from all active (non-attack attribute) alterations.
        /// </summary>
        public IEnumerable<CharacterStateType> GetStates()
        {
            return GetAlterations().Select(x => x.State);
        }

        public IEnumerable<AlterationCost> GetAlterationCosts()
        {
            return this.PerStepAlterationCosts.Values;
        }

        /// <summary>
        /// Returns true if alteration applied. False if it is not applied because it's not stackable.
        /// </summary>
        public bool ActiveAlteration(AlterationContainer alteration, bool isSource)
        {
            // CALCULATION FOR IS-STACKABLE
            if (!alteration.IsStackable && this.GetStackableSpellIds().Any(x => x == alteration.GeneratingSpellId))
                return false;

            if (isSource)
                ApplySourceAlteration(alteration);

            else
                ApplyTargetAlteration(alteration);

            return true;
        }

        public virtual void DeactivatePassiveAlteration(string spellId)
        {
            // Per Step Costs
            this.PerStepAlterationCosts.Filter(x => x.Key.SpellId == spellId);

            // Passive Effets
            this.ActivePassiveEffects.Filter(x => x.Key.SpellId == spellId);

            // Passive Attack Attribute Effeccts
            this.AttackAttributePassiveEffects.Filter(x => x.Key.SpellId == spellId);
        }

        public IEnumerable<AlterationEffect> DecrementEventTimes()
        {
            var updateFunction = new Func<IDictionary<SpellReference, AlterationEffect>, IEnumerable<AlterationEffect>>(dictionary =>
            {
                var finishedEffects = new List<AlterationEffect>();
                for (int i = dictionary.Count - 1; i >= 0; i--)
                {
                    var element = dictionary.ElementAt(i);
                    var effect = element.Value;

                    //Check temporary event time
                    effect.EventTime--;
                    if (effect.EventTime <= 0)
                    {
                        // Store the result
                        finishedEffects.Add(effect);

                        // Remove the effect
                        dictionary.Remove(element.Key);
                    }
                }
                return finishedEffects;
            });

            var result = new List<AlterationEffect>();
            result.AddRange(updateFunction(this.ActiveTemporaryEffects));
            result.AddRange(updateFunction(this.AttackAttributeTemporaryFriendlyEffects));
            result.AddRange(updateFunction(this.AttackAttributeTemporaryMalignEffects));

            return result;
        }

        public IEnumerable<AlterationEffect> ApplyRemedy(string remediedSpellName)
        {
            // Gather all effects that are remedied
            var result =
                this.ActiveTemporaryEffects.Where(x => x.Key.SpellRogueName == remediedSpellName)
                    .Union(this.AttackAttributeTemporaryFriendlyEffects.Where(x => x.Key.SpellRogueName == remediedSpellName))
                    .Union(this.AttackAttributeTemporaryMalignEffects.Where(x => x.Key.SpellRogueName == remediedSpellName))
                    .Select(x => x.Value)
                    .ToList();

            // Remove these results from the lists
            this.ActiveTemporaryEffects.Filter(x => result.Contains(x.Value));
            this.AttackAttributeTemporaryFriendlyEffects.Filter(x => result.Contains(x.Value));
            this.AttackAttributeTemporaryMalignEffects.Filter(x => result.Contains(x.Value));

            // Return the remedied alteration effects
            return result;
        }
        #endregion

        #region (protected) Methods
        /// <summary>
        /// Applies source effect(s) from the alteration
        /// </summary>
        protected virtual void ApplySourceAlteration(AlterationContainer alteration)
        {
            // Apply Cost (MUST APPLY ONE-TIME EXTERNALLY)
            if (alteration.Cost.Type == AlterationCostType.PerStep)
                this.PerStepAlterationCosts.Add(new SpellReference(alteration), alteration.Cost);

            // Apply Effect
            switch (alteration.Type)
            {
                case AlterationType.PassiveSource:
                    this.ActivePassiveEffects.Add(new SpellReference(alteration), alteration.Effect);
                    break;
                case AlterationType.PassiveAura:
                    // PlayerAlteration will handle (CONSIDER RE-WRITE)
                    // throw new Exception("Passive Aura NOT SUPPORTED FOR ENEMIES");
                    break;
                case AlterationType.TemporarySource:
                    this.ActiveTemporaryEffects.Add(new SpellReference(alteration), alteration.Effect);
                    break;
                // These types are handled for the target CharacterAlteration
                case AlterationType.TemporaryTarget:
                case AlterationType.TemporaryAllTargets:
                    break;
                // These types aren't supported here - but should be applied externally
                case AlterationType.Remedy:
                case AlterationType.PermanentSource:
                case AlterationType.PermanentTarget:
                case AlterationType.PermanentAllTargets:
                case AlterationType.Steal:
                case AlterationType.RunAway:
                case AlterationType.TeleportSelf:
                case AlterationType.TeleportTarget:
                case AlterationType.TeleportAllTargets:
                case AlterationType.OtherMagicEffect:
                    throw new Exception("Alteration Type not supported here");
                case AlterationType.AttackAttribute:
                    {
                        switch (alteration.AttackAttributeType)
                        {
                            // These types are handled by for the target (not here)
                            case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                            case AlterationAttackAttributeType.TemporaryMalignTarget:
                                break;
                            case AlterationAttackAttributeType.MeleeTarget:
                            case AlterationAttackAttributeType.ImbueArmor:
                            case AlterationAttackAttributeType.ImbueWeapon:
                                throw new Exception("Attack Attribute Alteration Type not supported here");
                            case AlterationAttackAttributeType.Passive:
                                this.AttackAttributePassiveEffects.Add(new SpellReference(alteration), alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.TemporaryFriendlySource:
                                this.AttackAttributeTemporaryFriendlyEffects.Add(new SpellReference(alteration), alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.TemporaryMalignSource:
                                this.AttackAttributeTemporaryMalignEffects.Add(new SpellReference(alteration), alteration.Effect);
                                break;
                            default:
                                throw new Exception("Unhandled Attack Attribute Alteration Type");
                        }
                    }
                    break;
                default:
                    throw new Exception("Unhandled Alteration Type");
            }
        }

        /// <summary>
        /// Applies target effect(s) from the alteration
        /// </summary>
        /// <param name="alteration"></param>
        protected virtual void ApplyTargetAlteration(AlterationContainer alteration)
        {
            switch (alteration.Type)
            {
                // This type is handled for the source CharacterAlteration
                case AlterationType.PassiveSource:
                    break;
                case AlterationType.PassiveAura:
                    throw new Exception("Passive Aura NOT SUPPORTED FOR ENEMIES");
                // This type is handled for the source CharacterAlteration
                case AlterationType.TemporarySource:
                    break;
                case AlterationType.TemporaryTarget:
                case AlterationType.TemporaryAllTargets:
                    this.ActiveTemporaryEffects.Add(new SpellReference(alteration), alteration.Effect);
                    break;
                // These types aren't supported here - but should be applied externally
                case AlterationType.Remedy:
                case AlterationType.PermanentSource:
                case AlterationType.PermanentTarget:
                case AlterationType.PermanentAllTargets:
                case AlterationType.Steal:
                case AlterationType.RunAway:
                case AlterationType.TeleportSelf:
                case AlterationType.TeleportTarget:
                case AlterationType.TeleportAllTargets:
                case AlterationType.OtherMagicEffect:
                    throw new Exception("Alteration Type not supported here");
                case AlterationType.AttackAttribute:
                    {
                        switch (alteration.AttackAttributeType)
                        {
                            case AlterationAttackAttributeType.TemporaryFriendlyTarget:
                                this.AttackAttributeTemporaryFriendlyEffects.Add(new SpellReference(alteration), alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.TemporaryMalignTarget:
                                this.AttackAttributeTemporaryMalignEffects.Add(new SpellReference(alteration), alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.MeleeTarget:
                            case AlterationAttackAttributeType.ImbueArmor:
                            case AlterationAttackAttributeType.ImbueWeapon:
                            case AlterationAttackAttributeType.Passive:
                                throw new Exception("Attack Attribute Alteration Type not supported here");

                            // These types are handled by for the target (not here)
                            case AlterationAttackAttributeType.TemporaryFriendlySource:
                            case AlterationAttackAttributeType.TemporaryMalignSource:
                                break;
                            default:
                                throw new Exception("Unhandled Attack Attribute Alteration Type");
                        }
                    }
                    break;
                default:
                    throw new Exception("Unhandled Alteration Type");
            }
        }
        #endregion
    }
}
