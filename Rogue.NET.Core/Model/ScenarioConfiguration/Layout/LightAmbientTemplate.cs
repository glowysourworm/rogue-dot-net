using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Layout
{
    [Serializable]
    public class LightAmbientTemplate : Template
    {
        LightTemplate _light;
        Range<double> _intensityRange;
        TerrainAmbientLightingType _type;
        double _fillRatio;

        public LightTemplate Light
        {
            get { return _light; }
            set { this.RaiseAndSetIfChanged(ref _light, value); }
        }

        public Range<double> IntensityRange
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

        public LightAmbientTemplate()
        {
            this.Light = new LightTemplate();
            this.IntensityRange = new Range<double>(0, 1);
            this.Type = TerrainAmbientLightingType.None;
            this.FillRatio = 1;
        }
    }
}
