using Rogue.NET.Core.Model.Enums;

using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Service.Cache
{
    /// <summary>
    /// Class for storing scenario data (something like a file header) when it's read into the cache
    /// </summary>
    public class ScenarioInfo
    {
        public string ScenarioName { get; set; }
        public string RogueName { get; set; }
        public string CharacterClass { get; set; }
        public int Seed { get; set; }
        public int CurrentLevel { get; set; }
        public bool IsObjectiveAcheived { get; set; }
        public bool SurvivorMode { get; set; }
        public SmileyExpression SmileyExpression { get; set; }
        public Color SmileyBodyColor { get; set; }
        public Color SmileyLineColor { get; set; }
    }
}
