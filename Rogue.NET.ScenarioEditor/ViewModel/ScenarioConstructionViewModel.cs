using Prism.Commands;
using Prism.Events;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    [Export(typeof(IScenarioConstructionViewModel))]
    public class ScenarioConstructionViewModel : IScenarioConstructionViewModel
    {
        readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public ScenarioConstructionViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public ICommand LoadConstructionCommand
        {
            get
            {
                return new DelegateCommand<string>((construction) =>
                {
                    _eventAggregator.GetEvent<LoadConstructionEvent>().Publish(new LoadConstructionEventArgs()
                    {
                        ConstructionName = construction
                    });
                });
            }
        }

    }
}
