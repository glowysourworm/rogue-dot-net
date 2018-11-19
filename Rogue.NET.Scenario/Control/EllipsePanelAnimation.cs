using Rogue.NET.Core.Media;
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
    /// Create an animation using the provided ellipse geometry for the given FrameworkElement. A relative offset
    /// (from 0 to 1) is used to specify where the starting point is for the animation. This must be provided by the 
    /// owner.
    /// </summary>
    public class EllipsePanelAnimation
    {
        private const int TOTAL_MILLISEC = 1000;

        TranslateTransform _translateTransform;
        ScaleTransform _scaleTransform;

        FrameworkElement _element;

        TimeSpan _currentEndTime;

        bool _clockReady = false;
        double _relativeOffset = 0;

        public FrameworkElement Element { get { return _element; } }

        public double Offset { get; set; }

        public EllipsePanelAnimation(FrameworkElement element)
        {
            _scaleTransform = new ScaleTransform();
            _translateTransform = new TranslateTransform();

            // Set render transform
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(_scaleTransform);            // Animated
            transformGroup.Children.Add(_translateTransform);        // Animated

            element.RenderTransform = transformGroup;

            _element = element;
        }


        /// <summary>
        /// Animates target element along the specified path until it has reached offsetEnd. The Offset public parameter
        /// here and offsetEnd are relative to a [0, 1) path geometry trajectory (repeating)
        /// </summary>
        /// <param name="offsetEnd">the final desired value for the offset (current offset is set publicly)</param>
        public void Animate(PathGeometry pathGeometry, double offsetEnd)
        {
            // Have to calculate the start and end parameters for the elliptical path of the element. These
            // are started here and then let to animate until the clocks expire

            // 0) Calculate the path geometry involved for the ellipse portion to animate (use key frame collection)
            // 1) Calculate opacity key frames
            // 2) Calculate scael key frames

            // Path Key Frames
            var xKeyFrames = new List<LinearDoubleKeyFrame>();
            var yKeyFrames = new List<LinearDoubleKeyFrame>();
            var scalingKeyFrames = new List<LinearDoubleKeyFrame>();

            // Increment position until end point is reached
            var position = this.Offset;
            var progress = 0D;
            var increment = (offsetEnd - this.Offset) / 100.0D; // Create 100 key frames
            do
            {
                var unitPosition = position < 0 ? position + (int)position + 1 : 
                                   position > 0 ? position - (int)position :
                                   position;

                // take the position % 1 to give you a unit position [0, 1)
                var scalingFactor = CalculateScalingFactor(unitPosition % 1);

                // Get Point at Fraction Length must be relative to [0, 1). So, use modulo relative position to calculate
                Point point, tangent;
                pathGeometry.GetPointAtFractionLength(unitPosition % 1, out point, out tangent);

                // Path
                xKeyFrames.Add(new LinearDoubleKeyFrame(point.X, KeyTime.FromPercent(progress)));
                yKeyFrames.Add(new LinearDoubleKeyFrame(point.Y, KeyTime.FromPercent(progress)));

                // Scaling
                scalingKeyFrames.Add(new LinearDoubleKeyFrame(scalingFactor, KeyTime.FromPercent(progress)));

                // Increment carries the sign for the position. Progress is measure from [0,1) so for 100 key
                // frames - increment by 0.01
                position += increment;
                progress = 1 - Math.Abs((position - offsetEnd) / (this.Offset - offsetEnd));
            }
            while (increment < 0 ? position > offsetEnd : position < offsetEnd);

            // Calculate Animation parameters
            var duration = TimeSpan.FromMilliseconds((Math.Abs(offsetEnd - this.Offset) % 1) * TOTAL_MILLISEC);

            // X Animation
            var xAnimation = new DoubleAnimationUsingKeyFrames();
            xAnimation.Duration = duration;
            xAnimation.KeyFrames.AddRange(xKeyFrames);

            // Y Animation
            var yAnimation = new DoubleAnimationUsingKeyFrames();
            yAnimation.Duration = duration;
            yAnimation.KeyFrames.AddRange(yKeyFrames);

            // Opacity Animation
            var opacityAnimation = new DoubleAnimationUsingKeyFrames();
            opacityAnimation.Duration = duration;
            opacityAnimation.KeyFrames.AddRange(scalingKeyFrames);

            // Width Animation
            var widthAnimation = new DoubleAnimationUsingKeyFrames();
            widthAnimation.Duration = duration;
            widthAnimation.KeyFrames.AddRange(scalingKeyFrames);

            // Height Animation
            var heightAnimation = new DoubleAnimationUsingKeyFrames();
            heightAnimation.Duration = duration;
            heightAnimation.KeyFrames.AddRange(scalingKeyFrames);

            // Apply clocks to render transforms
            _translateTransform.ApplyAnimationClock(TranslateTransform.XProperty, xAnimation.CreateClock());
            _translateTransform.ApplyAnimationClock(TranslateTransform.YProperty, yAnimation.CreateClock());
            _element.ApplyAnimationClock(FrameworkElement.OpacityProperty, opacityAnimation.CreateClock());
            _scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleXProperty, widthAnimation.CreateClock());
            _scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleYProperty, heightAnimation.CreateClock());

            // UPDATE THE OFFSET
            this.Offset = offsetEnd;
        }

        /// <summary>
        /// Returns a fraction based on the absolute offset [0,1) in the animation
        /// timeline. This will scale the opacity and size of the animated parameters.
        /// </summary>
        private double CalculateScalingFactor(double offset)
        {
            // Interpolate based on these positions
            if (offset < 0.25)
                return offset + 0.75;

            else if (offset < 0.75)
                return (-1 * offset) + 1.25;

            else
                return offset - 0.25;
        }
    }

    public static class AnimationCollectionExtensions
    {
        public static void AddRange<T>(this DoubleKeyFrameCollection collection, IEnumerable<T> source) where T : DoubleKeyFrame
        {
            foreach (var keyFrame in source)
                collection.Add(keyFrame);
        }
    }
}
