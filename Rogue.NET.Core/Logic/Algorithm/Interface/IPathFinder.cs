using Rogue.NET.Core.Model.Scenario.Content.Layout;

namespace Rogue.NET.Core.Logic.Algorithm.Interface
{
    /// <summary>
    /// Component to calculate path finding algorithm
    /// </summary>
    public interface IPathFinder
    {
        /// <summary>
        /// Returns next point towards point2. If limits are reached algorithm returns point1.
        /// </summary>
        /// <param name="maxRadius">Maximum euclidean distance between points before algorithm terminates</param>
        CellPoint FindPath(CellPoint point1, CellPoint point2, double maxRadius);
    }
}
