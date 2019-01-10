using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Religion;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface ICharacterGenerator
    {
        Player GeneratePlayer(PlayerTemplate playerTemplate, string religionName, IEnumerable<Religion> religions);

        Enemy GenerateEnemy(EnemyTemplate enemyTemplate, IEnumerable<Religion> religions);
    }
}
