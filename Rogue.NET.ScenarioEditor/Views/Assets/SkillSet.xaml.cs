using Rogue.NET.Common.Extension;
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
            var window = new Window();
            var model = this.DataContext as SkillSetTemplateViewModel;


            window.Content = new SymbolEditor();
            window.Title = "Rogue Symbol Editor";
            var ctrl = window.Content as SymbolEditor;
            ctrl.Width = 600;
            ctrl.DataContext = model;
            ctrl.WindowMode = true;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ResizeMode = ResizeMode.NoResize;

            window.ShowDialog();
        }
    }
}
