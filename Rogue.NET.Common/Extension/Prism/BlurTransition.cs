using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Rogue.NET.Common.Extension.Prism
{
    public class BlurTransition : Transition
    {
        public int FromRadius { get; set; }
        public int ToRadius { get; set; }
        public Duration Time { get; set; }

        public BlurTransition()
        {
            this.FromRadius = 0;
            this.ToRadius = 10;
            this.Time = new Duration(new TimeSpan(0, 0, 0, 0, 300));
        }

        protected override void BeginTransition(TransitionPresenter transitionElement, ContentPresenter oldContent, ContentPresenter newContent)
        {
            var blurEffect = new BlurEffect();
            var doubleAnimation = new DoubleAnimation(this.FromRadius, this.ToRadius, this.Time);

            doubleAnimation.EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseOut, Exponent = 1 };
            doubleAnimation.Completed += (obj, e) =>
            {
                oldContent.Effect = null;
                EndTransition(transitionElement, oldContent, newContent);
            };

            oldContent.Effect = blurEffect;
            blurEffect.BeginAnimation(BlurEffect.RadiusProperty, doubleAnimation);
        }
    }
}
