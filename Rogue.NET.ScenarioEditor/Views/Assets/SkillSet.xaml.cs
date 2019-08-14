using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System.ComponentModel.Composition;
using System.Linq;
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
            view.DataContext = (this.DataContext as SkillSetTemplateViewModel).SymbolDetails;
            view.WindowMode = true;
            view.Width = 600;

            DialogWindowFactory.Show(view, "Rogue Symbol Editor");
        }

        private void AddSkillButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as SkillSetTemplateViewModel;
            if (viewModel == null)
                return;

            viewModel.Skills.Add(new SkillTemplateViewModel()
            {
                Name = NameGenerator.Get(viewModel.Skills.Select(x => x.Name), viewModel.Name)
            });
        }

        private void RemoveSkillButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as SkillSetTemplateViewModel;
            if (viewModel == null)
                return;

            // Remove the Skill from the SkillSet
            viewModel.Skills.Remove((sender as Button).DataContext as SkillTemplateViewModel);
        }
    }
}
