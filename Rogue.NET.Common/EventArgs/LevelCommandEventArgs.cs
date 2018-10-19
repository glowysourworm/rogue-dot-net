namespace Rogue.NET.Common.EventArgs
{
    public class LevelCommandEventArgs : System.EventArgs
    {
        public LevelAction Action = LevelAction.Null;
        public Compass Direction = Compass.Null;

        /// <summary>
        /// Id of associated item or skill
        /// </summary>
        public string ItemId = "";

        public LevelCommandEventArgs(LevelAction action, Compass direction, string id)
        {
            Action = action;
            Direction = direction;
            ItemId = id;
        }
    }
}
