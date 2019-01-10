using Rogue.NET.Intro.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.Scenario.Intro.Views.GameSetup.Parameters
{
    [Export]
    public partial class ChooseReligion : UserControl
    {
        public ChooseReligion()
        {
            InitializeComponent();

            this.Loaded += (sender, e) =>
            {
                var viewModel = this.DataContext as GameSetupViewModel;
                if (viewModel != null)
                {
                    if (viewModel.SelectedReligion != null)
                        this.ReligionLB.SelectedItem = viewModel.SelectedReligion;

                    else if (viewModel.SelectedConfiguration != null)
                        this.ReligionLB.SelectedItem = viewModel.SelectedConfiguration.Religions.FirstOrDefault();
                }
            };
        }
    }
}
