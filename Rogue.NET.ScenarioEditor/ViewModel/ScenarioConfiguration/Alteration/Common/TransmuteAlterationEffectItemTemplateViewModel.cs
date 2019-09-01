using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class TransmuteAlterationEffectItemTemplateViewModel : TemplateViewModel
    {
        // NOTE*** Supporting each type of product possibility because not sure
        //         of serialization dealing with inheritance from the base class.
        //
        //         NEEDS TO BE FIXED! This has to be solved so there's no issues.

        EquipmentTemplateViewModel _equipmentProduct;
        ConsumableTemplateViewModel _consumableProduct;

        // Putting two flags in because it's more obvious from the designer's perspective; but
        // only ONE can be set (validated in editor). FIX THE REFERENCE ISSUE!
        bool _isEquipmentProduct;
        bool _isConsumableProduct;
        double _weighting;

        public bool IsEquipmentProduct
        {
            get { return _isEquipmentProduct; }
            set { this.RaiseAndSetIfChanged(ref _isEquipmentProduct, value); }
        }
        public bool IsConsumableProduct
        {
            get { return _isConsumableProduct; }
            set { this.RaiseAndSetIfChanged(ref _isConsumableProduct, value); }
        }
        public double Weighting
        {
            get { return _weighting; }
            set { this.RaiseAndSetIfChanged(ref _weighting, value); }
        }
        public EquipmentTemplateViewModel EquipmentProduct
        {
            get { return _equipmentProduct; }
            set { this.RaiseAndSetIfChanged(ref _equipmentProduct, value); }
        }
        public ConsumableTemplateViewModel ConsumableProduct
        {
            get { return _consumableProduct; }
            set { this.RaiseAndSetIfChanged(ref _consumableProduct, value); }
        }

        public ObservableCollection<EquipmentTemplateViewModel> EquipmentRequirements { get; set; }
        public ObservableCollection<ConsumableTemplateViewModel> ConsumableRequirements { get; set; }

        public TransmuteAlterationEffectItemTemplateViewModel()
        {
            this.EquipmentRequirements = new ObservableCollection<EquipmentTemplateViewModel>();
            this.ConsumableRequirements = new ObservableCollection<ConsumableTemplateViewModel>();
        }
    }
}
