using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.Interface
{
    public interface IPlacementGroupViewModel
    {
        ObservableCollection<IPlacementViewModel> PlacementCollection { get; set; }
    }
}
