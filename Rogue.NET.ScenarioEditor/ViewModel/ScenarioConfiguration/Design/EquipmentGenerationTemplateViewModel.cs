using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design
{
    public class EquipmentGenerationTemplateViewModel : TemplateViewModel
    {
        EquipmentTemplateViewModel _asset;
        double _generationWeight;
        public EquipmentTemplateViewModel Asset
        {
            get { return _asset; }
            set { this.RaiseAndSetIfChanged(ref _asset, value); }
        }
        public double GenerationWeight
        {
            get { return _generationWeight; }
            set { this.RaiseAndSetIfChanged(ref _generationWeight, value); }
        }

        public EquipmentGenerationTemplateViewModel()
        {
            this.GenerationWeight = 1.0;
        }
    }
}
