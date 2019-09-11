using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface
{
    /// <summary>
    /// Component that "Collects" alterations and keeps them catalogued for
    /// lookup. (Overused the verb "Container")
    /// 
    /// NOTE*** Design of alteration effects separates them by type - with the
    ///         intent to PREVENT common public properties. So, having a common
    ///         interface or base type to combine different alteration effects - 
    ///         and thereby alteration collector methods actually ISN'T the intention.
    ///         
    ///         I'm adding the interface just for a marker; and trying to decide how to
    ///         deal with having 16 different types of alteration effects and write 
    ///         as few as possible disparate methods to process them. 
    ///         
    ///         The purpose of having the other marker interfaces is to prevent 
    ///         using one alteration effect type in ways it should not be used. Example,
    ///         would be consumable projectile alteration (should not be an aura). This
    ///         distinction I think prevents a lot of un-intended parameter overlap in
    ///         the alterations and makes them much easier to think about and deal with.
    ///         
    ///         The only trick is how to deal with all the effect types without writing
    ///         redundant code.
    ///         
    ///         UPDATE:  Had a thought that there are some specific calculations ON 
    ///                  alteration effects that can be made internal to each collector.
    ///                  I'll add public interface methods for those. 
    ///         UPDATE:  Finding that the number of collectors is expanding to deal with
    ///                  all the different "application types" for alterations. (I've used
    ///                  the term "application types" to describe attack attribute sub-types; 
    ///                  but am finding it a convenient term to differentiate passive, temporary,
    ///                  aura, melee). So, I'm going to create several parallel interfaces to 
    ///                  describe the collectors. These will cover most of the method space
    ///                  used by the CharacterAlteration to prevent redundancy.
    ///
    ///         UPDATE:  Added the template parameter T to specify the alteration effect type. This
    ///                  turned out to be the commonality for creating different collectors (having
    ///                  a different base effect). 
    ///
    ///         UPDATE:  Changing T to represent the whole Alteration
    ///
    /// </summary>
    public interface IAlterationCollector
    {
        /// <summary>
        /// Applies Alteration based on type
        /// </summary>
        /// <returns>False if effect didn't stack, or True if the effect was applied.</returns>
        bool Apply(Scenario.Alteration.Common.AlterationContainer alteration);

        /// <summary>
        /// (Pass-through method) Removes and returns the specified alteration(s) from the IAlterationCollector if it exists
        /// based on the RogueBase.RogueName of the Alteration
        /// </summary>
        IEnumerable<Scenario.Alteration.Common.AlterationContainer> Filter(string alterationName);

        /// <summary>
        /// Returns all per-step AlterationCosts by Alteration name (NOTE*** This is the only cost type that
        /// should be collected)
        /// </summary>
        IEnumerable<KeyValuePair<string, AlterationCost>> GetCosts();
    }
}
