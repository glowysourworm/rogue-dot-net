using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System.Collections.Generic;

namespace Rogue.NET.Common.Events.Scenario
{
    public class UserCommandEventArgs : System.EventArgs
    {
        public ActionType Type { get; set; } 
        public LevelActionType LevelAction { get; set;}
        public ViewActionType ViewAction { get; set; }
        public Compass Direction { get; set; }

        /// <summary>
        /// Id of associated item or skill
        /// </summary>
        public string ItemId { get; set; }

        public UserCommandEventArgs()
        {
            this.Type = ActionType.LevelAction;
            this.LevelAction = LevelActionType.None;
            this.ViewAction = ViewActionType.None;
            this.Direction = Compass.Null;
            this.ItemId = "";
        }
        public UserCommandEventArgs(ViewActionType action)
        {
            this.Type = ActionType.ViewAction;
            this.ViewAction = action;
        }
        public UserCommandEventArgs(LevelActionType action, Compass direction, string id)
        {
            this.Type = ActionType.LevelAction;
            this.LevelAction = action;
            this.Direction = direction;
            this.ItemId = id;
        }
    }
    public class UserCommandEvent : RogueAsyncEvent<UserCommandEventArgs>
    {
    }
}
