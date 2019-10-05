using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [Export(typeof(IAttackAttributeGenerator))]
    public class AttackAttributeGenerator : IAttackAttributeGenerator
    {
        private readonly IRandomSequenceGenerator _randomSequenceGenerator;
        private readonly ISymbolDetailsGenerator _symbolDetailsGenerator;

        [ImportingConstructor]
        public AttackAttributeGenerator(IRandomSequenceGenerator randomSequenceGenerator, ISymbolDetailsGenerator symbolDetailsGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _symbolDetailsGenerator = symbolDetailsGenerator;
        }

        public AttackAttribute GenerateAttackAttribute(AttackAttributeTemplate attackAttributeTemplate)
        {
            var attackAttribute = new AttackAttribute();

            attackAttribute.RogueName = attackAttributeTemplate.Name;

            // Map symbol details
            _symbolDetailsGenerator.MapSymbolDetails(attackAttributeTemplate.SymbolDetails, attackAttribute);

            attackAttribute.Attack = _randomSequenceGenerator.GetRandomValue(attackAttributeTemplate.Attack);
            attackAttribute.Resistance = _randomSequenceGenerator.GetRandomValue(attackAttributeTemplate.Resistance);
            attackAttribute.Weakness = _randomSequenceGenerator.GetRandomValue(attackAttributeTemplate.Weakness);
            attackAttribute.Immune = attackAttributeTemplate.Immune;

            return attackAttribute;
        }
    }
}
