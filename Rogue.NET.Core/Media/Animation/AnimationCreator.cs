using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

using Rogue.NET.Core.Model.Enums;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Media.Animation.Interface;
using Rogue.NET.Core.Media.Animation.Extension;

namespace Rogue.NET.Core.Media.Animation
{
    [Export(typeof(IAnimationCreator))]
    public class AnimationCreator : IAnimationCreator
    {
        IRandomSequenceGenerator _randomSequenceGenerator;

        [ImportingConstructor]
        public AnimationCreator(IRandomSequenceGenerator randomSequenceGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public AnimationPrimitiveGroup CreateAnimation(AnimationBase animation, Rect bounds, Point sourceLocation, Point[] targetLocations)
        {
            // Calculate point targets
            var pointTargetLocations = CalculatePointTargets(animation.PointTargetType, sourceLocation, targetLocations);

            // Create animation primitive group
            if (animation is AnimationAura)
            {
                var pointAnimations = pointTargetLocations.Select(x => CreateAura(animation as AnimationAura, x));

                return CombineAnimationGroups(pointAnimations);
            }
            else if (animation is AnimationBarrage)
            {
                var pointAnimations = pointTargetLocations.Select(x => CreateBarrage(animation as AnimationBarrage, x));

                return CombineAnimationGroups(pointAnimations);
            }
            else if (animation is AnimationBlink)
            {
                return CreateBlink(animation as AnimationBlink, bounds);
            }
            else if (animation is AnimationBubbles)
            {
                var pointAnimations = pointTargetLocations.Select(x => CreateBubbles(animation as AnimationBubbles, x));

                return CombineAnimationGroups(pointAnimations);
            }
            else if (animation is AnimationChain)
            {
                var animationChain = animation as AnimationChain;
                var points = new List<Point>();

                // Re-calculating the source / targets for the animation to order them
                var source = animationChain.Reverse ? targetLocations.PickRandom() : sourceLocation;
                var targets = targetLocations.Append(sourceLocation).Except(new Point[] { source });
                points.Add(source);
                points.AddRange(targets);

                return CreateChain(animationChain, points.ToArray());
            }
            else if (animation is AnimationChainConstantVelocity)
            {
                var animationChain = animation as AnimationChainConstantVelocity;
                var points = new List<Point>();

                // Re-calculating the source / targets for the animation to order them
                var source = animationChain.Reverse ? targetLocations.PickRandom() : sourceLocation;
                var targets = targetLocations.Append(sourceLocation).Except(new Point[] { source });
                points.Add(source);
                points.AddRange(targets);

                return CreateChainConstantVelocity(animationChain, points.ToArray());
            }
            else if (animation is AnimationLightning)
            {
                return CreateLightning(animation as AnimationLightning, sourceLocation, targetLocations);
            }
            else if (animation is AnimationLightningChain)
            {
                var points = new List<Point>();

                // Re-calculating the source / targets for the animation to order them
                points.Add(sourceLocation);
                points.AddRange(targetLocations.Shuffle());

                return CreateLightningChain(animation as AnimationLightningChain, points.ToArray());
            }
            else if (animation is AnimationProjectile)
            {
                // Can use the select statement because the animation times are the same
                var pathAnimations = targetLocations.Select(point => CreateProjectile(animation as AnimationProjectile, sourceLocation, point));

                return CombineAnimationGroups(pathAnimations);
            }
            else if (animation is AnimationProjectileConstantVelocity)
            {
                return CreateProjectileConstantVelocity(animation as AnimationProjectileConstantVelocity, sourceLocation, targetLocations.ToArray());
            }
            else if (animation is AnimationSpiral)
            {
                var pointAnimations = pointTargetLocations.Select(x => CreateSpiral(animation as AnimationSpiral, x));

                return CombineAnimationGroups(pointAnimations);
            }
            else
                throw new Exception("Unhandled Animation Type AnimationCreator");
        }

        public AnimationPrimitiveGroup CreateTargetingAnimation(Point point, Color fillColor)
        {
            var length = 20;
            var cellBounds = new Rect(point, new Size(10, 15));
            var verticalSize = new Size(1, 8);
            var pointSize = new Size(1, 1);

            var animationTime = 300;

            var fill = new SolidColorBrush(fillColor);

            var northWest = new Point(point.X - length, point.Y - length);
            var northEast = new Point(cellBounds.TopRight.X + length, cellBounds.TopRight.Y - length);
            var southEast = new Point(cellBounds.BottomRight.X + length, cellBounds.BottomRight.Y + length);
            var southWest = new Point(cellBounds.BottomLeft.X - length, cellBounds.BottomLeft.Y + length);

            var path1 = CreatePathGeometry(new Point[] { northWest, cellBounds.TopLeft });
            var path2 = CreatePathGeometry(new Point[] { northEast, cellBounds.TopRight });
            var path3 = CreatePathGeometry(new Point[] { southEast, cellBounds.BottomRight });
            var path4 = CreatePathGeometry(new Point[] { southWest, cellBounds.BottomLeft });

            var animation1 = GeneratePathAnimation(path1, fill, pointSize, verticalSize, 1, 0, 1, animationTime, int.MaxValue, true, AnimationEasingType.None, 0);
            var animation2 = GeneratePathAnimation(path2, fill, pointSize, verticalSize, 1, 0, 1, animationTime, int.MaxValue, true, AnimationEasingType.None, 0);
            var animation3 = GeneratePathAnimation(path3, fill, pointSize, verticalSize, 1, 0, 1, animationTime, int.MaxValue, true, AnimationEasingType.None, 0);
            var animation4 = GeneratePathAnimation(path4, fill, pointSize, verticalSize, 1, 0, 1, animationTime, int.MaxValue, true, AnimationEasingType.None, 0);
            var animation5 = GenerateTimedFigure(new RectangleGeometry(cellBounds), fill, 1, 0, animationTime, int.MaxValue, true);

            return new AnimationPrimitiveGroup(new AnimationPrimitive[] { animation1, animation2, animation3, animation4, animation5 }, animationTime);
        }

        #region (private) Animation Methods
        private AnimationPrimitiveGroup CreateChain(AnimationChain animation, Point[] points)
        {
            var path = CreatePathGeometry(points);

            var primitive = GeneratePathAnimation(path, animation.FillTemplate.GenerateBrush(),
                                                     new Size(animation.Width1, animation.Height1),
                                                     new Size(animation.Width2, animation.Height2),
                                                     animation.Opacity1, animation.Opacity2, animation.Erradicity,
                                                     animation.AnimationTime, animation.RepeatCount, animation.AutoReverse, 
                                                     animation.EasingType, animation.EasingAmount);

            return new AnimationPrimitiveGroup(new AnimationPrimitive[] { primitive }, animation.AnimationTime);
        }

        private AnimationPrimitiveGroup CreateChainConstantVelocity(AnimationChainConstantVelocity animation, Point[] points)
        {
            var animationGroups = new List<AnimationPrimitiveGroup>();

            var path = CreatePathGeometry(points);

            var pathLength = CalculateLength(points);

            var animationTime = (int)((pathLength / animation.Velocity) * 1000.0);

            var primitive = GeneratePathAnimation(path, animation.FillTemplate.GenerateBrush(),
                                                     new Size(animation.Width1, animation.Height1),
                                                     new Size(animation.Width2, animation.Height2),
                                                     animation.Opacity1, animation.Opacity2, animation.Erradicity,
                                                     animationTime, animation.RepeatCount, animation.AutoReverse,
                                                     animation.EasingType, animation.EasingAmount);

            return new AnimationPrimitiveGroup(new AnimationPrimitive[] { primitive }, animationTime);
        }

        private AnimationPrimitiveGroup CreateProjectile(AnimationProjectile animation, Point point1, Point point2)
        {
            var path = CreatePathGeometry(new Point[] { point1, point2 });

            var primitive = GeneratePathAnimation(path, animation.FillTemplate.GenerateBrush(),
                                                     new Size(animation.Width1, animation.Height1),
                                                     new Size(animation.Width2, animation.Height2),
                                                     animation.Opacity1, animation.Opacity2, animation.Erradicity,
                                                     animation.AnimationTime, animation.RepeatCount, animation.AutoReverse,
                                                     animation.EasingType, animation.EasingAmount);

            return new AnimationPrimitiveGroup(new AnimationPrimitive[] { primitive }, animation.AnimationTime);
        }

        private AnimationPrimitiveGroup CreateProjectileConstantVelocity(AnimationProjectileConstantVelocity animation, Point sourceLocation, Point[] targetLocations)
        {
            // Have to match animation times for each target location in order to create just a single primitive group
            var maxLength = targetLocations.Max(x => CalculateLength(sourceLocation, x));
            var maxAnimationTime = (int)(maxLength / (animation as AnimationProjectileConstantVelocity).Velocity);

            var primitives = new List<AnimationPrimitive>();

            for (int i=0;i<targetLocations.Length - 1;i++)
            {
                var path = CreatePathGeometry(new Point[] { targetLocations[i], targetLocations[i + 1] });

                var primitive = GeneratePathAnimation(path, animation.FillTemplate.GenerateBrush(),
                                                         new Size(animation.Width1, animation.Height1),
                                                         new Size(animation.Width2, animation.Height2),
                                                         animation.Opacity1, animation.Opacity2, animation.Erradicity,
                                                         maxAnimationTime, animation.RepeatCount, animation.AutoReverse,
                                                         animation.EasingType, animation.EasingAmount);

                primitives.Add(primitive);
            }


            return new AnimationPrimitiveGroup(primitives, maxAnimationTime);
        }

        private AnimationPrimitiveGroup CreateBarrage(AnimationBarrage animation, Point focus)
        {
            var primitives = new List<AnimationPrimitive>();

            for (int i = 0; i < animation.ChildCount; i++)
            {
                double angle = _randomSequenceGenerator.Get() * Math.PI * 2;
                double radius = _randomSequenceGenerator.Get() * animation.Radius;

                var point = new Point(focus.X + (radius * Math.Cos(angle)), 
                                      focus.Y + (radius * Math.Sin(angle)));

                var path = animation.Reverse ? CreatePathGeometry(new Point[] { focus, point }) :
                                               CreatePathGeometry(new Point[] { point, focus });

                // Generate the path animation
                primitives.Add(GeneratePathAnimation(path, animation.FillTemplate.GenerateBrush(),
                                                     new Size(animation.Width1, animation.Height1),
                                                     new Size(animation.Width2, animation.Height2),
                                                     animation.Opacity1, animation.Opacity2, animation.Erradicity,
                                                     animation.AnimationTime, 1, false,
                                                     animation.EasingType, animation.EasingAmount));
            }

            return new AnimationPrimitiveGroup(primitives, animation.AnimationTime);
        }

        private AnimationPrimitiveGroup CreateSpiral(AnimationSpiral animation, Point focus)
        {
            var primitives = new List<AnimationPrimitive>();

            // Calculate # of rotations - (convert rotations / second -> rotations / millisecond)
            var rotations = animation.AnimationTime * animation.RotationRate * 0.001;

            // Calculate spiral rate (parameter is in units of pixels / second) -> (pixels / rotation)
            var spiralRate = animation.RadiusChangeRate / animation.RotationRate;

            // Foreach ellipse on the spiral
            for (int i = 0; i < animation.ChildCount; i++)
            {
                // Calculate point to start the animation
                var points = new List<Point>();

                double startAngle = (2 * Math.PI * i) / animation.ChildCount;
                double angle = startAngle;
                double angleIncrement = 0.1;

                // Calculate points in the spiral to animate
                while (angle < ((2 * Math.PI * rotations) + startAngle))
                {
                    // Calculate Spiral Rate Offset
                    var spiralRateOffset = (angle - startAngle) * spiralRate;

                    // Calculate amplitude
                    var amplitude = animation.Radius + (spiralRateOffset);

                    // Calculate path point
                    var point = new Point((amplitude * Math.Cos(animation.Clockwise ? angle : -1 * angle)) + focus.X, 
                                          (amplitude * Math.Sin(animation.Clockwise ? angle : -1 * angle)) + focus.Y);

                    points.Add(point);

                    angle += angleIncrement;
                }

                // Create path using animation points
                var path = CreatePathGeometry(points);

                // Generate the path animation
                primitives.Add(GeneratePathAnimation(path, animation.FillTemplate.GenerateBrush(), 
                                                     new Size(animation.Width1, animation.Height1),
                                                     new Size(animation.Width2, animation.Height2),
                                                     animation.Opacity1, animation.Opacity2, animation.Erradicity, 
                                                     animation.AnimationTime, 1, false,
                                                     animation.EasingType, animation.EasingAmount));
            }

            return new AnimationPrimitiveGroup(primitives, animation.AnimationTime);
        }

        private AnimationPrimitiveGroup CreateAura(AnimationAura animation, Point location)
        {
            var primitive = GeneratePointAnimation(location,
                                                   animation.FillTemplate.GenerateBrush(),
                                                   new Size(animation.Width1, animation.Height1),
                                                   new Size(animation.Width2, animation.Height2),
                                                   animation.Opacity1, animation.Opacity2,
                                                   1, animation.AnimationTime, animation.RepeatCount, 
                                                   animation.AutoReverse, animation.EasingType, animation.EasingAmount);

            return new AnimationPrimitiveGroup(new AnimationPrimitive[] { primitive }, animation.AnimationTime);
        }

        private AnimationPrimitiveGroup CreateBubbles(AnimationBubbles animation, Point focus)
        {
            var result = new List<AnimationPrimitive>();

            // Create roam points for the bubbles
            for (int i = 0; i < animation.ChildCount; i++)
            {
                var roamPoints = new List<Point>();

                //Get random point in radius from focus
                var angle = _randomSequenceGenerator.Get() * Math.PI * 2;
                var radius = _randomSequenceGenerator.Get() * animation.Radius;
                roamPoints.Add(new Point(radius * Math.Cos(angle) + focus.X, radius * Math.Sin(angle) + focus.Y));

                // Calculate roam points
                for (int j = 0; j < Math.Max(2, animation.Erradicity); j++)
                {
                    var lastRoamPoint = roamPoints.Last();

                    //Get random point in roam radius from that random point
                    double roam_a = _randomSequenceGenerator.Get() * Math.PI * 2;
                    double roam_r = _randomSequenceGenerator.Get() * animation.RoamRadius;
                    var roamPoint = new Point(roam_r * Math.Cos(roam_a) + lastRoamPoint.X, roam_r * Math.Sin(roam_a) + lastRoamPoint.Y);
                    roamPoints.Add(roamPoint);
                }

                // Create path for the animation
                var path = CreatePathGeometry(roamPoints);

                // Create animations for this bubble
                result.Add(GeneratePathAnimation(path, animation.FillTemplate.GenerateBrush(), 
                                                 new Size(animation.Width1, animation.Height1), 
                                                 new Size(animation.Width2, animation.Height2), 
                                                 animation.Opacity1, animation.Opacity2, animation.Erradicity, 
                                                 animation.AnimationTime, 1, false,
                                                 animation.EasingType, animation.EasingAmount));
            }
            return new AnimationPrimitiveGroup(result, animation.AnimationTime);
        }

        private AnimationPrimitiveGroup CreateBlink(AnimationBlink animation, Rect bounds)
        {
            var primitive = GenerateTimedFigure(new RectangleGeometry(bounds), animation.FillTemplate.GenerateBrush(),
                                                animation.Opacity1, animation.Opacity2,
                                                animation.AnimationTime, animation.RepeatCount, animation.AutoReverse);

            return new AnimationPrimitiveGroup(new AnimationPrimitive[] { primitive }, animation.AnimationTime);
        }

        private AnimationPrimitiveGroup CreateLightning(AnimationLightning animation, Point sourceLocation, Point[] targetLocations)
        {
            // Create animatino groups with the same animation time
            var animationGroups = targetLocations.Select(target => CreateLightning(new Point[] { sourceLocation, target },
                                                                                   animation.AnimationTime,
                                                                                   animation.HoldEndTime,
                                                                                   animation.IncrementHeightLimit,
                                                                                   animation.IncrementWidthLimit,
                                                                                   animation.FillTemplate.GenerateBrush()));

            // Combine the groups to create a "forked lightning" effect
            return CombineAnimationGroups(animationGroups);
        }

        private AnimationPrimitiveGroup CreateLightningChain(AnimationLightningChain animation, Point[] points)
        {
            return CreateLightning(points,
                                   animation.AnimationTime,
                                   animation.HoldEndTime,
                                   animation.IncrementHeightLimit,
                                   animation.IncrementWidthLimit,
                                   animation.FillTemplate.GenerateBrush());
        }

        private AnimationPrimitiveGroup CreateLightning(Point[] points, int animationTime, int holdEndTime, int incrementHeightLimit, int incrementWidthLimit, Brush fill)
        {
            // Procedure:  Generate "Random Walk" toward the target
            //
            // 1) Create line segment with startup delay
            // 2) Hold opacity at 1 until all segments have been created
            // 3) Fade opacity for HoldEndTime

            var primitives = new List<AnimationPrimitive>();

            var totalPathLength = CalculateLength(points);
            var totalAnimationTime = animationTime + holdEndTime;

            var accumulatedPathLength = 0.0;

            for (int i = 0; i < points.Length - 1; i++)
            {
                var point1 = points[i];
                var point2 = points[i + 1];
                var pathLength = CalculateLength(point1, point2);

                var angle = Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
                var lastPoint = new Point(point1.X, point1.Y);
                var lastLightningPoint = new Point(point1.X, point1.Y);

                var pathAnimationTime = (int)(animationTime * (pathLength / totalPathLength));
                var pathAnimationTimeOffset = (int)(animationTime * (accumulatedPathLength / totalPathLength));
                
                // Start incrementing towards the target
                var distance = 0.0;
                var sign = 1;

                while (distance < pathLength)
                {
                    // Clip the increment at the end point
                    var maxIncrementX = pathLength - distance;

                    // Oscillate between positive and negative amplitude
                    sign *= -1;

                    // Coordinates relative to the rotated frame
                    var incrementX = (_randomSequenceGenerator.Get() * incrementWidthLimit).Clip(1, maxIncrementX);
                    var incrementY = sign * _randomSequenceGenerator.Get() * incrementHeightLimit;

                    // Angle' = Angle + Increment Angle (which is random)
                    var incrementAngle = Math.Atan2(incrementY, incrementX);
                    var incrementHypotenuse = Math.Sqrt((incrementX * incrementX) + (incrementY * incrementY));

                    // Angle'
                    var totalAngle = angle + incrementAngle;

                    // Get point along new vector with the combined angle (from last point)
                    var lightningPoint = new Point(incrementHypotenuse * Math.Cos(totalAngle) + lastPoint.X,
                                                   incrementHypotenuse * Math.Sin(totalAngle) + lastPoint.Y);

                    // Calculate time required for this section of the animation         
                    var segmentAnimationOffset = (int)(pathAnimationTime * (distance / pathLength));
                    var absoluteAnimationOffset = pathAnimationTimeOffset + segmentAnimationOffset;

                    // Create animated figure for this line segment
                    var geometry = new LineGeometry(lastLightningPoint, lightningPoint);
                    var primitive = GenerateLightningFigure(geometry, fill,1.5, 1, 0, totalAnimationTime, absoluteAnimationOffset, 1, false);

                    // Add stroke for the lightning
                    primitive.Stroke = fill;
                    primitive.StrokeThickness = 1.5;

                    primitives.Add(primitive);

                    // Calculate point along vector as the "last point"
                    lastPoint = new Point(lastPoint.X + (incrementX * Math.Cos(angle)),
                                          lastPoint.Y + (incrementX * Math.Sin(angle)));

                    lastLightningPoint = lightningPoint;

                    // Increment the distance along the original vector
                    distance += incrementX;
                }

                // Increment path length
                accumulatedPathLength += pathLength;
            }

            return new AnimationPrimitiveGroup(primitives, totalAnimationTime);
        }
        #endregion

        #region (private) Animation Primitive Methods
        /// <summary>
        /// Generates an opacity animated figure
        /// </summary>
        private AnimationPrimitive GenerateTimedFigure(Geometry geometry, Brush fill, 
                                                       double opacity1, double opacity2, 
                                                       int time, int repeatCount, 
                                                       bool autoreverse)
        {
            var animation = new DoubleAnimation(opacity1, opacity2, new Duration(TimeSpan.FromMilliseconds(time)));
            animation.RepeatBehavior = new RepeatBehavior(repeatCount);
            animation.AutoReverse = autoreverse;

            var clock = animation.CreateClock();
            clock.Controller.Begin();
            clock.Controller.Pause();

            var primitive = new AnimationPrimitive(geometry, new AnimationClock[] { clock }, time);
            primitive.Fill = fill;
            primitive.ApplyAnimationClock(Shape.OpacityProperty, clock);

            return primitive;
        }

        private AnimationPrimitive GenerateLightningFigure(Geometry geometry, Brush stroke, double strokeThickness,
                                                           double opacity1, double opacity2,
                                                           int time, int startupDelay, int repeatCount,
                                                           bool autoreverse)
        {
            var easingAmount = (double)startupDelay / (double)time;

            var animation = new CustomEaseDoubleAnimation(opacity1, opacity2, AnimationEasingType.JoltEase, 
                                                          easingAmount, new Duration(TimeSpan.FromMilliseconds(time)));
            animation.RepeatBehavior = new RepeatBehavior(repeatCount);
            animation.AutoReverse = autoreverse;

            var clock = animation.CreateClock();
            clock.Controller.Begin();
            clock.Controller.Pause();

            var primitive = new AnimationPrimitive(geometry, new AnimationClock[] { clock }, time);
            primitive.Stroke = stroke;
            primitive.StrokeThickness = strokeThickness;
            primitive.ApplyAnimationClock(Shape.OpacityProperty, clock);

            return primitive;
        }

        private AnimationPrimitive GeneratePathAnimation(PathGeometry pathGeometry, Brush fill, Size size1, Size size2, 
                                                         double opacity1, double opacity2, int erradicity, int animationTime, 
                                                         int repeatCount, bool autoreverse, AnimationEasingType easingType, double easingAmount)
        {
            //Generate Animations
            var duration = new Duration(TimeSpan.FromMilliseconds(animationTime));
            var durationErradicity = (erradicity > 0) ? new Duration(TimeSpan.FromMilliseconds((int)(animationTime / (double) erradicity))) : duration;
            var repeatBehavior = new RepeatBehavior(repeatCount);
            var repeatErradicityBehavior = new RepeatBehavior(repeatCount * erradicity);
            var xAnimation = new CustomEaseDoublePathAnimation(pathGeometry, PathAnimationSource.X, easingType, easingAmount, duration);
            var yAnimation = new CustomEaseDoublePathAnimation(pathGeometry, PathAnimationSource.Y, easingType, easingAmount, duration);
            var angleAnimation = new CustomEaseDoublePathAnimation(pathGeometry, PathAnimationSource.Angle, easingType, easingAmount, duration);
            var widthAnimation = new CustomEaseDoubleAnimation(1, size2.Width / size1.Width, easingType, easingAmount, durationErradicity);
            var heightAnimation = new CustomEaseDoubleAnimation(1, size2.Height / size1.Height, easingType, easingAmount, durationErradicity);
            var opacityAnimation = new CustomEaseDoubleAnimation(opacity1, opacity2, easingType, easingAmount, durationErradicity);

            xAnimation.RepeatBehavior = repeatBehavior;
            yAnimation.RepeatBehavior = repeatBehavior;
            angleAnimation.RepeatBehavior = repeatBehavior;
            widthAnimation.RepeatBehavior = repeatErradicityBehavior;
            heightAnimation.RepeatBehavior = repeatErradicityBehavior;
            opacityAnimation.RepeatBehavior = repeatErradicityBehavior;

            xAnimation.AutoReverse = autoreverse;
            yAnimation.AutoReverse = autoreverse;
            angleAnimation.AutoReverse = autoreverse;
            widthAnimation.AutoReverse = autoreverse;
            heightAnimation.AutoReverse = autoreverse;

            var xClock = xAnimation.CreateClock();
            var yClock = yAnimation.CreateClock();
            var angleClock = angleAnimation.CreateClock();
            var widthClock = widthAnimation.CreateClock();
            var heightClock = heightAnimation.CreateClock();
            var opacityClock = opacityAnimation.CreateClock();

            xClock.Controller.Begin();
            yClock.Controller.Begin();
            angleClock.Controller.Begin();
            widthClock.Controller.Begin();
            heightClock.Controller.Begin();
            opacityClock.Controller.Begin();

            xClock.Controller.Pause();
            yClock.Controller.Pause();
            angleClock.Controller.Pause();
            widthClock.Controller.Pause();
            heightClock.Controller.Pause();
            opacityClock.Controller.Pause();

            var ellipse = new EllipseGeometry(new Point(0, 0), size1.Width, size1.Height);

            var translateTransform = new TranslateTransform();
            var scaleTransform = new ScaleTransform();
            var rotateTransform = new RotateTransform();
            var transform = new TransformGroup();
            transform.Children.Add(scaleTransform);
            transform.Children.Add(rotateTransform);
            transform.Children.Add(translateTransform);

            rotateTransform.ApplyAnimationClock(RotateTransform.AngleProperty, angleClock);
            translateTransform.ApplyAnimationClock(TranslateTransform.XProperty, xClock);
            translateTransform.ApplyAnimationClock(TranslateTransform.YProperty, yClock);
            scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleXProperty, widthClock);
            scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleYProperty, heightClock);

