using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System.Collections.Generic;

namespace Rogue.NET.Common.Events.Scenario
{
    public class UserCommandEventArgs : System.EventArgs
    {
        public LevelAction Action { get; set;}
        public Compass Direction { get; set; }

        /// <summary>
        /// Id of associated item or skill
        /// </summary>
        public string ItemId { get; set; }

        public UserCommandEventArgs()
        {
            this.Action = LevelAction.Null;
            this.Direction = Compass.Null;
            this.ItemId = "";
        }
        public UserCommandEventArgs(LevelAction action, Compass direction, string id)
        {
            Action = action;
            Direction = direction;
            ItemId = id;
        }
    }
    public class UserCommandEvent : RogueAsyncEvent<UserCommandEventArgs>
    {
    }
}
