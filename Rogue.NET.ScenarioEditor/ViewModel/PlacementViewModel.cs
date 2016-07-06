using Rogue.NET.Common;
using Rogue.NET.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
