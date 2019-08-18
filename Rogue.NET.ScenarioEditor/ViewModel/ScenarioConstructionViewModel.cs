using Prism.Commands;
using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using System;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    [Export(typeof(IScenarioConstructionViewModel))]
    public class ScenarioConstructionViewModel : IScenarioConstructionViewModel
    {
        [ImportingConstructor]
        public ScenarioConstructionViewModel(IRogueEventAggregator eventAggregator)
        {
            this.LoadConstructionCommand = new DelegateCommand<Type>((construction) =>
            {
                eventAggregator.GetEvent<LoadConstructionEvent>().Publish(construction);
            });
        }

        public DelegateCommand<Type> LoadConstructionCommand { get; private set; }
    }
}
