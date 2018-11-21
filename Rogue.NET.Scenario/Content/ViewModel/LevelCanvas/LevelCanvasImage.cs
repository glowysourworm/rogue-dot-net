using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.ViewModel.LevelCanvas
{
    public class LevelCanvasImage : Image
    {
        public static readonly DependencyProperty ZIndexProperty =
            DependencyProperty.Register("ZIndex", typeof(int), typeof(LevelCanvasImage));

        public int ZIndex
        {
            get { return (int)GetValue(ZIndexProperty); }
            set { SetValue(ZIndexProperty, value); }
        }

        public LevelCanvasImage()
        {
        }
    }
}
