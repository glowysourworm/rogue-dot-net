using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    public class AnimationGroupTemplateViewModel : TemplateViewModel
    {
        AlterationTargetType _targetType;

        public AlterationTargetType TargetType
        {
            get { return _targetType; }
            set { this.RaiseAndSetIfChanged(ref _targetType, value); }
        }

        public ObservableCollection<AnimationTemplateViewModel> Animations { get; set; }

        public AnimationGroupTemplateViewModel()
        {
            this.Animations = new ObservableCollection<AnimationTemplateViewModel>();
        }
    }
}
