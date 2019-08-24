using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Generator
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
                Icon = template.SymbolDetails.Icon,
                SmileyLightRadiusColor = template.SymbolDetails.SmileyAuraColor,
                SmileyBodyColor = template.SymbolDetails.SmileyBodyColor,
                SmileyLineColor = template.SymbolDetails.SmileyLineColor,
                SmileyMood = template.SymbolDetails.SmileyMood,
                SymbolType = template.SymbolDetails.Type
            };
        }
    }
}
