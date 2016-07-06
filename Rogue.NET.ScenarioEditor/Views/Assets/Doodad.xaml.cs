using Rogue.NET.Model;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
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

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    public partial class Doodad : UserControl
    {
        public Doodad()
        {
            InitializeComponent();
        }

        public void SetConfigurationData(ScenarioConfiguration config)
        {
            this.AutomaticSpellCB.ItemsSource = config.MagicSpells;
            this.InvokedSpellCB.ItemsSource = config.MagicSpells;
        }

        private void CreateSymbol_Click(object sender, RoutedEventArgs e)
        {
            var window = new Window();
            var model = this.DataContext as DoodadTemplate;
            var copy = (SymbolDetailsTemplate)ResourceManager.CreateDeepCopy(model.SymbolDetails);

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
