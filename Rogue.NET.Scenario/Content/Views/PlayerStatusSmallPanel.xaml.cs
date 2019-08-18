using Rogue.NET.Scenario.Content.ViewModel.Content;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class PlayerStatusSmallPanel : UserControl
    {
        [ImportingConstructor]
        public PlayerStatusSmallPanel(PlayerViewModel playerViewModel)
        {
            this.DataContext = playerViewModel;

            InitializeComponent();
        }
    }
}
