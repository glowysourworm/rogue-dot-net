using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design
{
    public class LevelTemplateViewModel : TemplateViewModel
    {
        public ObservableCollection<LevelBranchGenerationTemplateViewModel> LevelBranches { get; set; }
        public LevelTemplateViewModel()
        {
            this.LevelBranches = new ObservableCollection<LevelBranchGenerationTemplateViewModel>();
        }
    }
}
