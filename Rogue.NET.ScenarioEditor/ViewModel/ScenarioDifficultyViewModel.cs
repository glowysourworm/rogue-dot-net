using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using System.ComponentModel.Composition;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    public interface IScenarioDifficultyViewModel
    {

    }
    [Export(typeof(IScenarioDifficultyViewModel))]
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
