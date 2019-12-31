using Rogue.NET.Scenario.Content.ViewModel.LevelCanvas.Inteface;

using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.ViewModel.LevelCanvas
{
    public class LevelCanvasImage : Image, ILevelCanvasElement
    {
        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register("Location", typeof(Point), typeof(LevelCanvasImage));

        public string ScenarioObjectId { get; private set; }

        public Point Location
        {
            get { return (Point)GetValue(LocationProperty); }
            set
            {
                Canvas.SetLeft(this, value.X);
                Canvas.SetTop(this, value.Y);

                SetValue(LocationProperty, value);
            }
        }

        public LevelCanvasImage(string scenarioObjectId)
        {
            this.ScenarioObjectId = scenarioObjectId;
        }
    }
}
