using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Common.Events.Scenario
{
    public class UserCommandEventArgs : System.EventArgs
    {
        public LevelAction Action = LevelAction.Null;
        public Compass Direction = Compass.Null;

        /// <summary>
        /// Id of associated item or skill
        /// </summary>
        public string ItemId = "";

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
