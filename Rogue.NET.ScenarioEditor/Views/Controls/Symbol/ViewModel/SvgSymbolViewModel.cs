using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
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
        SymbolType _symbolType;

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
        public bool UseColorMask
        {
            get { return _useColorMask; }
            set { this.RaiseAndSetIfChanged(ref _useColorMask, value); }
        }
        public SymbolType SymbolType
        {
            get { return _symbolType; }
            set { this.RaiseAndSetIfChanged(ref _symbolType, value); }
        }

        // Character Symbol Constructor
        public static SvgSymbolViewModel CreateCharacterSymbol(ImageSource imageSource, string category, string character, double symbolScale)
        {
            return new SvgSymbolViewModel()
            {
                SymbolType = SymbolType.Character,
                ImageSource = imageSource,
                Category = category,
                Character = character,
                CharacterScale = symbolScale
            };
        }
        public static SvgSymbolViewModel CreateSymbol(ImageSource imageSource, string symbol, double hue, double saturation, double lightness, bool useColorMask)
        {
            return new SvgSymbolViewModel()
            {
                SymbolType = SymbolType.Symbol,
                ImageSource = imageSource,
                Symbol = symbol,
                Hue = hue,
                Saturation = saturation,
                Lightness = lightness,
                UseColorMask = useColorMask
            };
        }
        public static SvgSymbolViewModel CreateOrientedSymbol(ImageSource imageSource, string symbol, double hue, double saturation, double lightness, bool useColorMask)
        {
            return new SvgSymbolViewModel()
            {
                SymbolType = SymbolType.OrientedSymbol,
                ImageSource = imageSource,
                Symbol = symbol,
                Hue = hue,
                Saturation = saturation,
                Lightness = lightness,
                UseColorMask = useColorMask
            };
        }
        public static SvgSymbolViewModel CreateTerrainSymbol(ImageSource imageSource, string symbol, double hue, double saturation, double lightness, bool useColorMask)
        {
            return new SvgSymbolViewModel()
            {
                SymbolType = SymbolType.Terrain,
                ImageSource = imageSource,
                Symbol = symbol,
                Hue = hue,
                Saturation = saturation,
                Lightness = lightness,
                UseColorMask = useColorMask
            };
        }
        public static SvgSymbolViewModel CreateGameSymbol(ImageSource imageSource, string symbol)
        {
            return new SvgSymbolViewModel()
            {
                SymbolType = SymbolType.Game,
                ImageSource = imageSource,
                Symbol = symbol
            };
        }

        public SvgSymbolViewModel() { }
    }
}
