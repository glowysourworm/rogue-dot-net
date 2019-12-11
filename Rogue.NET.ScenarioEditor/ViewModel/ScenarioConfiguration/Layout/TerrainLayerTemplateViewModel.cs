using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout
{
    public class TerrainLayerTemplateViewModel : TemplateViewModel
    {
        TerrainType _type;
        TerrainLayoutType _layoutType;
        bool _isPassable;
        bool _emitsLight;
        TerrainLayer _layer;
        SymbolDetailsTemplateViewModel _symbolDetails;
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
        public SymbolDetailsTemplateViewModel SymbolDetails
        {
            get { return _symbolDetails; }
            set { this.RaiseAndSetIfChanged(ref _symbolDetails, value); }
        }
        public LightTemplateViewModel EmittedLight
        {
            get { return _emittedLight; }
            set { this.RaiseAndSetIfChanged(ref _emittedLight, value); }
        }

        public TerrainLayerTemplateViewModel()
        {
            this.SymbolDetails = new SymbolDetailsTemplateViewModel();
            this.EmittedLight = new LightTemplateViewModel();
        }
    }
}
