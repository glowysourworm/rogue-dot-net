using ProtoBuf;
using Rogue.NET.Core.Model.Enums;

using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Abstract
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true, SkipConstructor = true)]
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

        [ProtoMember(1)]
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
        [ProtoMember(2)]
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
        [ProtoMember(3)]
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
        [ProtoMember(4)]
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
        [ProtoMember(5)]
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
        [ProtoMember(6)]
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
        [ProtoMember(7)]
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
        [ProtoMember(8)]
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
        [ProtoMember(9)]
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
        [ProtoMember(10)]
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
        [ProtoMember(11)]
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
        [ProtoMember(12)]
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
        [ProtoMember(13)]
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
        [ProtoMember(14)]
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
        [ProtoMember(15)]
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
        [ProtoMember(16)]
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
