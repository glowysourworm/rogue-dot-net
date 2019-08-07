using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class TeleportAlterationEffect
        : RogueBase, IConsumableAlterationEffect,
                     IDoodadAlterationEffect,
                     IEnemyAlterationEffect,
                     ISkillAlterationEffect
    {
        public AlterationRandomPlacementType TeleportType { get; set; }

        // TODO:ALTERATION
        /// <summary>
        /// Range that is applied to InRangeOfCharacter teleport type
        /// </summary>
        public int Range { get; set; }

        public TeleportAlterationEffect()
        {

        }
    }
}
