using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Processing.Command.Backend.CommandData
{
    public class LevelCommandData : BackendCommandData
    {
        public LevelCommandType LevelAction { get; set; }
        public Compass Direction { get; set; }

        /// <summary>
        /// Id of associated item or skill
        /// </summary>
        public string Id { get; set; }

        public LevelCommandData(LevelCommandType type, Compass direction, string id)
        {
            this.LevelAction = type;
            this.Direction = direction;
            this.Id = id;
        }
    }
}
