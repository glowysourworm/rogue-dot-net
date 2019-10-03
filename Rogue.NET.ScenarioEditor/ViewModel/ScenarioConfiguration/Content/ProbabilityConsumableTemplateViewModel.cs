using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class ProbabilityConsumableTemplateViewModel : TemplateViewModel
    {
        private ConsumableTemplateViewModel _theTemplate;
        private double _generationProbability;

        public ConsumableTemplateViewModel TheTemplate
        {
            get { return _theTemplate; }
            set { this.RaiseAndSetIfChanged(ref _theTemplate, value); }
        }
        public double GenerationProbability
        {
            get { return _generationProbability; }
            set { this.RaiseAndSetIfChanged(ref _generationProbability, value); }
        }

        public ProbabilityConsumableTemplateViewModel()
        {
            this.TheTemplate = new ConsumableTemplateViewModel();
            this.GenerationProbability = 1.0D;
        }
    }
}
