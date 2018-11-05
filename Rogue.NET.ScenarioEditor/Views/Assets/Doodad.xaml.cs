using Rogue.NET.ScenarioEditor.Views.Controls;
using Rogue.NET.Common.Extension;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    [Export]
    public partial class Doodad : UserControl
    {
        public Doodad()
        {
            InitializeComponent();
        }

        public void SetConfigurationData(ScenarioConfigurationContainerViewModel config)
        {
            this.AutomaticSpellCB.ItemsSource = config.MagicSpells;
            this.InvokedSpellCB.ItemsSource = config.MagicSpells;
        }

        private void CreateSymbol_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window();
            var model = this.DataContext as DoodadTemplateViewModel;
            var copy = model.SymbolDetails.Copy();

            window.Content = new SymbolEditor();
            var ctrl = window.Content as SymbolEditor;
            ctrl.Width = 600;
            ctrl.DataContext = copy;
            ctrl.WindowMode = true;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ResizeMode = ResizeMode.NoResize;

            if ((bool)window.ShowDialog())
                model.SymbolDetails = copy;
        }
    }
}
