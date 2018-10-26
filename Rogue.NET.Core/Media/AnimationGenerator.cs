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

namespace Rogue.NET.Core.Media
{
    public class AnimationGenerator : IAnimationGenerator
    {
        public IEnumerable<ITimedGraphic> CreateAnimation(IEnumerable<AnimationTemplate> animationTemplates, Rect bounds, Point sourcePoint, Point[] targetPoints)
        {
            return animationTemplates.Select(x => CreateAnimation(x, bounds, sourcePoint, targetPoints))
                                     .ToList();
        }

        public ITimedGraphic CreateAnimation(AnimationTemplate t, Rect bounds, Point pPlayer, Point[] pEnemy)
        {
            switch (t.Type)
            {
                case AnimationType.AuraTarget:
                    return CreateAuraModulation(pEnemy[0], t.StrokeTemplate.GenerateBrush(),
                        t.FillTemplate.GenerateBrush(), t.StrokeThickness, t.Opacity1, t.Opacity2, new Size(t.Width1, t.Height1),
                        new Size(t.Width2, t.Height2), t.AnimationTime, t.RepeatCount, t.AutoReverse);
                case AnimationType.AuraSelf:
                    return CreateAuraModulation(pPlayer, t.StrokeTemplate.GenerateBrush(),
                        t.FillTemplate.GenerateBrush(), t.StrokeThickness, t.Opacity1, t.Opacity2, new Size(t.Width1, t.Height1),
                        new Size(t.Width2, t.Height2), t.AnimationTime, t.RepeatCount, t.AutoReverse);
                case AnimationType.BarrageTarget:
                    return CreateEllipseBarrage(t.ChildCount, pEnemy[0], t.RadiusFromFocus, t.StrokeTemplate.GenerateBrush(),
                        t.FillTemplate.GenerateBrush(), t.StrokeThickness, t.Opacity1, t.Opacity2, new Size(t.Width1, t.Height1), 
                        new Size(t.Width2, t.Height2), t.AccelerationRatio, t.Erradicity, t.AnimationTime, t.RepeatCount, t.AutoReverse);
                case AnimationType.BarrageSelf:
                    return CreateEllipseBarrage(t.ChildCount, pPlayer, t.RadiusFromFocus, t.StrokeTemplate.GenerateBrush(),
                        t.FillTemplate.GenerateBrush(), t.StrokeThickness, t.Opacity1, t.Opacity2, new Size(t.Width1, t.Height1),
                        new Size(t.Width2, t.Height2), t.AccelerationRatio, t.Erradicity,
                        t.AnimationTime, t.RepeatCount, t.AutoReverse);
                case AnimationType.BubblesTarget:
                    return CreateBubbles(t.ChildCount, pEnemy[0], t.RadiusFromFocus, t.StrokeTemplate.GenerateBrush(), t.FillTemplate.GenerateBrush(),
                        t.StrokeThickness, t.Opacity1, t.Opacity2, new Size(t.Width1, t.Height1),
                        new Size(t.Width2, t.Height2), t.RoamRadius, t.Erradicity, t.AnimationTime, t.RepeatCount);
                case AnimationType.BubblesSelf:
                    return CreateBubbles(t.ChildCount, pPlayer, t.RadiusFromFocus, t.StrokeTemplate.GenerateBrush(), t.FillTemplate.GenerateBrush(),
                         t.StrokeThickness, t.Opacity1, t.Opacity2, new Size(t.Width1, t.Height1),
                        new Size(t.Width2, t.Height2), t.RoamRadius, t.Erradicity, t.AnimationTime, t.RepeatCount);
                case AnimationType.BubblesScreen:
                    return CreateBubbles(t.ChildCount, bounds, t.StrokeTemplate.GenerateBrush(), t.FillTemplate.GenerateBrush(),
                            t.StrokeThickness, t.Opacity1, t.Opacity2, new Size(t.Width1, t.Height1),
                        new Size(t.Width2, t.Height2), Math.Min(bounds.Width, bounds.Height), t.Erradicity, t.AnimationTime, t.RepeatCount);
                case AnimationType.ChainSelfToTargetsInRange:
                    {
                        List<Point> list = new List<Point>();
                        list.Add(pPlayer);
                        list.AddRange(pEnemy);
                        return new AnimationGroup(new Animation[]{GenerateProjectilePath(list.ToArray()
                                , new Size(t.Width1, t.Height1)
                                , new Size(t.Width2, t.Height2)
                                , t.StrokeTemplate.GenerateBrush()
                                , t.FillTemplate.GenerateBrush()
                                , t.Opacity1, t.Opacity2, t.StrokeThickness
                                , t.AccelerationRatio, t.Erradicity, t.AnimationTime
                                , t.Velocity, t.RepeatCount, t.AutoReverse, t.ConstantVelocity)});
                    }
                case AnimationType.ProjectileTargetsInRangeToSelf:
                    {
                        List<Animation> list = new List<Animation>();
                        foreach (Point ep in pEnemy)
                        {
                            Animation a = GenerateProjectilePath(new Point[]{ep, pPlayer}
                                , new Size(t.Width1, t.Height1)
                                , new Size(t.Width2, t.Height2)
                                , t.StrokeTemplate.GenerateBrush()
                                , t.FillTemplate.GenerateBrush()
                                , t.Opacity1, t.Opacity2, t.StrokeThickness
                                , t.AccelerationRatio, t.Erradicity, t.AnimationTime
                                , t.Velocity, t.RepeatCount, t.AutoReverse, t.ConstantVelocity);
                            list.Add(a);
                        }
                        return new AnimationGroup(list.ToArray());
                    }
                case AnimationType.ProjectileTargetToSelf:
                    return CreateEllipseProjectile(pEnemy[0], pPlayer, t.StrokeTemplate.GenerateBrush(), t.FillTemplate.GenerateBrush(),
                        t.StrokeThickness, t.Opacity1, t.Opacity2, new Size(t.Width1, t.Height1),
                        new Size(t.Width2, t.Height2),
                        t.AccelerationRatio, t.Erradicity, t.AnimationTime, t.Velocity, t.RepeatCount, t.AutoReverse, t.ConstantVelocity);
                case AnimationType.ProjectileSelfToTargetsInRange:
                    {
                        List<Animation> list = new List<Animation>();
                        foreach (Point ep in pEnemy)
                        {
                            Animation a = GenerateProjectilePath(new Point[] { pPlayer, ep }
                                , new Size(t.Width1, t.Height1)
                                , new Size(t.Width2, t.Height2)
                                , t.StrokeTemplate.GenerateBrush()
                                , t.FillTemplate.GenerateBrush()
                                , t.Opacity1, t.Opacity2, t.StrokeThickness
                                , t.AccelerationRatio, t.Erradicity, t.AnimationTime
                                , t.Velocity, t.RepeatCount, t.AutoReverse, t.ConstantVelocity);
                            list.Add(a);
                        }
                        return new AnimationGroup(list.ToArray());
                    }
                case AnimationType.ProjectileSelfToTarget:
                    return CreateEllipseProjectile(pPlayer, pEnemy[0], t.StrokeTemplate.GenerateBrush(), t.FillTemplate.GenerateBrush(),
                        t.StrokeThickness, t.Opacity1, t.Opacity2, new Size(t.Width1, t.Height1),
                        new Size(t.Width2, t.Height2),
                        t.AccelerationRatio, t.Erradicity, t.AnimationTime, t.Velocity, t.RepeatCount, t.AutoReverse, t.ConstantVelocity);
                case AnimationType.ScreenBlink:
                    {
                        RectangleGeometry rg = new RectangleGeometry(bounds);
                        Animation a = GenerateTimedFigure(rg, t.StrokeTemplate.GenerateBrush(), t.FillTemplate.GenerateBrush(), t.StrokeThickness,
                            t.Opacity1, t.Opacity2, t.Erradicity, t.AnimationTime, t.RepeatCount, t.AutoReverse);
                        return new AnimationGroup(new Animation[] { a });
                    }
                case AnimationType.SpiralTarget:
                    return CreateEllipseSpiral(t.ChildCount, pEnemy[0], t.RadiusFromFocus, t.SpiralRate, t.StrokeTemplate.GenerateBrush(),
                        t.FillTemplate.GenerateBrush(), t.StrokeThickness, t.Opacity1, t.Opacity2, new Size(t.Width1, t.Height1),
                        new Size(t.Width2, t.Height2),t.AccelerationRatio, t.Erradicity,
                        t.AnimationTime, t.RepeatCount, t.AutoReverse);
                case AnimationType.SpiralSelf:
                    return CreateEllipseSpiral(t.ChildCount, pPlayer, t.RadiusFromFocus, t.SpiralRate, t.StrokeTemplate.GenerateBrush(),
                        t.FillTemplate.GenerateBrush(), t.StrokeThickness, t.Opacity1, t.Opacity2, new Size(t.Width1, t.Height1),
                        new Size(t.Width2, t.Height2), t.AccelerationRatio, t.Erradicity,
                        t.AnimationTime, t.RepeatCount, t.AutoReverse);
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
        public Animation GenerateProjectilePath(Point[] pts, Size s1, Size s2, Brush stroke, Brush fill, double opacity1, double opacity2, double strokeThickness, double accelerationRatio, int erradicity, int time, int velocity, int repeatCount, bool autoreverse, bool constVelocity)
        {
            //Generate Path geometry from points
            PathGeometry pg = new PathGeometry();
            PathFigure f = new PathFigure();
            double dpath = 0;
            for (int i = 0; i < pts.Length - 1; i++)
            {
                Point p1 = pts[i];
                Point p2 = pts[i + 1];
                if (i == 0)
                    f.StartPoint = p1;
                f.Segments.Add(new LineSegment(p2, false));
                dpath += Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
            }
            pg.Figures.Add(f);

            //Check for const velocity
            if (constVelocity)
            {
                //x = vt - making sure to set defaults if bad parameters
                if (velocity <= 0)
                    velocity = 1;

                //convert to seconds
                time = (int)((dpath * 1000) / velocity);
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
            DoubleAnimation sxa = new DoubleAnimation(1, s2.Width / s1.Width, d);
            DoubleAnimation sya = new DoubleAnimation(1, s2.Height / s1.Height, d);
            DoubleAnimation oa = new DoubleAnimation(opacity1, opacity2, dErr);
            ax.PathGeometry = pg;
            ay.PathGeometry = pg;
            aa.PathGeometry = pg;
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

            EllipseGeometry eg = new EllipseGeometry(new Point(0, 0), s1.Width, s1.Height);

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
    }
}
