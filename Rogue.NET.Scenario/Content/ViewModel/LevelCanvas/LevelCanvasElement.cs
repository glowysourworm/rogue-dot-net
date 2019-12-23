using Rogue.NET.Scenario.Content.ViewModel.LevelCanvas.Inteface;

using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.ViewModel.LevelCanvas
{
    public class LevelCanvasElement : FrameworkElement, ILevelCanvasElement
    {
        public string Id { get; private set; }

        public Point Location
        {
            get { return new Point(Canvas.GetLeft(this), Canvas.GetTop(this)); }
            set
            {
                if (value != null)
                {
                    Canvas.SetLeft(this, value.X);
                    Canvas.SetTop(this, value.Y);
                }
            }
        }

        public LevelCanvasElement(string scenarioObjectId)
        {
            this.Id = scenarioObjectId;
        }
    }
}
