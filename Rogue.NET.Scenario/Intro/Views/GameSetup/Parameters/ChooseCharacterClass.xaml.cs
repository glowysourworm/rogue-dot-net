using Rogue.NET.Intro.ViewModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Intro.Views.GameSetup.Parameters
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class ChooseCharacterClass : UserControl
    {
        public ChooseCharacterClass()
        {
            InitializeComponent();

            this.Loaded += (sender, e) =>
            {
                var viewModel = this.DataContext as GameSetupViewModel;
                if (viewModel != null)
                {
                    if (viewModel.SelectedCharacterClass != null)
                        this.CharacterClassLB.SelectedItem = viewModel.SelectedCharacterClass;

                    else if (viewModel.SelectedConfiguration != null)
                        this.CharacterClassLB.SelectedItem = viewModel.SelectedConfiguration.CharacterClasses.FirstOrDefault();
                }
            };
        }
    }
}
