using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;

namespace Rogue.NET.Core.Processing.Service.Cache
{
    public struct ScenarioCacheImage
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
        public bool SymbolUseColorMask { get; set; }
        public string GameSymbol { get; set; }

        // Lighting
        public byte LightRed { get; set; }
        public byte LightGreen { get; set; }
        public byte LightBlue { get; set; }
        public double LightIntensity { get; set; }

        // Gray-scale flag
        public bool IsGrayScale { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ScenarioCacheImage)
            {
                var image = (ScenarioCacheImage)obj;

                return image.CharacterColor == this.CharacterColor &&
                       image.CharacterScale == this.CharacterScale &&
                       image.CharacterSymbol == this.CharacterSymbol &&
                       image.CharacterSymbolCategory == this.CharacterSymbolCategory &&
                       image.GameSymbol == this.GameSymbol &&
                       image.IsGrayScale == this.IsGrayScale &&
                       image.LightBlue == this.LightBlue &&
                       image.LightGreen == this.LightGreen &&
                       image.LightIntensity == this.LightIntensity &&
                       image.LightRed == this.LightRed &&
                       image.Scale == this.Scale &&
                       image.SmileyAuraColor == this.SmileyAuraColor &&
                       image.SmileyBodyColor == this.SmileyBodyColor &&
                       image.SmileyExpression == this.SmileyExpression &&
                       image.SmileyLineColor == this.SmileyLineColor &&
                       image.Symbol == this.Symbol &&
                       image.SymbolHue == this.SymbolHue &&
                       image.SymbolLightness == this.SymbolLightness &&
                       image.SymbolSaturation == this.SymbolSaturation &&
                       image.SymbolScale == this.SymbolScale &&
                       image.SymbolUseColorMask == this.SymbolUseColorMask &&
                       image.Type == this.Type;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Scale.GetHashCode() +
                   this.Type.GetHashCode() +
                   this.SmileyExpression.GetHashCode() +
                   this.SmileyBodyColor?.GetHashCode() ?? 0 +
                   this.SmileyLineColor?.GetHashCode() ?? 0 +
                   this.SmileyAuraColor?.GetHashCode() ?? 0 +
                   this.CharacterSymbol?.GetHashCode() ?? 0 +
                   this.CharacterSymbolCategory?.GetHashCode() ?? 0+
                   this.CharacterColor?.GetHashCode() ?? 0 +
                   this.CharacterScale.GetHashCode() +
                   this.Symbol?.GetHashCode() ?? 0 +
                   this.SymbolHue.GetHashCode() +
                   this.SymbolSaturation.GetHashCode() +
                   this.SymbolLightness.GetHashCode() +
                   this.SymbolScale.GetHashCode() +
                   this.SymbolUseColorMask.GetHashCode() +
                   this.GameSymbol?.GetHashCode() ?? 0 +
                   this.LightRed.GetHashCode() +
                   this.LightGreen.GetHashCode() +
                   this.LightBlue.GetHashCode() +
                   this.LightIntensity.GetHashCode() +
                   this.IsGrayScale.GetHashCode();
        }

        public ScenarioCacheImage(ScenarioImage scenarioImage, bool isGrayScale, double scale, Light lighting)
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
            this.SymbolUseColorMask = scenarioImage.SymbolUseColorMask;
            this.GameSymbol = scenarioImage.GameSymbol;

            this.LightRed = lighting.Red;
            this.LightGreen = lighting.Green;
            this.LightBlue = lighting.Blue;
            this.LightIntensity = lighting.Intensity;

            this.IsGrayScale = isGrayScale;
        }

        /// <summary>
        /// Constructor that supports image sources only - no option for black background
        /// </summary>
        public ScenarioCacheImage(SymbolDetailsTemplate symbolDetails, bool grayScale, double scale, Light lighting)
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
            this.SymbolUseColorMask = symbolDetails.SymbolUseColorMask;
            this.GameSymbol = symbolDetails.GameSymbol;

            this.LightRed = lighting.Red;
            this.LightGreen = lighting.Green;
            this.LightBlue = lighting.Blue;
            this.LightIntensity = lighting.Intensity;

            this.IsGrayScale = grayScale;
        }
    }
}
