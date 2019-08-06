using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;


namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface IAttackAttributeGenerator
    {
        AttackAttribute GenerateAttackAttribute(AttackAttributeTemplate attackAttributeTemplate);
    }
}
