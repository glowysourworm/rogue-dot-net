using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Skill;

namespace Rogue.NET.Core.Logic.Processing
{
    public class LevelProcessingAction : ILevelProcessingAction
    {
        public LevelProcessingActionType Type { get; set; }

        public Character Actor { get; set; }

        // TODO:ALTERATION
        public Spell Spell { get; set; }

        public AlterationBase Alteration { get; set; }
    }
}
