using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Skill;

namespace Rogue.NET.Core.Logic.Interface
{
    /// <summary>
    /// Component responsible for processing events involved with using a player or enemy spell. This includes
    /// alterations, animations, and events back to UI listeners.
    /// </summary>
    public interface ISpellEngine : IRogueEngine
    {
        LevelContinuationAction InvokePlayerMagicSpell(Spell spell);
        LevelContinuationAction InvokeEnemyMagicSpell(Enemy enemy, Spell spell);
    }
}
