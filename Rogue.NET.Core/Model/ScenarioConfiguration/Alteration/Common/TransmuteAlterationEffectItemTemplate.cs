using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    /// <summary>
    /// Class to specify a Transmute Item - which relates INPUT items to OUTPUT product in a
    /// Transmute Alteration Effect
    /// </summary>
    [Serializable]
    public class TransmuteAlterationEffectItemTemplate : Template
    {
        // NOTE*** Supporting each type of product possibility because not sure
        //         of serialization dealing with inheritance from the base class.
        //
        //         NEEDS TO BE FIXED! This has to be solved so there's no issues.

        EquipmentTemplate _equipmentProduct;
        ConsumableTemplate _consumableProduct;

        // Putting two flags in because it's more obvious from the designer's perspective; but
        // only ONE can be set (validated in editor). FIX THE REFERENCE ISSUE!
        bool _isEquipmentProduct;
        bool _isConsumableProduct;
        double _weighting;

        public bool IsEquipmentProduct
        {
            get { return _isEquipmentProduct; }
            set
            {
                if (_isEquipmentProduct != value)
                {
                    _isEquipmentProduct = value;
                    OnPropertyChanged("IsEquipmentProduct");
                }
            }
        }
        public bool IsConsumableProduct
        {
            get { return _isConsumableProduct; }
            set
            {
                if (_isConsumableProduct != value)
                {
                    _isConsumableProduct = value;
                    OnPropertyChanged("IsConsumableProduct");
                }
            }
        }
        public double Weighting
        {
            get { return _weighting; }
            set
            {
                if (_weighting != value)
                {
                    _weighting = value;
                    OnPropertyChanged("Weighting");
                }
            }
        }
        public EquipmentTemplate EquipmentProduct
        {
            get { return _equipmentProduct; }
            set
            {
                if (_equipmentProduct != value)
                {
                    _equipmentProduct = value;
                    OnPropertyChanged("EquipmentProduct");
                }
            }
        }
        public ConsumableTemplate ConsumableProduct
        {
            get { return _consumableProduct; }
            set
            {
                if (_consumableProduct != value)
                {
                    _consumableProduct = value;
                    OnPropertyChanged("ConsumableProduct");
                }
            }
        }

        public List<EquipmentTemplate> EquipmentRequirements { get; set; }
        public List<ConsumableTemplate> ConsumableRequirements { get; set; }

        public TransmuteAlterationEffectItemTemplate()
        {
            this.EquipmentRequirements = new List<EquipmentTemplate>();
            this.ConsumableRequirements = new List<ConsumableTemplate>();
        }
    }
}
