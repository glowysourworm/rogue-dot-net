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
        DisplayImageResources _displayIcon;
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
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
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
        public DisplayImageResources DisplayIcon
        {
            get { return _displayIcon; }
            set { this.RaiseAndSetIfChanged(ref _displayIcon, value); }
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

        public void UpdateSymbol(ScenarioImage scenarioImage)
        {
            this.CharacterSymbol = scenarioImage.CharacterSymbol;
            this.CharacterColor = scenarioImage.CharacterColor;
            this.Icon = scenarioImage.Icon;
            this.DisplayIcon = scenarioImage.DisplayIcon;
            this.SmileyMood = scenarioImage.SmileyMood;
            this.SmileyBodyColor = scenarioImage.SmileyBodyColor;
            this.SmileyLineColor = scenarioImage.SmileyLineColor;
            this.SmileyAuraColor = scenarioImage.SmileyAuraColor;
            this.SymbolType = scenarioImage.SymbolType;
        }

        public ScenarioImageViewModel() { }
        public ScenarioImageViewModel(ScenarioImage scenarioObject)
        {
            this.Id = scenarioObject.Id;
            this.RogueName = scenarioObject.RogueName;
            this.DisplayName = scenarioObject.RogueName;  // TODO - set if identified

            UpdateSymbol(scenarioObject);
        }
    }
}
