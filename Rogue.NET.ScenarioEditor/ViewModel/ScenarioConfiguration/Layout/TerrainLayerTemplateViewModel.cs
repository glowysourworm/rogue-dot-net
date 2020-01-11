using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout
{
    public class TerrainLayerTemplateViewModel : TemplateViewModel
    {
        TerrainType _type;
        TerrainLayoutType _layoutType;
        TerrainConnectionType _connectionType;
        TerrainMaskingType _maskingType;
        bool _isPassable;
        bool _emitsLight;
        bool _hasEdgeSymbol;
        TerrainLayer _layer;
        SymbolDetailsTemplateViewModel _edgeSymbolDetails;
        SymbolDetailsTemplateViewModel _fillSymbolDetails;
        LightTemplateViewModel _emittedLight;

        public bool IsPassable
        {
            get { return _isPassable; }
            set { this.RaiseAndSetIfChanged(ref _isPassable, value); }
        }
        public bool EmitsLight
        {
            get { return _emitsLight; }
            set { this.RaiseAndSetIfChanged(ref _emitsLight, value); }
        }
        public bool HasEdgeSymbol
        {
            get { return _hasEdgeSymbol; }
            set { this.RaiseAndSetIfChanged(ref _hasEdgeSymbol, value); }
        }
        public TerrainLayer Layer
        {
            get { return _layer; }
            set { this.RaiseAndSetIfChanged(ref _layer, value); }
        }
        public TerrainType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public TerrainLayoutType LayoutType
        {
            get { return _layoutType; }
            set { this.RaiseAndSetIfChanged(ref _layoutType, value); }
        }
        public TerrainConnectionType ConnectionType
        {
            get { return _connectionType; }
            set { this.RaiseAndSetIfChanged(ref _connectionType, value); }
        }
        public TerrainMaskingType MaskingType
        {
            get { return _maskingType; }
            set { this.RaiseAndSetIfChanged(ref _maskingType, value); }
        }
        public SymbolDetailsTemplateViewModel EdgeSymbolDetails
        {
            get { return _edgeSymbolDetails; }
            set { this.RaiseAndSetIfChanged(ref _edgeSymbolDetails, value); }
        }
        public SymbolDetailsTemplateViewModel FillSymbolDetails
        {
            get { return _fillSymbolDetails; }
            set { this.RaiseAndSetIfChanged(ref _fillSymbolDetails, value); }
        }
        public LightTemplateViewModel EmittedLight
        {
            get { return _emittedLight; }
            set { this.RaiseAndSetIfChanged(ref _emittedLight, value); }
        }

        public TerrainLayerTemplateViewModel()
        {
            this.EdgeSymbolDetails = new SymbolDetailsTemplateViewModel();
            this.FillSymbolDetails = new SymbolDetailsTemplateViewModel();
            this.EmittedLight = new LightTemplateViewModel();
            this.HasEdgeSymbol = false;
        }
    }
}
