using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    [Export]
    public partial class PlayerDesign : UserControl
    {
        readonly IRogueEventAggregator _eventAggregator;

        [ImportingConstructor]
        public PlayerDesign(
                IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;
        }

        private void RemovePlayerButton_Click(object sender, RoutedEventArgs e)
        {
            var configuration = this.DataContext as ScenarioConfigurationContainerViewModel;
            var player = (sender as Button).DataContext as PlayerTemplateViewModel;

            if (configuration != null &&
                player != null)
            {
                _eventAggregator.GetEvent<RemoveGeneralAssetEvent>()
                                .Publish(player);
            }
        }

        private void AddPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            var configuration = this.DataContext as ScenarioConfigurationContainerViewModel;

            if (configuration != null)
            {
                // Show rename control to name Player
                var view = new RenameControl();
                var player = new PlayerTemplateViewModel();

                // TODO: Consider a better design. The player template is being treated more
                //       like an Asset; but it's not categorized as an "Asset". So, it would
                //       not be a big deal to do; and it would follow the design better.. 
                //
                //       For now, just going to set the attack attributes; but it should probably
                //       be treated as an "Asset". 
                player.AttackAttributes.AddRange(configuration.AttackAttributes.Select(x => x.DeepCopy()));

                view.DataContext = player;

                if (DialogWindowFactory.Show(view, "Create Player Class"))
                {
                    _eventAggregator.GetEvent<AddGeneralAssetEvent>()
                                    .Publish(player);
                }
            }
        }
    }
}
