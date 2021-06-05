using Rogue.NET.Common.Constant;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model.Enums;

using System;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract
{
    public class SymbolDetailsTemplateViewModel : TemplateViewModel
    {
        private string _symbolPoolCategory;
        private bool _randomize;
        private SymbolType _symbolType;
        private CharacterSymbolSize _symbolSize;

        private SmileyExpression _smileyExpression;
        private string _smileyBodyColor;
        private string _smileyLineColor;

        private CharacterSymbolEffectType _symbolEffectType;
        private string _symbolPath;
        private double _symbolHue;
        private double _symbolSaturation;
        private double _symbolLightness;
        private string _symbolClampColor;
        private string _backgroundColor;

        public string SymbolPoolCategory
        {
            get { return _symbolPoolCategory; }
            set { this.RaiseAndSetIfChanged(ref _symbolPoolCategory, value); }
        }
        public bool Randomize
        {
            get { return _randomize; }
            set { this.RaiseAndSetIfChanged(ref _randomize, value); }
        }
        public SymbolType SymbolType
        {
            get { return _symbolType; }
            set { this.RaiseAndSetIfChanged(ref _symbolType, value); }
        }
        public CharacterSymbolSize SymbolSize
        {
            get { return _symbolSize; }
            set { this.RaiseAndSetIfChanged(ref _symbolSize, value); }
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
        public CharacterSymbolEffectType SymbolEffectType
        {
            get { return _symbolEffectType; }
            set { this.RaiseAndSetIfChanged(ref _symbolEffectType, value); }
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

        public SymbolDetailsTemplateViewModel()
        {
            this.SymbolType = SymbolType.Smiley;
            this.SymbolSize = CharacterSymbolSize.Medium;
            this.SymbolEffectType = CharacterSymbolEffectType.None;

            this.BackgroundColor = ColorOperations.ConvertBack(Colors.Transparent);
            this.SymbolClampColor = ColorOperations.ConvertBack(Colors.White);
            this.SymbolPath = GameSymbol.Identify;

            this.SmileyExpression = SmileyExpression.Happy;
            this.SmileyBodyColor = ColorOperations.ConvertBack(Colors.Yellow);
            this.SmileyLineColor = ColorOperations.ConvertBack(Colors.Black);

            this.SymbolHue = 0;
            this.SymbolLightness = 0;
            this.SymbolSaturation = 0;
        }
    }
}
