using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Event.Scenario.Level.EventArgs
{
    public class PlayerMultiItemCommandEventArgs : System.EventArgs
    {
        public PlayerMultiItemActionType Type { get; set; }
        public IEnumerable<string> ItemIds { get; set; }

        public PlayerMultiItemCommandEventArgs(PlayerMultiItemActionType type, params string[] itemIds)
        {
            this.Type = type;
            this.ItemIds = itemIds;
        }
    }
}
