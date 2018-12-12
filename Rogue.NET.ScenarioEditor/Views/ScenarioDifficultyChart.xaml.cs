using Prism.Events;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views
{
    [Export]
    public partial class ScenarioDifficultyChart : UserControl
    {
        readonly IScenarioEditorController _scenarioEditorController;

        [ImportingConstructor]
        public ScenarioDifficultyChart(
            IScenarioEditorController scenarioEditorController,
            IEventAggregator eventAggregator)
        {
            _scenarioEditorController = scenarioEditorController;

            InitializeComponent();

            eventAggregator.GetEvent<ScenarioLoadedEvent>().Subscribe(configuration =>
            {
                this.DataContext = new ScenarioDifficultyViewModel(configuration);
            });

            this.Loaded += (sender, e) =>
            {
                this.DataContext = new ScenarioDifficultyViewModel(_scenarioEditorController.CurrentConfig);
            };
        }
    }
}
