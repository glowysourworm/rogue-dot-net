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

        public AnimationQueue CreateAnimation(AnimationData animation, Rect bounds, Point sourceLocation, Point[] targetLocations)
        {
            switch (animation.BaseType)
            {
                case AnimationBaseType.Projectile:
                case AnimationBaseType.ProjectileReverse:
                case AnimationBaseType.Chain:
                case AnimationBaseType.ChainReverse:
                    return CreateProjectileAnimation(animation, sourceLocation, targetLocations);
                case AnimationBaseType.Aura:                
                case AnimationBaseType.Barrage:
                case AnimationBaseType.Spiral:
                case AnimationBaseType.Bubbles:
                    {
                        switch (animation.PointTargetType)
                        {
                            case AnimationPointTargetType.Source:
                                return new AnimationQueue(new AnimationPrimitiveGroup[]{
                                    CreateSinglePointAnimation(animation, sourceLocation)
                                });
                            case AnimationPointTargetType.AffectedCharacters:
                                {
                                    var animationGroups = targetLocations.Select(point =>
                                    {
                                        return CreateBubbles(animation.ChildCount,
                                                              point,
                                                              animation.RadiusFromFocus,
                                                              animation.FillTemplate.GenerateBrush(),
                                                              animation.FillTemplate.GenerateBrush(),
                                                              animation.StrokeThickness,
                                                              animation.Opacity1,
                                                              animation.Opacity2,
                                                              new Size(animation.Width1, animation.Height1),
                                                              new Size(animation.Width2, animation.Height2),
                                                              animation.RoamRadius,
                                                              animation.Erradicity,
                                                              animation.AnimationTime,
                                                              animation.RepeatCount);
                                    });

                                    // Combine groups so that they're started simultaneously instead of in sequence
                                    if (animationGroups.Any())
                                    {
                                        var combinedGroup = animationGroups.First();
                                        for (int i = 1; i < animationGroups.Count(); i++)
                                            combinedGroup.Combine(animationGroups.ElementAt(i));

                                        return new AnimationQueue(new AnimationPrimitiveGroup[] { combinedGroup });
                                    }
                                    else
                                        return AnimationQueue.Empty;
                                }
                            default:
                                throw new Exception("Unhandled Point Target Type");
                        }
                    }
                    
                case AnimationBaseType.ScreenBlink:
                    return CreateScreenAnimation(animation, bounds);
                default:
                    throw new Exception("Unhandled AnimationBaseType");
            }
        }

        private AnimationQueue CreateScreenAnimation(AnimationData animation, Rect bounds)
        {
            var stroke = Brushes.Transparent;
            var fill = animation.FillTemplate.GenerateBrush();
            var rectangle = new RectangleGeometry(bounds);

            switch (animation.BaseType)
            {
                case AnimationBaseType.ScreenBlink:
                    return new AnimationQueue(new AnimationPrimitiveGroup[]{
                        new AnimationPrimitiveGroup(new AnimationPrimitive[]
                        {
                            GenerateTimedFigure(rectangle,
                                                stroke,
                                                fill,
                                                animation.StrokeThickness,
                                                animation.Opacity1,
                                                animation.Opacity2,
                                                animation.Erradicity,
                                                animation.AnimationTime,
                                                animation.RepeatCount,
                                                animation.AutoReverse)
                        }, animation.AnimationTime) 
                    });
                default:
                    throw new Exception("Unhandled Screen Animation Type");
            }
        }

        private AnimationPrimitiveGroup CreateSinglePointAnimation(AnimationData animation, Point location)
        {
            var stroke = Brushes.Transparent;
            var fill = animation.FillTemplate.GenerateBrush();

            switch (animation.BaseType)
            {
                case AnimationBaseType.Aura:
                    return CreateAuraModulation(location, 
                                                stroke, fill, 
                                                animation.StrokeThickness, 
                                                animation.Opacity1, animation.Opacity2, 
                                                new Size(animation.Width1, animation.Height1),
                                                new Size(animation.Width2, animation.Height2), 
                                                animation.AnimationTime, animation.RepeatCount, animation.AutoReverse);
                case AnimationBaseType.Barrage:
                    return CreateEllipseBarrage(animation.ChildCount, 
                                                location, 
                                                animation.RadiusFromFocus, 
                                                stroke, fill, 
                                                animation.StrokeThickness, 
                                                animation.Opacity1, animation.Opacity2, 
                                                new Size(animation.Width1, animation.Height1),
                                                new Size(animation.Width2, animation.Height2), 
                                                animation.Erradicity, 
                                                animation.AnimationTime, animation.RepeatCount, animation.AutoReverse);
                case AnimationBaseType.Bubbles:
                    return CreateEllipseBarrage(animation.ChildCount, 
                                                location, 
                                                animation.RadiusFromFocus, 
                                                stroke, fill, 
                                                animation.StrokeThickness, 
                                                animation.Opacity1, animation.Opacity2, 
                                                new Size(animation.Width1, animation.Height1),
                                                new Size(animation.Width2, animation.Height2), 
                                                animation.Erradicity,
                                                animation.AnimationTime, animation.RepeatCount, animation.AutoReverse);
                case AnimationBaseType.Spiral:
                    return CreateEllipseSpiral(animation.ChildCount, 
                                               location, 
                                               animation.RadiusFromFocus, 
                                               animation.SpiralRate, 
                                               stroke, fill, 
                                               animation.StrokeThickness, 
                                               animation.Opacity1, animation.Opacity2, 
                                               new Size(animation.Width1, animation.Height1),
                                               new Size(animation.Width2, animation.Height2), 
                                               animation.Erradicity,
                                               animation.AnimationTime, animation.RepeatCount, animation.AutoReverse);
                default:
                    throw new Exception("Unhandled Single Point Animation Type");
            }
        }

        private AnimationQueue CreateProjectileAnimation(AnimationData animation, Point sourceLocation, Point[] targetLocations)
        {
            var stroke = Brushes.Transparent;
            var fill = animation.FillTemplate.GenerateBrush();
            var isReverse = animation.BaseType == AnimationBaseType.ChainReverse ||
                            animation.BaseType == AnimationBaseType.ProjectileReverse;

            var points = new List<Point>();

            switch (animation.BaseType)
            {
                case AnimationBaseType.Chain:
                case AnimationBaseType.ChainReverse:
                    {
                        // Re-calculating the source / targets for the animation to order them
                        var source = isReverse ? targetLocations.PickRandom() : sourceLocation;
                        var targets = targetLocations.Append(sourceLocation).Except(new Point[] { source });
                        points.Add(source);
                        points.AddRange(targets);
                    }
                    break;
                case AnimationBaseType.Projectile:
                case AnimationBaseType.ProjectileReverse:
                    {
                        points.Add(sourceLocation);
                        points.Add(targetLocations.First());
                    }
                    break;
                default:
                    throw new Exception("Unhanded Projectile Animation Type");
            }

            return CreateEllipseProjectilePath(points.ToArray(), stroke, fill,
                                               animation.StrokeThickness,
                                               animation.Opacity1, animation.Opacity2,
                                               new Size(animation.Width1, animation.Height1),
                                               new Size(animation.Width2, animation.Height2),
                                               animation.Erradicity, animation.AnimationTime,
                                               animation.Velocity, animation.RepeatCount,
                                               animation.AutoReverse, animation.ConstantVelocity);
        }

        public AnimationQueue CreateTargetingAnimation(Point point, Color fillColor, Color strokeColor)
        {
            var length = 20;
            var cellBounds = new Rect(point, new Size(10, 15));
            var verticalSize = new Size(1, 8);
            var pointSize = new Size(1, 1);

            var animationTime = 300;

            var stroke = new SolidColorBrush(strokeColor);
            var fill = new SolidColorBrush(fillColor);

            var northWest = new Point(point.X - length, point.Y - length);
            var northEast = new Point(cellBounds.TopRight.X + length, cellBounds.TopRight.Y - length);
            var southEast = new Point(cellBounds.BottomRight.X + length, cellBounds.BottomRight.Y + length);
            var southWest = new Point(cellBounds.BottomLeft.X - length, cellBounds.BottomLeft.Y + length);

            var path1 = CreatePathGeometry(new Point[] { northWest, cellBounds.TopLeft });
            var path2 = CreatePathGeometry(new Point[] { northEast, cellBounds.TopRight });
            var path3 = CreatePathGeometry(new Point[] { southEast, cellBounds.BottomRight });
            var path4 = CreatePathGeometry(new Point[] { southWest, cellBounds.BottomLeft });

            var animation1 = GeneratePathAnimation(path1, pointSize, verticalSize, stroke, fill, 1, 0, 0, 0, animationTime, int.MaxValue, false);
            var animation2 = GeneratePathAnimation(path2, pointSize, verticalSize, stroke, fill, 1, 0, 0, 0, animationTime, int.MaxValue, false);
            var animation3 = GeneratePathAnimation(path3, pointSize, verticalSize, stroke, fill, 1, 0, 0, 0, animationTime, int.MaxValue, false);
            var animation4 = GeneratePathAnimation(path4, pointSize, verticalSize, stroke, fill, 1, 0, 0, 0, animationTime, int.MaxValue, false);
            var animation5 = GenerateTimedFigure(new RectangleGeometry(cellBounds), stroke, fill, 1, 0, 1, 1, animationTime, int.MaxValue, true);

            var animationGroup = new AnimationPrimitiveGroup(new AnimationPrimitive[] { animation1, animation2, animation3, animation4, animation5 }, animationTime);

            return new AnimationQueue(new AnimationPrimitiveGroup[] { animationGroup });
        }

        #region (private) Animation Methods
        private AnimationQueue CreateEllipseProjectilePath(Point[] points, Brush stroke, Brush fill,
                                                            double strokethickness, double opacity1, double opacity2,
                                                            Size size1, Size size2, int erradicity,
                                                            int time, int velocity, int repeatCount, bool autoreverse,
                                                            bool constVelocity)
        {
            var animationGroups = new List<AnimationPrimitiveGroup>();

            // Generate animation groups for the path
            for (int i=0;i < points.Length - 1;i++)
            {
                // Check constant velocity animation
                var animationTime = constVelocity ? (int)Point.Subtract(points[i], points[i + 1]).Length / velocity : time;

                // Set default minimum time
                if (animationTime < 50)
                    animationTime = 50;


                var group = CreateEllipseProjectile(points[i], points[i + 1], stroke, fill, strokethickness, 
                                                    opacity1, opacity2, size1, size2, erradicity,
                                                    animationTime, velocity, repeatCount, autoreverse, constVelocity);

                animationGroups.Add(group);
            }

            return new AnimationQueue(animationGroups);
        }
        private AnimationPrimitiveGroup CreateEllipseProjectile(Point point1, Point point2, Brush stroke, Brush fill, 
                                                                double strokethickness, double opacity1, double opacity2, 
                                                                Size size1, Size size2, int erradicity, int time, int velocity, 
                                                                int repeatCount, bool autoreverse, bool constVelocity)
        {
            // Check constant velocity animation
            var animationTime = constVelocity ? (int)Point.Subtract(point1, point2).Length / velocity : time;

            // Set default minimum time
            if (animationTime < 50)
                animationTime = 50;

            var path = CreatePathGeometry(new Point[] { point1, point2 });

            var primitive = GeneratePathAnimation(path, size1, size2, stroke,fill,
                                                  opacity1, opacity2, strokethickness, erradicity,
                                                  animationTime, repeatCount, autoreverse);

            return new AnimationPrimitiveGroup(new AnimationPrimitive[] { primitive }, animationTime);
        }

        private AnimationPrimitiveGroup CreateEllipseBarrage(int count, Point focus, double radiusFromFocus, Brush stroke, 
                                                             Brush fill, double strokeThickness, double opacity1, double opacity2, 
                                                             Size size1, Size size2, int erradicity, 
                                                             int time, int repeatcount, bool autoreverse)
        {
            var primitives = new List<AnimationPrimitive>();

            for (int i = 0; i < count; i++)
            {
                double angle = _randomSequenceGenerator.Get() * Math.PI * 2;
                double radius = _randomSequenceGenerator.Get() * radiusFromFocus;

                var point = new Point(focus.X + (radius * Math.Cos(angle)), focus.Y + (radius * Math.Sin(angle)));

                var path = CreatePathGeometry(new Point[] { point, focus });

                primitives.Add(GeneratePathAnimation(path, size1, size2, stroke, fill,opacity1, opacity2, 
                                                     strokeThickness, erradicity, time, repeatcount, autoreverse));
            }

            return new AnimationPrimitiveGroup(primitives, time);
        }

        private AnimationPrimitiveGroup CreateEllipseSpiral(int count, Point focus, double radius, double spiralRate, Brush stroke, Brush fill, 
                                                            double strokeThickness, double opacity1, double opacity2, Size size1, Size size2, 
                                                            int erradicity, int time, int repeatcount, bool autoreverse)
        {
            var primitives = new List<AnimationPrimitive>();

            // Foreach ellipse on the spiral
            for (int i = 0; i < count; i++)
            {
                // Calculate point to start the animation
                var points = new List<Point>();

                double startAngle = (2 * Math.PI * i) / count;
                double angle = startAngle, angleIncrement = 0.1;

                // Calculate points in the spiral to animate
                while (angle < (2 * Math.PI * (repeatcount + 1)) + startAngle)
                {
                    double M = radius + ((angle - startAngle) * spiralRate);
                    Point p = new Point((M * Math.Cos(angle)) + focus.X, (M * Math.Sin(angle)) + focus.Y);
                    points.Add(p);
                    angle += angleIncrement;
                }

                // Create path using animation points
                var path = CreatePathGeometry(points);

                // Generate the path animation
                primitives.Add(GeneratePathAnimation(path, size1, size2, stroke, fill, 
                                                     opacity1, opacity2, strokeThickness, erradicity, 
                                                     time, repeatcount, autoreverse));
            }

            return new AnimationPrimitiveGroup(primitives, time);
        }

        private AnimationPrimitiveGroup CreateAuraModulation(Point point, Brush stroke, Brush fill, double strokeThickness, double opacity1, double opacity2, Size size1, Size size2, int time, int repeatCount, bool autoreverse)
        {
            var primitive = GeneratePointAnimation(point, size1, size2, stroke, fill, opacity1, opacity2,
                                                   strokeThickness, 1, time, repeatCount, autoreverse);

            return new AnimationPrimitiveGroup(new AnimationPrimitive[] { primitive }, time);
        }

        private AnimationPrimitiveGroup CreateBubbles(int count, Point focus, double radiusFromFocus, 
                                                      Brush stroke, Brush fill, double strokeThickness, 
                                                     double opacity1, double opacity2, Size size1, Size size2, 
                                                     double roamRadius, int erradicity, int time, int repeatCount)
        {
            var result = new List<AnimationPrimitive>();

            // Create roam points for the bubbles
            for (int i = 0; i < count; i++)
            {
                var roamPoints = new List<Point>();

                //Get random point in radius from focus
                var angle = _randomSequenceGenerator.Get() * Math.PI * 2;
                var radius = _randomSequenceGenerator.Get() * radiusFromFocus;
                roamPoints.Add(new Point(radius * Math.Cos(angle) + focus.X, radius * Math.Sin(angle) + focus.Y));

                // Calculate roam points
                for (int j = 0; j < Math.Max(2, erradicity); j++)
                {
                    var lastRoamPoint = roamPoints.Last();

                    //Get random point in roam radius from that random point
                    double roam_a = _randomSequenceGenerator.Get() * Math.PI * 2;
                    double roam_r = _randomSequenceGenerator.Get() * roamRadius;
                    var roamPoint = new Point(roam_r * Math.Cos(roam_a) + lastRoamPoint.X, roam_r * Math.Sin(roam_a) + lastRoamPoint.Y);
                    roamPoints.Add(roamPoint);
                }

                // Create path for the animation
                var path = CreatePathGeometry(roamPoints);

                // Create animations for this bubble
                result.Add(GeneratePathAnimation(path, size1, size2, stroke, fill, opacity1, opacity2, strokeThickness, 0, time, repeatCount, false));
            }
            return new AnimationPrimitiveGroup(result, time);
        }

        /// <summary>
        /// Generates an opacity animated figure
        /// </summary>
        private AnimationPrimitive GenerateTimedFigure(Geometry geometry, 
                                                       Brush stroke, 
                                                       Brush fill, 
                                                       double strokethickness, 
                                                       double opacity1, 
                                                       double opacity2, 
                                                       int erradicity, 
                                                       int time, 
                                                       int repeatCount, 
                                                       bool autoreverse)
        {
            var animation = new DoubleAnimation(opacity1, opacity2, new Duration(new TimeSpan(0, 0, 0, 0, (int)((erradicity == 0) ? time : time / (double)erradicity))));
            animation.RepeatBehavior = new RepeatBehavior(repeatCount * erradicity);
            animation.AutoReverse = autoreverse;

            var clock = animation.CreateClock();
            clock.Controller.Begin();
            clock.Controller.Pause();

            var primitive = new AnimationPrimitive(geometry, new AnimationClock[] { clock }, time);
            primitive.Fill = fill;
            primitive.Stroke = stroke;
            primitive.StrokeThickness = strokethickness;
            primitive.ApplyAnimationClock(Shape.OpacityProperty, clock);

            return primitive;
        }

        private AnimationPrimitive GeneratePathAnimation(PathGeometry pathGeometry,
                                                                 Size size1, 
                                                                 Size size2, 
                                                                 Brush stroke, 
                                                                 Brush fill, 
                                                                 double opacity1, 
                                                                 double opacity2, 
                                                                 double strokeThickness, 
                                                                 int erradicity, 
                                                                 int time, 
                                                                 int repeatCount, 
                                                                 bool autoreverse)
        {
            //Generate Animations
            var duration = new Duration(TimeSpan.FromMilliseconds(time));
            var durationErradicity = (erradicity > 0) ? new Duration(TimeSpan.FromMilliseconds((int)(time / (double) erradicity))) : duration;
            var repeatBehavior = new RepeatBehavior(repeatCount);
            var repeatErradicityBehavior = new RepeatBehavior(repeatCount * erradicity);
            var xAnimation = new DoubleAnimationUsingPath();
            var yAnimation = new DoubleAnimationUsingPath();
            var angleAnimation = new DoubleAnimationUsingPath();
            var widthAnimation = new DoubleAnimation(1, size2.Width / size1.Width, duration);
            var heightAnimation = new DoubleAnimation(1, size2.Height / size1.Height, duration);
            var opacityAnimation = new DoubleAnimation(opacity1, opacity2, durationErradicity);

            xAnimation.PathGeometry = pathGeometry;
            yAnimation.PathGeometry = pathGeometry;
            angleAnimation.PathGeometry = pathGeometry;
            xAnimation.RepeatBehavior = repeatBehavior;
            yAnimation.RepeatBehavior = repeatBehavior;
            angleAnimation.RepeatBehavior = repeatBehavior;
            widthAnimation.RepeatBehavior = repeatErradicityBehavior;
            heightAnimation.RepeatBehavior = repeatErradicityBehavior;
            opacityAnimation.RepeatBehavior = repeatErradicityBehavior;

            xAnimation.Duration = duration;
            yAnimation.Duration = duration;
            angleAnimation.Duration = duration;
            widthAnimation.Duration = durationErradicity;
            heightAnimation.Duration = durationErradicity;
            opacityAnimation.Duration = durationErradicity;
            xAnimation.AutoReverse = autoreverse;
            yAnimation.AutoReverse = autoreverse;
            angleAnimation.AutoReverse = autoreverse;
            widthAnimation.AutoReverse = autoreverse;
            heightAnimation.AutoReverse = autoreverse;
            xAnimation.Source = PathAnimationSource.X;
            yAnimation.Source = PathAnimationSource.Y;
            angleAnimation.Source = PathAnimationSource.Angle;

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

            var primitive = new AnimationPrimitive(ellipse, new AnimationClock[] { xClock, yClock, angleClock, widthClock, heightClock, opacityClock }, time);
            primitive.RenderTransform = transform;
            primitive.ApplyAnimationClock(Shape.OpacityProperty, opacityClock);
            primitive.Fill = fill;
            primitive.Stroke = stroke;
            primitive.StrokeThickness = strokeThickness;

            return primitive;
        }

        private AnimationPrimitive GeneratePointAnimation(Point point, 
                                                          Size size1, 
                                                          Size size2, 
                                                          Brush stroke, 
                                                          Brush fill, 
                                                          double opacity1, 
                                                          double opacity2, 
                                                          double strokeThickness, 
                                                          int erradicity, 
                                                          int time, 
                                                          int repeatCount, 
                                                          bool autoreverse)
        {
            //Generate Animations
            Duration duration = new Duration(new TimeSpan(0, 0, 0, 0, (int)time));
            Duration durationErradicity = (erradicity > 0) ? new Duration(new TimeSpan(0, 0, 0, 0, (int)(time / (double)erradicity))) : duration;
            RepeatBehavior repeatBehaviorErradicity = new RepeatBehavior(repeatCount * erradicity);
            DoubleAnimation sizeAnimationX = new DoubleAnimation(1, size2.Width / size1.Width, duration);
            DoubleAnimation sizeAnimationY = new DoubleAnimation(1, size2.Height / size1.Height, duration);
            DoubleAnimation opacityAnimation = new DoubleAnimation(opacity1, opacity2, durationErradicity);
            sizeAnimationX.RepeatBehavior = repeatBehaviorErradicity;
            sizeAnimationY.RepeatBehavior = repeatBehaviorErradicity;
            opacityAnimation.RepeatBehavior = repeatBehaviorErradicity;
            sizeAnimationX.Duration = durationErradicity;
            sizeAnimationY.Duration = durationErradicity;
            opacityAnimation.Duration = durationErradicity;
            sizeAnimationX.AutoReverse = autoreverse;
            sizeAnimationY.AutoReverse = autoreverse;

            AnimationClock clockX = sizeAnimationX.CreateClock();
            AnimationClock clockY = sizeAnimationY.CreateClock();
            AnimationClock clockOpacity = opacityAnimation.CreateClock();
            clockX.Controller.Begin();
            clockY.Controller.Begin();
            clockOpacity.Controller.Begin();
            clockX.Controller.Pause();
            clockY.Controller.Pause();
            clockOpacity.Controller.Pause();

            EllipseGeometry geometry = new EllipseGeometry(new Point(0,0), size1.Width, size1.Height);

            ScaleTransform scaleTransform = new ScaleTransform();
            TranslateTransform translateTransform = new TranslateTransform(point.X, point.Y);
            TransformGroup transform = new TransformGroup();
            transform.Children.Add(scaleTransform);
            transform.Children.Add(translateTransform);
            scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleXProperty, clockX);
            scaleTransform.ApplyAnimationClock(ScaleTransform.ScaleYProperty, clockY);
            //st.CenterX = 0.5;
            //st.CenterY = 0.5;

            AnimationPrimitive primitive = new AnimationPrimitive(geometry, new AnimationClock[] { clockX, clockY, clockOpacity }, time);
            primitive.RenderTransform = transform;
            //prim.RenderTransformOrigin = new Point(.5, .5);
            primitive.ApplyAnimationClock(Shape.OpacityProperty, clockOpacity);
            primitive.Fill = fill;
            primitive.Stroke = stroke;
            primitive.StrokeThickness = strokeThickness;

            return primitive;
        }
        private PathGeometry CreatePathGeometry(IEnumerable<Point> points)
        {
            if (points.Count() < 2)
                throw new Exception("Trying to create path animation path with less than two points");

            var geometry = new PathGeometry();
            var figure = new PathFigure();

            figure.StartPoint = points.First();

            for (int i = 0; i < points.Count(); i++)
                figure.Segments.Add(new LineSegment(points.ElementAt(i), true));

            figure.IsClosed = false;

            return geometry;
        }
        #endregion
    }
}
