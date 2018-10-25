﻿using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Model.Scenario.Content
{
    public class ScenarioImage : RogueBase
    {
        public SymbolTypes SymbolType { get; set; }

        //Smiley Details
        public SmileyMoods SmileyMood { get; set; }
        public string SmileyBodyColor { get; set; }
        public string SmileyLineColor { get; set; }
        public string SmileyAuraColor { get; set; }

        //Character Details
        public string CharacterSymbol { get; set; }
        public string CharacterColor { get; set; }

        //Image Details
        public ImageResources Icon { get; set; }

        public ScenarioImage() { }
        public ScenarioImage(string name, ImageResources icon)
            : base(name)
        {
            this.Icon = icon;

            this.SymbolType = SymbolTypes.Image;
        }
        public ScenarioImage(string name, string characterSymbol, string characterColor)
            : base(name)
        {
            this.CharacterColor = characterColor;
            this.CharacterSymbol = characterSymbol;

            this.SymbolType = SymbolTypes.Character;
        }
        public ScenarioImage(string name, SmileyMoods mood, string smileyBodyColor, string smileyLineColor, string smileyAuraColor)
            : base(name)
        {
            this.SmileyMood = mood;
            this.SmileyAuraColor = smileyAuraColor;
            this.SmileyBodyColor = smileyBodyColor;
            this.SmileyLineColor = smileyLineColor;

            this.SymbolType = SymbolTypes.Smiley;
        }
    }
}
