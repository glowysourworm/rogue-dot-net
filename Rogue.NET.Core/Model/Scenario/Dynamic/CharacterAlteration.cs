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
        #region (Passives / Per Step Costs) Managed via AlterationContainer.Id
        /// <summary>
        /// Keeps a list of alteration costs for the character - applied on turn. The key is the
        /// Spell.Id that is it's parent
        /// </summary>
        protected IDictionary<string, AlterationCost> PerStepAlterationCosts { get; set; }

        /// <summary>
        /// Active passive effects on the character. The key is the Id for the Spell.Id
        /// </summary>
        protected IDictionary<string, AlterationEffect> ActivePassiveEffects { get; set; }

        /// <summary>
        /// List that holds set of passive attack attribute alterations - THESE ARE APPLIED WITH 
        /// AN EQUIPMENT ITEM. The key is the Id for the Spell
        /// </summary>
        protected IDictionary<string, AlterationEffect> AttackAttributePassiveEffects { get; set; }
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
        protected IDictionary<string, AlterationEffect> ActiveTemporaryEffects { get; set; }

        /// <summary>
        /// List of temporary friendly effects using attack attributes only (Means that defense
        /// is calculated per turn of melee for this character)
        /// </summary>
        protected IDictionary<string, AlterationEffect> AttackAttributeTemporaryFriendlyEffects { get; set; }

        /// <summary>
        /// List of temporary malign effects using attack attributes only (Means that an attack is
        /// calculated per step on the character for the specified attack attribute(s))
        /// </summary>
        protected IDictionary<string, AlterationEffect> AttackAttributeTemporaryMalignEffects { get; set; }
        #endregion

        public CharacterAlteration()
        {
            this.ActivePassiveEffects = new Dictionary<string, AlterationEffect>();
            this.AttackAttributePassiveEffects = new Dictionary<string, AlterationEffect>();
            this.PerStepAlterationCosts = new Dictionary<string, AlterationCost>();

            this.ActiveAuraEffects = new List<AlterationEffect>();

            this.ActiveTemporaryEffects = new Dictionary<string, AlterationEffect>();
            this.AttackAttributeTemporaryFriendlyEffects = new Dictionary<string, AlterationEffect>();
            this.AttackAttributeTemporaryMalignEffects = new Dictionary<string, AlterationEffect>();
        }

        #region Serialization
        //public CharacterAlteration(SerializationInfo info, StreamingContext context)
        //{
        //    this.ActivePassiveEffects = (Dictionary<string, AlterationEffect>)info.GetValue("ActivePassiveEffects", typeof(Dictionary<string, AlterationEffect>));
        //    this.AttackAttributePassiveEffects = (Dictionary<string, AlterationEffect>)info.GetValue("AttackAttributePassiveEffects", typeof(Dictionary<string, AlterationEffect>));
        //    this.PerStepAlterationCosts = (Dictionary<string, AlterationCost>)info.GetValue("PerStepAlterationCosts", typeof(Dictionary<string, AlterationCost>));

        //    this.ActiveAuraEffects = (List<AlterationEffect>)info.GetValue("ActiveAuraEffects", typeof(List<AlterationEffect>));

        //    this.ActiveTemporaryEffects = (Dictionary<string, AlterationEffect>)info.GetValue("ActiveTemporaryEffects", typeof(Dictionary<string, AlterationEffect>));
        //    this.AttackAttributeTemporaryFriendlyEffects = (Dictionary<string, AlterationEffect>)info.GetValue("AttackAttributeTemporaryFriendlyEffects", typeof(Dictionary<string, AlterationEffect>));
        //    this.AttackAttributeTemporaryMalignEffects = (Dictionary<string, AlterationEffect>)info.GetValue("AttackAttributeTemporaryMalignEffects", typeof(Dictionary<string, AlterationEffect>));
        //}

        //public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("ActivePassiveEffects", this.ActivePassiveEffects, typeof(Dictionary<string, AlterationEffect>));
        //    info.AddValue("AttackAttributePassiveEffects", this.AttackAttributePassiveEffects, typeof(Dictionary<string, AlterationEffect>));
        //    info.AddValue("PerStepAlterationCosts", this.PerStepAlterationCosts, typeof(Dictionary<string, AlterationCost>));

        //    info.AddValue("ActiveAuraEffects", this.ActiveAuraEffects, typeof(List<AlterationEffect>));

        //    info.AddValue("ActiveTemporaryEffects", this.ActiveTemporaryEffects, typeof(List<AlterationEffect>));
        //    info.AddValue("AttackAttributeTemporaryFriendlyEffects", this.AttackAttributeTemporaryFriendlyEffects, typeof(List<AlterationEffect>));
        //    info.AddValue("AttackAttributeTemporaryMalignEffects", this.AttackAttributeTemporaryMalignEffects, typeof(List<AlterationEffect>));
        //}
        #endregion

        #region (public) Methods
        /// <summary>
        /// Convienence method to get all magic (non-attack attribute) effects acting on the character
        /// </summary>
        public IEnumerable<AlterationEffect> GetAlterations()
        {
            return this.ActiveTemporaryEffects.Values
                       .Union(this.ActivePassiveEffects.Values)
                       .Union(this.ActiveAuraEffects);
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
            return this.ActivePassiveEffects.Keys
                  .Union(this.AttackAttributePassiveEffects.Keys)
                  .Union(this.ActiveTemporaryEffects.Keys)
                  .Union(this.AttackAttributeTemporaryFriendlyEffects.Keys)
                  .Union(this.AttackAttributeTemporaryMalignEffects.Keys);
        }

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
            if (this.PerStepAlterationCosts.ContainsKey(spellId))
                this.PerStepAlterationCosts.Remove(spellId);

            if (this.ActivePassiveEffects.ContainsKey(spellId))
                this.ActivePassiveEffects.Remove(spellId);

            if (this.AttackAttributePassiveEffects.ContainsKey(spellId))
                this.AttackAttributePassiveEffects.Remove(spellId);
        }

        public IEnumerable<AlterationEffect> DecrementEventTimes()
        {
            var updateFunction = new Func<IDictionary<string, AlterationEffect>, IEnumerable<AlterationEffect>>(dictionary =>
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
            // TODO
            //// Gather all effects that are remedied
            //var result =
            //    this.ActiveTemporaryEffects.Values.Where(x => x.RogueName == remediedSpellName)
            //        .Union(this.AttackAttributeTemporaryFriendlyEffects.Where(x => x.RogueName == remediedSpellName))
            //        .Union(this.AttackAttributeTemporaryMalignEffects.Where(x => x.RogueName == remediedSpellName))
            //        .ToList();

            //// Remove these results from the lists
            //foreach (var effect in result)
            //{
            //    if (this.ActiveTemporaryEffects.Contains(effect))
            //        this.ActiveTemporaryEffects.Remove(effect);

            //    else if (this.AttackAttributeTemporaryFriendlyEffects.Contains(effect))
            //        this.AttackAttributeTemporaryFriendlyEffects.Remove(effect);

            //    else if (this.AttackAttributeTemporaryMalignEffects.Contains(effect))
            //        this.AttackAttributeTemporaryMalignEffects.Remove(effect);
            //}

            // Return the remedied alteration effects
            // return result;

            throw new NotImplementedException();
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
                this.PerStepAlterationCosts.Add(alteration.GeneratingSpellId, alteration.Cost);

            // Apply Effect
            switch (alteration.Type)
            {
                case AlterationType.PassiveSource:
                    this.ActivePassiveEffects.Add(alteration.GeneratingSpellId, alteration.Effect);
                    break;
                case AlterationType.PassiveAura:
                    // PlayerAlteration will handle (CONSIDER RE-WRITE)
                    // throw new Exception("Passive Aura NOT SUPPORTED FOR ENEMIES");
                    break;
                case AlterationType.TemporarySource:
                    this.ActiveTemporaryEffects.Add(alteration.GeneratingSpellId, alteration.Effect);
                    break;
                // These types are handled for the target CharacterAlteration
                case AlterationType.TemporaryTarget:
                case AlterationType.TemporaryAllTargets:
                    break;
                // These types aren't supported here - but should be applied externally
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
                            case AlterationAttackAttributeType.Imbue:
                                throw new Exception("Attack Attribute Alteration Type not supported here");
                            case AlterationAttackAttributeType.Passive:
                                this.AttackAttributePassiveEffects.Add(alteration.GeneratingSpellId, alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.TemporaryFriendlySource:
                                this.AttackAttributeTemporaryFriendlyEffects.Add(alteration.GeneratingSpellId, alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.TemporaryMalignSource:
                                this.AttackAttributeTemporaryMalignEffects.Add(alteration.GeneratingSpellId, alteration.Effect);
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
                    this.ActiveTemporaryEffects.Add(alteration.GeneratingSpellId, alteration.Effect);
                    break;
                // These types aren't supported here - but should be applied externally
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
                                this.AttackAttributeTemporaryFriendlyEffects.Add(alteration.GeneratingSpellId, alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.TemporaryMalignTarget:
                                this.AttackAttributeTemporaryMalignEffects.Add(alteration.GeneratingSpellId, alteration.Effect);
                                break;
                            case AlterationAttackAttributeType.MeleeTarget:
                            case AlterationAttackAttributeType.Imbue:
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
