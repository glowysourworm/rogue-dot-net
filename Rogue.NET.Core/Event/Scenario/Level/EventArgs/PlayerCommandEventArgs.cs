using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Event.Scenario.Level.EventArgs
{
    public class PlayerCommandEventArgs : UserCommandEventArgs
    {
        public PlayerActionType Type { get; set; }

        public string Id { get; set; }

        public PlayerCommandEventArgs(PlayerActionType type, string id)
        {
            this.Type = type;
            this.Id = id;
        }
    }
}
