using Prism.Commands;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.Interface
{
    public interface IScenarioConstructionViewModel
    {
        DelegateCommand<Type> LoadConstructionCommand { get; }
    }
}
