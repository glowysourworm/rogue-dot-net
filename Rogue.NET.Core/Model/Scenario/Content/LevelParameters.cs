using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Content
{
    [Serializable]
    public class LevelParameters
    {
        public int Number { get; set; }
        public string LevelBranchName { get; set; }
        public string LayoutName { get; set; }
        public double EnemyGenerationPerStep { get; set; }
    }
}
