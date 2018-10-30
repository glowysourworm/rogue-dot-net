using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Views
{
    public partial class StatusCtrl : UserControl
    {
        public StatusCtrl()
        {
            InitializeComponent();

            this.HPBar.BarColor1 = Colors.Red;
            this.MPBar.BarColor1 = Colors.Blue;
            this.ExperienceBar.BarColor1 = Colors.Cyan;
            this.HaulBar.BarColor1 = Colors.Goldenrod;
            this.HungerBar.BarColor1 = Colors.DarkGreen;
        }
    }
}
