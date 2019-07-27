using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Logic.Processing
{
    public class PlayerAdvancementCommandAction : IPlayerAdvancementCommandAction
    {
        public PlayerActionType Type { get; set; }
        public string Id { get; set; }
        public double Strength { get; set; }
        public double Agility { get; set; }
        public double Intelligence { get; set; }
        public int SkillPoints { get; set; }
    }
}
