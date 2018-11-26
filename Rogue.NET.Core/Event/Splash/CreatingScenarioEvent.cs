using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using System.Windows.Media;

namespace Rogue.NET.Common.Events.Splash
{
    public class CreatingScenarioEventArgs : System.EventArgs
    {
        public string ScenarioName { get; set; }
        public Color SmileyLineColor { get; set; }
        public Color SmileyBodyColor { get; set; }

        public string Message { get; set; }
        public double Progress { get; set; }

        public CreatingScenarioEventArgs()
        {
            this.SmileyLineColor = Colors.Transparent;
            this.SmileyBodyColor = Colors.Transparent;
        }
    }
    public class CreatingScenarioEvent : RogueAsyncEvent<CreatingScenarioEventArgs>
    {

    }
}
