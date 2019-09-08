using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Processing.Command.Backend.CommandData
{
    public class PlayerCommandData : BackendCommandData
    {
        public PlayerCommandType Type { get; set; }

        public string Id { get; set; }

        public PlayerCommandData(PlayerCommandType type, string id)
        {
            this.Type = type;
            this.Id = id;
        }
    }
}
