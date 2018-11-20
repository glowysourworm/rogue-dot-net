using Prism.Events;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views
{
    [Export]
    public partial class GameInfoView : UserControl
    {
        [ImportingConstructor]
        public GameInfoView(GameViewModel viewModel)
        {
            this.DataContext = viewModel;

            InitializeComponent();
        }
    }
}
