using Rogue.NET.Scenario.Content.ViewModel.Content;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Content.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class StatusCtrl : UserControl
    {
        [ImportingConstructor]
        public StatusCtrl(PlayerViewModel playerViewModel)
        {
            InitializeComponent();

            this.DataContext = playerViewModel;
        }
    }
}
