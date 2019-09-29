using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design
{
    public class ConsumableGenerationTemplateViewModel : TemplateViewModel
    {
        ConsumableTemplateViewModel _asset;
        double _generationWeight;
        public ConsumableTemplateViewModel Asset
        {
            get { return _asset; }
            set { this.RaiseAndSetIfChanged(ref _asset, value); }
        }
        public double GenerationWeight
        {
            get { return _generationWeight; }
            set { this.RaiseAndSetIfChanged(ref _generationWeight, value); }
        }

        public ConsumableGenerationTemplateViewModel()
        {
            this.GenerationWeight = 1.0;
        }
    }
}
