using Rogue.NET.Scenario.Content.ViewModel.Content;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Threading;

namespace Rogue.NET.Scenario.Content.Views
{
    [Export]
    public partial class ReligionCtrl : UserControl
    {
        [ImportingConstructor]
        public ReligionCtrl(PlayerViewModel playerViewModel)
        {
            this.DataContext = playerViewModel;

            InitializeComponent();

            this.Dispatcher.Invoke(() =>
            {
                if (playerViewModel.Religion.AttackParameters.Any())
                    this.ReligionLB.SelectedItem = playerViewModel.Religion.AttackParameters.FirstOrDefault();

            }, DispatcherPriority.ApplicationIdle);            
        }
    }
}
