using Prism.Events;
using Rogue.NET.Common.Extension;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.EnemyControl
{
    [Export]
    public partial class EnemyBehavior : UserControl
    {
        [ImportingConstructor]
        public EnemyBehavior()
        {
            InitializeComponent();

            // Run this to initialize all the behavior names
            this.DataContextChanged += (sender, e) =>
            {
                var viewModel = this.DataContext as EnemyTemplateViewModel;
                if (viewModel != null)
                {
                    for (int i = 0; i < viewModel.BehaviorDetails.Behaviors.Count; i++)
                        viewModel.BehaviorDetails.Behaviors[i].Name = (i + 1).ToOrdinal() + " Behavior";
                }
            };
        }

        private void RemoveBehaviorButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as EnemyTemplateViewModel;
            var behaviorViewModel = (sender as Button).DataContext as BehaviorTemplateViewModel;
            if (viewModel != null)
                viewModel.BehaviorDetails.Behaviors.Remove(behaviorViewModel);
        }

        private void AddBehaviorButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as EnemyTemplateViewModel;
            if (viewModel != null)
                viewModel.BehaviorDetails.Behaviors.Add(new BehaviorTemplateViewModel()
                {
                    Name = (viewModel.BehaviorDetails.Behaviors.Count + 1).ToOrdinal() + " Behavior"
                });
        }
    }
}
