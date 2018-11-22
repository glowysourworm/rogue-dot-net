using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class ScenarioImageViewModel : NotifyViewModel
    {
        #region (private) Backing Fields
        // Base Data
        string _id;
        string _rogueName;
        string _displayName;

        // Symbol Data
        string _characterSymbol;
        string _characterColor;
        ImageResources _icon;
        SmileyMoods _smileyMood;
        string _smileyBodyColor;
        string _smileyLineColor;
        string _smileyAuraColor;
        SymbolTypes _symbolType;
        #endregion

        #region (public) Properties
        public string Id
        {
            get { return _id; }
            private set { this.RaiseAndSetIfChanged(ref _id, value); }
        }
        public string RogueName
        {
            get { return _rogueName; }
            protected set { this.RaiseAndSetIfChanged(ref _rogueName, value); }
        }
        public string DisplayName
        {
            get { return _displayName; }
            protected set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }
        public string CharacterSymbol
        {
            get { return _characterSymbol; }
            set { this.RaiseAndSetIfChanged(ref _characterSymbol, value); }
        }
        public string CharacterColor
        {
            get { return _characterColor; }
            set { this.RaiseAndSetIfChanged(ref _characterColor, value); }
        }
        public ImageResources Icon
        {
            get { return _icon; }
            set { this.RaiseAndSetIfChanged(ref _icon, value); }
        }
        public SmileyMoods SmileyMood
        {
            get { return _smileyMood; }
            set { this.RaiseAndSetIfChanged(ref _smileyMood, value); }
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
        public SymbolTypes SymbolType
        {
            get { return _symbolType; }
            set { this.RaiseAndSetIfChanged(ref _symbolType, value); }
        }
        #endregion

        public ScenarioImageViewModel() { }
        public ScenarioImageViewModel(ScenarioImage scenarioObject)
        {
            this.Id = scenarioObject.Id;
            this.RogueName = scenarioObject.RogueName;
            this.DisplayName = scenarioObject.RogueName;  // TODO - set if identified

            this.CharacterSymbol = scenarioObject.CharacterSymbol;
            this.CharacterColor = scenarioObject.CharacterColor;
            this.Icon = scenarioObject.Icon;
            this.SmileyMood = scenarioObject.SmileyMood;
            this.SmileyBodyColor = scenarioObject.SmileyBodyColor;
            this.SmileyLineColor = scenarioObject.SmileyLineColor;
            this.SmileyAuraColor = scenarioObject.SmileyAuraColor;
            this.SymbolType = scenarioObject.SymbolType;
        }
    }
}
