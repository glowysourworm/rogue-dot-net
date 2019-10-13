using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Common
{
    [Serializable]
    public class AlterationCategory : ScenarioImage
    {
        public AlterationAlignmentType AlignmentType { get; set; }

        public AlterationCategory() { }
    }
}
