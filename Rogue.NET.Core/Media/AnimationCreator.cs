using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Media.Interface;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Generator.Interface;

namespace Rogue.NET.Core.Media
{
    [Export(typeof(IAnimationCreator))]
    public class AnimationCreator : IAnimationCreator
    {
        IRandomSequenceGenerator _randomSequenceGenerator;

        const int DEFAULT_VELOCITY = 150; // milli-seconds (if set to negative or zero value)

        [ImportingConstructor]
        public AnimationCreator(IRandomSequenceGenerator randomSequenceGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public IEnumerable<AnimationQueue> CreateAnimation(AnimationData animation, Rect bounds, Point sourceLocation, Point[] targetLocations)
        {
            switch (animation.BaseType)
            {
                case AnimationBaseType.Projectile:
                case AnimationBaseType.ProjectileReverse:
                case AnimationBaseType.Chain:
                case AnimationBaseType.ChainReverse:
                    return new AnimationQueue[]
                    {
                        CreateProjectileAnimation(animation, sourceLocation, targetLocations)
                    };
                case AnimationBaseType.Aura:                
                case AnimationBaseType.Barrage:
                case AnimationBaseType.Spiral:
                case AnimationBaseType.Bubbles:
                    {
                        switch (animation.PointTargetType)
                        {
                            case AnimationPointTargetType.Source:
                                return new AnimationQueue[] { CreateSinglePointAnimation(animation, sourceLocation) };
                            case AnimationPointTargetType.AffectedCharacters:
                                return targetLocations.Select(location =>
                                {
                                    return CreateSinglePointAnimation(animation, location);
                                });
                            default:
                                throw new Exception("Unhandled Point Target Type");
                        }
                    }
                    
                case AnimationBaseType.ScreenBlink:
                    return new AnimationQueue[] { CreateScreenAnimation(animation, bounds) };
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
                case AnimationBaseType.Bubbles:
                    return CreateBubbles(animation.ChildCount, 
                                         bounds, 
                                         stroke, 
                                         fill,
                                         animation.StrokeThickness, 
                                         animation.Opacity1, 
                                         animation.Opacity2, 
                                         new Size(animation.Width1, animation.Height1),
                                         new Size(animation.Width2, animation.Height2), 
                                         Math.Min(bounds.Width, bounds.Height), 
                                         animation.Erradicity, 
                                         animation.AnimationTime, 
                                         animation.RepeatCount);
                case AnimationBaseType.ScreenBlink:
                    return new AnimationQueue(new Animation[]
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
                    });
                default:
                    throw new Exception("Unhandled Screen Animation Type");
            }
        }

        private AnimationQueue CreateSinglePointAnimation(AnimationData animation, Point location)
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
                                                animation.AccelerationRatio, animation.Erradicity, 
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
                                                animation.AccelerationRatio, animation.Erradicity,
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
                                               animation.AccelerationRatio, animation.Erradicity,
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


            switch (animation.BaseType)
            {
                case AnimationBaseType.Chain:
                case AnimationBaseType.ChainReverse:
                    {
                        // Re-calculating the source / targets for the animation
                        var list = new List<Point>();
                        var source = isReverse ? targetLocations.PickRandom() : sourceLocation;
                        var targets = targetLocations.Append(sourceLocation).Except(new Point[] { source });
                        list.Add(source);
                        list.AddRange(targets);

                        // Create Projectile Path for the animation
                        return new AnimationQueue(new Animation[]{GenerateProjectilePath(list.ToArray()
                                , new Size(animation.Width1, animation.Height1)
                                , new Size(animation.Width2, animation.Height2)
                                , stroke
                                , fill
                                , animation.Opacity1, animation.Opacity2, animation.StrokeThickness
                                , animation.AccelerationRatio, animation.Erradicity, animation.AnimationTime
                                , animation.Velocity, animation.RepeatCount, animation.AutoReverse, animation.ConstantVelocity)});
                    }
                case AnimationBaseType.Projectile:
                case AnimationBaseType.ProjectileReverse:
                    {
                        var list = new List<Animation>();


                        foreach (Point target in targetLocations)
                        {
                            Animation a = GenerateProjectilePath(
                                isReverse ? new Point[] { target, sourceLocation } : new Point[] { sourceLocation, target }
                                , new Size(animation.Width1, animation.Height1)
                                , new Size(animation.Width2, animation.Height2)
                                , Brushes.Transparent
                                , animation.FillTemplate.GenerateBrush()
                                , animation.Opacity1, animation.Opacity2, animation.StrokeThickness
                                , animation.AccelerationRatio, animation.Erradicity, animation.AnimationTime
                                , animation.Velocity, animation.RepeatCount, animation.AutoReverse, animation.ConstantVelocity);
                            list.Add(a);
                        }
                        return new AnimationQueue(list.ToArray());
                    }
                default:
                    throw new Exception("Unhanded Projectile Animation Type");
            }
        }
        
