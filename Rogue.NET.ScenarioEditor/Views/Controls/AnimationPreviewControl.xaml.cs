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
        readonly IAnimationSequenceCreator _animationSequenceCreator;
        readonly IAnimationGenerator _animationGenerator;

        IAnimationPlayer _animation;

        bool _updating = false;

        [ImportingConstructor]
        public AnimationPreviewControl(IAnimationSequenceCreator animationSequenceCreator, IAnimationGenerator animationGenerator)
        {
            _animationSequenceCreator = animationSequenceCreator;
            _animationGenerator = animationGenerator;

            InitializeComponent();
        }

        private void PlayAnimation()
        {
            var viewModel = this.DataContext as AnimationSequenceTemplateViewModel;

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

                // Map to the Core namespace
                var template = viewModel.Map<AnimationSequenceTemplateViewModel, AnimationSequenceTemplate>();

                // Create the animation sequence data
                var animationSequence = _animationGenerator.GenerateAnimation(template);

                // Create animation points
                Point sourceLocation;
                Point[] targetLocations = CreateTargetPoints(animationSequence.TargetType, out sourceLocation);

                // Queue Animations
                _animation = _animationSequenceCreator.CreateAnimation(animationSequence, new Rect(this.TheCanvas.RenderSize), sourceLocation, targetLocations);

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

        private bool Validate(AnimationSequenceTemplateViewModel viewModel)
        {
            return !(viewModel.Animations.Any(x => x is AnimationChainTemplateViewModel ||
                                                   x is AnimationChainConstantVelocityTemplateViewModel ||
                                                   x is AnimationProjectileTemplateViewModel ||
                                                   x is AnimationProjectileConstantVelocityTemplateViewModel) &&
                     viewModel.TargetType == AlterationTargetType.Source);
        }

        private Point[] CreateTargetPoints(AlterationTargetType targetType, out Point sourceLocation)
        {
            var playerLocation = new Point(Canvas.GetLeft(this.TheSmiley), Canvas.GetTop(this.TheSmiley));
            var enemy1Location = new Point(Canvas.GetLeft(this.TheEnemy), Canvas.GetTop(this.TheEnemy));
            var enemy2Location = new Point(Canvas.GetLeft(this.TheSecondEnemy), Canvas.GetTop(this.TheSecondEnemy));
            var enemy3Location = new Point(Canvas.GetLeft(this.TheThirdEnemy), Canvas.GetTop(this.TheThirdEnemy));

            playerLocation.X += ModelConstants.CellWidth / 2.0D;
            playerLocation.Y += ModelConstants.CellHeight / 2.0D;
            enemy1Location.X += ModelConstants.CellWidth / 2.0D;
            enemy1Location.Y += ModelConstants.CellHeight / 2.0D;
            enemy2Location.X += ModelConstants.CellWidth / 2.0D;
            enemy2Location.Y += ModelConstants.CellHeight / 2.0D;
            enemy3Location.X += ModelConstants.CellWidth / 2.0D;
            enemy3Location.Y += ModelConstants.CellHeight / 2.0D;

            sourceLocation = playerLocation;

            switch (targetType)
            {
                case AlterationTargetType.Source:
                    return new Point[] { playerLocation };
                case AlterationTargetType.Target:
                    return new Point[] { new Point[] { enemy1Location, enemy2Location, enemy3Location }.PickRandom() };
                case AlterationTargetType.AllInRange:
                    return new Point[] { enemy1Location, enemy2Location, enemy3Location, playerLocation };
                case AlterationTargetType.AllInRangeExceptSource:
                    return new Point[] { enemy1Location, enemy2Location, enemy3Location };
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
