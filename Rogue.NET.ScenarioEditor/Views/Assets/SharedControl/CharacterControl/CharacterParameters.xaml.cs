﻿using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;

using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.CharacterControl
{
    [Export]
    public partial class CharacterParameters : UserControl
    {
        [ImportingConstructor]
        public CharacterParameters(IRogueEventAggregator eventAggregator)
        {
            InitializeComponent();
        }

        private void DeathAnimationsLB_AddEvent(object sender, object e)
        {
            // TODO:ANIMATION
            //var viewModel = this.DataContext as EnemyTemplateViewModel;
            //if (viewModel != null)
            //    viewModel.DeathAnimationSequence.Animations.Add(e as AnimationTemplateViewModel);
        }

        private void DeathAnimationsLB_RemoveEvent(object sender, object e)
        {
            // TODO:ANIMATION
            //var viewModel = this.DataContext as EnemyTemplateViewModel;
            //if (viewModel != null)
            //    viewModel.DeathAnimationSequence.Animations.Remove(e as AnimationTemplateViewModel);
        }
    }
}