        public IEnumerable<AnimationQueue> CreateTargetingAnimation(Point[] points)
        {
            var result = new List<AnimationQueue>();

            foreach (var point in points)
            {
                var length = 20;
                var cellBounds = new Rect(point, new Size(10, 15));
                var verticalSize = new Size(1, 8);
                var pointSize = new Size(1, 1);

                var northWest = new Point(point.X - length, point.Y - length);
                var northEast = new Point(cellBounds.TopRight.X + length, cellBounds.TopRight.Y - length);
                var southEast = new Point(cellBounds.BottomRight.X + length, cellBounds.BottomRight.Y + length);
                var southWest = new Point(cellBounds.BottomLeft.X - length, cellBounds.BottomLeft.Y + length);

                var animation1 = GenerateProjectilePath(new Point[] { northWest, cellBounds.TopLeft }, pointSize, verticalSize, Brushes.Magenta, Brushes.Magenta, 1, 0, 0, 0, 1, 300, 1, int.MaxValue, true, false);
                var animation2 = GenerateProjectilePath(new Point[] { northEast, cellBounds.TopRight }, pointSize, verticalSize, Brushes.Magenta, Brushes.Magenta, 1, 0, 0, 0, 1, 300, 1, int.MaxValue, true, false);
                var animation3 = GenerateProjectilePath(new Point[] { southEast, cellBounds.BottomRight }, pointSize, verticalSize, Brushes.Magenta, Brushes.Magenta, 1, 0, 0, 0, 1, 300, 1, int.MaxValue, true, false);
                var animation4 = GenerateProjectilePath(new Point[] { southWest, cellBounds.BottomLeft }, pointSize, verticalSize, Brushes.Magenta, Brushes.Magenta, 1, 0, 0, 0, 1, 300, 1, int.MaxValue, true, false);
                var animation5 = GenerateTimedFigure(new RectangleGeometry(cellBounds), Brushes.LimeGreen, Brushes.Magenta, 1, 0, 1, 1, 300, int.MaxValue, true);

                result.Add(new AnimationQueue(new Animation[] { animation1, animation2, animation3, animation4, animation5 }));
            }

            return result;
        }

        #region (private) Animation Methods
        private AnimationQueue CreateEllipseProjectile(Point p1, Point p2, Brush stroke, Brush fill, 
            double strokethickness, double opacity1, double opacity2, Size s1, Size s2, double accRatio, int erradicity, int time, int velocity,
            int repeatCount, bool autoreverse, bool constVelocity)
        {
            Animation a = GenerateProjectilePath(new Point[] { p1, p2 }, s1, s2, stroke,fill,opacity1, opacity2, strokethickness, accRatio, erradicity, time, velocity, repeatCount, autoreverse, constVelocity);
            return new AnimationQueue(new Animation[] { a });
        }

        private AnimationQueue CreateEllipseBarrage(int count, Point focus, double radiusFromFocus, Brush stroke, 
            Brush fill, double strokeThickness, double opacity1, double opacity2, Size s1, Size s2, double accRatio, int erradicity, 
            int time, int repeatcount, bool autoreverse)
        {
            List<Animation> list = new List<Animation>();
            for (int i = 0; i < count; i++)
            {
                double angle = _randomSequenceGenerator.Get() * Math.PI * 2;
                double radius = _randomSequenceGenerator.Get() * radiusFromFocus;
                Point p = new Point(focus.X + (radius * Math.Cos(angle)), focus.Y + (radius * Math.Sin(angle)));
                Animation a = GenerateProjectilePath(new Point[] { p, focus }, s1, s2, stroke, fill,opacity1, opacity2, strokeThickness, accRatio, erradicity, time, 1,  repeatcount, autoreverse, false);
                list.Add(a);
            }
            return new AnimationQueue(list.ToArray());
        }

