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
        /// <summary>
        /// Keeps a list of alteration costs for the character - applied on turn
        /// </summary>
        public IList<AlterationCost> PerStepAlterationCosts { get; set; }

        #region Normal Effects
        /// <summary>
        /// Active temporary effects on the character
        /// </summary>
        public IList<AlterationEffect> ActiveTemporaryEffects { get; set; }

        /// <summary>
        /// Active passive effects on the character
        /// </summary>
        public IList<AlterationEffect> ActivePassiveEffects { get; set; }

        /// <summary>
        /// List of all active aura effects that act on the character
        /// </summary>
        public IList<AlterationEffect> ActiveAuraEffects { get; set; }
        #endregion

        #region Attack Attribute Effects
        /// <summary>
        /// List that holds set of passive attack attribute alterations
        /// </summary>
        public IList<AlterationEffect> AttackAttributePassiveEffects { get; set; }

        /// <summary>
        /// List of temporary friendly effects using attack attributes only
        /// </summary>
        public IList<AlterationEffect> AttackAttributeTemporaryFriendlyEffects { get; set; }

        /// <summary>
        /// List of temporary malign effects using attack attributes only
        /// </summary>
        public IList<AlterationEffect> AttackAttributeTemporaryMalignEffects { get; set; }
        #endregion

        public CharacterAlteration()
        {
            this.ActiveAuraEffects = new List<AlterationEffect>();
            this.ActivePassiveEffects = new List<AlterationEffect>();
            this.ActiveTemporaryEffects = new List<AlterationEffect>();
            this.AttackAttributePassiveEffects = new List<AlterationEffect>();
            this.AttackAttributeTemporaryFriendlyEffects = new List<AlterationEffect>();
            this.AttackAttributeTemporaryMalignEffects = new List<AlterationEffect>();
            this.PerStepAlterationCosts = new List<AlterationCost>();
        }

        /// <summary>
        /// Convienence method to get all magic effects acting on the character
        /// </summary>
        public IEnumerable<AlterationEffect> GetAlterations()
        {
            return this.ActiveTemporaryEffects
                       .Union(this.ActivePassiveEffects)
                       .Union(this.ActiveAuraEffects);
        }

        public IEnumerable<CharacterStateType> GetStates()
        {
            return GetAlterations().Select(x => x.State);
        }
    }
}
