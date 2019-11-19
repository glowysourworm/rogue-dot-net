using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout
{
    public class TerrainLayerGenerationTemplateViewModel : TemplateViewModel
    {
        double _generationWeight;
        double _fillRatio;
        double _frequency;
        TerrainGenerationType _generationType;
        TerrainLayerTemplateViewModel _terrainLayer;

        public double GenerationWeight
        {
            get { return _generationWeight; }
            set { this.RaiseAndSetIfChanged(ref _generationWeight, value); }
        }
        public double FillRatio
        {
            get { return _fillRatio; }
            set { this.RaiseAndSetIfChanged(ref _fillRatio, value); }
        }
        public double Frequency
        {
            get { return _frequency; }
            set { this.RaiseAndSetIfChanged(ref _frequency, value); }
        }
        public TerrainGenerationType GenerationType
        {
            get { return _generationType; }
            set { this.RaiseAndSetIfChanged(ref _generationType, value); }
        }
        public TerrainLayerTemplateViewModel TerrainLayer
        {
            get { return _terrainLayer; }
            set { this.RaiseAndSetIfChanged(ref _terrainLayer, value); }
        }

        public TerrainLayerGenerationTemplateViewModel()
        {
            this.GenerationWeight = 0;
            this.FillRatio = 0;
            this.Frequency = 1;
            this.GenerationType = TerrainGenerationType.PerlinNoise;
            this.TerrainLayer = new TerrainLayerTemplateViewModel();
        }
    }
}
