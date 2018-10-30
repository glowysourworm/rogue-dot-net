using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Animation
{
    public partial class AnimationProjectile : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public AnimationProjectile()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(AnimationParametersMain); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;
            this.DataContext = model;

            if (model == null)
                return;

            switch ((model as AnimationTemplate).Type)
            {
                case AnimationType.ProjectileSelfToTarget:
                    this.SelfTargetRB.IsChecked = true;
                    break;
                case AnimationType.ProjectileSelfToTargetsInRange:
                    this.SelfTargetsRB.IsChecked = true;
                    break;
                case AnimationType.ProjectileTargetsInRangeToSelf:
                    this.TargetsSelfRB.IsChecked = true;
                    break;
                case AnimationType.ProjectileTargetToSelf:
                default:
                    this.TargetSelfRB.IsChecked = true;
                    break;
            }
        }

        private void RB_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            var model = this.DataContext as AnimationTemplate;

            if (radioButton == this.SelfTargetRB)
                model.Type = AnimationType.ProjectileSelfToTarget;
            else if (radioButton == this.SelfTargetsRB)
                model.Type = AnimationType.ProjectileSelfToTargetsInRange;
            else if (radioButton == this.TargetSelfRB)
                model.Type = AnimationType.ProjectileTargetToSelf;
            else
                model.Type = AnimationType.ProjectileTargetsInRangeToSelf;
        }
    }
}
