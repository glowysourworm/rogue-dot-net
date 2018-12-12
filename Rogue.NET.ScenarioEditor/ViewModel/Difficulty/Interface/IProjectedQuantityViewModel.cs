using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface
{
    public interface IProjectedQuantityViewModel
    {
        double Low { get; set; }
        double High { get; set; }
        double Average { get; set; }
        int Level { get; set; }
    }
}
