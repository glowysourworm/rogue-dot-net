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

        [ImportingConstructor]
        public AttackAttributeGenerator(IRandomSequenceGenerator randomSequenceGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public AttackAttribute GenerateAttackAttribute(AttackAttributeTemplate attackAttributeTemplate)
        {
            var attackAttribute = new AttackAttribute();

            attackAttribute.RogueName = attackAttributeTemplate.Name;
            attackAttribute.CharacterColor = attackAttributeTemplate.SymbolDetails.CharacterColor;
            attackAttribute.CharacterSymbol = attackAttributeTemplate.SymbolDetails.CharacterSymbol;
            attackAttribute.Icon = attackAttributeTemplate.SymbolDetails.Icon;
            attackAttribute.SmileyLightRadiusColor = attackAttributeTemplate.SymbolDetails.SmileyAuraColor;
            attackAttribute.SmileyBodyColor = attackAttributeTemplate.SymbolDetails.SmileyBodyColor;
            attackAttribute.SmileyLineColor = attackAttributeTemplate.SymbolDetails.SmileyLineColor;
            attackAttribute.SmileyExpression = attackAttributeTemplate.SymbolDetails.SmileyExpression;
            attackAttribute.SymbolType = attackAttributeTemplate.SymbolDetails.Type;
            attackAttribute.Attack = _randomSequenceGenerator.GetRandomValue(attackAttributeTemplate.Attack);
            attackAttribute.Resistance = _randomSequenceGenerator.GetRandomValue(attackAttributeTemplate.Resistance);
            attackAttribute.Weakness = _randomSequenceGenerator.GetRandomValue(attackAttributeTemplate.Weakness);

            return attackAttribute;
        }
    }
}
