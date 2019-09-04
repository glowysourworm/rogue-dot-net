using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;

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
        public EquipmentViewModel(Equipment equipment, string displayName) : base(equipment, displayName)
        {

        }
    }
}
