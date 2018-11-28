using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ObjectiveViewModel(ScenarioMetaData metaData, IScenarioResourceService scenarioResourceService) 
            : base(metaData, scenarioResourceService)
        {
        }
    }
}
