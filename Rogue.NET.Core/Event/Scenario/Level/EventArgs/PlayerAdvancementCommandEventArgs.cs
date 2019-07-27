using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Event.Scenario.Level.EventArgs
{
    public class PlayerAdvancementCommandEventArgs : PlayerCommandEventArgs
    {
        public int SkillPoints { get; set; }
        public double Agility { get; set; }
        public double Intelligence { get; set; }
        public double Strength { get; set; }

        public PlayerAdvancementCommandEventArgs() : base(PlayerActionType.PlayerAdvancement, "")
        {

        }
    }
}
