using Rogue.NET.Core.Model.Enums;

using System;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract
{
    public class SymbolDetailsTemplateViewModel : TemplateViewModel
    {
        private SymbolType _symbolType;
        private SmileyExpression _smileyExpression;
        private string _smileyBodyColor;
        private string _smileyLineColor;
        private string _smileyAuraColor;
        private string _characterSymbol;
        private string _characterSymbolCategory;
        private string _characterColor;
        private double _characterScale;
        private string _symbol;
        private double _symbolHue;
        private double _symbolSaturation;
        private double _symbolLightness;
        private double _symbolScale;
        private bool _symbolUseColorMask;
        private string _gameSymbol;

        public SymbolType SymbolType
        {
            get { return _symbolType; }
            set { this.RaiseAndSetIfChanged(ref _symbolType, value); }
        }
        public SmileyExpression SmileyExpression
        {
            get { return _smileyExpression; }
            set { this.RaiseAndSetIfChanged(ref _smileyExpression, value); }
        }
        public string SmileyBodyColor
        {
            get { return _smileyBodyColor; }
            set { this.RaiseAndSetIfChanged(ref _smileyBodyColor, value); }
        }
        public string SmileyLineColor
        {
            get { return _smileyLineColor; }
            set { this.RaiseAndSetIfChanged(ref _smileyLineColor, value); }
        }
        public string SmileyAuraColor
        {
            get { return _smileyAuraColor; }
            set { this.RaiseAndSetIfChanged(ref _smileyAuraColor, value); }
        }
        public string CharacterSymbol
        {
            get { return _characterSymbol; }
            set { this.RaiseAndSetIfChanged(ref _characterSymbol, value); }
        }
        public string CharacterSymbolCategory
        {
            get { return _characterSymbolCategory; }
            set { this.RaiseAndSetIfChanged(ref _characterSymbolCategory, value); }
        }
        public string CharacterColor
        {
            get { return _characterColor; }
            set { this.RaiseAndSetIfChanged(ref _characterColor, value); }
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
        public double SymbolHue
        {
            get { return _symbolHue; }
            set { this.RaiseAndSetIfChanged(ref _symbolHue, value); }
        }
        public double SymbolSaturation
        {
            get { return _symbolSaturation; }
            set { this.RaiseAndSetIfChanged(ref _symbolSaturation, value); }
        }
        public double SymbolLightness
        {
            get { return _symbolLightness; }
            set { this.RaiseAndSetIfChanged(ref _symbolLightness, value); }
        }
        public double SymbolScale
        {
            get { return _symbolScale; }
            set { this.RaiseAndSetIfChanged(ref _symbolScale, value); }
        }
        public bool SymbolUseColorMask
        {
            get { return _symbolUseColorMask; }
            set { this.RaiseAndSetIfChanged(ref _symbolUseColorMask, value); }
        }
        public string GameSymbol
        {
            get { return _gameSymbol; }
            set { this.RaiseAndSetIfChanged(ref _gameSymbol, value); }
        }

        public SymbolDetailsTemplateViewModel()
        {
            this.SymbolType = SymbolType.Smiley;

            this.SmileyExpression = SmileyExpression.Happy;
            this.SmileyBodyColor = Colors.Yellow.ToString();
            this.SmileyAuraColor = Colors.Yellow.ToString();
            this.SmileyLineColor = Colors.Black.ToString();

            this.CharacterColor = Colors.White.ToString();
            this.CharacterSymbol = Rogue.NET.Common.Constant.CharacterSymbol.DefaultCharacterSymbol;
            this.CharacterSymbolCategory = Rogue.NET.Common.Constant.CharacterSymbol.DefaultCharacterCategory;
            this.CharacterScale = 1;

            this.GameSymbol = Rogue.NET.Common.Constant.GameSymbol.Identify;

            this.Symbol = Rogue.NET.Common.Constant.Symbol.DefaultSymbol;
            this.SymbolScale = 1;
            this.SymbolHue = 0;
            this.SymbolLightness = 0;
            this.SymbolSaturation = 0;
            this.SymbolUseColorMask = false;
        }
    }
}
