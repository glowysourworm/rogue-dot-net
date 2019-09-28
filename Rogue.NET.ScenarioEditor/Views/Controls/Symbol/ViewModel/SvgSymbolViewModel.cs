using Rogue.NET.Common.ViewModel;

using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.Views.Controls.Symbol.ViewModel
{
    public class SvgSymbolViewModel : NotifyViewModel
    {
        ImageSource _imageSource;
        string _category;
        string _character;
        double _characterScale;
        string _symbol;
        double _hue;
        double _saturation;
        double _lightness;
        bool _useColorMask;
        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set { this.RaiseAndSetIfChanged(ref _imageSource, value); }
        }
        public string Category
        {
            get { return _category; }
            set { this.RaiseAndSetIfChanged(ref _category, value); }
        }
        public string Character
        {
            get { return _character; }
            set { this.RaiseAndSetIfChanged(ref _character, value); }
        }
        public double CharacterScale
        {
            get { return _characterScale; }
            set { this.RaiseAndSetIfChanged(ref _characterScale, value); }
        }
        public string Symbol
        {
            get { return _symbol; }
            set { this.RaiseAndSetIfChanged(ref _symbol, value); }
        }
        public double Hue
        {
            get { return _hue; }
            set { this.RaiseAndSetIfChanged(ref _hue, value); }
        }
        public double Saturation
        {
            get { return _saturation; }
            set { this.RaiseAndSetIfChanged(ref _saturation, value); }
        }
        public double Lightness
        {
            get { return _lightness; }
            set { this.RaiseAndSetIfChanged(ref _lightness, value); }
        }
        public SvgSymbolViewModel(ImageSource imageSource, string category, string character, double symbolScale)
        {
            this.ImageSource = imageSource;
            this.Category = category;
            this.Character = character;
            this.CharacterScale = symbolScale;
        }
        public SvgSymbolViewModel(ImageSource imageSource, 
                                  string symbol, 
                                  double hue, 
                                  double saturation, 
                                  double lightness)
        {
            this.ImageSource = imageSource;
            this.Symbol = symbol;
            this.Hue = hue;
            this.Saturation = saturation;
            this.Lightness = lightness;
        }
    }
}
