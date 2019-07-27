using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Logic.Processing
{
    public class PlayerCommandAction : IPlayerCommandAction
    {
        public PlayerActionType Type { get; set; }

        public string Id { get; set; }
    }
}
