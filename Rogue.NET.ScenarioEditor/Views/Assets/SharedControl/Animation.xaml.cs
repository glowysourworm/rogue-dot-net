using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AnimationControl;
using System;
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
        public Animation(IRogueEventAggregator eventAggregator, IUITypeAttributeProvider uITypeAttributeProvider)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();

            this.AnimationTypeCB.ItemsSource = uITypeAttributeProvider.GetUITypeDescriptions(UITypeAttributeBaseType.Animation);

            this.DataContextChanged += (sender, e) =>
            {
                var viewModel = e.NewValue as AnimationSequenceTemplateViewModel;

                if (viewModel != null)
                {
                    // Select first item of the animation list
                    this.AnimationListBox.SelectedItem = viewModel.Animations.FirstOrDefault();
                }
            };

            this.AnimationParametersTC.DataContextChanged += (sender, e) =>
            {
                var animationTemplate = e.NewValue as AnimationBaseTemplateViewModel;

                if (animationTemplate != null)
                {
                    // Get the UIType attribute description that matches the selected view model
                    var uiDescription = uITypeAttributeProvider.GetUITypeDescriptions(UITypeAttributeBaseType.Animation)
                                                               .FirstOrDefault(x => x.ViewModelType == e.NewValue.GetType());

                    if (uiDescription == null)
                        throw new Exception("Unhandled UI Type (Animation)");

                    eventAggregator.GetEvent<LoadRegionEvent>()
                                   .Publish(this.AnimationParametersRegion, new LoadRegionEventData()
                                   {
                                       ViewType = uiDescription.ViewType
                                   });
                }
                else
                    eventAggregator.GetEvent<LoadRegionEvent>()
                                   .Publish(this.AnimationParametersRegion, new LoadRegionEventData()
                                   {
                                       ViewType = typeof(AnimationInstructions)
                                   });

                this.AnimationParametersTC.SelectedItem = this.DefaultTab;
            };
        }

        private void AddAnimationButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as AnimationSequenceTemplateViewModel;
            var uiDescription = this.AnimationTypeCB.SelectedItem as UITypeAttributeViewModel;

            if (viewModel != null &&
                uiDescription != null)
            {
                var animationName = NameGenerator.Get(viewModel.Animations.Select(x => x.Name), uiDescription.DisplayName);
                var animation = uiDescription.ViewModelType.Construct() as AnimationBaseTemplateViewModel;

                animation.Name = animationName;

                viewModel.Animations.Add(animation);
            }
        }

        private void RemoveAnimationButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as AnimationSequenceTemplateViewModel;
            var selectedItem = this.AnimationListBox.SelectedItem as AnimationBaseTemplateViewModel;

            if (viewModel != null &&
                selectedItem != null)
            {
                viewModel.Animations.Remove(selectedItem);
            }
        }
    }
}
