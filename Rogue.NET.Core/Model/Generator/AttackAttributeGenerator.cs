using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;

using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Model.Generator
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
            attackAttribute.SmileyMood = attackAttributeTemplate.SymbolDetails.SmileyMood;
            attackAttribute.SymbolType = attackAttributeTemplate.SymbolDetails.Type;
            attackAttribute.Attack = _randomSequenceGenerator.GetRandomValue(attackAttributeTemplate.Attack);
            attackAttribute.Resistance = _randomSequenceGenerator.GetRandomValue(attackAttributeTemplate.Resistance);
            attackAttribute.Weakness = _randomSequenceGenerator.GetRandomValue(attackAttributeTemplate.Weakness);

            return attackAttribute;
        }
    }
}
