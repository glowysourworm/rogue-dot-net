using Rogue.NET.Core.Model;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design
{
    public class LayoutGenerationTemplateViewModel : TemplateViewModel
    {
        LayoutTemplateViewModel _asset;
        double _generationWeight;
        private double _partyRoomGenerationRate;

        public LayoutTemplateViewModel Asset
        {
            get { return _asset; }
            set { this.RaiseAndSetIfChanged(ref _asset, value); }
        }
        public double PartyRoomGenerationRate
        {
            get { return _partyRoomGenerationRate; }
            set { this.RaiseAndSetIfChanged(ref _partyRoomGenerationRate, value); }
        }
        public double GenerationWeight
        {
            get { return _generationWeight; }
            set { this.RaiseAndSetIfChanged(ref _generationWeight, value); }
        }
        public LayoutGenerationTemplateViewModel()
        {
            this.PartyRoomGenerationRate = ModelConstants.Scenario.PartyRoomGenerationRateDefault;
            this.GenerationWeight = 1.0;
        }
    }
}
