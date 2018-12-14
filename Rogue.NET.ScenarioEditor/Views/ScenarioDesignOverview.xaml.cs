using Prism.Events;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views
{
    [Export]
    public partial class ScenarioDesignOverview : UserControl
    {
        readonly IScenarioEditorController _scenarioEditorController;

        [ImportingConstructor]
        public ScenarioDesignOverview(
            IScenarioEditorController scenarioEditorController,
            IScenarioValidationService scenarioValidationService,
            IEventAggregator eventAggregator)
        {            
            InitializeComponent();

            this.Loaded += (sender, e) =>
            {
                this.DataContext = new ScenarioDesignOverviewViewModel(
                                    scenarioEditorController.CurrentConfig, 
                                    scenarioValidationService, 
                                    eventAggregator);
            };
        }
    }
}
