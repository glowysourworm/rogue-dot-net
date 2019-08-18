using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Intro.ViewModel;
using Rogue.NET.Scenario.Events;
using Rogue.NET.Scenario.Intro.ViewModel;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Intro.Views.GameSetup
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class ChooseScenario : UserControl
    {
        readonly IRogueEventAggregator _eventAggregator;

        [ImportingConstructor]
        public ChooseScenario(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;

            this.Loaded += (obj, e) => { this.SmallLB.SelectedIndex = 0; };
        }

        private void LeftArrowTB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.SmallLB.SelectedIndex = Math.Max(0, this.SmallLB.SelectedIndex - 1);
        }

        private void RightArrowTB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.SmallLB.SelectedIndex = Math.Min(this.SmallLB.Items.Count, this.SmallLB.SelectedIndex + 1);
        }

        private void BigLB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _eventAggregator.GetEvent<GameSetupDisplayFinished>().Publish(new GameSetupDisplayFinishedEventArgs()
            {
                NextDisplayType = typeof(ChooseParameters)
            });
        }
    }
}
