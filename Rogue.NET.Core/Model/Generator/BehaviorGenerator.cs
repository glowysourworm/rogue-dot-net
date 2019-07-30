using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(IBehaviorGenerator))]
    public class BehaviorGenerator : IBehaviorGenerator
    {
        private readonly ISpellGenerator _spellGenerator;

        [ImportingConstructor]
        public BehaviorGenerator(ISpellGenerator spellGenerator)
        {
            _spellGenerator = spellGenerator;
        }

        public Behavior GenerateBehavior(BehaviorTemplate behaviorTemplate, IEnumerable<CharacterClass> religions)
        {
            Behavior behavior = new Behavior();
            behavior.AttackType = behaviorTemplate.AttackType;
            behavior.EnemySkill = _spellGenerator.GenerateSpell(behaviorTemplate.EnemySpell, religions);
            behavior.MovementType = behaviorTemplate.MovementType;
            return behavior;
        }
    }
}
