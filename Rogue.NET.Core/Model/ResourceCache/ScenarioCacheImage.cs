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
        public SymbolTypes SymbolType { get; set; }
        public SmileyExpression SmileyExpression { get; set; }
        public string SmileyBodyColor { get; set; }
        public string SmileyLineColor { get; set; }
        public string SmileyAuraColor { get; set; }
        public string CharacterSymbol { get; set; }
        public string CharacterColor { get; set; }
        public ImageResources Icon { get; set; }
        public DisplayImageResources DisplayIcon { get; set; }

        // Gray-scale flag
        public bool IsGrayScale { get; set; }

        // Output Format
        public ScenarioCacheImageType OutputType { get; set; }

        public ScenarioCacheImage(ScenarioImage scenarioImage, ScenarioCacheImageType outputType, bool isGrayScale, double scale)
        {
            this.Scale = scale;

            this.SymbolType = scenarioImage.SymbolType;
            this.SmileyExpression = scenarioImage.SmileyExpression;
            this.SmileyBodyColor = scenarioImage.SmileyBodyColor;
            this.SmileyLineColor = scenarioImage.SmileyLineColor;
            this.SmileyAuraColor = scenarioImage.SmileyLightRadiusColor;
            this.CharacterColor = scenarioImage.CharacterColor;
            this.CharacterSymbol = scenarioImage.CharacterSymbol;
            this.Icon = scenarioImage.Icon;
            this.DisplayIcon = scenarioImage.DisplayIcon;

            this.IsGrayScale = isGrayScale;

            this.OutputType = outputType;
        }

        /// <summary>
        /// Constructor that supports image sources only - no option for black background
        /// </summary>
        public ScenarioCacheImage(SymbolDetailsTemplate symbolDetails, bool grayScale, double scale)
        {
            this.Scale = scale;

            this.SymbolType = symbolDetails.Type;
            this.SmileyExpression = symbolDetails.SmileyExpression;
            this.SmileyBodyColor = symbolDetails.SmileyBodyColor;
            this.SmileyLineColor = symbolDetails.SmileyLineColor;
            this.SmileyAuraColor = symbolDetails.SmileyAuraColor;
            this.CharacterColor = symbolDetails.CharacterColor;
            this.CharacterSymbol = symbolDetails.CharacterSymbol;
            this.DisplayIcon = symbolDetails.DisplayIcon;
            this.Icon = symbolDetails.Icon;

            this.IsGrayScale = grayScale;

            this.OutputType = ScenarioCacheImageType.ImageSource;
        }
    }
}
