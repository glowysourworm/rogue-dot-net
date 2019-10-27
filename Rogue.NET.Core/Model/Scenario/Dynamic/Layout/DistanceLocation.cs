using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Model.Static;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Layout
{
    /// <summary>
    /// Class for storing dynamic distance (FROM A SOURCE) information along with calculated line-of-sight data
    /// </summary>
    public class DistanceLocation
    {
        public GridLocation Location { get; private set; }
        public double RoguianDistance { get; private set; }
        public double EuclideanDistance { get; private set; }

        public DistanceLocation(GridLocation source, GridLocation location)
        {
            this.Location = location;
            this.RoguianDistance = RogueCalculator.RoguianDistance(source, location);
            this.EuclideanDistance = RogueCalculator.EuclideanDistance(source, location);
        }
    }
}
