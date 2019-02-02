using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Media.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Media
{
    [Export(typeof(IAnimationGenerator))]
    public class AnimationGenerator : IAnimationGenerator
    {
        const int DEFAULT_VELOCITY = 150; // milli-seconds (if set to negative or zero value)

        public AnimationGenerator() { }

        public int CalculateRunTime(AnimationTemplate animationTemplate, Point sourcePoint, Point[] targetPoints)
        {
            // First, check for non-constant velocity animations
            if (!animationTemplate.ConstantVelocity)
            {
                return animationTemplate.AnimationTime *
                       animationTemplate.RepeatCount *
                      (animationTemplate.AutoReverse ? 2 : 1);
            }

            // Check target points
            if (targetPoints.Length == 0)
                return 0;

            // Else, calculate total distance and then return total time
            switch (animationTemplate.Type)
            {
                case AnimationType.ProjectileSelfToTarget:
                case AnimationType.ProjectileTargetToSelf:
                case AnimationType.ProjectileSelfToTargetsInRange:
                case AnimationType.ProjectileTargetsInRangeToSelf:
                    {
                        // Calculate the max distance between source and target
                        var maxDistance = targetPoints.Max(point =>
                        {
                            return Math.Sqrt(Math.Pow(point.X - sourcePoint.X, 2) + Math.Pow(point.Y - sourcePoint.Y, 2));
                        });

                        // Pixels per second
                        var velocity = animationTemplate.Velocity;

                        // x = vt - making sure to set defaults if bad parameters
                        if (velocity <= 0)
                            velocity = DEFAULT_VELOCITY;

                        // Return total number of milliseconds
                        return (int)(maxDistance / velocity) * 1000;
                    }
                case AnimationType.ChainSelfToTargetsInRange:
                    {
                        // Calculate total distance to determine constant velocity period
                        var distance = 0D;
                        var point1 = sourcePoint;

                        for (int i = 0; i < targetPoints.Length - 1; i++)
                        {
                            var point2 = targetPoints[i];

                            distance += Math.Sqrt(Math.Pow((point2.X - point1.X), 2) + Math.Pow((point2.Y - point1.Y), 2));

                            point1 = point2;
                        }

                        // Pixels per second
                        var velocity = animationTemplate.Velocity;

                        // x = vt - making sure to set defaults if bad parameters
                        if (velocity <= 0)
                            velocity = DEFAULT_VELOCITY;

                        // Return total number of milliseconds
                        return (int)(distance / velocity) * 1000;
                    }
                default:
                    throw new Exception("Tried to set constant velocity for animation type " + animationTemplate.Type.ToString());
            }
        }

        public IEnumerable<ITimedGraphic> CreateAnimation(IEnumerable<AnimationTemplate> animationTemplates, Rect bounds, Point sourcePoint, Point[] targetPoints)
        {
            return animationTemplates.Select(x => CreateAnimation(x, bounds, sourcePoint, targetPoints))
                                     .ToList();
        }

        public ITimedGraphic CreateAnimation(AnimationTemplate template, Rect bounds, Point sourceLocation, Point[] targetLocations)
        {
            switch (template.Type)
            {
                case AnimationType.AuraTarget:
                    return CreateAuraModulation(targetLocations[0], template.StrokeTemplate.GenerateBrush(),
                        template.FillTemplate.GenerateBrush(), template.StrokeThickness, template.Opacity1, template.Opacity2, new Size(template.Width1, template.Height1),
                        new Size(template.Width2, template.Height2), template.AnimationTime, template.RepeatCount, template.AutoReverse);
                case AnimationType.AuraSelf:
                    return CreateAuraModulation(sourceLocation, template.StrokeTemplate.GenerateBrush(),
                        template.FillTemplate.GenerateBrush(), template.StrokeThickness, template.Opacity1, template.Opacity2, new Size(template.Width1, template.Height1),
                        new Size(template.Width2, template.Height2), template.AnimationTime, template.RepeatCount, template.AutoReverse);
                case AnimationType.BarrageTarget:
                    return CreateEllipseBarrage(template.ChildCount, targetLocations[0], template.RadiusFromFocus, template.StrokeTemplate.GenerateBrush(),
                        template.FillTemplate.GenerateBrush(), template.StrokeThickness, template.Opacity1, template.Opacity2, new Size(template.Width1, template.Height1), 
                        new Size(template.Width2, template.Height2), template.AccelerationRatio, template.Erradicity, template.AnimationTime, template.RepeatCount, template.AutoReverse);
                case AnimationType.BarrageSelf:
                    return CreateEllipseBarrage(template.ChildCount, sourceLocation, template.RadiusFromFocus, template.StrokeTemplate.GenerateBrush(),
                        template.FillTemplate.GenerateBrush(), template.StrokeThickness, template.Opacity1, template.Opacity2, new Size(template.Width1, template.Height1),
                        new Size(template.Width2, template.Height2), template.AccelerationRatio, template.Erradicity,
                        template.AnimationTime, template.RepeatCount, template.AutoReverse);
                case AnimationType.BubblesTarget:
                    return CreateBubbles(template.ChildCount, targetLocations[0], template.RadiusFromFocus, template.StrokeTemplate.GenerateBrush(), template.FillTemplate.GenerateBrush(),
                        template.StrokeThickness, template.Opacity1, template.Opacity2, new Size(template.Width1, template.Height1),
                        new Size(template.Width2, template.Height2), template.RoamRadius, template.Erradicity, template.AnimationTime, template.RepeatCount);
                case AnimationType.BubblesSelf:
                    return CreateBubbles(template.ChildCount, sourceLocation, template.RadiusFromFocus, template.StrokeTemplate.GenerateBrush(), template.FillTemplate.GenerateBrush(),
                         template.StrokeThickness, template.Opacity1, template.Opacity2, new Size(template.Width1, template.Height1),
                        new Size(template.Width2, template.Height2), template.RoamRadius, template.Erradicity, template.AnimationTime, template.RepeatCount);
                case AnimationType.BubblesScreen:
                    return CreateBubbles(template.ChildCount, bounds, template.StrokeTemplate.GenerateBrush(), template.FillTemplate.GenerateBrush(),
                            template.StrokeThickness, template.Opacity1, template.Opacity2, new Size(template.Width1, template.Height1),
                        new Size(template.Width2, template.Height2), Math.Min(bounds.Width, bounds.Height), template.Erradicity, template.AnimationTime, template.RepeatCount);
                case AnimationType.ChainSelfToTargetsInRange:
                    {
                        List<Point> list = new List<Point>();
                        list.Add(sourceLocation);
                        list.AddRange(targetLocations);
                        return new AnimationGroup(new Animation[]{GenerateProjectilePath(list.ToArray()
                                , new Size(template.Width1, template.Height1)
                                , new Size(template.Width2, template.Height2)
                                , template.StrokeTemplate.GenerateBrush()
                                , template.FillTemplate.GenerateBrush()
                                , template.Opacity1, template.Opacity2, template.StrokeThickness
                                , template.AccelerationRatio, template.Erradicity, template.AnimationTime
                                , template.Velocity, template.RepeatCount, template.AutoReverse, template.ConstantVelocity)});
                    }
                case AnimationType.ProjectileTargetsInRangeToSelf:
                    {
                        List<Animation> list = new List<Animation>();
                        foreach (Point ep in targetLocations)
                        {
                            Animation a = GenerateProjectilePath(new Point[]{ep, sourceLocation}
                                , new Size(template.Width1, template.Height1)
                                , new Size(template.Width2, template.Height2)
                                , template.StrokeTemplate.GenerateBrush()
                                , template.FillTemplate.GenerateBrush()
                                , template.Opacity1, template.Opacity2, template.StrokeThickness
                                , template.AccelerationRatio, template.Erradicity, template.AnimationTime
                                , template.Velocity, template.RepeatCount, template.AutoReverse, template.ConstantVelocity);
                            list.Add(a);
                        }
                        return new AnimationGroup(list.ToArray());
                    }
                case AnimationType.ProjectileTargetToSelf:
                    return CreateEllipseProjectile(targetLocations[0], sourceLocation, template.StrokeTemplate.GenerateBrush(), template.FillTemplate.GenerateBrush(),
                        template.StrokeThickness, template.Opacity1, template.Opacity2, new Size(template.Width1, template.Height1),
                        new Size(template.Width2, template.Height2),
                        template.AccelerationRatio, template.Erradicity, template.AnimationTime, template.Velocity, template.RepeatCount, template.AutoReverse, template.ConstantVelocity);
                case AnimationType.ProjectileSelfToTargetsInRange:
                    {
                        List<Animation> list = new List<Animation>();
                        foreach (Point ep in targetLocations)
                        {
                            Animation a = GenerateProjectilePath(new Point[] { sourceLocation, ep }
                                , new Size(template.Width1, template.Height1)
                                , new Size(template.Width2, template.Height2)
                                , template.StrokeTemplate.GenerateBrush()
                                , template.FillTemplate.GenerateBrush()
                                , template.Opacity1, template.Opacity2, template.StrokeThickness
                                , template.AccelerationRatio, template.Erradicity, template.AnimationTime
                                , template.Velocity, template.RepeatCount, template.AutoReverse, template.ConstantVelocity);
                            list.Add(a);
                        }
                        return new AnimationGroup(list.ToArray());
                    }
                case AnimationType.ProjectileSelfToTarget:
                    return CreateEllipseProjectile(sourceLocation, targetLocations[0], template.StrokeTemplate.GenerateBrush(), template.FillTemplate.GenerateBrush(),
                        template.StrokeThickness, template.Opacity1, template.Opacity2, new Size(template.Width1, template.Height1),
                        new Size(template.Width2, template.Height2),
                        template.AccelerationRatio, template.Erradicity, template.AnimationTime, template.Velocity, template.RepeatCount, template.AutoReverse, template.ConstantVelocity);
                case AnimationType.ScreenBlink:
                    {
                        RectangleGeometry rg = new RectangleGeometry(bounds);
                        Animation a = GenerateTimedFigure(rg, template.StrokeTemplate.GenerateBrush(), template.FillTemplate.GenerateBrush(), template.StrokeThickness,
                            template.Opacity1, template.Opacity2, template.Erradicity, template.AnimationTime, template.RepeatCount, template.AutoReverse);
                        return new AnimationGroup(new Animation[] { a });
                    }
                case AnimationType.SpiralTarget:
                    return CreateEllipseSpiral(template.ChildCount, targetLocations[0], template.RadiusFromFocus, template.SpiralRate, template.StrokeTemplate.GenerateBrush(),
                        template.FillTemplate.GenerateBrush(), template.StrokeThickness, template.Opacity1, template.Opacity2, new Size(template.Width1, template.Height1),
                        new Size(template.Width2, template.Height2),template.AccelerationRatio, template.Erradicity,
                        template.AnimationTime, template.RepeatCount, template.AutoReverse);
                case AnimationType.SpiralSelf:
                    return CreateEllipseSpiral(template.ChildCount, sourceLocation, template.RadiusFromFocus, template.SpiralRate, template.StrokeTemplate.GenerateBrush(),
                        template.FillTemplate.GenerateBrush(), template.StrokeThickness, template.Opacity1, template.Opacity2, new Size(template.Width1, template.Height1),
                        new Size(template.Width2, template.Height2), template.AccelerationRatio, template.Erradicity,
                        template.AnimationTime, template.RepeatCount, template.AutoReverse);
            }
            return null;
        }
        public AnimationGroup CreateEllipseProjectile(Point p1, Point p2, Brush stroke, Brush fill, 
            double strokethickness, double opacity1, double opacity2, Size s1, Size s2, double accRatio, int erradicity, int time, int velocity,
            int repeatCount, bool autoreverse, bool constVelocity)
        {
            Animation a = GenerateProjectilePath(new Point[] { p1, p2 }, s1, s2, stroke,fill,opacity1, opacity2, strokethickness, accRatio, erradicity, time, velocity, repeatCount, autoreverse, constVelocity);
            return new AnimationGroup(new Animation[] { a });
        }
        public AnimationGroup CreateEllipseBarrage(int count, Point focus, double radiusFromFocus, Brush stroke, 
            Brush fill, double strokeThickness, double opacity1, double opacity2, Size s1, Size s2, double accRatio, int erradicity, 
            int time, int repeatcount, bool autoreverse)
        {
            Random r = new Random(12342);
            List<Animation> list = new List<Animation>();
            for (int i = 0; i < count; i++)
            {
                double angle = r.NextDouble() * Math.PI * 2;
                double radius = r.NextDouble() * radiusFromFocus;
                Point p = new Point(focus.X + (radius * Math.Cos(angle)), focus.Y + (radius * Math.Sin(angle)));
                Animation a = GenerateProjectilePath(new Point[] { p, focus }, s1, s2, stroke, fill,opacity1, opacity2, strokeThickness, accRatio, erradicity, time, 1,  repeatcount, autoreverse, false);
                list.Add(a);
            }
            return new AnimationGroup(list.ToArray());
        }
        public AnimationGroup CreateEllipseSpiral(int count, Point focus, double radius, double spiralRate, 
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
            return new AnimationGroup(list.ToArray());
        }
        public AnimationGroup CreateAuraModulation(Point p, Brush stroke, Brush fill, double strokeThickness, double opacity1, double opacity2, Size s1, Size s2, int time, int repeatCount, bool autoreverse)
        {
            Animation a = GenerateAuraAnimation(p, s1, s2,stroke, fill, opacity1, opacity2,  strokeThickness, 0, 1, time, repeatCount, autoreverse);
            return new AnimationGroup(new Animation[] { a });
        }
        public AnimationGroup CreateBubbles(int count, Rect rect, Brush stroke, Brush fill, double strokeThickness, double opacity1, double opacity2, Size s1, Size s2, double roamRadius, int erradicity, int time, int repeatCount)
        {
            Random r = new Random(13322);
            List<Animation> list = new List<Animation>();
            for (int i = 0; i < count; i++)
            {
                List<Point> pts = new List<Point>();
                for (int j = 0; j < erradicity; j++)
                {
                    Point pt = new Point(rect.X + (r.NextDouble() * roamRadius), rect.Y + (r.NextDouble() * roamRadius));
                    pts.Add(pt);
                }
                Animation a = GenerateProjectilePath(pts.ToArray(), s1, s2,stroke, fill, opacity1, opacity2, strokeThickness, 0, erradicity, time, 1,repeatCount, false, false);
                list.Add(a);
                pts.Clear();
                pts = null;
            }
            return new AnimationGroup(list.ToArray());
        }
        public AnimationGroup CreateBubbles(int count, Point focus, double radiusFromFocus, Brush stroke, Brush fill, double strokeThickness, double opacity1, double opacity2, Size s1, Size s2, double roamRadius, int erradicity, int time, int repeatCount)
        {
            Random r = new Random(13322);
            List<Animation> list = new List<Animation>();
            for (int i = 0; i < count; i++)
            {
                List<Point> pts = new List<Point>();

                //Get random point in radius from focus
                double angle = r.NextDouble() * Math.PI * 2;
                double radius = r.NextDouble() * radiusFromFocus;
                Point pt = new Point(radius * Math.Cos(angle) + focus.X, radius * Math.Sin(angle) + focus.Y);

                for (int j = 0; j < Math.Max(2, erradicity); j++)
                {
                    //Get random point in roam radius from that random point
                    double roam_a = r.NextDouble() * Math.PI * 2;
                    double roam_r = r.NextDouble() * roamRadius;
                    Point p = new Point(roam_r * Math.Cos(roam_a) + pt.X, roam_r * Math.Sin(roam_a) + pt.Y);
                    pts.Add(p);
                }
                Animation a = GenerateProjectilePath(pts.ToArray(), s1, s2, stroke, fill, opacity1, opacity2, strokeThickness, 0, erradicity, time, 5, repeatCount, false, false);
                list.Add(a);
                pts.Clear();
                pts = null;
            }
            return new AnimationGroup(list.ToArray());
        }
        public AnimationGroup CreateFadingChain(Point[] pts, Brush stroke, double strokeThickness,double opacity1, double opacity2, int erradicity, int time, int repeatCount, bool autoreverse)
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
                Animation a = new Animation(new ITimedGraphic[] { fig });
                a.SetStartupDelay((int)((time / (double)(pts.Length - 1)) * i));
                list.Add(a);
            }
            return new AnimationGroup(list.ToArray());
        }
        public Animation GenerateTimedFigure(Geometry g, Brush stroke, Brush fill, double strokethickness, double opacity1, double opacity2, int erradicity, int time, int repeatCount, bool autoreverse)
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
            prim.SetAnimations(new AnimationClock[] { clock });
            return new Animation(new ITimedGraphic[] { prim });
        }
        public Animation GenerateProjectilePath(Point[] points, Size size1, Size size2, Brush stroke, Brush fill, double opacity1, double opacity2, double strokeThickness, double accelerationRatio, int erradicity, int time, int velocity, int repeatCount, bool autoreverse, bool constVelocity)
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
            Duration d = new Duration(new TimeSpan(0, 0, 0, 0, (int)time));
            Duration dErr = (erradicity > 0)?new Duration(new TimeSpan(0, 0, 0, 0, (int)(time / (double) erradicity))) : d;
            RepeatBehavior rb = new RepeatBehavior(repeatCount);
            RepeatBehavior rbErr = new RepeatBehavior(repeatCount * erradicity);
            DoubleAnimationUsingPath ax = new DoubleAnimationUsingPath();
            DoubleAnimationUsingPath ay = new DoubleAnimationUsingPath();
            DoubleAnimationUsingPath aa = new DoubleAnimationUsingPath();
            DoubleAnimation sxa = new DoubleAnimation(1, size2.Width / size1.Width, d);
            DoubleAnimation sya = new DoubleAnimation(1, size2.Height / size1.Height, d);
            DoubleAnimation oa = new DoubleAnimation(opacity1, opacity2, dErr);
            ax.PathGeometry = geometry;
            ay.PathGeometry = geometry;
            aa.PathGeometry = geometry;
            ax.RepeatBehavior = rb;
            ay.RepeatBehavior = rb;
            aa.RepeatBehavior = rb;
            sxa.RepeatBehavior = rbErr;
            sya.RepeatBehavior = rbErr;
            oa.RepeatBehavior = rbErr;
            ax.Duration = d;
            ay.Duration = d;
            aa.Duration = d;
            sxa.Duration = dErr;
            sya.Duration = dErr;
            oa.Duration = dErr;
            ax.AutoReverse = autoreverse;
            ay.AutoReverse = autoreverse;
            aa.AutoReverse = autoreverse;
            sxa.AutoReverse = autoreverse;
            sya.AutoReverse = autoreverse;
            ax.Source = PathAnimationSource.X;
            ay.Source = PathAnimationSource.Y;
            aa.Source = PathAnimationSource.Angle;

            AnimationClock cx = ax.CreateClock();
            AnimationClock cy = ay.CreateClock();
            AnimationClock ca = aa.CreateClock();
            AnimationClock csx = sxa.CreateClock();
            AnimationClock csy = sya.CreateClock();
            AnimationClock co = oa.CreateClock();
            cx.Controller.Begin();
            cy.Controller.Begin();
            ca.Controller.Begin();
            csx.Controller.Begin();
            csy.Controller.Begin();
            co.Controller.Begin();
            cx.Controller.Pause();
            cy.Controller.Pause();
            ca.Controller.Pause();
            csx.Controller.Pause();
            csy.Controller.Pause();
            co.Controller.Pause();

            EllipseGeometry eg = new EllipseGeometry(new Point(0, 0), size1.Width, size1.Height);

            RotateTransform rt = new RotateTransform();
            TranslateTransform tt = new TranslateTransform();
            ScaleTransform st = new ScaleTransform();
            TransformGroup tg = new TransformGroup();
            tg.Children.Add(st);
            tg.Children.Add(rt);
            tg.Children.Add(tt);
            rt.ApplyAnimationClock(RotateTransform.AngleProperty, ca);
            tt.ApplyAnimationClock(TranslateTransform.XProperty, cx);
            tt.ApplyAnimationClock(TranslateTransform.YProperty, cy);
            st.ApplyAnimationClock(ScaleTransform.ScaleXProperty, csx);
            st.ApplyAnimationClock(ScaleTransform.ScaleYProperty, csy);
            //st.CenterX = 0.5;
            //st.CenterY = 0.5;

            AnimationPrimitive prim = new AnimationPrimitive();
            prim.SetGeometry(eg);
            prim.RenderTransform = tg;
            //prim.RenderTransformOrigin = new Point(.5, .5);
            prim.ApplyAnimationClock(Shape.OpacityProperty, co);
            prim.SetAnimations(new AnimationClock[] { cx, cy, ca, csx, csy, co });
            prim.Fill = fill;
            prim.Stroke = stroke;
            prim.StrokeThickness = strokeThickness;

            return new Animation(new AnimationPrimitive[] { prim });
        }
        public Animation GenerateAuraAnimation(Point pt, Size s1, Size s2, Brush stroke, Brush fill, double opacity1, double opacity2, double strokeThickness, double accelerationRatio, int erradicity, int time, int repeatCount, bool autoreverse)
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
            prim.SetAnimations(new AnimationClock[] { csx, csy, co });
            prim.Fill = fill;
            prim.Stroke = stroke;
            prim.StrokeThickness = strokeThickness;

            return new Animation(new AnimationPrimitive[] { prim });
        }

        public IEnumerable<ITimedGraphic> CreateTargetingAnimation(Point[] points)
        {
            var result = new List<ITimedGraphic>();

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

                result.Add(new AnimationGroup(new Animation[] { animation1, animation2, animation3, animation4, animation5 }));
            }

            return result;
        }
    }
}
