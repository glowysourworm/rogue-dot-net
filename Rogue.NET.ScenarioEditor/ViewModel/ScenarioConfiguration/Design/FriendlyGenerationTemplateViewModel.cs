using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design
{
    public class FriendlyGenerationTemplateViewModel : TemplateViewModel
    {
        FriendlyTemplateViewModel _asset;
        double _generationWeight;
        public FriendlyTemplateViewModel Asset
        {
            get { return _asset; }
            set { this.RaiseAndSetIfChanged(ref _asset, value); }
        }
        public double GenerationWeight
        {
            get { return _generationWeight; }
            set { this.RaiseAndSetIfChanged(ref _generationWeight, value); }
        }

        public FriendlyGenerationTemplateViewModel()
        {
            this.GenerationWeight = 1.0;
        }
    }
}
