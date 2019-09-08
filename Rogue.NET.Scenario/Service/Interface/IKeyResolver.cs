using Rogue.NET.Core.Processing.Command.Backend.CommandData;
using Rogue.NET.Core.Processing.Command.Frontend.Data;
using Rogue.NET.Core.Processing.Command.View.CommandData;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Service.Interface
{
    public interface IKeyResolver
    {
        /// <summary>
        /// Resolves a user command during any front-end game mode [Targeting, etc...]
        /// </summary>
        FrontendCommandData ResolveFrontendCommand(Key k, bool shift, bool ctrl, bool alt);

        /// <summary>
        /// Resolves a user command during normal game mode
        /// </summary>
        LevelCommandData ResolveLevelCommand(Key k, bool shift, bool ctrl, bool alt);

        /// <summary>
        /// Resolves a user command during normal game mode
        /// </summary>
        PlayerCommandData ResolvePlayerCommand(Key k, bool shift, bool ctrl, bool alt);

        /// <summary>
        /// Resolves a view command during normal game mode
        /// </summary>
        ViewCommandData ResolveViewCommand(Key k, bool shift, bool ctrl, bool alt);
    }
}
