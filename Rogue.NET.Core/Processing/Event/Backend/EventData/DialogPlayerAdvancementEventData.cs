﻿
namespace Rogue.NET.Core.Processing.Event.Backend.EventData
{
    public class DialogPlayerAdvancementEventData : DialogEventData
    {
        public int PlayerPoints { get; set; }

        public double HpPerPoint { get; set; }
        public double StaminaPerPoint { get; set; }
        public double StrengthPerPoint { get; set; }
        public double AgilityPerPoint { get; set; }
        public double IntelligencePerPoint { get; set; }
        public int SkillPointsPerPoint { get; set; }

        public double Hp { get; set; }
        public double Stamina { get; set; }
        public double Strength { get; set; }
        public double Agility { get; set; }
        public double Intelligence { get; set; }
        public int SkillPoints { get; set; }
    }
}
