using Rogue.NET.Common.Extension;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    [Export]
    public partial class SkillSet : UserControl
    {
        public SkillSet()
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
