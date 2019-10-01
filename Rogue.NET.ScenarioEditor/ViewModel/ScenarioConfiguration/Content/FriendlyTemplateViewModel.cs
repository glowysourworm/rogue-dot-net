using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.Views.Assets;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    [UIType(DisplayName = "Friendly",
            Description = "Character that is aligned with the Player and will fight for them",
            ViewType = typeof(Friendly))]
    public class FriendlyTemplateViewModel : NonPlayerCharacterTemplateViewModel
    {
        public FriendlyTemplateViewModel()
        {
            this.AlignmentType = CharacterAlignmentType.PlayerAligned;
        }
    }
}
