using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.ViewModel.LevelCanvas
{
    public class LevelCanvasImage : Image
    {
        public string ScenarioObjectId { get; private set; }

        public LevelCanvasImage(string scenarioObjectId)
        {
            this.ScenarioObjectId = scenarioObjectId;
        }
    }
}
