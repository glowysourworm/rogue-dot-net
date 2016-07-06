using Microsoft.Practices.Unity;
using Rogue.NET.ScenarioEditor.ViewModel;
using System;
using System.Collections.Generic;
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

namespace Rogue.NET.ScenarioEditor.Views
{
    public partial class ScenarioConstruction : UserControl
    {
        public ScenarioConstruction()
        {
            InitializeComponent();
        }

        [InjectionConstructor]
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
