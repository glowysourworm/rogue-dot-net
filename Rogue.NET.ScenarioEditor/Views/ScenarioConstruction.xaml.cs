using Rogue.NET.ScenarioEditor.ViewModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views
{
    [Export]
    public partial class ScenarioConstruction : UserControl
    {
        [ImportingConstructor]
        public ScenarioConstruction(IScenarioConstructionViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
        }

        private void AssetLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = this.DataContext as IScenarioConstructionViewModel;
            if (viewModel != null && e.AddedItems.Count > 0)
            {
                var construction = ((ListBoxItem)e.AddedItems[0]).Tag.ToString();
                var command = viewModel.LoadConstructionCommand;

                if (command.CanExecute(construction))
                    command.Execute(construction);
            }
        }
    }
}
