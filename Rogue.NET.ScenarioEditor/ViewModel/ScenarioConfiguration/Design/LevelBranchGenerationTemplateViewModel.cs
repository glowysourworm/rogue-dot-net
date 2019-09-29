using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design
{
    public class LevelBranchGenerationTemplateViewModel : TemplateViewModel
    {
        LevelBranchTemplateViewModel _levelBranch;
        double _generationWeight;
        public LevelBranchTemplateViewModel LevelBranch
        {
            get { return _levelBranch; }
            set { this.RaiseAndSetIfChanged(ref _levelBranch, value); }
        }
        public double GenerationWeight
        {
            get { return _generationWeight; }
            set { this.RaiseAndSetIfChanged(ref _generationWeight, value); }
        }
        public LevelBranchGenerationTemplateViewModel()
        {
            this.LevelBranch = new LevelBranchTemplateViewModel();
            this.GenerationWeight = 1.0;
        }
    }
}
