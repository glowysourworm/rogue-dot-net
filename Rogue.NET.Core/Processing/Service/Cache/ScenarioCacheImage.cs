using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System.Collections.Generic;

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
        public Light[] Lighting { get; set; }

        // Gray-scale flag
        public bool IsGrayScale { get; set; }

        //public override int GetHashCode()
        //{
        //    var hash = 17;

        //    hash = (hash * 397) + this.Scale.GetHashCode();
        //    hash = (hash * 397) + this.Type.GetHashCode();
        //    hash = (hash * 397) + this.SmileyExpression.GetHashCode();
        //    hash = (hash * 397) + this.SmileyBodyColor?.GetHashCode() ?? 0;
        //    hash = (hash * 397) + this.SmileyLineColor?.GetHashCode() ?? 0;
        //    hash = (hash * 397) + this.CharacterSymbol?.GetHashCode() ?? 0;
        //    hash = (hash * 397) + this.CharacterSymbolCategory?.GetHashCode() ?? 0;
        //    hash = (hash * 397) + this.CharacterColor?.GetHashCode() ?? 0;
        //    hash = (hash * 397) + this.CharacterScale.GetHashCode();
        //    hash = (hash * 397) + this.Symbol?.GetHashCode() ?? 0;
        //    hash = (hash * 397) + this.SymbolHue.GetHashCode();
        //    hash = (hash * 397) + this.SymbolSaturation.GetHashCode();
        //    hash = (hash * 397) + this.SymbolLightness.GetHashCode();
        //    hash = (hash * 397) + this.SymbolScale.GetHashCode();
        //    hash = (hash * 397) + this.SymbolUseColorMask.GetHashCode();
        //    hash = (hash * 397) + this.GameSymbol?.GetHashCode() ?? 0;
        //    hash = (hash * 397) + this.IsGrayScale.GetHashCode();

        //    for (int i=0;i<this.Lighting.Length;i++)
        //        hash = (hash * 397) + this.Lighting[i].GetHashCode();
                
        //    return hash;
        //}

        /// <summary>
        /// Creates a static hash key for the cache image properties
        /// </summary>
        public static int CreateHash(double scale, 
                                     SymbolType type, 
                                     SmileyExpression smileyExpression, 
                                     string smileyBodyColor,
                                     string smileyLineColor,
                                     string characterSymbol,
                                     string characterSymbolCategory,
                                     string characterColor,
                                     double characterScale,
                                     string symbol,
                                     double symbolHue,
                                     double symbolSaturation,
                                     double symbolLightness,
                                     double symbolScale,
                                     bool symbolUseColorMask,
                                     string gameSymbol,
                                     bool isGrayScale,
                                     Light[] lighting)
        {
            var hash = 17;

            hash = (hash * 397) + scale.GetHashCode();
            hash = (hash * 397) + type.GetHashCode();
            hash = (hash * 397) + smileyExpression.GetHashCode();
            hash = (hash * 397) + smileyBodyColor?.GetHashCode() ?? 0;
            hash = (hash * 397) + smileyLineColor?.GetHashCode() ?? 0;
            hash = (hash * 397) + characterSymbol?.GetHashCode() ?? 0;
            hash = (hash * 397) + characterSymbolCategory?.GetHashCode() ?? 0;
            hash = (hash * 397) + characterColor?.GetHashCode() ?? 0;
            hash = (hash * 397) + characterScale.GetHashCode();
            hash = (hash * 397) + symbol?.GetHashCode() ?? 0;
            hash = (hash * 397) + symbolHue.GetHashCode();
            hash = (hash * 397) + symbolSaturation.GetHashCode();
            hash = (hash * 397) + symbolLightness.GetHashCode();
            hash = (hash * 397) + symbolScale.GetHashCode();
            hash = (hash * 397) + symbolUseColorMask.GetHashCode();
            hash = (hash * 397) + gameSymbol?.GetHashCode() ?? 0;
            hash = (hash * 397) + isGrayScale.GetHashCode();

            for (int i = 0; i < lighting.Length; i++)
                hash = (hash * 397) + lighting[i].GetHashCode();

            return hash;
        }

        public ScenarioCacheImage(ScenarioImage scenarioImage, bool isGrayScale, double scale, Light[] lighting)
        {
            this.Scale = scale;

            this.Type = scenarioImage.SymbolType;
            this.SmileyExpression = scenarioImage.SmileyExpression;
            this.SmileyBodyColor = scenarioImage.SmileyBodyColor;
            this.SmileyLineColor = scenarioImage.SmileyLineColor;
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
            this.Lighting = lighting;
            this.IsGrayScale = isGrayScale;
        }

        /// <summary>
        /// Constructor that supports image sources only - no option for black background
        /// </summary>
        public ScenarioCacheImage(SymbolDetailsTemplate symbolDetails, bool grayScale, double scale, Light[] lighting)
        {
            this.Scale = scale;

            this.Type = symbolDetails.SymbolType;
            this.SmileyExpression = symbolDetails.SmileyExpression;
            this.SmileyBodyColor = symbolDetails.SmileyBodyColor;
            this.SmileyLineColor = symbolDetails.SmileyLineColor;
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
            this.Lighting = lighting;
            this.IsGrayScale = grayScale;
        }
    }
}
