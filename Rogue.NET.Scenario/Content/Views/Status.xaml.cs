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

            this.HPBar.BarColor1 = Colors.Red;
            this.StaminaBar.BarColor1 = Colors.Orange;
            this.ExperienceBar.BarColor1 = Colors.Cyan;
            this.HaulBar.BarColor1 = Colors.Goldenrod;
            this.HungerBar.BarColor1 = Colors.DarkGreen;

            this.DataContext = playerViewModel;
        }
    }
}
