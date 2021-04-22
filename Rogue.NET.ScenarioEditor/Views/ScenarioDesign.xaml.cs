using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.Validation.Interface;
using Rogue.NET.ScenarioEditor.Views.Constants;
using Rogue.NET.ScenarioEditor.Views.Design;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views
{
    [Export]
    public partial class ScenarioDesign : UserControl
    {
        [ImportingConstructor]
        public ScenarioDesign(IRogueEventAggregator eventAggregator, IScenarioValidationViewModel scenarioValidationViewModel)
        {
            InitializeComponent();

            this.ValidationItem.DataContext = scenarioValidationViewModel;

            this.GeneralItem.PreviewMouseDown += (sender, e) =>
            {
                eventAggregator.GetEvent<LoadDesignEvent>()
                               .Publish(DesignMode.General);
            };
            this.LevelDesign.PreviewMouseDown += (sender, e) =>
            {
                eventAggregator.GetEvent<LoadDesignEvent>()
                               .Publish(DesignMode.Level);
            };
            // TODO: Complete or remove overview mode
            //this.ScenarioDesignOverviewItem.PreviewMouseDown += (sender, e) =>
            //{
            //    eventAggregator.GetEvent<LoadDesignEvent>()
            //                   .Publish(DesignMode.Overview);
            //};
            this.ValidationItem.PreviewMouseDown += (sender, e) =>
            {
                eventAggregator.GetEvent<LoadDesignEvent>()
                               .Publish(DesignMode.Validation);
            };
        }
    }
}
