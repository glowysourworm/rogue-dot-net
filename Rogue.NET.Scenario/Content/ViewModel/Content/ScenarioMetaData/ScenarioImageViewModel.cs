using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData
{
    public class ScenarioImageViewModel : RogueBaseViewModel
    {
        #region (private) Backing Fields
        // Base Data
        string _displayName;

        // Symbol Data
        SymbolType _symbolType;
        string _characterSymbol;
        string _characterSymbolCategory;
        string _characterColor;
        SmileyExpression _smileyExpression;
        string _smileyBodyColor;
        string _smileyLineColor;
        string _smileyAuraColor;
        string _symbol;
        double _symbolHue;
        double _symbolSaturation;
        double _symbolLightness;
        #endregion

        #region (public) Properties
        public string DisplayName
        {
            get { return _displayName; }
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
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
        public string SmileyAuraColor
        {
            get { return _smileyAuraColor; }
            set { this.RaiseAndSetIfChanged(ref _smileyAuraColor, value); }
        }
        public string Symbol
        {
            get { return _symbol; }
            set { this.RaiseAndSetIfChanged(ref _symbol, value); }
        }
        public double SymbolHue
        {
            get { return _symbolHue; }
            set { this.RaiseAndSetIfChanged(ref _symbolHue, value); }
        }
        public double SymbolLightness
        {
            get { return _symbolLightness; }
            set { this.RaiseAndSetIfChanged(ref _symbolLightness, value); }
        }
        public double SymbolSaturation
        {
            get { return _symbolSaturation; }
            set { this.RaiseAndSetIfChanged(ref _symbolSaturation, value); }
        }
        public SymbolType SymbolType
        {
            get { return _symbolType; }
            set { this.RaiseAndSetIfChanged(ref _symbolType, value); }
        }
        #endregion

        public void UpdateSymbol(ScenarioImage scenarioImage)
        {
            this.CharacterSymbol = scenarioImage.CharacterSymbol;

            this.CharacterColor = scenarioImage.CharacterColor;
            this.SmileyExpression = scenarioImage.SmileyExpression;
            this.SmileyBodyColor = scenarioImage.SmileyBodyColor;
            this.SmileyLineColor = scenarioImage.SmileyLineColor;
            this.SmileyAuraColor = scenarioImage.SmileyLightRadiusColor;
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

            this.CharacterColor = symbolDetailsTemplate.CharacterColor;
            this.CharacterSymbol = symbolDetailsTemplate.CharacterSymbol;
            this.CharacterSymbolCategory = symbolDetailsTemplate.CharacterSymbolCategory;
            this.SmileyAuraColor = symbolDetailsTemplate.SmileyAuraColor;
            this.SmileyBodyColor = symbolDetailsTemplate.SmileyBodyColor;
            this.SmileyLineColor = symbolDetailsTemplate.SmileyLineColor;
            this.SmileyExpression = symbolDetailsTemplate.SmileyExpression;
            this.Symbol = symbolDetailsTemplate.Symbol;
            this.SymbolHue = symbolDetailsTemplate.SymbolHue;
            this.SymbolLightness = symbolDetailsTemplate.SymbolLightness;
            this.SymbolSaturation = symbolDetailsTemplate.SymbolSaturation;
            this.SymbolType = symbolDetailsTemplate.SymbolType;
        }
    }
}
