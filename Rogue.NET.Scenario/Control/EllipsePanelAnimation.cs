using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Rogue.NET.Scenario.Control
{
    /// <summary>
    /// Animation class with methods that are scaled to [0, 1] (beginning -> end)
    /// </summary>
    public class EllipsePanelAnimation
    {
        private const int TOTAL_MILLISEC = 1000;

        AnimationClock _xAnimationClock;
        AnimationClock _yAnimationClock;
        AnimationClock _opacityAnimationClock;
        AnimationClock _widthAnimationClock;
        AnimationClock _heightAnimationClock;

        bool _clockReady = false;
        double _relativeOffset = 0;

        /// <summary>
        /// Create an animation using the provided ellipse geometry for the given FrameworkElement. A relative offset
        /// (from 0 to 1) is used to specify where the starting point is for the animation. This must be provided by the 
        /// owner.
        /// </summary>
        public EllipsePanelAnimation()
        {
        }

        public void SetRelativeOffset(double relativeOffset)
        {
            _relativeOffset = relativeOffset;
        }

        /// <summary>
        /// Defines geometry for animation clocks. Call to re-initialize clocks
        /// </summary>
        public void DefineGeometry(PathGeometry geometry, FrameworkElement element)
        {
            CreateClocks(geometry, element);
        }

        /// <summary>
        /// Seeks to point in animation (0 -> 1)
        /// </summary>
        public void Seek(double point)
        {
            if (!_clockReady)
                throw new Exception("Must start clocks before calling Seek");

            var offset = point + _relativeOffset;

            if (offset > 1)
                offset = offset % 1;

            var position = TimeSpan.FromMilliseconds(offset * TOTAL_MILLISEC);

            _xAnimationClock.Controller.Seek(position, TimeSeekOrigin.BeginTime);
            _yAnimationClock.Controller.Seek(position, TimeSeekOrigin.BeginTime);
            _opacityAnimationClock.Controller.Seek(position, TimeSeekOrigin.BeginTime);
            _widthAnimationClock.Controller.Seek(position, TimeSeekOrigin.BeginTime);
            _heightAnimationClock.Controller.Seek(position, TimeSeekOrigin.BeginTime);

            // TODO: Set Z-Index
        }

        private void StopClocks()
        {
            if (_clockReady)
            {
                _xAnimationClock.Controller.Stop();
                _yAnimationClock.Controller.Stop();
                _opacityAnimationClock.Controller.Stop();
                _widthAnimationClock.Controller.Stop();
                _heightAnimationClock.Controller.Stop();

                _xAnimationClock = null;
                _yAnimationClock = null;
                _opacityAnimationClock = null;
                _widthAnimationClock = null;
                _heightAnimationClock = null;

                _clockReady = false;
            }
        }

        /// <summary>
        /// Creates animation clocks and attaches them to the element opacity and render transform
        /// </summary>
        private void CreateClocks(PathGeometry geometry, FrameworkElement element)
        {
            StopClocks();

            var scaleTransform = new ScaleTransform();
            //var translateTransformCenter = new TranslateTransform(-1 * element.RenderSize.Width / 2, -1 * element.RenderSize.Height / 2);
            var translateTransform = new TranslateTransform();

            var xAnimation = new DoubleAnimationUsingPath();
            xAnimation.PathGeometry = geometry;
            xAnimation.Source = PathAnimationSource.X;
            xAnimation.Duration = TimeSpan.FromMilliseconds(TOTAL_MILLISEC);
            xAnimation.BeginTime = TimeSpan.FromMilliseconds(0);

            var yAnimation = new DoubleAnimationUsingPath();
            yAnimation.PathGeometry = geometry;
            yAnimation.Source = PathAnimationSource.Y;
            yAnimation.Duration = TimeSpan.FromMilliseconds(TOTAL_MILLISEC);
            yAnimation.BeginTime = TimeSpan.FromMilliseconds(0);

            var opacity = new DoubleAnimationUsingKeyFrames();
            opacity.Duration = TimeSpan.FromMilliseconds(TOTAL_MILLISEC);
            opacity.BeginTime = TimeSpan.FromMilliseconds(0);
            var opacityKeyFrame1 = new LinearDoubleKeyFrame(0.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)));
            var opacityKeyFrame2 = new LinearDoubleKeyFrame(1,    KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((TOTAL_MILLISEC / 4) - 10)));
            var opacityKeyFrame3 = new LinearDoubleKeyFrame(1,    KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(TOTAL_MILLISEC / 4)));
            var opacityKeyFrame4 = new LinearDoubleKeyFrame(1,    KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((TOTAL_MILLISEC / 4) + 10)));
            var opacityKeyFrame5 = new LinearDoubleKeyFrame(0.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(TOTAL_MILLISEC / 2)));
            var opacityKeyFrame6 = new LinearDoubleKeyFrame(0.5,  KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((TOTAL_MILLISEC * 3) / 4)));
            var opacityKeyFrame7 = new LinearDoubleKeyFrame(0.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(TOTAL_MILLISEC)));
            opacity.KeyFrames.Add(opacityKeyFrame1);
            opacity.KeyFrames.Add(opacityKeyFrame2);
            opacity.KeyFrames.Add(opacityKeyFrame3);
            opacity.KeyFrames.Add(opacityKeyFrame4);
            opacity.KeyFrames.Add(opacityKeyFrame5);
            opacity.KeyFrames.Add(opacityKeyFrame6);
            opacity.KeyFrames.Add(opacityKeyFrame7);

            var widthAnimation = new DoubleAnimationUsingKeyFrames();
            widthAnimation.Duration = TimeSpan.FromMilliseconds(TOTAL_MILLISEC);
            widthAnimation.BeginTime = TimeSpan.FromMilliseconds(0);
            var widthKeyFrame1 = new LinearDoubleKeyFrame(.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)));
            var widthKeyFrame2 = new LinearDoubleKeyFrame(1,   KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((TOTAL_MILLISEC / 4) - 10)));
            var widthKeyFrame3 = new LinearDoubleKeyFrame(1,   KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(TOTAL_MILLISEC / 4)));
            var widthKeyFrame4 = new LinearDoubleKeyFrame(1,   KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((TOTAL_MILLISEC / 4) + 10)));
            var widthKeyFrame5 = new LinearDoubleKeyFrame(.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(TOTAL_MILLISEC / 2)));
            var widthKeyFrame6 = new LinearDoubleKeyFrame(.5,  KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((TOTAL_MILLISEC * 3) / 4)));
            var widthKeyFrame7 = new LinearDoubleKeyFrame(.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(TOTAL_MILLISEC)));
            widthAnimation.KeyFrames.Add(widthKeyFrame1);
            widthAnimation.KeyFrames.Add(widthKeyFrame2);
            widthAnimation.KeyFrames.Add(widthKeyFrame3);
            widthAnimation.KeyFrames.Add(widthKeyFrame4);
            widthAnimation.KeyFrames.Add(widthKeyFrame5);
            widthAnimation.KeyFrames.Add(widthKeyFrame6);
            widthAnimation.KeyFrames.Add(widthKeyFrame7);

            var heightAnimation = new DoubleAnimationUsingKeyFrames();
            heightAnimation.Duration = TimeSpan.FromMilliseconds(TOTAL_MILLISEC);
            heightAnimation.BeginTime = TimeSpan.FromMilliseconds(0);
            var heightKeyFrame1 = new LinearDoubleKeyFrame(.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)));
            var heightKeyFrame2 = new LinearDoubleKeyFrame(1,   KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((TOTAL_MILLISEC / 4) - 10)));
            var heightKeyFrame3 = new LinearDoubleKeyFrame(1,   KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(TOTAL_MILLISEC / 4)));
            var heightKeyFrame4 = new LinearDoubleKeyFrame(1,   KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((TOTAL_MILLISEC / 4) + 10)));
            var heightKeyFrame5 = new LinearDoubleKeyFrame(.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(TOTAL_MILLISEC / 2)));
            var heightKeyFrame6 = new LinearDoubleKeyFrame(.5,  KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((TOTAL_MILLISEC * 3) / 4)));
            var heightKeyFrame7 = new LinearDoubleKeyFrame(.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(TOTAL_MILLISEC)));
            heightAnimation.KeyFrames.Add(heightKeyFrame1);
            heightAnimation.KeyFrames.Add(heightKeyFrame2);
            heightAnimation.KeyFrames.Add(heightKeyFrame3);
            heightAnimation.KeyFrames.Add(heightKeyFrame4);
            heightAnimation.KeyFrames.Add(heightKeyFrame5);
            heightAnimation.KeyFrames.Add(heightKeyFrame6);
            heightAnimation.KeyFrames.Add(heightKeyFrame7);

            // Create the clocks
            _xAnimationClock = xAnimation.CreateClock();
            _yAnimationClock = yAnimation.CreateClock();
            _opacityAnimationClock = opacity.CreateClock();
            _widthAnimationClock = widthAnimation.CreateClock();
            _heightAnimationClock = heightAnimation.CreateClock();

            // Apply clocks to render transforms
            translateTransform.ApplyAnimationClock(TranslateTransform.XProperty, _xAnimationClock);
            translateTransform.ApplyAnimationClock(TranslateTransform.YProperty, _yAnimationClock);
            element.ApplyAnimationClock(FrameworkElement.OpacityProperty, _opacityAnimationClock);
            scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleXProperty, _widthAnimationClock);
            scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleYProperty, _heightAnimationClock);

            _xAnimationClock.Controller.Begin();
            _yAnimationClock.Controller.Begin();
            _opacityAnimationClock.Controller.Begin();
            _widthAnimationClock.Controller.Begin();
            _heightAnimationClock.Controller.Begin();

            _xAnimationClock.Controller.Pause();
            _yAnimationClock.Controller.Pause();
            _opacityAnimationClock.Controller.Pause();
            _widthAnimationClock.Controller.Pause();
            _heightAnimationClock.Controller.Pause();

            _clockReady = true;

            // Set render transform
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(scaleTransform);            // Animated
            transformGroup.Children.Add(translateTransform);        // Animated
            //transformGroup.Children.Add(translateTransformCenter);  // Static

            element.RenderTransform = transformGroup;
        }
    }
}
