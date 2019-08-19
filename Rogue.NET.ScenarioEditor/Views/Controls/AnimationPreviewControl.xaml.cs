using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Media;
using Rogue.NET.Core.Media.Interface;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class AnimationPreviewControl : UserControl
    {
        readonly IAnimationCreator _animationCreator;
        readonly IAnimationGenerator _animationGenerator;

        IList<AnimationQueue> _animation = new List<AnimationQueue>();

        bool _updating = false;

        [ImportingConstructor]
        public AnimationPreviewControl(IAnimationCreator animationCreator, IAnimationGenerator animationGenerator)
        {
            _animationCreator = animationCreator;
            _animationGenerator = animationGenerator;

            InitializeComponent();
        }

        private void PlayAnimation()
        {
            var viewModel = this.DataContext as AnimationGroupTemplateViewModel;

            if (viewModel != null)
            {
                // Validate TargetType to prevent exception
                if (!Validate(viewModel))
                {
                    Xceed.Wpf
                         .Toolkit
                         .MessageBox
                         .Show("Invalid Target Type / Animation Type (Check for Source / Projectile (or) Chain)");

                    return;
                }

                // Map over animation list
                var animations = viewModel.Animations.Select(x =>
                {
                    // Map ViewModel -> Template
                    var template = x.Map<AnimationTemplateViewModel, AnimationTemplate>();

                    // Generate Animation Data
                    return _animationGenerator.GenerateAnimation(template);
                });

                // Queue Animations
                _animation = animations.SelectMany(x => CreateNewAnimation(x, viewModel.TargetType)).ToList();

                if (_animation.Any())
                {
                    StartAnimation(_animation[0]);
                }
                
            }
        }

        private bool Validate(AnimationGroupTemplateViewModel viewModel)
        {
            return !(viewModel.Animations.Any(x => x.BaseType == AnimationBaseType.Chain ||
                                                  x.BaseType == AnimationBaseType.ChainReverse ||
                                                  x.BaseType == AnimationBaseType.Projectile ||
                                                  x.BaseType == AnimationBaseType.ProjectileReverse) &&
                     viewModel.TargetType == AlterationTargetType.Source);
        }

        private void StartAnimation(ITimedGraphic sender)
        {
            if (!_animation.Any())
                return;

            // Put graphics on canvas
            foreach (var graphic in sender.GetGraphics())
            {
                Canvas.SetZIndex(graphic, 100);
                this.TheCanvas.Children.Add(graphic);
            }

            // Set Slider Maximum (ASSUME QUEUE HAS SET ALL EQUAL ANIMATION TIMES)
            this.AnimationSlider.Maximum = sender.AnimationTime / 1000.0;

            // Hook Finished Event
            sender.TimeElapsed += StopAnimation;

            // Hook Time Changed
            sender.AnimationTimeChanged += UpdateAnimationTime;

            sender.Start();
        }

        private void StopAnimation(ITimedGraphic sender)
        {
            if (!_animation.Any())
                return;

            // Remove Graphics From Canvas
            foreach (var graphic in sender.GetGraphics())
                this.TheCanvas.Children.Remove(graphic);

            // Unhook Finished Event
            sender.TimeElapsed -= StopAnimation;

            // Unhook Time Changed Event
            sender.AnimationTimeChanged -= UpdateAnimationTime;

            // Clean Up Resources
            sender.Stop();
            sender.CleanUp();

            // Remove Animation from the list
            if (_animation.Contains(sender))
                _animation.Remove(sender as AnimationQueue);

            if (_animation.Any())
                StartAnimation(_animation[0]);
        }

        private void ForceStopAnimation()
        {
            if (_animation == null)
                return;

            // Stop the animation that's running
            foreach (var animation in _animation)
            {
                // Remove Graphics From Canvas
                foreach (var graphic in animation.GetGraphics())
                {
                    if (this.TheCanvas.Children.Contains(graphic))
                        this.TheCanvas.Children.Remove(graphic);
                }

                // Unhook Finished Event
                animation.TimeElapsed -= StopAnimation;

                // Unhook Time Changed Event
                animation.AnimationTimeChanged -= UpdateAnimationTime;

                // Clean Up Resources
                animation.Stop();
                animation.CleanUp();
            }

            // Clear running animation
            _animation.Clear();
        }

        private void UpdateAnimationTime(object sender, AnimationTimeChangedEventArgs e)
        {
            _updating = true;
            this.AnimationSlider.Value = e.CurrentTimeMilliseconds / 1000.0;
            _updating = false;
        }

        private IEnumerable<AnimationQueue> CreateNewAnimation(AnimationData animation, AlterationTargetType targetType)
        {
            var playerLocation = new Point(Canvas.GetLeft(this.TheSmiley), Canvas.GetTop(this.TheSmiley));
            var enemy1Location = new Point(Canvas.GetLeft(this.TheEnemy), Canvas.GetTop(this.TheEnemy));
            var enemy2Location = new Point(Canvas.GetLeft(this.TheSecondEnemy), Canvas.GetTop(this.TheSecondEnemy));
            var enemy3Location = new Point(Canvas.GetLeft(this.TheThirdEnemy), Canvas.GetTop(this.TheThirdEnemy));

            var bounds = new Rect(this.TheCanvas.RenderSize);

            playerLocation.X += ModelConstants.CellWidth / 2.0D;
            playerLocation.Y += ModelConstants.CellHeight / 2.0D;
            enemy1Location.X += ModelConstants.CellWidth / 2.0D;
            enemy1Location.Y += ModelConstants.CellHeight / 2.0D;
            enemy2Location.X += ModelConstants.CellWidth / 2.0D;
            enemy2Location.Y += ModelConstants.CellHeight / 2.0D;
            enemy3Location.X += ModelConstants.CellWidth / 2.0D;
            enemy3Location.Y += ModelConstants.CellHeight / 2.0D;

            switch (targetType)
            {
                case AlterationTargetType.Source:
                    return _animationCreator.CreateAnimation(animation, bounds, playerLocation, new Point[] { playerLocation });
                case AlterationTargetType.Target:
                    return _animationCreator.CreateAnimation(animation, bounds, playerLocation, new Point[] { enemy1Location });
                case AlterationTargetType.AllInRange:
                    return _animationCreator.CreateAnimation(animation, bounds, playerLocation, new Point[] { enemy1Location, enemy2Location, enemy3Location, playerLocation });
                case AlterationTargetType.AllInRangeExceptSource:
                    return _animationCreator.CreateAnimation(animation, bounds, playerLocation, new Point[] { enemy1Location, enemy2Location, enemy3Location });
                default:
                    throw new Exception("Unhandled AlterationTargetType");
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            ForceStopAnimation();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            // Force animations to stop
            if (_animation.Any())
                ForceStopAnimation();

            // Then, play animation
            PlayAnimation();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_animation.Any())
            {
                foreach (var animation in _animation)
                {
                    if (!animation.IsPaused)
                        animation.Pause();
                    else
                        animation.Resume();
                }
            }
        }

        private void AnimationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_animation.Any() && !_updating)
            {
                foreach (var animation in _animation)
                    animation.Seek((int)(e.NewValue * 1000));
            }
        }
    }
}
