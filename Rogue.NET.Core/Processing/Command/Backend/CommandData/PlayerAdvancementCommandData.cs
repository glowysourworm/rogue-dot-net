using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Processing.Command.Backend.CommandData
{
    public class PlayerAdvancementCommandData : PlayerCommandData
    {
        public int SkillPoints { get; set; }
        public double Agility { get; set; }
        public double Intelligence { get; set; }
        public double Strength { get; set; }
        public double Health { get; set; }
        public double Stamina { get; set; }

        public PlayerAdvancementCommandData() : base(PlayerCommandType.PlayerAdvancement, "")
        {

        }
    }
}
