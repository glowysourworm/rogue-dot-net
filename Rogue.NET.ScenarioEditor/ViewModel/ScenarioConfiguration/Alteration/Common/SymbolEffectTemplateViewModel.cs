using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    public class SymbolEffectTemplateViewModel : TemplateViewModel
    {
        private SmileyExpression _smileyExpression;
        private string _smileyBodyColor;
        private string _smileyLineColor;
        private string _characterSymbol;
        private string _characterSymbolCategory;
        private string _characterColor;
        private SymbolDetailsTemplateViewModel _fullSymbolChangeDetails;
        private bool _isFullSymbolChange;
        private bool _isSmileyExpressionChange;
        private bool _isSmileyBodyColorChange;
        private bool _isSmileyLineColorChange;
        private bool _isCharacterSymbolChange;
        private bool _isCharacterColorChange;
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
        public bool IsFullSymbolChange
        {
            get { return _isFullSymbolChange; }
            set { this.RaiseAndSetIfChanged(ref _isFullSymbolChange, value); }
        }
        public bool IsSmileyExpressionChange
        {
            get { return _isSmileyExpressionChange; }
            set { this.RaiseAndSetIfChanged(ref _isSmileyExpressionChange, value); }
        }
        public bool IsSmileyBodyColorChange
        {
            get { return _isSmileyBodyColorChange; }
            set { this.RaiseAndSetIfChanged(ref _isSmileyBodyColorChange, value); }
        }
        public bool IsSmileyLineColorChange
        {
            get { return _isSmileyLineColorChange; }
            set { this.RaiseAndSetIfChanged(ref _isSmileyLineColorChange, value); }
        }
        public bool IsCharacterSymbolChange
        {
            get { return _isCharacterSymbolChange; }
            set { this.RaiseAndSetIfChanged(ref _isCharacterSymbolChange, value); }
        }
        public bool IsCharacterColorChange
        {
            get { return _isCharacterColorChange; }
            set { this.RaiseAndSetIfChanged(ref _isCharacterColorChange, value); }
        }
        public SymbolDetailsTemplateViewModel FullSymbolChangeDetails
        {
            get { return _fullSymbolChangeDetails; }
            set { this.RaiseAndSetIfChanged(ref _fullSymbolChangeDetails, value); }
        }
        public SymbolEffectTemplateViewModel()
        {
            this.FullSymbolChangeDetails = new SymbolDetailsTemplateViewModel();

            this.SmileyExpression = SmileyExpression.Happy;
            this.SmileyBodyColor = Colors.Yellow.ToString();
            this.SmileyLineColor = Colors.Black.ToString();

            this.CharacterColor = Colors.White.ToString();
            this.CharacterSymbol = Rogue.NET.Common.Constant.CharacterSymbol.DefaultCharacterSymbol;
            this.CharacterSymbolCategory = Rogue.NET.Common.Constant.CharacterSymbol.DefaultCharacterCategory;
        }
    }
}