            var primitive = new AnimationPrimitive(ellipse, new AnimationClock[] { xClock, yClock, angleClock, widthClock, heightClock, opacityClock }, animationTime);
            primitive.RenderTransform = transform;
            primitive.ApplyAnimationClock(Shape.OpacityProperty, opacityClock);
            primitive.Fill = fill;

            return primitive;
        }

        private AnimationPrimitive GeneratePointAnimation(Point point, Brush fill, Size size1, Size size2, 
                                                          double opacity1, double opacity2, 
                                                          int erradicity, int animationTime, int repeatCount, 
                                                          bool autoreverse, AnimationEasingType easingType, double easingAmount)
        {
            //Generate Point Animation
            //

            var duration = new Duration(TimeSpan.FromMilliseconds(animationTime));
            var durationErradicity = (erradicity > 0) ? new Duration(new TimeSpan(0, 0, 0, 0, (int)(animationTime / (double)erradicity))) : duration;
            var repeatBehaviorErradicity = new RepeatBehavior(repeatCount * erradicity);

            var sizeAnimationX = new CustomEaseDoubleAnimation(1, size2.Width / size1.Width, easingType, easingAmount, durationErradicity);
            var sizeAnimationY = new CustomEaseDoubleAnimation(1, size2.Height / size1.Height, easingType, easingAmount, durationErradicity);
            var opacityAnimation = new CustomEaseDoubleAnimation(opacity1, opacity2, easingType, easingAmount, durationErradicity);

            sizeAnimationX.RepeatBehavior = repeatBehaviorErradicity;
            sizeAnimationY.RepeatBehavior = repeatBehaviorErradicity;
            opacityAnimation.RepeatBehavior = repeatBehaviorErradicity;

            sizeAnimationX.AutoReverse = autoreverse;
            sizeAnimationY.AutoReverse = autoreverse;

            var clockX = sizeAnimationX.CreateClock();
            var clockY = sizeAnimationY.CreateClock();
            var clockOpacity = opacityAnimation.CreateClock();

            clockX.Controller.Begin();
            clockY.Controller.Begin();
            clockOpacity.Controller.Begin();
            clockX.Controller.Pause();
            clockY.Controller.Pause();
            clockOpacity.Controller.Pause();

            var geometry = new EllipseGeometry(new Point(0,0), size1.Width, size1.Height);

            var scaleTransform = new ScaleTransform();
            var translateTransform = new TranslateTransform(point.X, point.Y);
            var transform = new TransformGroup();
            transform.Children.Add(scaleTransform);
            transform.Children.Add(translateTransform);
            scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleXProperty, clockX);
            scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleYProperty, clockY);
            //st.CenterX = 0.5;
            //st.CenterY = 0.5;

            var primitive = new AnimationPrimitive(geometry, new AnimationClock[] { clockX, clockY, clockOpacity }, animationTime);
            primitive.RenderTransform = transform;
            //prim.RenderTransformOrigin = new Point(.5, .5);
            primitive.ApplyAnimationClock(Shape.OpacityProperty, clockOpacity);
            primitive.Fill = fill;

            return primitive;
        }

        private PathGeometry CreatePathGeometry(IEnumerable<Point> points)
        {
            if (points.Count() < 2)
                throw new Exception("Trying to create path animation path with less than two points");

            var geometry = new PathGeometry();
            var figure = new PathFigure();

            figure.StartPoint = points.First();

            for (int i = 1; i < points.Count(); i++)
                figure.Segments.Add(new LineSegment(points.ElementAt(i), true));

            figure.IsClosed = false;

            geometry.Figures.Add(figure);

            return geometry;
        }
        #endregion

        #region (private) Methods
        private IEnumerable<Point> CalculatePointTargets(AnimationPointTargetType pointTargetType, Point sourceLocation, IEnumerable<Point> targetLocations)
        {
            switch (pointTargetType)
            {
                case AnimationPointTargetType.Source:
                    return new Point[] { sourceLocation };
                case AnimationPointTargetType.AffectedCharacters:
                    return targetLocations;
                default:
                    throw new Exception("Unhandled Point Target Type");
            }
        }

        private AnimationPrimitiveGroup CombineAnimationGroups(IEnumerable<AnimationPrimitiveGroup> groups)
        {
            if (groups.None())
                throw new Exception("Trying to combine empty animation group collection");

            // Combine groups so that they're started simultaneously
            var combinedGroup = groups.First();
            for (int i = 1; i < groups.Count(); i++)
                combinedGroup.Combine(groups.ElementAt(i));

            return combinedGroup;
        }

        private double CalculateLength(Point point1, Point point2)
        {
            return Math.Abs(Point.Subtract(point1, point2).Length);
        }

        /// <summary>
        /// Calculates total length along point from start to finish
        /// </summary>
        private double CalculateLength(Point[] pathPoints)
        {
            var length = 0.0;
            for (int i=0;i<pathPoints.Length - 1;i++)
                length += CalculateLength(pathPoints[i], pathPoints[i + 1]);

            return length;
        }
        #endregion
    }
}
