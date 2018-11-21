using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;

namespace Rogue.NET.Core.Model.ResourceCache
{
    public class ScenarioCacheImage
    {
        // Symbol Details
        public SymbolTypes SymbolType { get; set; }
        public SmileyMoods SmileyMood { get; set; }
        public string SmileyBodyColor { get; set; }
        public string SmileyLineColor { get; set; }
        public string SmileyAuraColor { get; set; }
        public string CharacterSymbol { get; set; }
        public string CharacterColor { get; set; }
        public ImageResources Icon { get; set; }

        // Black Background Specification
        public bool BlackBackground { get; set; }

        // Output Format
        public ScenarioCacheImageType OutputType { get; set; }

        public ScenarioCacheImage(ScenarioImage scenarioImage, bool blackBackground, ScenarioCacheImageType outputType)
        {
            this.SymbolType = scenarioImage.SymbolType;
            this.SmileyMood = scenarioImage.SmileyMood;
            this.SmileyBodyColor = scenarioImage.SmileyBodyColor;
            this.SmileyLineColor = scenarioImage.SmileyLineColor;
            this.SmileyAuraColor = scenarioImage.SmileyAuraColor;
            this.CharacterColor = scenarioImage.CharacterColor;
            this.CharacterSymbol = scenarioImage.CharacterSymbol;
            this.Icon = scenarioImage.Icon;

            this.BlackBackground = blackBackground;

            this.OutputType = outputType;
        }

        /// <summary>
        /// Constructor that supports image sources only - no option for black background
        /// </summary>
        public ScenarioCacheImage(SymbolDetailsTemplate symbolDetails)
        {
            this.SymbolType = symbolDetails.Type;
            this.SmileyMood = symbolDetails.SmileyMood;
            this.SmileyBodyColor = symbolDetails.SmileyBodyColor;
            this.SmileyLineColor = symbolDetails.SmileyLineColor;
            this.SmileyAuraColor = symbolDetails.SmileyAuraColor;
            this.CharacterColor = symbolDetails.CharacterColor;
            this.CharacterSymbol = symbolDetails.CharacterSymbol;
            this.Icon = symbolDetails.Icon;

            this.BlackBackground = false;

            this.OutputType = ScenarioCacheImageType.ImageSource;
        }
    }
}
