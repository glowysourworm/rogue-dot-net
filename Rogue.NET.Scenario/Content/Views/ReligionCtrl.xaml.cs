using Rogue.NET.Scenario.Content.ViewModel.Content;
using System.ComponentModel.Composition;
using System.Windows.Controls;

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
        }
    }
}
