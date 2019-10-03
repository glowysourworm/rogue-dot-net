using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.ScenarioEditor.Controller.Interface;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.Overview.Interface;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Design
{
    [Export]
    public partial class Overview : UserControl
    {
        [ImportingConstructor]
        public Overview(
            IScenarioOverviewViewModel scenarioOverviewViewModel,
            IRogueEventAggregator eventAggregator)
        {            
            InitializeComponent();

            this.DataContext = scenarioOverviewViewModel;
        }
    }
}
