using Rogue.NET.Core.Logic.Processing.Interface;

namespace Rogue.NET.Core.Logic.Processing
{
    public class LevelProcessingAction : ILevelProcessingAction
    {
        public LevelProcessingActionType Type { get; set; }

        public string CharacterId { get; set; }
    }
}
