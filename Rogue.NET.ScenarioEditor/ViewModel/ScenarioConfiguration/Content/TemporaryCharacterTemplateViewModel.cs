using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class TemporaryCharacterTemplateViewModel : NonPlayerCharacterTemplateViewModel
    {
        // NOTE*** The purpose of a temporary character is to be created in 
        //         the level with some specified life time. So, there's no
        //         lifetime counter here; but there will be alteration effects
        //         that have one.
        //

        public TemporaryCharacterTemplateViewModel()
        {
            this.AlignmentType = CharacterAlignmentType.PlayerAligned;
        }
    }
}
