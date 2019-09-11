using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class FriendlyTemplateViewModel : NonPlayerCharacterTemplateViewModel
    {
        public FriendlyTemplateViewModel()
        {
            this.AlignmentType = CharacterAlignmentType.PlayerAligned;
        }
    }
}
