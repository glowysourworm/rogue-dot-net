using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class SymbolEffectTemplate : Template
    {
        private SmileyExpression _smileyExpression;
        private string _smileyBodyColor;
        private string _smileyLineColor;
        private string _smileyLightRadiusColor;
        private string _characterSymbol;
        private string _characterSymbolCategory;
        private string _characterColor;
        private SymbolDetailsTemplate _fullSymbolChangeDetails;
        private bool _isFullSymbolChange;
        private bool _isSmileyExpressionChange;
        private bool _isSmileyBodyColorChange;
        private bool _isSmileyLineColorChange;
        private bool _isSmileyLightRadiusColorChange;
        private bool _isCharacterSymbolChange;
        private bool _isCharacterColorChange;
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
        public string SmileyLightRadiusColor
        {
            get { return _smileyLightRadiusColor; }
            set
            {
                if (_smileyLightRadiusColor != value)
                {
                    _smileyLightRadiusColor = value;
                    OnPropertyChanged("SmileyLightRadiusColor");
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
        public bool IsFullSymbolChange
        {
            get { return _isFullSymbolChange; }
            set
            {
                if (_isFullSymbolChange != value)
                {
                    _isFullSymbolChange = value;
                    OnPropertyChanged("IsFullSymbolChange");
                }
            }
        }
        public bool IsSmileyExpressionChange
        {
            get { return _isSmileyExpressionChange; }
            set
            {
                if (_isSmileyExpressionChange != value)
                {
                    _isSmileyExpressionChange = value;
                    OnPropertyChanged("IsSmileyExpressionChange");
                }
            }
        }
        public bool IsSmileyBodyColorChange
        {
            get { return _isSmileyBodyColorChange; }
            set
            {
                if (_isSmileyBodyColorChange != value)
                {
                    _isSmileyBodyColorChange = value;
                    OnPropertyChanged("IsSmileyBodyColorChange");
                }
            }
        }
        public bool IsSmileyLineColorChange
        {
            get { return _isSmileyLineColorChange; }
            set
            {
                if (_isSmileyLineColorChange != value)
                {
                    _isSmileyLineColorChange = value;
                    OnPropertyChanged("IsSmileyLineColorChange");
                }
            }
        }
        public bool IsSmileyLightRadiusColorChange
        {
            get { return _isSmileyLightRadiusColorChange; }
            set
            {
                if (_isSmileyLightRadiusColorChange != value)
                {
                    _isSmileyLightRadiusColorChange = value;
                    OnPropertyChanged("IsSmileyLightRadiusColorChange");
                }
            }
        }
        public bool IsCharacterSymbolChange
        {
            get { return _isCharacterSymbolChange; }
            set
            {
                if (_isCharacterSymbolChange != value)
                {
                    _isCharacterSymbolChange = value;
                    OnPropertyChanged("IsCharacterSymbolChange");
                }
            }
        }
        public bool IsCharacterColorChange
        {
            get { return _isCharacterColorChange; }
            set
            {
                if (_isCharacterColorChange != value)
                {
                    _isCharacterColorChange = value;
                    OnPropertyChanged("IsCharacterColorChange");
                }
            }
        }
        public SymbolDetailsTemplate FullSymbolChangeDetails
        {
            get { return _fullSymbolChangeDetails; }
            set
            {
                if (_fullSymbolChangeDetails != value)
                {
                    _fullSymbolChangeDetails = value;
                    OnPropertyChanged("FullSymbolChangeDetails");
                }
            }
        }
        public bool HasSymbolChange()
        {
            return this.IsCharacterColorChange ||
                   this.IsCharacterSymbolChange ||
                   this.IsFullSymbolChange ||
                   this.IsSmileyBodyColorChange ||
                   this.IsSmileyExpressionChange ||
                   this.IsSmileyLightRadiusColorChange ||
                   this.IsSmileyLineColorChange;
        }
        public SymbolEffectTemplate()
        {
            this.FullSymbolChangeDetails = new SymbolDetailsTemplate();

            this.SmileyExpression = SmileyExpression.Happy;
            this.SmileyBodyColor = Colors.Yellow.ToString();
            this.SmileyLightRadiusColor = Colors.Yellow.ToString();
            this.SmileyLineColor = Colors.Black.ToString();

            this.CharacterColor = Colors.White.ToString();
            this.CharacterSymbol = Rogue.NET.Common.Constant.CharacterSymbol.DefaultCharacterSymbol;
            this.CharacterSymbolCategory = Rogue.NET.Common.Constant.CharacterSymbol.DefaultCharacterCategory;
        }
    }
}
