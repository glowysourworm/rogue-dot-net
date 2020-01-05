
using Rogue.NET.Core.Model.Enums;
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData
{
    public class DialogPlayerAdvancementEventData : DialogEventData
    {
        public string PlayerName { get; set; }
        public int PlayerLevel { get; set; }
        public Color SmileyColor { get; set; }
        public Color SmileyLineColor { get; set; }
        public SmileyExpression SmileyExpression { get; set; }

        public int PlayerPoints { get; set; }

        public double HealthPerPoint { get; set; }
        public double StaminaPerPoint { get; set; }
        public double StrengthPerPoint { get; set; }
        public double AgilityPerPoint { get; set; }
        public double IntelligencePerPoint { get; set; }
        public int SkillPointsPerPoint { get; set; }

        public double Health { get; set; }
        public double Stamina { get; set; }
        public double Strength { get; set; }
        public double Agility { get; set; }
        public double Intelligence { get; set; }
        public int SkillPoints { get; set; }
    }
}
