using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
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
        public ScenarioDesign(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.GeneralItem.PreviewMouseDown += (sender, e) =>
            {
                eventAggregator.GetEvent<LoadDesignEvent>()
                               .Publish(DesignMode.General);
            };
            this.AssetsItem.PreviewMouseDown += (sender, e) =>
            {
                eventAggregator.GetEvent<LoadDesignEvent>()
                               .Publish(DesignMode.Assets);
            };
            this.ObjectiveItem.PreviewMouseDown += (sender, e) =>
            {
                eventAggregator.GetEvent<LoadDesignEvent>()
                               .Publish(DesignMode.Objective);
            };
            this.LevelDesign.PreviewMouseDown += (sender, e) =>
            {
                eventAggregator.GetEvent<LoadDesignEvent>()
                               .Publish(DesignMode.Level);
            };
            this.ScenarioDesignOverviewItem.PreviewMouseDown += (sender, e) =>
            {
                eventAggregator.GetEvent<LoadDesignEvent>()
                               .Publish(DesignMode.Overview);
            };
            this.ValidationItem.PreviewMouseDown += (sender, e) =>
            {
                eventAggregator.GetEvent<LoadDesignEvent>()
                               .Publish(DesignMode.Validation);
            };
        }
    }
}
