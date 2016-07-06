using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Splash.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.Splash.Views
{
    public partial class CreatingScenarioWindow : Window
    {
        public CreatingScenarioWindow(CreatingScenarioViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
