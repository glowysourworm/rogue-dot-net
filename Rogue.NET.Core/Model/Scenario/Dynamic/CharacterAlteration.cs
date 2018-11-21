using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Dynamic
{
    /// <summary>
    /// Complete list of active character alterations - costs and effects. This is serialized along with the model
    /// to maintain dynamic (game-time) state.
    /// </summary>
    [Serializable]
    public class CharacterAlteration
    {
        #region (Passives / Per Step Costs) Managed via Spell.Id
        /// <summary>
        /// Keeps a list of alteration costs for the character - applied on turn
        /// </summary>
        public IDictionary<string, AlterationCost> PerStepAlterationCosts { get; set; }

        /// <summary>
        /// Active passive effects on the character. The key is the Id for the generating
        /// Spell object.
        /// </summary>
        public IDictionary<string, AlterationEffect> ActivePassiveEffects { get; set; }

        /// <summary>
        /// List of all active aura effects that act on the character (THIS MUST BE CALCUATED
        /// BY A CONTAINER)
        /// </summary>
        public IDictionary<string, AlterationEffect> ActiveAuraEffects { get; set; }

        /// <summary>
        /// List that holds set of passive attack attribute alterations
        /// </summary>
        public IDictionary<string, AlterationEffect> AttackAttributePassiveEffects { get; set; }
        #endregion

        #region Temporary Effects - Managed via Event Time
        /// <summary>
        /// Active temporary effects on the character
        /// </summary>
        public IList<AlterationEffect> ActiveTemporaryEffects { get; set; }

        /// <summary>
        /// List of temporary friendly effects using attack attributes only (Means that defense
        /// is calculated per turn of melee for this character)
        /// </summary>
        public IList<AlterationEffect> AttackAttributeTemporaryFriendlyEffects { get; set; }

        /// <summary>
        /// List of temporary malign effects using attack attributes only (Means that an attack is
        /// calculated per step on the character for the specified attack attribute(s))
        /// </summary>
        public IList<AlterationEffect> AttackAttributeTemporaryMalignEffects { get; set; }
        #endregion

        public CharacterAlteration()
        {
            this.ActiveAuraEffects = new Dictionary<string, AlterationEffect>();
            this.ActivePassiveEffects = new Dictionary<string, AlterationEffect>();
            this.AttackAttributePassiveEffects = new Dictionary<string, AlterationEffect>();
            this.PerStepAlterationCosts = new Dictionary<string, AlterationCost>();

            this.ActiveTemporaryEffects = new List<AlterationEffect>();
            this.AttackAttributeTemporaryFriendlyEffects = new List<AlterationEffect>();
            this.AttackAttributeTemporaryMalignEffects = new List<AlterationEffect>();
        }

        /// <summary>
        /// Convienence method to get all magic (non-attack attribute) effects acting on the character
        /// </summary>
        public IEnumerable<AlterationEffect> GetAlterations()
        {
            return this.ActiveTemporaryEffects
                       .Union(this.ActivePassiveEffects.Values)
                       .Union(this.ActiveAuraEffects.Values);
        }

        public IEnumerable<CharacterStateType> GetStates()
        {
            return GetAlterations().Select(x => x.State);
        }
    }
}