        private AnimationQueue CreateEllipseSpiral(int count, Point focus, double radius, double spiralRate, 
            Brush stroke, Brush fill, double strokeThickness, double opacity1, double opacity2, Size s1, Size s2, 
            double accRatio, int erradicity, int time, int repeatcount, bool autoreverse)
        {
            List<Animation> list = new List<Animation>();
            for (int i = 0; i < count; i++)
            {
                List<Point> pts = new List<Point>();
                double startAngle = (2 * Math.PI * i) / count;
                double angle = startAngle, da = 0.1;
                while (angle < (2 * Math.PI * (repeatcount + 1)) + startAngle)
                {
                    double M = radius + ((angle - startAngle) * spiralRate);
                    Point p = new Point((M * Math.Cos(angle)) + focus.X, (M * Math.Sin(angle)) + focus.Y);
                    pts.Add(p);
                    angle += da;
                }
                Animation a = GenerateProjectilePath(pts.ToArray(), s1, s2,stroke, fill, strokeThickness,opacity1, opacity2, accRatio, erradicity, time, 1, repeatcount, autoreverse, false);
                list.Add(a);
                pts.Clear();
                pts = null;
            }
            return new AnimationQueue(list.ToArray());
        }

        private AnimationQueue CreateAuraModulation(Point p, Brush stroke, Brush fill, double strokeThickness, double opacity1, double opacity2, Size s1, Size s2, int time, int repeatCount, bool autoreverse)
        {
            Animation a = GenerateAuraAnimation(p, s1, s2,stroke, fill, opacity1, opacity2,  strokeThickness, 0, 1, time, repeatCount, autoreverse);
            return new AnimationQueue(new Animation[] { a });
        }

        private AnimationQueue CreateBubbles(int count, Rect rect, Brush stroke, Brush fill, double strokeThickness, double opacity1, double opacity2, Size s1, Size s2, double roamRadius, int erradicity, int time, int repeatCount)
        {
            List<Animation> list = new List<Animation>();
            for (int i = 0; i < count; i++)
            {
                List<Point> pts = new List<Point>();
                for (int j = 0; j < erradicity; j++)
                {
                    Point pt = new Point(rect.X + (_randomSequenceGenerator.Get() * roamRadius), rect.Y + (_randomSequenceGenerator.Get() * roamRadius));
                    pts.Add(pt);
                }
                Animation a = GenerateProjectilePath(pts.ToArray(), s1, s2,stroke, fill, opacity1, opacity2, strokeThickness, 0, erradicity, time, 1,repeatCount, false, false);
                list.Add(a);
                pts.Clear();
                pts = null;
            }
            return new AnimationQueue(list.ToArray());
        }

        private AnimationQueue CreateBubbles(int count, Point focus, double radiusFromFocus, Brush stroke, Brush fill, double strokeThickness, double opacity1, double opacity2, Size s1, Size s2, double roamRadius, int erradicity, int time, int repeatCount)
        {
            List<Animation> list = new List<Animation>();
            for (int i = 0; i < count; i++)
            {
                List<Point> pts = new List<Point>();

                //Get random point in radius from focus
                double angle = _randomSequenceGenerator.Get() * Math.PI * 2;
                double radius = _randomSequenceGenerator.Get() * radiusFromFocus;
                Point pt = new Point(radius * Math.Cos(angle) + focus.X, radius * Math.Sin(angle) + focus.Y);

                for (int j = 0; j < Math.Max(2, erradicity); j++)
                {
                    //Get random point in roam radius from that random point
                    double roam_a = _randomSequenceGenerator.Get() * Math.PI * 2;
                    double roam_r = _randomSequenceGenerator.Get() * roamRadius;
                    Point p = new Point(roam_r * Math.Cos(roam_a) + pt.X, roam_r * Math.Sin(roam_a) + pt.Y);
                    pts.Add(p);
                }
                Animation a = GenerateProjectilePath(pts.ToArray(), s1, s2, stroke, fill, opacity1, opacity2, strokeThickness, 0, erradicity, time, 5, repeatCount, false, false);
                list.Add(a);
                pts.Clear();
                pts = null;
            }
            return new AnimationQueue(list.ToArray());
        }

