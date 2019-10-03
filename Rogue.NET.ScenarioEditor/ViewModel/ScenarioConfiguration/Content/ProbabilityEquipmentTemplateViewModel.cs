using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class ProbabilityEquipmentTemplateViewModel : TemplateViewModel
    {
        private EquipmentTemplateViewModel _theTemplate;
        private double _generationProbability;
        private bool _equipOnStartup;

        public EquipmentTemplateViewModel TheTemplate
        {
            get { return _theTemplate; }
            set { this.RaiseAndSetIfChanged(ref _theTemplate, value); }
        }
        public double GenerationProbability
        {
            get { return _generationProbability; }
            set { this.RaiseAndSetIfChanged(ref _generationProbability, value); }
        }
        public bool EquipOnStartup
        {
            get { return _equipOnStartup; }
            set { this.RaiseAndSetIfChanged(ref _equipOnStartup, value); }
        }

        public ProbabilityEquipmentTemplateViewModel()
        {
            this.TheTemplate = new EquipmentTemplateViewModel();
            this.GenerationProbability = 1.0D;
        }
    }
}
