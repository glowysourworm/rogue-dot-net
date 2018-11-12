using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Skill;

namespace Rogue.NET.Core.Logic.Processing
{
    public class LevelProcessingAction : ILevelProcessingAction
    {
        public LevelProcessingActionType Type { get; set; }

        public string CharacterId { get; set; }

        // Animation Processing Related
        public Spell PlayerSpell { get; set; }
    }
}
