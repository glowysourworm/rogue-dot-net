using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.Scenario.Content
{
    [Serializable]
    public class ScenarioImage : RogueBase
    {
        public SymbolTypes SymbolType { get; set; }

        //Smiley Details
        public SmileyExpression SmileyExpression { get; set; }
        public string SmileyBodyColor { get; set; }
        public string SmileyLineColor { get; set; }
        public string SmileyLightRadiusColor { get; set; }

        //Character Details
        public string CharacterSymbol { get; set; }
        public string CharacterColor { get; set; }

        //Image Details
        public ImageResources Icon { get; set; }
        public DisplayImageResources DisplayIcon { get; set; }

        public ScenarioImage() { }
        public ScenarioImage(string name, ImageResources icon)
            : base(name)
        {
            this.Icon = icon;

            this.SymbolType = SymbolTypes.Image;
        }
        public ScenarioImage(string name, DisplayImageResources displayIcon)
            : base(name)
        {
            this.DisplayIcon = displayIcon;

            this.SymbolType = SymbolTypes.DisplayImage;
        }
        public ScenarioImage(string name, string characterSymbol, string characterColor)
            : base(name)
        {
            this.CharacterColor = characterColor;
            this.CharacterSymbol = characterSymbol;

            this.SymbolType = SymbolTypes.Character;
        }
        public ScenarioImage(string name, SmileyExpression expression, string smileyBodyColor, string smileyLineColor, string smileyAuraColor)
            : base(name)
        {
            this.SmileyExpression = expression;
            this.SmileyLightRadiusColor = smileyAuraColor;
            this.SmileyBodyColor = smileyBodyColor;
            this.SmileyLineColor = smileyLineColor;

            this.SymbolType = SymbolTypes.Smiley;
        }
        public ScenarioImage(SymbolDetailsTemplate template)
            : base(template.Name)
        {
            this.CharacterColor = template.CharacterColor;
            this.CharacterSymbol = template.CharacterSymbol;
            this.DisplayIcon = template.DisplayIcon;
            this.Icon = template.Icon;
            this.SmileyBodyColor = template.SmileyBodyColor;
            this.SmileyExpression = template.SmileyExpression;
            this.SmileyLightRadiusColor = template.SmileyAuraColor;
            this.SmileyLineColor = template.SmileyLineColor;
            this.SymbolType = template.Type;
        }
    }
}
