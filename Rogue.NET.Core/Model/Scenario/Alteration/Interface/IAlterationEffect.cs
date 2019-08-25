
namespace Rogue.NET.Core.Model.Scenario.Alteration.Interface
{
    /// <summary>
    /// Base interface for the *AlterationEffect interfaces. This is for convenience when
    /// processing. THESE ARE FOR MARKING ONLY. TRY NOT TO ADD ANY PROPERTIES OR METHODS.
    /// </summary>
    public interface IAlterationEffect
    {
        /// <summary>
        /// PROPERTY INHERITED FROM ROGUEBASE
        /// </summary>
        string Id { get; }

        /// <summary>
        /// PROPERTY INHERITED FROM ROGUEBASE
        /// </summary>
        string RogueName { get; set; }
    }
}
