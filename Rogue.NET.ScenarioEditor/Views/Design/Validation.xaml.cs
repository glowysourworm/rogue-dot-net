using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.Validation.Interface;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Design
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class Validation : UserControl
    {
        [ImportingConstructor]
        public Validation(
                IScenarioValidationViewModel viewModel,
                IScenarioEditorController scenarioEditorController,
                IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            this.ValidateButton.Click += (sender, e) => { scenarioEditorController.Validate(); };
        }
    }
}
