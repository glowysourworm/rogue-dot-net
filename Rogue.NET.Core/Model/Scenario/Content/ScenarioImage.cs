using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Windows.Media;

namespace Rogue.NET.Core.Model.Scenario.Content
{
    [Serializable]
    public class ScenarioImage : RogueBase
    {
        public SymbolType SymbolType { get; set; }

        //Smiley Details
        public SmileyExpression SmileyExpression { get; set; }
        public string SmileyBodyColor { get; set; }
        public string SmileyLineColor { get; set; }
        public string SmileyLightRadiusColor { get; set; }

        //Character Details
        public string CharacterSymbol { get; set; }
        public string CharacterSymbolCategory { get; set; }
        public string CharacterColor { get; set; }
        public double CharacterScale { get; set; }

        // Symbol Details
        public string Symbol { get; set; }
        public double SymbolHue { get; set; }
        public double SymbolSaturation { get; set; }
        public double SymbolLightness { get; set; }
        public double SymbolScale { get; set; }

        // Game Symbol Details
        public string GameSymbol { get; set; }
        public ScenarioImage() { }
        public ScenarioImage(string name, string symbol, double symbolHue, double symbolSaturation, double symbolLightness)
            : base(name)
        {
            this.SymbolType = SymbolType.Symbol;
            this.Symbol = symbol;
            this.SymbolHue = symbolHue;
            this.SymbolSaturation = symbolSaturation;
            this.SymbolLightness = symbolLightness;

            this.SymbolScale = 1.0;
        }
        public ScenarioImage(string name, string gameSymbol)
            : base(name)
        {
            this.SymbolType = SymbolType.Game;
            this.GameSymbol = gameSymbol;
        }
        public ScenarioImage(string name, string characterSymbol, string characterSymbolCategory, string characterColor, double characterScale)
            : base(name)
        {
            this.SymbolType = SymbolType.Character;
            this.CharacterColor = characterColor;
            this.CharacterSymbol = characterSymbol;
            this.CharacterSymbolCategory = characterSymbolCategory;
            this.CharacterScale = characterScale;
        }
        public ScenarioImage(string name, SmileyExpression expression, string smileyBodyColor, string smileyLineColor, string smileyAuraColor)
            : base(name)
        {
            this.SymbolType = SymbolType.Smiley;
            this.SmileyExpression = expression;
            this.SmileyLightRadiusColor = smileyAuraColor;
            this.SmileyBodyColor = smileyBodyColor;
            this.SmileyLineColor = smileyLineColor;
        }
        public ScenarioImage(SymbolDetailsTemplate template)
            : base(template.Name)
        {
            this.CharacterColor = template.CharacterColor;
            this.CharacterSymbol = template.CharacterSymbol;
            this.CharacterSymbolCategory = template.CharacterSymbolCategory;
            this.CharacterScale = template.CharacterScale;
            this.SmileyBodyColor = template.SmileyBodyColor;
            this.SmileyExpression = template.SmileyExpression;
            this.SmileyLightRadiusColor = template.SmileyAuraColor;
            this.SmileyLineColor = template.SmileyLineColor;
            this.Symbol = template.Symbol;
            this.SymbolHue = template.SymbolHue;
            this.SymbolLightness = template.SymbolLightness;
            this.SymbolSaturation = template.SymbolSaturation;
            this.SymbolHue = template.SymbolHue;
            this.SymbolScale = template.SymbolScale;
            this.SymbolType = template.SymbolType;
        }
    }
}
