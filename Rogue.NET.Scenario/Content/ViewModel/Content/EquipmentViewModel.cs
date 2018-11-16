using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class EquipmentViewModel : ScenarioImageViewModel
    {
        double _weight;

        public double Weight
        {
            get { return _weight; }
            set { this.RaiseAndSetIfChanged(ref _weight, value); }
        }

        public EquipmentViewModel() { }
        public EquipmentViewModel(Equipment equipment) : base(equipment)
        {

        }
    }
}
