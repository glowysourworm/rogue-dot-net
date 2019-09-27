using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [Export(typeof(IAlteredStateGenerator))]
    public class AlteredStateGenerator : IAlteredStateGenerator
    {
        public AlteredCharacterState GenerateAlteredState(AlteredCharacterStateTemplate template)
        {
            return new AlteredCharacterState()
            {
                RogueName = template.Name,
                BaseType = template.BaseType,
                CharacterColor = template.SymbolDetails.CharacterColor,
                CharacterSymbol = template.SymbolDetails.CharacterSymbol,
                CharacterSymbolCategory = template.SymbolDetails.CharacterSymbolCategory,
                SmileyLightRadiusColor = template.SymbolDetails.SmileyAuraColor,
                SmileyBodyColor = template.SymbolDetails.SmileyBodyColor,
                SmileyLineColor = template.SymbolDetails.SmileyLineColor,
                SmileyExpression = template.SymbolDetails.SmileyExpression,
                Symbol = template.SymbolDetails.Symbol,
                SymbolHue = template.SymbolDetails.SymbolHue,
                SymbolLightness = template.SymbolDetails.SymbolLightness,
                SymbolSaturation = template.SymbolDetails.SymbolSaturation,
                SymbolType = template.SymbolDetails.SymbolType
            };
        }
    }
}
