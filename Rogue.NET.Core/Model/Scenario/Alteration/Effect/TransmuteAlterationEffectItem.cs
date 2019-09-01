using System;
using System.Collections.Generic;

using EquipmentClass = Rogue.NET.Core.Model.Scenario.Content.Item.Equipment;
using ConsumableClass = Rogue.NET.Core.Model.Scenario.Content.Item.Consumable;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class TransmuteAlterationEffectItem : RogueBase
    {
        public bool IsEquipmentProduct { get; set; }
        public bool IsConsumableProduct { get; set; }
        public double Weighting { get; set; }
        public EquipmentClass EquipmentProduct { get; set; }
        public ConsumableClass ConsumableProduct { get; set; }

        // NOTE*** These lists are required just for managing REQUIREMENTS for the
        //         alteration effect. They're not necessary to generate; but are kept
        //         for the sake of "hard references" to configuration collections. 
        //
        //         Probably could use some re-designing there.
        //
        public List<EquipmentClass> EquipmentRequirements { get; set; }
        public List<ConsumableClass> ConsumableRequirements { get; set; }

        public TransmuteAlterationEffectItem()
        {
            this.EquipmentRequirements = new List<EquipmentClass>();
            this.ConsumableRequirements = new List<ConsumableClass>();
        }
    }
}
