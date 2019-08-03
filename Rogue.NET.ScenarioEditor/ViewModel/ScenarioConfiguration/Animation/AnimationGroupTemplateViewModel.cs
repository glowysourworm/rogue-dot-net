using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    public class AnimationGroupTemplateViewModel : TemplateViewModel
    {
        public ObservableCollection<AnimationTemplateViewModel> Animations { get; set; }

        public AnimationGroupTemplateViewModel()
        {
            this.Animations = new ObservableCollection<AnimationTemplateViewModel>();
        }
    }
}
