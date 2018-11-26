using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.View
{
    [Export]
    public partial class SplashView : UserControl
    {
        [ImportingConstructor]
        public SplashView()
        {
            InitializeComponent();
        }
    }
}
