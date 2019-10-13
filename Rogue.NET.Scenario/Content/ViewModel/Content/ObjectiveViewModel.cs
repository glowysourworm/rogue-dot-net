using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;

using ScenarioMetaDataClass = Rogue.NET.Core.Model.Scenario.Abstract.ScenarioMetaData;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    /// <summary>
    /// Extension of ScenarioMetaDataViewModel that has a flag to show whether the objective has been completed.
    /// </summary>
    public class ObjectiveViewModel : ScenarioMetaDataViewModel
    {
        bool _isCompleted;

        public bool IsCompleted
        {
            get { return _isCompleted; }
            set { this.RaiseAndSetIfChanged(ref _isCompleted, value); }
        }

        public ObjectiveViewModel(ScenarioMetaDataClass metaData, IScenarioResourceService scenarioResourceService) 
            : base(metaData, scenarioResourceService)
        {
        }
    }
}
