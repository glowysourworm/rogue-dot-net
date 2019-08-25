using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rogue.NET.Scenario.Content.ViewModel.LevelCanvas
{
    public class LevelCanvasElement : FrameworkElement
    {
        public string Id { get; private set; }

        public LevelCanvasElement(string scenarioObjectId)
        {
            this.Id = scenarioObjectId;
        }
    }
}
