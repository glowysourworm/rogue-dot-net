using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    // Piggy-backing off of DungeonObjectTemplateViewModel to create meta-data object
    public class AlterationCategoryTemplateViewModel : DungeonObjectTemplateViewModel
    {
        AlterationAlignmentType _alignmentType;

        public AlterationAlignmentType AlignmentType
        {
            get { return _alignmentType; }
            set { this.RaiseAndSetIfChanged(ref _alignmentType, value); }
        }

        public AlterationCategoryTemplateViewModel() { }
    }
}
