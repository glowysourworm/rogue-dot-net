using Rogue.NET.Common.Constant;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
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

        public ScenarioImage() 
        {
            this.SymbolType = SymbolType.Game;
            this.SymbolSize = CharacterSymbolSize.Large;
            this.SymbolEffectType = CharacterSymbolEffectType.None;

            this.SmileyBodyColor = ColorOperations.ConvertBack(Colors.Yellow);
            this.SmileyExpression = SmileyExpression.Happy;
            this.SmileyLineColor = ColorOperations.ConvertBack(Colors.Black);

            this.SymbolPath = GameSymbol.Identify;

            this.SymbolHue = 0;
            this.SymbolLightness = 0;
            this.SymbolSaturation = 0;
            this.SymbolClampColor = ColorOperations.ConvertBack(Colors.White);

            this.BackgroundColor = ColorOperations.ConvertBack(Colors.Transparent);
        }

        public ScenarioImage(SymbolDetailsTemplate template)
            : base(template.Name)
        {
            this.SymbolType = template.SymbolType;
            this.SymbolSize = template.SymbolSize;
            this.SymbolEffectType = template.SymbolEffectType;

            this.SmileyBodyColor = template.SmileyBodyColor;
            this.SmileyExpression = template.SmileyExpression;
            this.SmileyLineColor = template.SmileyLineColor;

            this.SymbolPath = template.SymbolPath;

            this.SymbolHue = template.SymbolHue;
            this.SymbolLightness = template.SymbolLightness;
            this.SymbolSaturation = template.SymbolSaturation;
            this.SymbolClampColor = template.SymbolClampColor;

            this.BackgroundColor = template.BackgroundColor;
        }

        public static ScenarioImage CreateGameSymbol(string rogueName, string symbolPath)
        {
            return new ScenarioImage()
            {
                RogueName = rogueName,
                SmileyBodyColor = Colors.Yellow.ToString(),
                SmileyExpression = SmileyExpression.Happy,
                SmileyLineColor = Colors.Black.ToString(),
                SymbolClampColor = Colors.White.ToString(),
                SymbolHue = 0,
                SymbolLightness = 0,
                SymbolSaturation = 0,
                SymbolType = SymbolType.Game,
                SymbolPath = symbolPath,
                SymbolSize = CharacterSymbolSize.Large,
                SymbolEffectType = CharacterSymbolEffectType.None,
                BackgroundColor = Colors.Transparent.ToString()
            };
        }

        /// <summary>
        /// NOTE*** Use this in place of mapper for performance
        /// </summary>
        public static ScenarioImage Copy(ScenarioImage source)
        {
            var dest = new ScenarioImage();

            dest.SymbolType = source.SymbolType;
            dest.SymbolSize = source.SymbolSize;
            dest.SymbolEffectType = source.SymbolEffectType;

            dest.SmileyBodyColor = source.SmileyBodyColor;
            dest.SmileyExpression = source.SmileyExpression;
            dest.SmileyLineColor = source.SmileyLineColor;

            dest.SymbolPath = source.SymbolPath;

            dest.SymbolHue = source.SymbolHue;
            dest.SymbolLightness = source.SymbolLightness;
            dest.SymbolSaturation = source.SymbolSaturation;
            dest.SymbolClampColor = source.SymbolClampColor;

            dest.BackgroundColor = source.BackgroundColor;

            return dest;
        }
    }
}
