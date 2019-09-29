using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Design
{
    [Serializable]
    public class LevelTemplate : Template
    {
        public List<LevelBranchGenerationTemplate> LevelBranches { get; set; }
        public LevelTemplate()
        {
            this.LevelBranches = new List<LevelBranchGenerationTemplate>();
        }
    }
}
