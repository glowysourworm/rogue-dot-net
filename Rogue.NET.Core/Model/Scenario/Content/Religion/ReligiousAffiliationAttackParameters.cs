using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Content.Religion
{
    [Serializable]
    public class ReligiousAffiliationAttackParameters : RogueBase
    {
        public string EnemyReligionName { get; set; }
        public double AttackMultiplier { get; set; }
        public double BlockMultiplier { get; set; }
        public double DefenseMultiplier { get; set; }

        public ReligiousAffiliationAttackParameters()
        {
            this.EnemyReligionName = "";
        }
    }
}
