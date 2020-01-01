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
        public bool SymbolUseColorMask { get; set; }

        // Game Symbol Details
        public string GameSymbol { get; set; }

        // Static Constructors
        public static ScenarioImage CreateSymbol(string name, string symbol, double hue, double saturation, double lightness, bool useColorMask)
        {
            return new ScenarioImage()
            {
                RogueName = name,
                SymbolType = SymbolType.Symbol,
                Symbol = symbol,
                SymbolHue = hue,
                SymbolSaturation = saturation,
                SymbolLightness = lightness,
                SymbolUseColorMask = useColorMask,
                SymbolScale = 1.0D
            };
        }
        public static ScenarioImage CreateOrientedSymbol(string name, string orientedSymbol, double hue, double saturation, double lightness, bool useColorMask)
        {
            return new ScenarioImage()
            {
                RogueName = name,
                SymbolType = SymbolType.OrientedSymbol,
                Symbol = orientedSymbol,
                SymbolHue = hue,
                SymbolSaturation = saturation,
                SymbolLightness = lightness,
                SymbolUseColorMask = useColorMask,
                SymbolScale = 1.0D
            };
        }
        public static ScenarioImage CreateTerrainSymbol(string name, string symbol, double hue, double saturation, double lightness, bool useColorMask)
        {
            return new ScenarioImage()
            {
                RogueName = name,
                SymbolType = SymbolType.Terrain,
                Symbol = symbol,
                SymbolHue = hue,
                SymbolSaturation = saturation,
                SymbolLightness = lightness,
                SymbolUseColorMask = useColorMask,
                SymbolScale = 1.0D
            };
        }
        public static ScenarioImage CreateGameSymbol(string name, string gameSymbol)
        {
            return new ScenarioImage()
            {
                RogueName = name,
                SymbolType = SymbolType.Game,
                GameSymbol = gameSymbol
            };
        }
        public static ScenarioImage CreateCharacterSymbol(string name, string characterSymbol, string characterSymbolCategory, string characterColor, double characterScale)
        {
            return new ScenarioImage()
            {
                RogueName = name,
                SymbolType = SymbolType.Character,
                CharacterSymbol = characterSymbol,
                CharacterSymbolCategory = characterSymbolCategory,
                CharacterColor = characterColor,
                CharacterScale = characterScale
            };
        }
        public static ScenarioImage CreateSmiley(string name, SmileyExpression expression, string smileyBodyColor, string smileyLineColor, string smileyLightRadiusColor)
        {
            return new ScenarioImage()
            {
                RogueName = name,
                SymbolType = SymbolType.Smiley,
                SmileyExpression = expression,
                SmileyBodyColor= smileyBodyColor,
                SmileyLightRadiusColor = smileyLightRadiusColor,
                SmileyLineColor = smileyLineColor
            };
        }

        public ScenarioImage() { }
        public ScenarioImage(SymbolDetailsTemplate template)
            : base(template.Name)
        {
            this.CharacterColor = template.CharacterColor;
            this.CharacterSymbol = template.CharacterSymbol;
            this.CharacterSymbolCategory = template.CharacterSymbolCategory;
            this.CharacterScale = template.CharacterScale;
            this.GameSymbol = template.GameSymbol;
            this.SmileyBodyColor = template.SmileyBodyColor;
            this.SmileyExpression = template.SmileyExpression;
            this.SmileyLightRadiusColor = template.SmileyAuraColor;
            this.SmileyLineColor = template.SmileyLineColor;
            this.Symbol = template.Symbol;
            this.SymbolHue = template.SymbolHue;
            this.SymbolLightness = template.SymbolLightness;
            this.SymbolSaturation = template.SymbolSaturation;
            this.SymbolScale = template.SymbolScale;
            this.SymbolType = template.SymbolType;
            this.SymbolUseColorMask = template.SymbolUseColorMask;
        }

        /// <summary>
        /// NOTE*** Use this in place of mapper for performance
        /// </summary>
        public static ScenarioImage Copy(ScenarioImage source)
        {
            var dest = new ScenarioImage();

            dest.CharacterColor = source.CharacterColor;
            dest.CharacterSymbol = source.CharacterSymbol;
            dest.CharacterSymbolCategory = source.CharacterSymbolCategory;
            dest.CharacterScale = source.CharacterScale;
            dest.GameSymbol = source.GameSymbol;
            dest.SmileyBodyColor = source.SmileyBodyColor;
            dest.SmileyExpression = source.SmileyExpression;
            dest.SmileyLightRadiusColor = source.SmileyLightRadiusColor;
            dest.SmileyLineColor = source.SmileyLineColor;
            dest.Symbol = source.Symbol;
            dest.SymbolHue = source.SymbolHue;
            dest.SymbolLightness = source.SymbolLightness;
            dest.SymbolSaturation = source.SymbolSaturation;
            dest.SymbolScale = source.SymbolScale;
            dest.SymbolType = source.SymbolType;
            dest.SymbolUseColorMask = source.SymbolUseColorMask;

            return dest;
        }
    }
}
