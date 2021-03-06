﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class CreateEnemyAlterationEffect : RogueBase, IConsumableAlterationEffect,
                                                          IDoodadAlterationEffect,
                                                          IEnemyAlterationEffect,
                                                          ISkillAlterationEffect
    {
        public AlterationRandomPlacementType RandomPlacementType { get; set; }

        public EnemyTemplate Enemy { get; set; }

        /// <summary>
        /// Range (in cell pseudo-euclidean distance) from acting character (based on the placement type)
        /// </summary>
        public int Range { get; set; }

        public CreateEnemyAlterationEffect() { }
    }
}
