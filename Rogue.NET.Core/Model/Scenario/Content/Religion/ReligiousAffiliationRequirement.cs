using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Religion
{
    [Serializable]
    public class ReligiousAffiliationRequirement : RogueBase
    {
        public string ReligionName { get; set; }
        public double RequiredAffiliationLevel { get; set; }

        public ReligiousAffiliationRequirement()
        {
            this.ReligionName = "";
        }
    }
}
