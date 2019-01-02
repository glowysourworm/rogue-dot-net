using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;

using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(ISpellGenerator))]
    public class SpellGenerator : ISpellGenerator
    {
        public SpellGenerator() { }

        public Spell GenerateSpell(SpellTemplate spellTemplate)
        {
            Spell spell = new Spell();
            spell.RogueName = spellTemplate.Name;
            spell.Cost = spellTemplate.Cost;
            spell.Effect = spellTemplate.Effect;
            spell.AuraEffect = spellTemplate.AuraEffect;
            spell.Type = spellTemplate.Type;
            spell.OtherEffectType = spellTemplate.OtherEffectType;
            spell.AttackAttributeType = spellTemplate.AttackAttributeType;
            spell.EffectRange = spellTemplate.EffectRange;
            spell.Animations = spellTemplate.Animations;
            spell.CreateMonsterEnemyName = spellTemplate.CreateMonsterEnemy;
            spell.DisplayName = spellTemplate.DisplayName;
            spell.IsStackable = spellTemplate.Stackable;
            spell.BlockType = spellTemplate.BlockType;
            return spell;
        }
    }
}
