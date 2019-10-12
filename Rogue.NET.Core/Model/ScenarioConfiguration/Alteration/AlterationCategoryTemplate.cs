using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    // Piggy-backing off of DungeonObjectTemplate to create meta-data object
    [Serializable]
    public class AlterationCategoryTemplate : DungeonObjectTemplate
    {
        AlterationAlignmentType _alignmentType;

        public AlterationAlignmentType AlignmentType
        {
            get { return _alignmentType; }
            set { this.RaiseAndSetIfChanged(ref _alignmentType, value); }
        }

        public AlterationCategoryTemplate() { }
    }
}
