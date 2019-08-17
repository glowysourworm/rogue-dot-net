using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl
{
    [Export]
    public partial class Animation : UserControl
    {
        public static readonly DependencyProperty TargetTypeCopyProperty =
            DependencyProperty.Register("TargetTypeCopy", typeof(AlterationTargetType), typeof(Animation));

        public AlterationTargetType TargetTypeCopy
        {
            get { return (AlterationTargetType)GetValue(TargetTypeCopyProperty); }
            set { SetValue(TargetTypeCopyProperty, value); }
        }

        [ImportingConstructor]
        public Animation()
        {
            InitializeComponent();
        }

        private void AddAnimationButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as AnimationGroupTemplateViewModel;
            if (viewModel != null)
            {
                viewModel.Animations.Add(new AnimationTemplateViewModel()
                {
                    Name = NameGenerator.Get(viewModel.Animations.Select(x => x.Name), "Animation")
                });
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
            }
        }
    }
}
