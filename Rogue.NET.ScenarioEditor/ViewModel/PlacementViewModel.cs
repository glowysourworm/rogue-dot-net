using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    public class PlacementViewModel : IPlacementViewModel
    {
        public ImageSource ImageSource { get; set; }
        public TemplateViewModel Template { get; set; }

        public PlacementViewModel()
        {

        }
    }
}
