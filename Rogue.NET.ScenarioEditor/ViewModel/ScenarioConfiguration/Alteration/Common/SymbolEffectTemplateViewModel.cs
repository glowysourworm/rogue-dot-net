using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    public class SymbolEffectTemplateViewModel : TemplateViewModel
    {
        private SymbolType _symbolType;
        private CharacterSymbolEffectType _symbolEffectType;
        private SmileyExpression _smileyExpression;
        private string _smileyBodyColor;
        private string _smileyLineColor;
        private string _symbolPath;
        private double _symbolHue;
        private double _symbolSaturation;
        private double _symbolLightness;
        private string _symbolClampColor;
        private string _backgroundColor;
        private bool _isSmileyExpressionChange;
        private bool _isSmileyBodyColorChange;
        private bool _isSmileyLineColorChange;
        private bool _isSymbolTypeChange;
        private bool _isSymbolPathChange;
        private bool _isBackgroundColorChange;


        public SymbolType SymbolType
        {
            get { return _symbolType; }
            set { this.RaiseAndSetIfChanged(ref _symbolType, value); }
        }
        public CharacterSymbolEffectType SymbolEffectType
        {
            get { return _symbolEffectType; }
            set { this.RaiseAndSetIfChanged(ref _symbolEffectType, value); }
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
        public string SymbolPath
        {
            get { return _symbolPath; }
            set { this.RaiseAndSetIfChanged(ref _symbolPath, value); }
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
        public string SymbolClampColor
        {
            get { return _symbolClampColor; }
            set { this.RaiseAndSetIfChanged(ref _symbolClampColor, value); }
        }
        public string BackgroundColor
        {
            get { return _backgroundColor; }
            set { this.RaiseAndSetIfChanged(ref _backgroundColor, value); }
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
        public bool IsSymbolTypeChange
        {
            get { return _isSymbolTypeChange; }
            set { this.RaiseAndSetIfChanged(ref _isSymbolTypeChange, value); }
        }
        public bool IsSymbolPathChange
        {
            get { return _isSymbolPathChange; }
            set { this.RaiseAndSetIfChanged(ref _isSymbolPathChange, value); }
        }
        public bool IsBackgroundColorChange
        {
            get { return _isBackgroundColorChange; }
            set { this.RaiseAndSetIfChanged(ref _isBackgroundColorChange, value); }
        }

        public bool HasSymbolChange()
        {
            return this.IsSymbolPathChange ||
                   this.IsSymbolTypeChange ||
                   this.IsBackgroundColorChange ||
                   this.IsSmileyBodyColorChange ||
                   this.IsSmileyExpressionChange ||
                   this.IsSmileyLineColorChange ||
                   this.SymbolEffectType != CharacterSymbolEffectType.None;
        }

        public SymbolEffectTemplateViewModel()
        {
            this.SmileyExpression = SmileyExpression.Happy;
            this.SmileyBodyColor = Colors.Yellow.ToString();
            this.SmileyLineColor = Colors.Black.ToString();

            this.SymbolType = SymbolType.Smiley;
            this.BackgroundColor = Colors.Transparent.ToString();
        }
    }
}
