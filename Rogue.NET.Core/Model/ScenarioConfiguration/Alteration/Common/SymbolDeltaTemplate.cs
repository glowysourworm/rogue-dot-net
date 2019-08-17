using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class SymbolDeltaTemplate : Template
    {
        private SymbolTypes _type;
        private SmileyMoods _smileyMood;
        private string _smileyBodyColor;
        private string _smileyLineColor;
        private string _smileyAuraColor;
        private string _characterSymbol;
        private string _characterColor;
        private ImageResources _icon;
        private DisplayImageResources _displayIcon;
        private bool _isFullSymbolDelta;
        private bool _isImageDelta;
        private bool _isMoodDelta;
        private bool _isBodyDelta;
        private bool _isLineDelta;
        private bool _isAuraDelta;
        private bool _isCharacterDelta;
        private bool _isColorDelta;

        public SymbolTypes Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }
        }
        public SmileyMoods SmileyMood
        {
            get { return _smileyMood; }
            set
            {
                if (_smileyMood != value)
                {
                    _smileyMood = value;
                    OnPropertyChanged("SmileyMood");
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
        public ImageResources Icon
        {
            get { return _icon; }
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    OnPropertyChanged("Icon");
                }
            }
        }
        public DisplayImageResources DisplayIcon
        {
            get { return _displayIcon; }
            set
            {
                if (_displayIcon != value)
                {
                    _displayIcon = value;
                    OnPropertyChanged("DisplayIcon");
                }
            }
        }
        public bool IsFullSymbolDelta
        {
            get { return _isFullSymbolDelta; }
            set
            {
                if (_isFullSymbolDelta != value)
                {
                    _isFullSymbolDelta = value;
                    OnPropertyChanged("IsFullSymbolDelta");
                }
            }
        }
        public bool IsImageDelta
        {
            get { return _isImageDelta; }
            set
            {
                if (_isImageDelta != value)
                {
                    _isImageDelta = value;
                    OnPropertyChanged("IsImageDelta");
                }
            }
        }
        public bool IsMoodDelta
        {
            get { return _isMoodDelta; }
            set
            {
                if (_isMoodDelta != value)
                {
                    _isMoodDelta = value;
                    OnPropertyChanged("IsMoodDelta");
                }
            }
        }
        public bool IsBodyDelta
        {
            get { return _isBodyDelta; }
            set
            {
                if (_isBodyDelta != value)
                {
                    _isBodyDelta = value;
                    OnPropertyChanged("IsBodyDelta");
                }
            }
        }
        public bool IsLineDelta
        {
            get { return _isLineDelta; }
            set
            {
                if (_isLineDelta != value)
                {
                    _isLineDelta = value;
                    OnPropertyChanged("IsLineDelta");
                }
            }
        }
        public bool IsAuraDelta
        {
            get { return _isAuraDelta; }
            set
            {
                if (_isAuraDelta != value)
                {
                    _isAuraDelta = value;
                    OnPropertyChanged("IsAuraDelta");
                }
            }
        }
        public bool IsCharacterDelta
        {
            get { return _isCharacterDelta; }
            set
            {
                if (_isCharacterDelta != value)
                {
                    _isCharacterDelta = value;
                    OnPropertyChanged("IsCharacterDelta");
                }
            }
        }
        public bool IsColorDelta
        {
            get { return _isColorDelta; }
            set
            {
                if (_isColorDelta != value)
                {
                    _isColorDelta = value;
                    OnPropertyChanged("IsColorDelta");
                }
            }
        }

        public bool HasSymbolDelta()
        {
            return this.IsFullSymbolDelta ||
                   this.IsImageDelta ||
                   this.IsMoodDelta ||
                   this.IsBodyDelta ||
                   this.IsLineDelta ||
                   this.IsAuraDelta ||
                   this.IsCharacterDelta ||
                   this.IsColorDelta;
        }

        public SymbolDeltaTemplate()
        {
            this.Type = SymbolTypes.Image;
            this.Icon = ImageResources.AmuletOrange;
            this.DisplayIcon = DisplayImageResources.ChristianBlack;

            this.SmileyBodyColor = Colors.Yellow.ToString();
            this.SmileyLineColor = Colors.Black.ToString();
            this.SmileyAuraColor = Colors.Yellow.ToString();

            this.CharacterColor = Colors.White.ToString();
            this.CharacterSymbol = "T";
        }
    }
}
