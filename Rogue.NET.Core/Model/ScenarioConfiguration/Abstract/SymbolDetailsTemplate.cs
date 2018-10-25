﻿using Rogue.NET.Core.Model.Enums;

using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Abstract
{
    [Serializable]
    public class SymbolDetailsTemplate : Template
    {
        private SymbolTypes _type;
        private SmileyMoods _smileyMood;
        private string _smileyBodyColor;
        private string _smileyLineColor;
        private string _smileyAuraColor;
        private string _characterSymbol;
        private string _characterColor;
        private ImageResources _icon;
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

        public SymbolDetailsTemplate()
        {
            this.Type = SymbolTypes.Image;
            this.Icon = ImageResources.AmuletOrange;

            this.SmileyBodyColor = Colors.Yellow.ToString();
            this.SmileyLineColor = Colors.Black.ToString();
            this.SmileyAuraColor = Colors.Yellow.ToString();

            this.CharacterColor = Colors.White.ToString();
            this.CharacterSymbol = "T";
        }
        public SymbolDetailsTemplate(SymbolDetailsTemplate tmp)
        {
            this.Type = tmp.Type;
            this.Icon = tmp.Icon;
            this.SmileyAuraColor = tmp.SmileyAuraColor;
            this.SmileyBodyColor = tmp.SmileyBodyColor;
            this.SmileyLineColor = tmp.SmileyLineColor;
            this.SmileyMood = tmp.SmileyMood;
            this.Type = tmp.Type;
        }
    }
}