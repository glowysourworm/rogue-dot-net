using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;

namespace Rogue.NET.Core.Logic.Processing
{
    public class DialogPlayerAdvancementUpdate : IDialogPlayerAdvancementUpdate
    {
        public DialogEventType Type { get; set; }
        public int PlayerPoints { get; set; }
        public double Strength { get; set; }
        public double Agility { get; set; }
        public double Intelligence { get; set; }
        public int SkillPoints { get; set; }
    }
}
