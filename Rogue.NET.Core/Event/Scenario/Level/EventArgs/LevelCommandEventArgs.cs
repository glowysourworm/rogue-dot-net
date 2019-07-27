using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Event.Scenario.Level.EventArgs
{
    public class LevelCommandEventArgs : UserCommandEventArgs
    {
        public LevelActionType LevelAction { get; set; }
        public Compass Direction { get; set; }

        /// <summary>
        /// Id of associated item or skill
        /// </summary>
        public string Id { get; set; }

        public LevelCommandEventArgs(LevelActionType type, Compass direction, string id)
        {
            this.LevelAction = type;
            this.Direction = direction;
            this.Id = id;
        }
    }
}
