using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.ViewModel.Interface
{
    public interface IPlacementViewModel
    {
        ImageSource ImageSource { get; set; }
        TemplateViewModel Template { get; set; }
    }
}
