using Rogue.NET.Scenario.Content.ViewModel.Content;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class SkillTree : UserControl
    {
        [ImportingConstructor]
        public SkillTree(PlayerViewModel playerViewModel)
        {
            this.DataContext = playerViewModel;

            InitializeComponent();

            this.Loaded += (sender, e) =>
            {
                if (playerViewModel.SkillSets.Any(x => x.HasLearnedSkills))
                    this.SkillSetLB.SelectedItem = playerViewModel.SkillSets.First(x => x.HasLearnedSkills);
            };

            this.SkillSetLB.SelectionChanged += (sender, e) =>
            {
                if (e.AddedItems.Count > 0)
                {
                    var skills = (e.AddedItems[0] as SkillSetViewModel).Skills;
                    if (skills.Count > 0)
                        this.SkillLB.SelectedItem = skills.First();
                }
            };
        }
    }
}
