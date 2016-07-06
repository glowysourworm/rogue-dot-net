using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Rogue.NET.ScenarioEditor.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    public interface IScenarioConstructionViewModel
    {
        ICommand LoadConstructionCommand { get; }
    }
    public class ScenarioConstructionViewModel : IScenarioConstructionViewModel
    {
        readonly IEventAggregator _eventAggregator;

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
                    _eventAggregator.GetEvent<LoadConstructionEvent>().Publish(new LoadConstructionEvent()
                    {
                        ConstructionName = construction
                    });
                });
            }
        }

    }
}
