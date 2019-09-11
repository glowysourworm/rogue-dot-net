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
        RangeViewModel<int> _lifetimeCounter;

        public RangeViewModel<int> LifetimeCounter
        {
            get { return _lifetimeCounter; }
            set { this.RaiseAndSetIfChanged(ref _lifetimeCounter, value); }
        }

        public TemporaryCharacterTemplateViewModel()
        {
            this.LifetimeCounter = new RangeViewModel<int>(100, 150);
            this.AlignmentType = CharacterAlignmentType.PlayerAligned;
        }
    }
}
