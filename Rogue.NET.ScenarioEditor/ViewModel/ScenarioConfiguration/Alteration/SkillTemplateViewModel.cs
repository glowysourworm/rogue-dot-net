using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    public class SkillTemplateViewModel : TemplateViewModel
    {
        int _levelRequirement;
        int _pointRequirement;
        SpellTemplateViewModel _alteration;

        public int LevelRequirement
        {
            get { return _levelRequirement; }
            set { this.RaiseAndSetIfChanged(ref _levelRequirement, value); }
        }
        public int PointRequirement
        {
            get { return _pointRequirement; }
            set { this.RaiseAndSetIfChanged(ref _pointRequirement, value); }
        }
        public SpellTemplateViewModel Alteration
        {
            get { return _alteration; }
            set { this.RaiseAndSetIfChanged(ref _alteration, value); }
        }

        public SkillTemplateViewModel()
        {
            this.Alteration = new SpellTemplateViewModel();
        }
    }
}
