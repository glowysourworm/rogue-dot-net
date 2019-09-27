using Rogue.NET.Core.Model.Enums;

using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Abstract
{
    [Serializable]
    public class SymbolDetailsTemplate : Template
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
        private string _symbolColorMapFrom;
        private string _symbolColorMapTo;
        private string _gameSymbol;

        public SymbolType SymbolType
        {
            get { return _symbolType; }
            set
            {
                if (_symbolType != value)
                {
                    _symbolType = value;
                    OnPropertyChanged("SymbolType");
                }
            }
        }
        public SmileyExpression SmileyExpression
        {
            get { return _smileyExpression; }
            set
            {
                if (_smileyExpression != value)
                {
                    _smileyExpression = value;
                    OnPropertyChanged("SmileyExpression");
                }
            }
        }
        public string SmileyBodyColor
        {
            get { return _smileyBodyColor; }
            set
            {
                if (_smileyBodyColor != value)
                {
                    _smileyBodyColor = value;
                    OnPropertyChanged("SmileyBodyColor");
                }
            }
        }
        public string SmileyLineColor
        {
            get { return _smileyLineColor; }
            set
            {
                if (_smileyLineColor != value)
                {
                    _smileyLineColor = value;
                    OnPropertyChanged("SmileyLineColor");
                }
            }
        }
        public string SmileyAuraColor
        {
            get { return _smileyAuraColor; }
            set
            {
                if (_smileyAuraColor != value)
                {
                    _smileyAuraColor = value;
                    OnPropertyChanged("SmileyAuraColor");
                }
            }
        }
        public string CharacterSymbol
        {
            get { return _characterSymbol; }
            set
            {
                if (_characterSymbol != value)
                {
                    _characterSymbol = value;
                    OnPropertyChanged("CharacterSymbol");
                }
            }
        }
        public string CharacterSymbolCategory
        {
            get { return _characterSymbolCategory; }
            set
            {
                if (_characterSymbolCategory != value)
                {
                    _characterSymbolCategory = value;
                    OnPropertyChanged("CharacterSymbolCategory");
                }
            }
        }
        public string CharacterColor
        {
            get { return _characterColor; }
            set
            {
                if (_characterColor != value)
                {
                    _characterColor = value;
                    OnPropertyChanged("CharacterColor");
                }
            }
        }
        public double CharacterScale
        {
            get { return _characterScale; }
            set
            {
                if (_characterScale != value)
                {
                    _characterScale = value;
                    OnPropertyChanged("CharacterScale");
                }
            }
        }
        public string Symbol
        {
            get { return _symbol; }
            set
            {
                if (_symbol != value)
                {
                    _symbol = value;
                    OnPropertyChanged("Symbol");
                }
            }
        }
        public double SymbolHue
        {
            get { return _symbolHue; }
            set
            {
                if (_symbolHue != value)
                {
                    _symbolHue = value;
                    OnPropertyChanged("SymbolHue");
                }
            }
        }
        public double SymbolSaturation
        {
            get { return _symbolSaturation; }
            set
            {
                if (_symbolSaturation != value)
                {
                    _symbolSaturation = value;
                    OnPropertyChanged("SymbolSaturation");
                }
            }
        }
        public double SymbolLightness
        {
            get { return _symbolLightness; }
            set
            {
                if (_symbolLightness != value)
                {
                    _symbolLightness = value;
                    OnPropertyChanged("SymbolLightness");
                }
            }
        }
        public double SymbolScale
        {
            get { return _symbolScale; }
            set
            {
                if (_symbolScale != value)
                {
                    _symbolScale = value;
                    OnPropertyChanged("SymbolScale");
                }
            }
        }
        public string SymbolColorMapFrom
        {
            get { return _symbolColorMapFrom; }
            set
            {
                if (_symbolColorMapFrom != value)
                {
                    _symbolColorMapFrom = value;
                    OnPropertyChanged("SymbolColorMapFrom");
                }
            }
        }
        public string SymbolColorMapTo
        {
            get { return _symbolColorMapTo; }
            set
            {
                if (_symbolColorMapTo != value)
                {
                    _symbolColorMapTo = value;
                    OnPropertyChanged("SymbolColorMapTo");
                }
            }
        }
        public string GameSymbol
        {
            get { return _gameSymbol; }
            set
            {
                if (_gameSymbol != value)
                {
                    _gameSymbol = value;
                    OnPropertyChanged("GameSymbol");
                }
            }
        }
        public SymbolDetailsTemplate()
        {
            this.SymbolType = SymbolType.Smiley;

            this.SmileyBodyColor = Colors.Yellow.ToString();
            this.SmileyLineColor = Colors.Black.ToString();
            this.SmileyAuraColor = Colors.Yellow.ToString();

            this.CharacterColor = Colors.White.ToString();
            this.CharacterScale = 1;

            this.SymbolScale = 1;
            this.SymbolHue = 0;
            this.SymbolLightness = 0;
            this.SymbolSaturation = 0;
            this.SymbolColorMapFrom = Colors.White.ToString();
            this.SymbolColorMapTo = Colors.White.ToString();
        }
    }
}
