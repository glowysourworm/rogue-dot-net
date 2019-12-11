using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Layout
{
    [Serializable]
    public class TerrainLayerGenerationTemplate : Template
    {
        double _fillRatio;
        double _frequency;
        TerrainGenerationType _generationType;
        TerrainLayerTemplate _terrainLayer;

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
        public TerrainLayerTemplate TerrainLayer
        {
            get { return _terrainLayer; }
            set { this.RaiseAndSetIfChanged(ref _terrainLayer, value); }
        }

        public TerrainLayerGenerationTemplate()
        {
            this.FillRatio = 0;
            this.Frequency = 1;
            this.GenerationType = TerrainGenerationType.PerlinNoise;
            this.TerrainLayer = new TerrainLayerTemplate();
        }
    }
}
