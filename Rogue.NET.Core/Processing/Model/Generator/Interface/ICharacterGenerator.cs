using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    public interface ICharacterGenerator
    {
        Player GeneratePlayer(PlayerTemplate playerTemplate);

        Enemy GenerateEnemy(EnemyTemplate enemyTemplate);
        Friendly GenerateFriendly(FriendlyTemplate friendlyTemplate);
        TemporaryCharacter GenerateTemporaryCharacter(TemporaryCharacterTemplate temporaryCharacter);
    }
}
