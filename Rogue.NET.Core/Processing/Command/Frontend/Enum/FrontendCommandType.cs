
namespace Rogue.NET.Core.Processing.Command.Frontend.Enum
{
    public enum FrontendCommandType
    {
        /// <summary>
        /// Command to enter targeting mode
        /// </summary>
        StartTargeting,

        /// <summary>
        /// Moves target during targeting mode
        /// </summary>
        MoveTarget,

        /// <summary>
        /// Cycles target between visible enemies
        /// </summary>
        CycleTarget,

        /// <summary>
        /// Selects target and ends targeting mode to commence backend process
        /// </summary>
        SelectTarget,

        /// <summary>
        /// Exit targeting mode and clear targeting information
        /// </summary>
        EndTargeting
    }
}
