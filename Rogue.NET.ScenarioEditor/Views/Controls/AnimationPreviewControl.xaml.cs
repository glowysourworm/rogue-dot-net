using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Media.Animation;
using Rogue.NET.Core.Media.Animation.EventData;
using Rogue.NET.Core.Media.Animation.Interface;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System;
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

        AnimationStoryboard _animation;

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

                if (_animation != null)
                    StopAnimation();

                // Map over animation list
                var animations = viewModel.Animations.Select(x =>
                {
                    // Map ViewModel -> Template
                    var template = x.Map<AnimationTemplateViewModel, AnimationTemplate>();

                    // Generate Animation Data
                    return _animationGenerator.GenerateAnimation(template);
                });

                // Queue Animations
                _animation = new AnimationStoryboard(animations.Select(x => CreateNewAnimation(x, viewModel.TargetType)).ToList());

                _animation.AnimationPlayerStartEvent += OnAnimationStart;
                _animation.AnimationPlayerChangeEvent += OnAnimationChange;
                _animation.AnimationTimeChanged += OnAnimationTimeChange;

                // Set Slider Maximum
                this.AnimationSlider.Maximum = _animation.AnimationTime / 1000.0;

                _animation.Start();
            }
        }

        private void StopAnimation()
        {
            // Force stop
            _animation.Stop();

            // Remove graphics from canvas because of the forced stop
            for (int i = this.TheCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (this.TheCanvas.Children[i] is AnimationPrimitive)
                    this.TheCanvas.Children.RemoveAt(i);
            }

            // Unhook events
            _animation.AnimationPlayerStartEvent -= OnAnimationStart;
            _animation.AnimationPlayerChangeEvent -= OnAnimationChange;
            _animation.AnimationTimeChanged -= OnAnimationTimeChange;

            _animation = null;
        }

        private void OnAnimationStart(AnimationPlayerStartEventData eventData)
        {
            // Put graphics on canvas
            foreach (var graphic in eventData.Primitives)
                this.TheCanvas.Children.Add(graphic);
        }

        private void OnAnimationChange(IAnimationPlayer sender, AnimationPlayerChangeEventData eventData)
        {
            // Remove graphics from canvas
            foreach (var graphic in eventData.OldPrimitives)
                this.TheCanvas.Children.Remove(graphic);

            // Add next graphics to canvas
            if (!eventData.SequenceFinished)
            {
                foreach (var graphic in eventData.NewPrimitives)
                    this.TheCanvas.Children.Add(graphic);
            }

            // SEQUENCE FINISHED: Unhook events
            else
            {
                _animation.AnimationPlayerStartEvent -= OnAnimationStart;
                _animation.AnimationPlayerChangeEvent -= OnAnimationChange;
                _animation.AnimationTimeChanged -= OnAnimationTimeChange;

                _animation = null;
            }
        }

        private void OnAnimationTimeChange(AnimationTimeChangedEventData sender)
        {
            _updating = true;
            this.AnimationSlider.Value = sender.CurrentTimeMilliseconds / 1000.0;
            _updating = false;
        }

        private bool Validate(AnimationGroupTemplateViewModel viewModel)
        {
            return !(viewModel.Animations.Any(x => x.BaseType == AnimationBaseType.Chain ||
                                                  x.BaseType == AnimationBaseType.ChainReverse ||
                                                  x.BaseType == AnimationBaseType.Projectile ||
                                                  x.BaseType == AnimationBaseType.ProjectileReverse) &&
                     viewModel.TargetType == AlterationTargetType.Source);
        }

        private AnimationQueue CreateNewAnimation(AnimationData animation, AlterationTargetType targetType)
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
                StopAnimation();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            // Force animations to stop
            if (_animation != null)
                StopAnimation();

            // Then, play animation
            PlayAnimation();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_animation != null)
                _animation.Pause();
        }
    }
}
