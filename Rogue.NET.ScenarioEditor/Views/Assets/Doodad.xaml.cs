using Rogue.NET.ScenarioEditor.Views.Controls;
using Rogue.NET.Common.Extension;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Utility;

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    [Export]
    public partial class Doodad : UserControl
    {
        public Doodad()
        {
            InitializeComponent();
        }

        private void CreateSymbol_Click(object sender, RoutedEventArgs e)
        {
            var view = new SymbolEditor();
            view.DataContext = this.DataContext;
            view.WindowMode = true;
            view.Width = 600;

            DialogWindowFactory.Show(view, "Rogue Symbol Editor");
        }
    }
}
