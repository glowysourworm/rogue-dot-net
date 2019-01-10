using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface IAlterationGenerator
    {
        /// <summary>
        /// Generates an alteration container (cost, effect, parameters) from a Spell and the caster's
        /// intelligence. This should be the total intelligence of the caster; and is only applied to
        /// the effects - not the cost.
        /// </summary>
        AlterationContainer GenerateAlteration(Spell spell);
    }
}
