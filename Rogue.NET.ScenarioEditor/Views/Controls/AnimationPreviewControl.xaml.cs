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
        readonly Queue<IEnumerable<AnimationQueue>> _animationQueue;

        IList<AnimationQueue> _animation = null;

        bool _updating = false;

        [ImportingConstructor]
        public AnimationPreviewControl(IAnimationCreator animationCreator, IAnimationGenerator animationGenerator)
        {
            _animationCreator = animationCreator;
            _animationGenerator = animationGenerator;
            _animationQueue = new Queue<IEnumerable<AnimationQueue>>();

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

                _animationQueue.Clear();

                // Queue Animations
                foreach (var animation in animations)
                    _animationQueue.Enqueue(CreateNewAnimation(animation, viewModel.TargetType));

                // Dequeue First Animation
                if (_animationQueue.Any())
                {
                    _animation = new List<AnimationQueue>(_animationQueue.Dequeue());

                    StartAnimation();
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

        private void StartAnimation()
        {
            if (_animation == null)
                return;

            // Put graphics on canvas
            foreach (var queue in _animation)
            {
                foreach (var graphic in queue.GetGraphics())
                {
                    Canvas.SetZIndex(graphic, 100);
                    this.TheCanvas.Children.Add(graphic);
                }

                // Set Slider Maximum (ASSUME QUEUE HAS SET ALL EQUAL ANIMATION TIMES)
                this.AnimationSlider.Maximum = queue.AnimationTime / 1000.0;

                // Hook Finished Event
                queue.TimeElapsed += StopAnimation;

                // Hook Time Changed
                queue.AnimationTimeChanged += UpdateAnimationTime;

                queue.Start();
            }
        }

        private void StopAnimation(ITimedGraphic sender)
        {
            if (_animation == null)
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

            // Queue Next Animation
            if (_animationQueue.Any() &&
                _animation.Count == 0)
            {
                _animation = new List<AnimationQueue>(_animationQueue.Dequeue());

                StartAnimation();
            }
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
            if (_animation != null)
            {
                // Empty animation queue to prevent next animation from starting
                _animationQueue.Clear();

                // Iterate backwards to stop all animation graphics and remove from list
                for (int i = _animation.Count - 1; i >= 0; i--)
                    StopAnimation(_animation[i]);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (_animation != null)
            {
                // Iterate backwards to stop all animation graphics and remove from list
                for (int i = _animation.Count - 1; i >= 0; i--)
                    StopAnimation(_animation[i]);

                // Then, play animation
                PlayAnimation();
            }

            else
                PlayAnimation();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_animation != null)
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
            if (_animation != null && !_updating)
            {
                foreach (var animation in _animation)
                    animation.Seek((int)(e.NewValue * 1000));
            }
        }
    }
}