        private AnimationQueue CreateFadingChain(Point[] pts, Brush stroke, double strokeThickness,double opacity1, double opacity2, int erradicity, int time, int repeatCount, bool autoreverse)
        {
            List<Animation> list = new List<Animation>();
            for (int i = 0; i < pts.Length - 1; i++)
            {
                PathGeometry g = new PathGeometry();
                PathFigure f = new PathFigure();
                f.StartPoint = pts[i];
                f.Segments.Add(new LineSegment(pts[i + 1], true));
                AnimationPrimitive fig = new AnimationPrimitive();
                DoubleAnimation anim = new DoubleAnimation(opacity1, opacity2, new Duration(new TimeSpan(0, 0, 0, 0,(int)((erradicity == 0) ? time : time / (double)erradicity))));
                AnimationClock c = anim.CreateClock();
                c.Controller.Begin();
                c.Controller.Pause();
                fig.SetGeometry(g);
                fig.ApplyAnimationClock(Shape.OpacityProperty, c);
                Animation a = new Animation(new ITimedGraphic[] { fig }, time);
                a.SetStartupDelay((int)((time / (double)(pts.Length - 1)) * i));
                list.Add(a);
            }
            return new AnimationQueue(list.ToArray());
        }

        private Animation GenerateTimedFigure(Geometry g, Brush stroke, Brush fill, double strokethickness, double opacity1, double opacity2, int erradicity, int time, int repeatCount, bool autoreverse)
        {
            AnimationPrimitive prim = new AnimationPrimitive();
            DoubleAnimation anim = new DoubleAnimation(opacity1, opacity2, new Duration(new TimeSpan(0, 0, 0, 0, (int)((erradicity == 0) ? time : time / (double)erradicity))));
            anim.RepeatBehavior = new RepeatBehavior(repeatCount * erradicity);
            anim.AutoReverse = autoreverse;
            AnimationClock clock = anim.CreateClock();
            clock.Controller.Begin();
            clock.Controller.Pause();
            prim.Fill = fill;
            prim.Stroke = stroke;
            prim.StrokeThickness = strokethickness;
            prim.SetGeometry(g);
            prim.ApplyAnimationClock(Shape.OpacityProperty, clock);
            prim.SetAnimations(new AnimationClock[] { clock }, time);
            return new Animation(new ITimedGraphic[] { prim }, time);
        }

        private Animation GenerateProjectilePath(Point[] points, Size size1, Size size2, Brush stroke, Brush fill, double opacity1, double opacity2, double strokeThickness, double accelerationRatio, int erradicity, int time, int velocity, int repeatCount, bool autoreverse, bool constVelocity)
        {
            //Generate Path geometry from points
            var geometry = new PathGeometry();
            var figure = new PathFigure();
            var pathLength = 0D;

            for (int i = 0; i < points.Length - 1; i++)
            {
                Point p1 = points[i];
                Point p2 = points[i + 1];

                if (i == 0)
                    figure.StartPoint = p1;

                figure.Segments.Add(new LineSegment(p2, false));
                pathLength += Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
            }
            geometry.Figures.Add(figure);

            //Check for const velocity
            if (constVelocity)
            {
                //x = vt - making sure to set defaults if bad parameters
                if (velocity <= 0)
                    velocity = DEFAULT_VELOCITY;

                //convert to seconds
                time = (int)((pathLength * 1000) / velocity);
                if (time <= 0)
                    time = 500;
            }

            //Generate Animations
            var duration = new Duration(new TimeSpan(0, 0, 0, 0, (int)time));
            var durationErradicity = (erradicity > 0) ? new Duration(new TimeSpan(0, 0, 0, 0, (int)(time / (double) erradicity))) : duration;
            var repeatBehavior = new RepeatBehavior(repeatCount);
            var repeatErradicityBehavior = new RepeatBehavior(repeatCount * erradicity);
            var xAnimation = new DoubleAnimationUsingPath();
            var yAnimation = new DoubleAnimationUsingPath();
            var angleAnimation = new DoubleAnimationUsingPath();
            var widthAnimation = new DoubleAnimation(1, size2.Width / size1.Width, duration);
            var heightAnimation = new DoubleAnimation(1, size2.Height / size1.Height, duration);
            var opacityAnimation = new DoubleAnimation(opacity1, opacity2, durationErradicity);

            xAnimation.PathGeometry = geometry;
            yAnimation.PathGeometry = geometry;
            angleAnimation.PathGeometry = geometry;
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

            var animation = new AnimationPrimitive();
            animation.SetGeometry(ellipse);
            animation.RenderTransform = transform;
            animation.ApplyAnimationClock(Shape.OpacityProperty, opacityClock);

            animation.SetAnimations(new AnimationClock[] { xClock, yClock, angleClock, widthClock, heightClock, opacityClock }, time);

            animation.Fill = fill;
            animation.Stroke = stroke;
            animation.StrokeThickness = strokeThickness;

            return new Animation(new AnimationPrimitive[] { animation }, time);
        }

