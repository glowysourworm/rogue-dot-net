﻿using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Processing.Action.Enum;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Action
{
    public class LevelProcessingAction
    {
        public LevelProcessingActionType Type { get; set; }

        public CharacterBase Actor { get; set; }

        public IEnumerable<CharacterBase> AlterationAffectedCharacters { get; set; }

        public AlterationContainer Alteration { get; set; }
    }
}
