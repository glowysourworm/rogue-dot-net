using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface IAlterationGenerator
    {
        AlterationContainer GenerateAlteration(Spell spell);

        AlterationEffect GenerateAlterationEffect(
            string spellName, 
            string spellDisplayName, 
            double effectRange, 
            AlterationEffectTemplate alterationEffectTemplate);
    }
}
