using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.AnimationControl
{
    [Export]
    public partial class AnimationBasicType : UserControl
    {
        public static readonly DependencyProperty IsProjectileAnimationTypeProperty =
            DependencyProperty.Register("IsProjectileAnimationType", typeof(bool), typeof(AnimationBasicType));

        public bool IsProjectileAnimationType
        {
            get { return (bool)GetValue(IsProjectileAnimationTypeProperty); }
            set { SetValue(IsProjectileAnimationTypeProperty, value); }
        }

        public AnimationBasicType()
        {
            InitializeComponent();

            this.DataContextChanged += AnimationBasicType_DataContextChanged;
        }

        private void AnimationBasicType_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var animationTemplate = this.DataContext as AnimationTemplateViewModel;
            if (animationTemplate == null)
                return;

            switch (animationTemplate.Type)
            {
                case AnimationType.ProjectileSelfToTarget:
                case AnimationType.ProjectileTargetToSelf:
                case AnimationType.ProjectileSelfToTargetsInRange:
                case AnimationType.ProjectileTargetsInRangeToSelf:
                    this.ProjectileRB.IsChecked = true;
                    this.AuraRB.IsChecked = false;
                    this.BubblesRB.IsChecked = false;
                    this.BarrageRB.IsChecked = false;
                    this.SpiralRB.IsChecked = false;
                    break;
                case AnimationType.AuraSelf:
                case AnimationType.AuraTarget:
                    this.ProjectileRB.IsChecked = false;
                    this.AuraRB.IsChecked = true;
                    this.BubblesRB.IsChecked = false;
                    this.BarrageRB.IsChecked = false;
                    this.SpiralRB.IsChecked = false;
                    break;
                case AnimationType.BubblesSelf:
                case AnimationType.BubblesTarget:
                case AnimationType.BubblesScreen:
                    this.ProjectileRB.IsChecked = false;
                    this.AuraRB.IsChecked = false;
                    this.BubblesRB.IsChecked = true;
                    this.BarrageRB.IsChecked = false;
                    this.SpiralRB.IsChecked = false;
                    break;
                case AnimationType.BarrageSelf:
                case AnimationType.BarrageTarget:
                    this.ProjectileRB.IsChecked = false;
                    this.AuraRB.IsChecked = false;
                    this.BubblesRB.IsChecked = false;
                    this.BarrageRB.IsChecked = true;
                    this.SpiralRB.IsChecked = false;
                    break;
                case AnimationType.SpiralSelf:
                case AnimationType.SpiralTarget:
                    this.ProjectileRB.IsChecked = false;
                    this.AuraRB.IsChecked = false;
                    this.BubblesRB.IsChecked = false;
                    this.BarrageRB.IsChecked = false;
                    this.SpiralRB.IsChecked = true;
                    break;
                //case AnimationType.ChainSelfToTargetsInRange:
                //    break;
                case AnimationType.ScreenBlink:
                    break;

                // If none of these then it's the projectile
                default:
                    break;
            }
        }
    }
}
