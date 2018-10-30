using Rogue.NET.Core.Media;
using Rogue.NET.Core.Media.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    public partial class AnimationPreviewControl : UserControl
    {
        readonly IAnimationGenerator _animationGenerator;

        ITimedGraphic _animation = null;

        bool _updating = false;

        [ImportingConstructor]
        public AnimationPreviewControl(IAnimationGenerator animationGenerator)
        {
            _animationGenerator = animationGenerator;

            InitializeComponent();
        }
        private void PlayAnimation()
        {
            var tmp = this.DataContext as AnimationTemplate;

            this.AnimationSlider.Maximum = tmp.AnimationTime / 1000.0;

            _animation = CreateNewAnimation(tmp);
            StartAnimation();
        }
        private void StartAnimation()
        {
            Graphic[] graphics = _animation.GetGraphics();
            foreach (Graphic g in graphics)
            {
                Canvas.SetZIndex(g, 100);
                this.TheCanvas.Children.Add(g);
            }
            _animation.TimeElapsed += new TimerElapsedHandler(OnEndAnimation);
            _animation.AnimationTimeChanged += (obj, e) =>
            {
                _updating = true;
                this.AnimationSlider.Value = e.CurrentTimeMilliseconds / 1000.0;
                _updating = false;
            };
            _animation.Start();
            //UpdateLayout();
        }
        private void OnEndAnimation(ITimedGraphic sender)
        {
            Graphic[] graphics = sender.GetGraphics();
            foreach (Graphic g in graphics)
                this.TheCanvas.Children.Remove(g);
            sender.TimeElapsed -= new TimerElapsedHandler(OnEndAnimation);
            sender.Stop();
            sender.CleanUp();
            sender = null;

            _animation = null;
        }
        private ITimedGraphic CreateNewAnimation(AnimationTemplate tmp)
        {
            Point p = new Point(Canvas.GetLeft(this.TheSmiley), Canvas.GetTop(this.TheSmiley));
            Point en1 = new Point(Canvas.GetLeft(this.TheEnemy), Canvas.GetTop(this.TheEnemy));
            Point en2 = new Point(Canvas.GetLeft(this.TheSecondEnemy), Canvas.GetTop(this.TheSecondEnemy));
            Point en3 = new Point(Canvas.GetLeft(this.TheThirdEnemy), Canvas.GetTop(this.TheThirdEnemy));
            p.X += 5;
            p.Y += 8;
            en1.X += 5;
            en1.Y += 8;
            en2.X += 5;
            en2.Y += 8;
            en3.X += 5;
            en3.Y += 8;
            return _animationGenerator.CreateAnimation(tmp, new Rect(this.TheCanvas.RenderSize), p, new Point[] { en1, en2, en3 });
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_animation != null)
            {
                OnEndAnimation(_animation);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (_animation != null)
            {
                OnEndAnimation(_animation);
                PlayAnimation();
            }

            else
                PlayAnimation();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_animation != null)
            {
                if (!_animation.IsPaused)
                    _animation.Pause();
                else
                    _animation.Resume();
            }
        }

        private void AnimationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_animation != null && !_updating)
                _animation.Seek((int)(e.NewValue * 1000));
        }
    }
}
