
namespace Rogue.NET.Core.Processing.Event.Backend.EventData
{
    public class DialogPlayerAdvancementEventData : DialogEventData
    {
        public int PlayerPoints { get; set; }
        public double Strength { get; set; }
        public double Agility { get; set; }
        public double Intelligence { get; set; }
        public int SkillPoints { get; set; }
    }
}
