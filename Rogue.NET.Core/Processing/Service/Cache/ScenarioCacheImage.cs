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
        public SymbolType SymbolType { get; set; }
        public CharacterSymbolSize SymbolSize { get; set; }
        public CharacterSymbolEffectType SymbolEffectType { get; set; }
        public SmileyExpression SmileyExpression { get; set; }
        public string SmileyBodyColor { get; set; }
        public string SmileyLineColor { get; set; }
        public string SymbolPath { get; set; }
        public double SymbolHue { get; set; }
        public double SymbolSaturation { get; set; }
        public double SymbolLightness { get; set; }
        public string SymbolClampColor { get; set; }
        public string BackgroundColor { get; set; }

        // Lighting
        public Light[] Lighting { get; set; }

        // Vision
        public double EffectiveVision { get; set; }

        // Gray-scale flag
        public bool IsGrayScale { get; set; }

        /// <summary>
        /// Creates a static hash key for the cache image properties
        /// </summary>
        public static int CreateHash(double scale,
                                     SymbolType symbolType,
                                     CharacterSymbolSize symbolSize,
                                     CharacterSymbolEffectType symbolEffectType,
                                     SmileyExpression smileyExpression,
                                     string smileyBodyColor,
                                     string smileyLineColor,
                                     string symbolPath,
                                     double symbolHue,
                                     double symbolSaturation,
                                     double symbolLightness,
                                     string symbolClampColor,
                                     string backgroundColor,
                                     bool isGrayScale,
                                     double effectiveVision,
                                     Light[] lighting)
        {
            var hash = 17;

            hash = (hash * 397) ^ scale.GetHashCode();
            hash = (hash * 397) ^ symbolType.GetHashCode();
            hash = (hash * 397) ^ symbolSize.GetHashCode();
            hash = (hash * 397) ^ symbolEffectType.GetHashCode();
            hash = (hash * 397) ^ smileyExpression.GetHashCode();
            hash = (hash * 397) ^ (smileyBodyColor?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (smileyLineColor?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (symbolPath?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ symbolHue.GetHashCode();
            hash = (hash * 397) ^ symbolSaturation.GetHashCode();
            hash = (hash * 397) ^ symbolLightness.GetHashCode();
            hash = (hash * 397) ^ (symbolClampColor?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ (backgroundColor?.GetHashCode() ?? 0);
            hash = (hash * 397) ^ isGrayScale.GetHashCode();
            hash = (hash * 397) ^ effectiveVision.GetHashCode();

            for (int i = 0; i < lighting.Length; i++)
                hash = (hash * 397) ^ lighting[i].GetHashCode();

            return hash;
        }

        public ScenarioCacheImage(ScenarioImage scenarioImage, bool isGrayScale, double scale, double effectiveVision, Light[] lighting)
        {
            this.Scale = scale;

            this.SymbolType = scenarioImage.SymbolType;
            this.SymbolSize = scenarioImage.SymbolSize;
            this.SymbolEffectType = scenarioImage.SymbolEffectType;
            this.SmileyExpression = scenarioImage.SmileyExpression;
            this.SmileyBodyColor = scenarioImage.SmileyBodyColor;
            this.SmileyLineColor = scenarioImage.SmileyLineColor;
            this.SymbolPath = scenarioImage.SymbolPath;
            this.SymbolHue = scenarioImage.SymbolHue;
            this.SymbolSaturation = scenarioImage.SymbolSaturation;
            this.SymbolLightness = scenarioImage.SymbolLightness;
            this.SymbolClampColor = scenarioImage.SymbolClampColor;
            this.BackgroundColor = scenarioImage.BackgroundColor;

            this.Lighting = lighting;
            this.EffectiveVision = effectiveVision;
            this.IsGrayScale = isGrayScale;
        }

        /// <summary>
        /// Constructor that supports image sources only - no option for black background
        /// </summary>
        public ScenarioCacheImage(SymbolDetailsTemplate symbolDetails, bool isGrayScale, double scale, double effectiveVision, Light[] lighting)
        {
            this.Scale = scale;

            this.SymbolType = symbolDetails.SymbolType;
            this.SymbolSize = symbolDetails.SymbolSize;
            this.SymbolEffectType = symbolDetails.SymbolEffectType;
            this.SmileyExpression = symbolDetails.SmileyExpression;
            this.SmileyBodyColor = symbolDetails.SmileyBodyColor;
            this.SmileyLineColor = symbolDetails.SmileyLineColor;
            this.SymbolPath = symbolDetails.SymbolPath;
            this.SymbolHue = symbolDetails.SymbolHue;
            this.SymbolSaturation = symbolDetails.SymbolSaturation;
            this.SymbolLightness = symbolDetails.SymbolLightness;
            this.SymbolClampColor = symbolDetails.SymbolClampColor;
            this.BackgroundColor = symbolDetails.BackgroundColor;

            this.Lighting = lighting;
            this.EffectiveVision = effectiveVision;
            this.IsGrayScale = isGrayScale;
        }
    }
}
