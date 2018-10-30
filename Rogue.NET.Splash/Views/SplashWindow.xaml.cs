using Rogue.NET.Splash.ViewModel;
using System.ComponentModel.Composition;
using System.Windows;

namespace Rogue.NET.Splash.Views
{
    public partial class SplashWindow : Window
    {
        [ImportingConstructor]
        public SplashWindow(SplashViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }
    }
}
