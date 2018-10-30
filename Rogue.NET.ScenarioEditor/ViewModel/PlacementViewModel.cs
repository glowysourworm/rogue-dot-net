using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    public interface IPlacementViewModel
    {
        ImageSource ImageSource { get; set; }
        Template Template { get; set; }
    }
    public class PlacementViewModel : IPlacementViewModel
    {
        public ImageSource ImageSource { get; set; }
        public Template Template { get; set; }

        public PlacementViewModel()
        {

        }
    }
}
