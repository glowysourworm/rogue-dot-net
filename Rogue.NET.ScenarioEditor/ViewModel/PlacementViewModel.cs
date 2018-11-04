using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    public class PlacementViewModel : IPlacementViewModel
    {
        public ImageSource ImageSource { get; set; }
        public Template Template { get; set; }

        public PlacementViewModel()
        {

        }
    }
}
