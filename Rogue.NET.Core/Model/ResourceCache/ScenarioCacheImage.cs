using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;

namespace Rogue.NET.Core.Model.ResourceCache
{
    public class ScenarioCacheImage
    {
        // Scale Relative to the ModelConstant Height / Width
        public double Scale { get; set; }

        // Symbol Details
        public SymbolType Type { get; set; }
        public SmileyExpression SmileyExpression { get; set; }
        public string SmileyBodyColor { get; set; }
        public string SmileyLineColor { get; set; }
        public string SmileyAuraColor { get; set; }
        public string CharacterSymbol { get; set; }
        public string CharacterSymbolCategory { get; set; }
        public string CharacterColor { get; set; }
        public double CharacterScale { get; set; }
        public string Symbol { get; set; }
        public double SymbolHue { get; set; }
        public double SymbolSaturation { get; set; }
        public double SymbolLightness { get; set; }
        public double SymbolScale { get; set; }
        public string GameSymbol { get; set; }

        // Gray-scale flag
        public bool IsGrayScale { get; set; }

        public ScenarioCacheImage(ScenarioImage scenarioImage, bool isGrayScale, double scale)
        {
            this.Scale = scale;

            this.Type = scenarioImage.SymbolType;
            this.SmileyExpression = scenarioImage.SmileyExpression;
            this.SmileyBodyColor = scenarioImage.SmileyBodyColor;
            this.SmileyLineColor = scenarioImage.SmileyLineColor;
            this.SmileyAuraColor = scenarioImage.SmileyLightRadiusColor;
            this.CharacterColor = scenarioImage.CharacterColor;
            this.CharacterSymbol = scenarioImage.CharacterSymbol;
            this.CharacterSymbolCategory = scenarioImage.CharacterSymbolCategory;
            this.CharacterScale = scenarioImage.CharacterScale;
            this.Symbol = scenarioImage.Symbol;
            this.SymbolHue = scenarioImage.SymbolHue;
            this.SymbolLightness = scenarioImage.SymbolLightness;
            this.SymbolSaturation = scenarioImage.SymbolSaturation;
            this.SymbolScale = scenarioImage.SymbolScale;
            this.GameSymbol = scenarioImage.GameSymbol;

            this.IsGrayScale = isGrayScale;
        }

        /// <summary>
        /// Constructor that supports image sources only - no option for black background
        /// </summary>
        public ScenarioCacheImage(SymbolDetailsTemplate symbolDetails, bool grayScale, double scale)
        {
            this.Scale = scale;

            this.Type = symbolDetails.SymbolType;
            this.SmileyExpression = symbolDetails.SmileyExpression;
            this.SmileyBodyColor = symbolDetails.SmileyBodyColor;
            this.SmileyLineColor = symbolDetails.SmileyLineColor;
            this.SmileyAuraColor = symbolDetails.SmileyAuraColor;
            this.CharacterColor = symbolDetails.CharacterColor;
            this.CharacterSymbol = symbolDetails.CharacterSymbol;
            this.CharacterSymbolCategory = symbolDetails.CharacterSymbolCategory;
            this.CharacterScale = symbolDetails.CharacterScale;
            this.Symbol = symbolDetails.Symbol;
            this.SymbolHue = symbolDetails.SymbolHue;
            this.SymbolLightness = symbolDetails.SymbolLightness;
            this.SymbolSaturation = symbolDetails.SymbolSaturation;
            this.SymbolScale = symbolDetails.SymbolScale;
            this.GameSymbol = symbolDetails.GameSymbol;

            this.IsGrayScale = grayScale;
        }
    }
}
