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
        double _characterScale;
        SmileyExpression _smileyExpression;
        string _smileyBodyColor;
        string _smileyLineColor;
        string _smileyAuraColor;
        string _symbol;
        double _symbolHue;
        double _symbolSaturation;
        double _symbolLightness;
        double _symbolScale;
        string _gameSymbol;
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
        public double CharacterScale
        {
            get { return _characterScale; }
            set { this.RaiseAndSetIfChanged(ref _characterScale, value); }
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
        public double SymbolScale
        {
            get { return _symbolScale; }
            set { this.RaiseAndSetIfChanged(ref _symbolScale, value); }
        }
        public string GameSymbol
        {
            get { return _gameSymbol; }
            set { this.RaiseAndSetIfChanged(ref _gameSymbol, value); }
        }
        public SymbolType SymbolType
        {
            get { return _symbolType; }
            set { this.RaiseAndSetIfChanged(ref _symbolType, value); }
        }
        #endregion

        public void UpdateSymbol(ScenarioImage scenarioImage)
        {

            this.CharacterColor = scenarioImage.CharacterColor;
            this.CharacterSymbol = scenarioImage.CharacterSymbol;
            this.CharacterSymbolCategory = scenarioImage.CharacterSymbolCategory;
            this.CharacterScale = scenarioImage.CharacterScale;

            this.SmileyAuraColor = scenarioImage.SmileyLightRadiusColor;
            this.SmileyBodyColor = scenarioImage.SmileyBodyColor;
            this.SmileyLineColor = scenarioImage.SmileyLineColor;
            this.SmileyExpression = scenarioImage.SmileyExpression;

            this.Symbol = scenarioImage.Symbol;
            this.SymbolHue = scenarioImage.SymbolHue;
            this.SymbolLightness = scenarioImage.SymbolLightness;
            this.SymbolSaturation = scenarioImage.SymbolSaturation;
            this.SymbolScale = scenarioImage.SymbolScale;

            this.GameSymbol = scenarioImage.GameSymbol;

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
            this.CharacterScale = symbolDetailsTemplate.CharacterScale;
            
            this.SmileyAuraColor = symbolDetailsTemplate.SmileyAuraColor;
            this.SmileyBodyColor = symbolDetailsTemplate.SmileyBodyColor;
            this.SmileyLineColor = symbolDetailsTemplate.SmileyLineColor;
            this.SmileyExpression = symbolDetailsTemplate.SmileyExpression;

            this.Symbol = symbolDetailsTemplate.Symbol;
            this.SymbolHue = symbolDetailsTemplate.SymbolHue;
            this.SymbolLightness = symbolDetailsTemplate.SymbolLightness;
            this.SymbolSaturation = symbolDetailsTemplate.SymbolSaturation;
            this.SymbolScale = symbolDetailsTemplate.SymbolScale;

            this.GameSymbol = symbolDetailsTemplate.GameSymbol;

            this.SymbolType = symbolDetailsTemplate.SymbolType;
        }
    }
}
