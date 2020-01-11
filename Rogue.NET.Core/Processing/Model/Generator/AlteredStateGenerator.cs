using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IAlteredStateGenerator))]
    public class AlteredStateGenerator : IAlteredStateGenerator
    {
        readonly ISymbolDetailsGenerator _symbolDetailsGenerator;

        [ImportingConstructor]
        public AlteredStateGenerator(ISymbolDetailsGenerator symbolDetailsGenerator)
        {
            _symbolDetailsGenerator = symbolDetailsGenerator;                 
        }

        public AlteredCharacterState GenerateAlteredState(AlteredCharacterStateTemplate template)
        {
            var alteredState = new AlteredCharacterState()
            {
                RogueName = template.Name,
                BaseType = template.BaseType
            };

            _symbolDetailsGenerator.MapSymbolDetails(template.SymbolDetails, alteredState);

            return alteredState;
        }
    }
}
