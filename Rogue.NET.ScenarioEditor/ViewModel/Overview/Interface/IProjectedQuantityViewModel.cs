using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.Overview.Interface
{
    public interface IProjectedQuantityViewModel
    {
        double Variance { get; set; }
        double Mean { get; set; }
        int Level { get; set; }
        string SeriesName { get; set; }
    }
}
