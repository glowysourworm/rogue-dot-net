using Rogue.NET.Core.Model.ScenarioConfiguration.Design;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    /// <summary>
    /// Class used for thread data during scenario generation - should be ONE PER LEVEL NUMBER
    /// </summary>
    public class ScenarioGeneratorThreadData
    {
        public int LevelNumber { get; set; }
        public bool HasStartedGeneration { get; set; }
        public LayoutTemplate Layout { get; set; }
        public LayoutGenerationTemplate LayoutGeneration { get; set; }
        public LevelBranchTemplate LevelBranch { get; set; }
    }
}
