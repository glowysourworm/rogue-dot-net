using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.Scenario.Dynamic;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Character
{
    [Serializable]
    public class Enemy : Character
    {
        public double ExperienceGiven { get; set; }
        public double TurnCounter { get; set; }
        public bool IsEngaged { get; set; }
        public bool IsInvisible { get; set; }

        // TODO:  Change this to "IsAlerted" and set appropriately (maybe all enemies in the area if
        //        the player is "seen"

        /// <summary>
        /// Flag to alert enemies to Player if they're invisible or transmorgified
        /// </summary>
        public bool WasAttackedByPlayer { get; set; }

        public BehaviorDetails BehaviorDetails { get; set; }

        public Dictionary<string, AttackAttribute> AttackAttributes { get; set; }

        public AnimationGroup DeathAnimation { get; set; }

        public Enemy() : base()
        {
            this.BehaviorDetails = new BehaviorDetails();
            this.AttackAttributes = new Dictionary<string, AttackAttribute>();
            this.DeathAnimation = new AnimationGroup();
        }
        public Enemy(string symbol, string name) : base(name, symbol)
        {
            this.BehaviorDetails = new BehaviorDetails();
            this.AttackAttributes = new Dictionary<string, AttackAttribute>();
            this.DeathAnimation = new AnimationGroup();
        }
    }
}
