using Prism.Commands;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Event.Scenario;
using System.Windows.Input;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Intro.ViewModel
{
    public class SavedGameViewModel : NotifyViewModel
    {
        public string Name { get; set; }
        public SmileyExpression SmileyExpression { get; set; }
        public Color SmileyBodyColor { get; set; }
        public Color SmileyLineColor { get; set; }
        public int CurrentLevel { get; set; }
        public bool ObjectiveAcheived { get; set; }

        public SavedGameViewModel()
        {
        }
    }
}
