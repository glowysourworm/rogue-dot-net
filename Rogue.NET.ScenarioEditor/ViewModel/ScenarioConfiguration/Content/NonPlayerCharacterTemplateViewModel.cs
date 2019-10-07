using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class NonPlayerCharacterTemplateViewModel : CharacterTemplateViewModel
    {
        private CharacterAlignmentType _alignmentType;
        private BehaviorDetailsTemplateViewModel _behaviorDetails;
        private AnimationSequenceTemplateViewModel _deathAnimation;

        public CharacterAlignmentType AlignmentType
        {
            get { return _alignmentType; }
            set { this.RaiseAndSetIfChanged(ref _alignmentType, value); }
        }
        public BehaviorDetailsTemplateViewModel BehaviorDetails
        {
            get { return _behaviorDetails; }
            set { this.RaiseAndSetIfChanged(ref _behaviorDetails, value); }
        }
        public AnimationSequenceTemplateViewModel DeathAnimation
        {
            get { return _deathAnimation; }
            set { this.RaiseAndSetIfChanged(ref _deathAnimation, value); }
        }

        public NonPlayerCharacterTemplateViewModel()
        {
            this.BehaviorDetails = new BehaviorDetailsTemplateViewModel();
            this.DeathAnimation = new AnimationSequenceTemplateViewModel()
            {
                TargetType = AlterationTargetType.Source
            };
        }
    }
}
