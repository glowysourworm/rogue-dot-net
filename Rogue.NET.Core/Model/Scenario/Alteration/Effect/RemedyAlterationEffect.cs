using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class RemedyAlterationEffect 
        : RogueBase, IConsumableAlterationEffect,
                     IDoodadAlterationEffect,
                     ISkillAlterationEffect
    {
        public AlteredCharacterState RemediedState { get; set; }

        public RemedyAlterationEffect()
        {
            this.RemediedState = new AlteredCharacterState();
        }
    }
}
