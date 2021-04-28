using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;

using System.Windows.Media;

namespace Rogue.NET.Scenario.Intro.ViewModel
{
    public class SavedGameViewModel : NotifyViewModel
    {
        public string RogueName { get; set; }
        public string ScenarioName { get; set; }
        public string CharacterClass { get; set; }
        public int Seed { get; set; }
        public SmileyExpression SmileyExpression { get; set; }
        public Color SmileyBodyColor { get; set; }
        public Color SmileyLineColor { get; set; }
        public int CurrentLevel { get; set; }
        public bool ObjectiveAcheived { get; set; }
        public bool SurvivorMode { get; set; }

        public SavedGameViewModel()
        {
        }
    }
}
