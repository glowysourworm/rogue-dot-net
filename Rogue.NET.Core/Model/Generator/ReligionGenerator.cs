using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content.Religion;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(IReligionGenerator))]
    public class ReligionGenerator : IReligionGenerator
    {
        readonly ISpellGenerator _spellGenerator;
        readonly IAttackAttributeGenerator _attackAttributeGenerator;
        readonly IAlterationGenerator _alterationGenerator;
        readonly ISkillSetGenerator _skillSetGenerator;

        [ImportingConstructor]
        public ReligionGenerator(
                ISpellGenerator spellGenerator,
                IAttackAttributeGenerator attackAttributeGenerator, 
                IAlterationGenerator alterationGenerator,
                ISkillSetGenerator skillSetGenerator)
        {
            _spellGenerator = spellGenerator;
            _attackAttributeGenerator = attackAttributeGenerator;
            _alterationGenerator = alterationGenerator;
            _skillSetGenerator = skillSetGenerator;
        }

        public Religion GenerateReligion(ReligionTemplate template, IEnumerable<SkillSetTemplate> skillSetTemplates)
        {
            return new Religion()
            {
                AllowsReAffiliation = template.AllowsReAffiliation,
                AllowsRenunciation = template.AllowsRenunciation,
                AttackParameters = new List<ReligiousAffiliationAttackParameters>(template.AttackParameters
                                                                                          .Select(x => new ReligiousAffiliationAttackParameters()
                {
                    AttackMultiplier = x.AttackMultiplier,
                    BlockMultiplier = x.BlockMultiplier,
                    DefenseMultiplier = x.DefenseMultiplier,
                    EnemyReligionName = x.EnemyReligionName
                })),

                // Create an alteration for the attack attribute bonus
                AttackAttributeAlteration = new AlterationEffect()
                {
                    DisplayName = template.Name + " - Attack Attributes",

                    // Don't allow symbol alterations for religion (TBD)
                    IsSymbolAlteration = false,
                    AttackAttributes = new List<AttackAttribute>(template.BonusAttackAttributes.Select(x => _attackAttributeGenerator.GenerateAttackAttribute(x)))
                },
                AttributeAlteration = new AlterationEffect()
                {
                    DisplayName = template.Name + " - Attribute",
                    
                    // Don't allow symbol alterations for religion (TBD)
                    IsSymbolAlteration = false,
                    Agility = template.BonusAttribute == CharacterAttribute.Agility ? template.BonusAttributeValue : 0,
                    Attack = template.BonusAttribute == CharacterAttribute.Attack ? template.BonusAttributeValue : 0,
                    AuraRadius = template.BonusAttribute == CharacterAttribute.LightRadius ? template.BonusAttributeValue : 0,
                    CriticalHit = template.BonusAttribute == CharacterAttribute.CriticalHit ? template.BonusAttributeValue : 0,
                    Defense = template.BonusAttribute == CharacterAttribute.Defense ? template.BonusAttributeValue : 0,
                    DodgeProbability = template.BonusAttribute == CharacterAttribute.Dodge ? template.BonusAttributeValue : 0,
                    FoodUsagePerTurn = template.BonusAttribute == CharacterAttribute.FoodUsagePerTurn ? template.BonusAttributeValue : 0,
                    HpPerStep = template.BonusAttribute == CharacterAttribute.HpRegen ? template.BonusAttributeValue : 0,
                    Intelligence = template.BonusAttribute == CharacterAttribute.Intelligence ? template.BonusAttributeValue : 0,
                    MagicBlockProbability = template.BonusAttribute == CharacterAttribute.MagicBlock ? template.BonusAttributeValue : 0,
                    MpPerStep = template.BonusAttribute == CharacterAttribute.MpRegen ? template.BonusAttributeValue : 0,
                    Speed = template.BonusAttribute == CharacterAttribute.Speed ? template.BonusAttributeValue : 0,
                    Strength = template.BonusAttribute == CharacterAttribute.Strength ? template.BonusAttributeValue : 0,
                },
                CharacterColor = template.SymbolDetails.CharacterColor,
                CharacterSymbol = template.SymbolDetails.CharacterSymbol,
                DisplayIcon = template.SymbolDetails.DisplayIcon,
                FollowerName = template.FollowerName,
                HasAttributeBonus = template.HasAttributeBonus,
                HasBonusAttackAttributes = template.HasBonusAttackAttributes,
                HasBonusSkillSet = template.HasBonusSkillSet,
                Icon = template.SymbolDetails.Icon,
                RenunciationAnimations = new List<AnimationTemplate>(template.RenunciationAnimations),
                RogueName = template.Name,
                SkillSet = template.HasBonusSkillSet ? _skillSetGenerator.GenerateSkillSet(skillSetTemplates.First(x => x.Name == template.BonusSkillSetName)) : new SkillSet(),
                SmileyAuraColor = template.SymbolDetails.SmileyAuraColor,
                SmileyBodyColor = template.SymbolDetails.SmileyBodyColor,
                SmileyLineColor = template.SymbolDetails.SmileyLineColor,
                SmileyMood = template.SymbolDetails.SmileyMood,
                SymbolType = template.SymbolDetails.Type
            };
        }
    }
}