        private Animation GenerateAuraAnimation(Point pt, Size s1, Size s2, Brush stroke, Brush fill, double opacity1, double opacity2, double strokeThickness, double accelerationRatio, int erradicity, int time, int repeatCount, bool autoreverse)
        {
            //Generate Animations
            Duration d = new Duration(new TimeSpan(0, 0, 0, 0, (int)time));
            Duration dErr = (erradicity > 0) ? new Duration(new TimeSpan(0, 0, 0, 0, (int)(time / (double)erradicity))) : d;
            RepeatBehavior rb = new RepeatBehavior(repeatCount);
            RepeatBehavior rbErr = new RepeatBehavior(repeatCount * erradicity);
            DoubleAnimation sxa = new DoubleAnimation(1, s2.Width / s1.Width, d);
            DoubleAnimation sya = new DoubleAnimation(1, s2.Height / s1.Height, d);
            DoubleAnimation oa = new DoubleAnimation(opacity1, opacity2, dErr);
            sxa.RepeatBehavior = rbErr;
            sya.RepeatBehavior = rbErr;
            oa.RepeatBehavior = rbErr;
            sxa.Duration = dErr;
            sya.Duration = dErr;
            oa.Duration = dErr;
            sxa.AutoReverse = autoreverse;
            sya.AutoReverse = autoreverse;

            AnimationClock csx = sxa.CreateClock();
            AnimationClock csy = sya.CreateClock();
            AnimationClock co = oa.CreateClock();
            csx.Controller.Begin();
            csy.Controller.Begin();
            co.Controller.Begin();
            csx.Controller.Pause();
            csy.Controller.Pause();
            co.Controller.Pause();

            EllipseGeometry eg = new EllipseGeometry(new Point(0,0), s1.Width, s1.Height);

            ScaleTransform st = new ScaleTransform();
            TranslateTransform tt = new TranslateTransform(pt.X, pt.Y);
            TransformGroup tg = new TransformGroup();
            tg.Children.Add(st);
            tg.Children.Add(tt);
            st.ApplyAnimationClock(ScaleTransform.ScaleXProperty, csx);
            st.ApplyAnimationClock(ScaleTransform.ScaleYProperty, csy);
            //st.CenterX = 0.5;
            //st.CenterY = 0.5;

            AnimationPrimitive prim = new AnimationPrimitive();
            prim.SetGeometry(eg);
            prim.RenderTransform = tg;
            //prim.RenderTransformOrigin = new Point(.5, .5);
            prim.ApplyAnimationClock(Shape.OpacityProperty, co);
            prim.SetAnimations(new AnimationClock[] { csx, csy, co }, time);
            prim.Fill = fill;
            prim.Stroke = stroke;
            prim.StrokeThickness = strokeThickness;

            return new Animation(new AnimationPrimitive[] { prim }, time);
        }
        #endregion
    }
}
