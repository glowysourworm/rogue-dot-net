﻿using Rogue.NET.Common.ViewModel;
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
        public ScenarioImageViewModel(ScenarioImage scenarioObject) : base(scenarioObject)
        {
            this.DisplayName = scenarioObject.RogueName;  // TODO - set if identified

            UpdateSymbol(scenarioObject);
        }
        public ScenarioImageViewModel(string id, string rogueName, SymbolDetailsTemplate symbolDetailsTemplate)
            : base (id, rogueName)
        {
            this.DisplayName = rogueName;

            this.CharacterColor = symbolDetailsTemplate.CharacterColor;
            this.CharacterSymbol = symbolDetailsTemplate.CharacterSymbol;
            this.DisplayIcon = symbolDetailsTemplate.DisplayIcon;
            this.Icon = symbolDetailsTemplate.Icon;
            this.SmileyAuraColor = symbolDetailsTemplate.SmileyAuraColor;
            this.SmileyBodyColor = symbolDetailsTemplate.SmileyBodyColor;
            this.SmileyLineColor = symbolDetailsTemplate.SmileyLineColor;
            this.SmileyMood = symbolDetailsTemplate.SmileyMood;
            this.SymbolType = symbolDetailsTemplate.Type;
        }
    }
}