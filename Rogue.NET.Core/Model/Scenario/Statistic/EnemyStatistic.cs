using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Statistic
{
    [Serializable]
    public class EnemyStatistic
    {
        public string RogueName { get; set; }
        public int Score { get; set; }
        public bool IsObjective { get; set; }
        public bool IsUnique { get; set; }
    }
}
