using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.Overview.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.Overview
{
    public class ProjectionSetViewModel : NotifyViewModel, IProjectionSetViewModel
    {
        Dictionary<string, List<IProjectedQuantityViewModel>> _projections;

        public int Count
        {
            get { return _projections.Count; }
        }

        public ProjectionSetViewModel()
        {
            _projections = new Dictionary<string, List<IProjectedQuantityViewModel>>();
        }

        public KeyValuePair<string, List<IProjectedQuantityViewModel>> GetProjection(int index)
        {
            return _projections.ElementAt(index);
        }

        public void Add(string seriesName, IEnumerable<IProjectedQuantityViewModel> projection)
        {
            _projections.Add(seriesName, projection.ToList());
        }
    }
}
