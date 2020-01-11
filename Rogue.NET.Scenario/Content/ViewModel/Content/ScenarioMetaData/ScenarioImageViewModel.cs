using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData
{
    public class ScenarioImageViewModel : RogueBaseViewModel
    {
        private string _displayName;

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

        public string DisplayName
        {
            get { return _displayName; }
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
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


        public void UpdateSymbol(ScenarioImage scenarioImage)
        {
            this.BackgroundColor = scenarioImage.BackgroundColor;
            this.SmileyBodyColor = scenarioImage.SmileyBodyColor;
            this.SmileyLineColor = scenarioImage.SmileyLineColor;
            this.SmileyExpression = scenarioImage.SmileyExpression;
            this.SymbolClampColor = scenarioImage.SymbolClampColor;
            this.SymbolEffectType = scenarioImage.SymbolEffectType;
            this.SymbolHue = scenarioImage.SymbolHue;
            this.SymbolLightness = scenarioImage.SymbolLightness;
            this.SymbolPath = scenarioImage.SymbolPath;
            this.SymbolSaturation = scenarioImage.SymbolSaturation;
            this.SymbolSize = scenarioImage.SymbolSize;
            this.SymbolType = scenarioImage.SymbolType;
        }

        public ScenarioImageViewModel() { }
        public ScenarioImageViewModel(ScenarioImage scenarioObject, string displayName) : base(scenarioObject)
        {
            this.DisplayName = displayName;  // TODO - set if identified

            UpdateSymbol(scenarioObject);
        }
        public ScenarioImageViewModel(string id, string rogueName, string displayName, SymbolDetailsTemplate symbolDetailsTemplate)
            : base (id, rogueName)
        {
            this.DisplayName = displayName;

            this.BackgroundColor = symbolDetailsTemplate.BackgroundColor;
            this.SmileyBodyColor = symbolDetailsTemplate.SmileyBodyColor;
            this.SmileyLineColor = symbolDetailsTemplate.SmileyLineColor;
            this.SmileyExpression = symbolDetailsTemplate.SmileyExpression;
            this.SymbolClampColor = symbolDetailsTemplate.SymbolClampColor;
            this.SymbolEffectType = symbolDetailsTemplate.SymbolEffectType;
            this.SymbolHue = symbolDetailsTemplate.SymbolHue;
            this.SymbolLightness = symbolDetailsTemplate.SymbolLightness;
            this.SymbolPath = symbolDetailsTemplate.SymbolPath;
            this.SymbolSaturation = symbolDetailsTemplate.SymbolSaturation;
            this.SymbolSize = symbolDetailsTemplate.SymbolSize;
            this.SymbolType = symbolDetailsTemplate.SymbolType;
        }
    }
}
