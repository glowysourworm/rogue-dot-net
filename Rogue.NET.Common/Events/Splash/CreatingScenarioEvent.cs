using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Rogue.NET.Common.Events.Splash
{
    public class CreatingScenarioEvent : CompositePresentationEvent<CreatingScenarioEvent>
    {
        public string ScenarioName { get; set; }
        public Color SmileyLineColor { get; set; }
        public Color SmileyBodyColor { get; set; }

        public string Message { get; set; }
        public double Progress { get; set; }

        public CreatingScenarioEvent()
        {
            this.SmileyLineColor = Colors.Transparent;
            this.SmileyBodyColor = Colors.Transparent;
        }
    }
}
