using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout
{
    public class LightAmbientTemplateViewModel : TemplateViewModel
    {
        LightTemplateViewModel _light;
        RangeViewModel<double> _intensityRange;
        TerrainAmbientLightingType _type;
        double _fillRatio;

        public LightTemplateViewModel Light
        {
            get { return _light; }
            set { this.RaiseAndSetIfChanged(ref _light, value); }
        }

        public RangeViewModel<double> IntensityRange
        {
            get { return _intensityRange; }
            set { this.RaiseAndSetIfChanged(ref _intensityRange, value); }
        }

        public TerrainAmbientLightingType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }

        public double FillRatio
        {
            get { return _fillRatio; }
            set { this.RaiseAndSetIfChanged(ref _fillRatio, value); }
        }

        public LightAmbientTemplateViewModel()
        {
            this.Light = new LightTemplateViewModel();
            this.IntensityRange = new RangeViewModel<double>(0, 1);
            this.Type = TerrainAmbientLightingType.None;
            this.FillRatio = 1;
        }
    }
}
