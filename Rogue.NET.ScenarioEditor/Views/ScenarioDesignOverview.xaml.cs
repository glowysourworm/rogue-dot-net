using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views
{
    [Export]
    public partial class ScenarioDesignOverview : UserControl
    {
        [ImportingConstructor]
        public ScenarioDesignOverview(
            IScenarioEditorController scenarioEditorController,
            IScenarioValidationService scenarioValidationService,
            IAlterationNameService alterationNameService,
            IRogueEventAggregator eventAggregator)
        {            
            InitializeComponent();

            this.Loaded += (sender, e) =>
            {
                this.DataContext = new ScenarioDesignOverviewViewModel(
                                    scenarioEditorController.CurrentConfig, 
                                    scenarioValidationService, 
                                    alterationNameService,
                                    eventAggregator);
            };
        }
    }
}
