using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System.Collections.ObjectModel;
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
            var window = new Window();
            var model = this.DataContext as SkillSetTemplate;
            var copy = (SymbolDetailsTemplate)model.SymbolDetails.Copy();

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
