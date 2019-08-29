using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface ICharacterGenerator
    {
        Player GeneratePlayer(PlayerTemplate playerTemplate, IEnumerable<AttackAttribute> scenarioAttributes);

        Enemy GenerateEnemy(EnemyTemplate enemyTemplate, IEnumerable<AttackAttribute> scenarioAttributes);
    }
}
