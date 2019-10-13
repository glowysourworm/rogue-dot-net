using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Detect Effects",
               Description = "Effect that finds objects in the scenario with hidden effects")]
    public class DetectAlterationAlignmentAlterationEffectViewModel : AlterationEffectViewModel
    {
        AlterationAlignmentType _alignmentType;

        public AlterationAlignmentType AlignmentType
        {
            get { return _alignmentType; }
            set { this.RaiseAndSetIfChanged(ref _alignmentType, value); }
        }

        public DetectAlterationAlignmentAlterationEffectViewModel(DetectAlterationAlignmentAlterationEffect effect) : base(effect)
        {
            this.AlignmentType = effect.AlignmentType;
        }

        public DetectAlterationAlignmentAlterationEffectViewModel(DetectAlterationAlignmentAlterationEffectTemplate template) : base(template)
        {
            this.AlignmentType = template.AlignmentType;
        }
    }
}
