using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.Overview.Interface
{
    public interface IProjectionSetViewModel
    {
        int Count { get; }

        KeyValuePair<string, List<IProjectedQuantityViewModel>> GetProjection(int index);

        void Add(string seriesName, IEnumerable<IProjectedQuantityViewModel> projection);
    }
}
