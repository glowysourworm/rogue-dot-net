using System.Windows.Media;

namespace Rogue.NET.Common.ViewModel
{
    public class ColorViewModel : NotifyViewModel
    {
        string _name;
        Brush _brush;
        Color _color;
        string _colorString;

        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
        public Brush Brush
        {
            get { return _brush; }
            set { this.RaiseAndSetIfChanged(ref _brush, value); }
        }
        public Color Color
        {
            get { return _color; }
            set { this.RaiseAndSetIfChanged(ref _color, value); }
        }
        public string ColorString
        {
            get { return _colorString; }
            set { this.RaiseAndSetIfChanged(ref _colorString, value); }
        }
    }
}
