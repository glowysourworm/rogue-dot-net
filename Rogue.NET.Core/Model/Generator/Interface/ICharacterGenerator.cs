using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;

namespace Rogue.NET.Core.Model.Generator.Interface
{
    public interface ICharacterGenerator
    {
        Player GeneratePlayer(PlayerTemplate playerTemplate);

        Enemy GenerateEnemy(EnemyTemplate enemyTemplate);
    }
}
