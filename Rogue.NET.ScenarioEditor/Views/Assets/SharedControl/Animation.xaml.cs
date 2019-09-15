using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class Animation : UserControl
    {
        readonly IRogueEventAggregator _eventAggregator;

        [ImportingConstructor]
        public Animation(IRogueEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();

            this.DataContextChanged += (sender, e) =>
            {
                var viewModel = e.NewValue as AnimationGroupTemplateViewModel;

                if (viewModel != null)
                {
                    // Select first item of the animation list
                    this.AnimationListBox.SelectedItem = viewModel.Animations.FirstOrDefault();
                }
            };
        }

        private void AddAnimationButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as AnimationGroupTemplateViewModel;
            if (viewModel != null)
            {
                var animation = new AnimationTemplateViewModel()
                {
                    Name = NameGenerator.Get(viewModel.Animations.Select(x => x.Name), "Animation")
                };

                viewModel.Animations.Add(animation);

                // Publish event to notify listeners about brushes
                _eventAggregator.GetEvent<BrushAddedEvent>()
                                .Publish(animation.FillTemplate);
            }
        }

        private void RemoveAnimationButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as AnimationGroupTemplateViewModel;
            var selectedItem = this.AnimationListBox.SelectedItem as AnimationTemplateViewModel;

            if (viewModel != null &&
                selectedItem != null)
            {
                viewModel.Animations.Remove(selectedItem);

                // Publish event to notify listeners about brushes
                _eventAggregator.GetEvent<BrushRemovedEvent>()
                                .Publish(selectedItem.FillTemplate);
            }
        }
    }
}
