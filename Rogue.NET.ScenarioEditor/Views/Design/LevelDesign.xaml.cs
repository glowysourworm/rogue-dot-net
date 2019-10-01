using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events.Browser;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Design
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class LevelDesign : UserControl
    {
        readonly IRogueEventAggregator _eventAggregator;

        [ImportingConstructor]
        public LevelDesign(IRogueEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();

            this.AddLevelBranchButton.Click += (sender, e) =>
            {
                var viewModel = this.DataContext as LevelTemplateViewModel;

                if (viewModel != null)
                {
                    eventAggregator.GetEvent<AddLevelBranchEvent>()
                                   .Publish(new AddLevelBranchEventData()
                                   {
                                       LevelName = viewModel.Name,
                                       LevelBranchUniqueName = NameGenerator.Get(viewModel.LevelBranches.Select(x => x.Name), viewModel.Name + " Branch")
                                   });
                }
            };
        }
        private void RemoveLevelBranchButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as LevelTemplateViewModel;
            var button = sender as Button;
            if (viewModel != null &&
                button != null)
            {
                var levelBranch = button.DataContext as LevelBranchGenerationTemplateViewModel;
                if (levelBranch != null)
                {
                    _eventAggregator.GetEvent<RemoveLevelBranchEvent>()
                                    .Publish(new RemoveLevelBranchEventData()
                                    {
                                        LevelName = viewModel.Name,
                                        LevelBranchName = levelBranch.Name
                                    });
                }
            }
        }

        private void CopyLevelBranchButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as LevelTemplateViewModel;
            var button = sender as Button;

            if (viewModel != null &&
                button != null)
            {
                var levelBranch = button.DataContext as LevelBranchGenerationTemplateViewModel;
                if (levelBranch != null)
                {
                    _eventAggregator.GetEvent<CopyLevelBranchEvent>()
                                    .Publish(new CopyLevelBranchEventData()
                                    {
                                        LevelName = viewModel.Name,
                                        LevelBranchName = levelBranch.Name
                                    });
                }
            }
        }
    }
}
