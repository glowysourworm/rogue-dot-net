﻿using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class PermanentAlterationEffect : RogueBase, IConsumableAlterationEffect,
                                                        IConsumableProjectileAlterationEffect,
                                                        IDoodadAlterationEffect,
                                                        IEnemyAlterationEffect,
                                                        IFriendlyAlterationEffect,
                                                        ITemporaryCharacterAlterationEffect,
                                                        IEquipmentAttackAlterationEffect,
                                                        ISkillAlterationEffect
    {
        public double Strength { get; set; }
        public double Intelligence { get; set; }
        public double Agility { get; set; }
        public double Speed { get; set; }
        public double Vision { get; set; }
        public double Experience { get; set; }
        public double Hunger { get; set; }
        public double Health { get; set; }
        public double Stamina { get; set; }

        public PermanentAlterationEffect()
        {
        }
    }
}
