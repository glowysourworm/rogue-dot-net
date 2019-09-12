using Rogue.NET.Core.Model.Enums;

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
            this.AlignmentType = CharacterAlignmentType.PlayerAligned;
            this.LifetimeCounter = new RangeViewModel<int>(100, 150);
        }
    }
}
