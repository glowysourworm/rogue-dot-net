using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Skill;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class SkillSet : UserControl
    {
        public SkillSet()
        {
            InitializeComponent();

            // Set symbol tab to be the default to show for the consumable
            this.Loaded += (sender, e) =>
            {
                this.DefaultTab.IsSelected = true;
            };
        }

        private void AddSkillButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as SkillSetTemplateViewModel;
            if (viewModel == null)
                return;

            // Create a new skill with a user name input
            var view = new RenameControl();
            var defaultName = NameGenerator.Get(viewModel.Skills.Select(x => x.Name), viewModel.Name);
            var skill = new SkillTemplateViewModel()
            {
                // NOTE*** SETTING DEFAULT FOR ALTERATION
                SkillAlteration = new SkillAlterationTemplateViewModel()
                {
                    Name = viewModel.Name + " Effect"
                }
            };

            // Set the name using the RenameControl
            view.DataContext = skill;

            if (DialogWindowFactory.Show(view, "Create Skill"))
                viewModel.Skills.Add(skill);
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
