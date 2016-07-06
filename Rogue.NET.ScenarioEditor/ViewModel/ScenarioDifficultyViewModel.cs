using Microsoft.Practices.Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    public interface IScenarioDifficultyViewModel
    {

    }
    public class ScenarioDifficultyViewModel : IScenarioDifficultyViewModel
    {
        readonly IEventAggregator _eventAggregator;

        public ScenarioDifficultyViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe(e =>
            {
                // TODO:  Calculate difficulty curves
            });
        }
    }
}
