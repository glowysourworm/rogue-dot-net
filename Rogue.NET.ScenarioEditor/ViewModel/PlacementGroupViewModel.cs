using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    public interface IPlacementGroupViewModel
    {
        ObservableCollection<IPlacementViewModel> PlacementCollection { get; set; }
    }
    public class PlacementGroupViewModel : IPlacementGroupViewModel
    {
        public ObservableCollection<IPlacementViewModel> PlacementCollection { get; set; }

        public PlacementGroupViewModel()
        {
            this.PlacementCollection = new ObservableCollection<IPlacementViewModel>();
        }

        public PlacementGroupViewModel(IEnumerable<IPlacementViewModel> collection)
        {
            this.PlacementCollection = new ObservableCollection<IPlacementViewModel>(collection);
        }
    }
}
