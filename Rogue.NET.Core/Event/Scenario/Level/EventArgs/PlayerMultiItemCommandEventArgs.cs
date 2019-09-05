using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Event.Scenario.Level.EventArgs
{
    public class PlayerMultiItemCommandEventArgs : UserCommandEventArgs
    {
        public PlayerMultiItemActionType Type { get; set; }
        public IEnumerable<string> ItemIds { get; set; }

        public PlayerMultiItemCommandEventArgs(PlayerMultiItemActionType type, string[] itemIds)
        {
            this.Type = type;
            this.ItemIds = itemIds;
        }
    }
}
