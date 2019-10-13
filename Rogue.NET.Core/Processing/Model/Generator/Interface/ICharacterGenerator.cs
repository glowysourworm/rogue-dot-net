using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    public interface ICharacterGenerator
    {
        Player GeneratePlayer(PlayerTemplate playerTemplate, ScenarioEncyclopedia encyclopedia);

        Enemy GenerateEnemy(EnemyTemplate enemyTemplate, ScenarioEncyclopedia encyclopedia);
        Friendly GenerateFriendly(FriendlyTemplate friendlyTemplate, ScenarioEncyclopedia encyclopedia);
        TemporaryCharacter GenerateTemporaryCharacter(TemporaryCharacterTemplate temporaryCharacter, ScenarioEncyclopedia encyclopedia);
    }
}
