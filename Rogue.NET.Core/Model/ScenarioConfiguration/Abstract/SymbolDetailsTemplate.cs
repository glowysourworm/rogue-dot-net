using Rogue.NET.Core.Model.Enums;

using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Abstract
{
    [Serializable]
    public class SymbolDetailsTemplate : Template
    {
        private SymbolTypes _type;
        private SmileyExpression _smileyExpression;
        private string _smileyBodyColor;
        private string _smileyLineColor;
        private string _smileyAuraColor;
        private string _characterSymbol;
        private string _characterColor;
        private ImageResources _icon;
        private DisplayImageResources _displayIcon;

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
            this.Type = tmp.Type;
        }
    }
}
