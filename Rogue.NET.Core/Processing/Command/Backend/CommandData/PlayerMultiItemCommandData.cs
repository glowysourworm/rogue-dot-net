using Rogue.NET.Core.Model.Enums;

using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Command.Backend.CommandData
{
    public class PlayerMultiItemCommandData : BackendCommandData
    {
        public PlayerMultiItemActionType Type { get; set; }
        public IEnumerable<string> ItemIds { get; set; }

        public PlayerMultiItemCommandData(PlayerMultiItemActionType type, string[] itemIds)
        {
            this.Type = type;
            this.ItemIds = itemIds;
        }
    }
}
