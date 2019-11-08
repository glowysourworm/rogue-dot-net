using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout
{
    public class TerrainLayerTemplateViewModel : TemplateViewModel
    {
        TerrainType _type;
        TerrainLayoutType _layoutType;
        bool _isPassable;
        bool _isWalkable;
        SymbolDetailsTemplateViewModel _symbolDetails;

        public bool IsPassable
        {
            get { return _isPassable; }
            set { this.RaiseAndSetIfChanged(ref _isPassable, value); }
        }
        public bool IsWalkable
        {
            get { return _isWalkable; }
            set { this.RaiseAndSetIfChanged(ref _isWalkable, value); }
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

        public TerrainLayerTemplateViewModel()
        {
            this.SymbolDetails = new SymbolDetailsTemplateViewModel();
        }
    }
}
