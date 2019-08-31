using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.View
{
    [Export]
    public partial class Smiley : UserControl
    {
        // All distances are measured from the MERIDIANS (Half way points)
        // of the control. 
        //
        // The control is broken down into three regions: left eye, right eye, mouth
        //
        // Each region is calculated based on the constants below - which define 
        // lengths measured from the meridians to create a rectangle in which the
        // path geometry figures are drawn.
        //

        protected const double EYE_X_INNER_RATIO = 0.15;
        protected const double EYE_X_OUTER_RATIO = 0.65;
        protected const double EYE_HEIGHT_RATIO = 0.35;      // Ratio of total height
        protected const double EYE_OFFSET_RATIO = 0.25;      // Ratio of eye region height to offset
        protected const double MOUTH_X_RATIO = 0.70;
        protected const double MOUTH_Y_TOP_RATIO = 0.48;     // Locates the top of the mouth region
        protected const double MOUTH_Y_BOTTOM_RATIO = 0.72;  // Locates the bottom of the mouth region

        // Body corner radius for the default cell-width
        protected const double BODY_RADIUS_MULTIPLIER = 2.0D;

        // Stroke thickness based on default cell-width
        protected const double STROKE_THICKNESS_MULTIPLIER = 0.75D;

        // Stroke thickness multipliers applied to the maximum scaled line thickness
        protected const double MOUTH_STROKE_THICKNESS_MULTIPLIER = 0.40;
        protected const double BODY_STROKE_THICKNESS_MULTIPLIER = 0.20;

        public static readonly DependencyProperty SmileyColorProperty =
            DependencyProperty.Register("SmileyColor", typeof(Color), typeof(Smiley),
                new PropertyMetadata(new PropertyChangedCallback(Smiley.OnColorChanged)));

        public static readonly DependencyProperty SmileyLineColorProperty =
            DependencyProperty.Register("SmileyLineColor", typeof(Color), typeof(Smiley),
                new PropertyMetadata(new PropertyChangedCallback(Smiley.OnLineColorChanged)));

        public static readonly DependencyProperty SmileyExpressionProperty =
            DependencyProperty.Register("SmileyExpression", typeof(SmileyExpression), typeof(Smiley),
                new PropertyMetadata(new PropertyChangedCallback(Smiley.OnExpressionChanged)));

        public Color SmileyColor
        {
            get { return (Color)GetValue(Smiley.SmileyColorProperty); }
            set { SetValue(Smiley.SmileyColorProperty, value); }
        }
        public Color SmileyLineColor
        {
            get { return (Color)GetValue(Smiley.SmileyLineColorProperty); }
            set { SetValue(Smiley.SmileyLineColorProperty, value); }
        }
        public SmileyExpression SmileyExpression
        {
            get { return (SmileyExpression)GetValue(Smiley.SmileyExpressionProperty); }
            set { SetValue(Smiley.SmileyExpressionProperty, value); }
        }

        protected Rect GetLeftEyeBounds()
        {
            var halfWidth = (this.Width / 2);
            var leftInnerPosition = halfWidth * (1 - EYE_X_INNER_RATIO);
            var leftOuterPosition = halfWidth * (1 - EYE_X_OUTER_RATIO);

            var halfHeight = (this.Height / 2);
            var eyeHeight = this.Height * EYE_HEIGHT_RATIO;
            var eyeHalfHeight = eyeHeight / 2.0D;
            var eyeHeightDelta = eyeHeight * EYE_OFFSET_RATIO;
            var bottomPosition = halfHeight + eyeHalfHeight - eyeHeightDelta;
            var topPosition = halfHeight - eyeHalfHeight - eyeHeightDelta;

            return new Rect(new Point(leftOuterPosition, topPosition),
                            new Point(leftInnerPosition, bottomPosition));
        }

        protected Rect GetRightEyeBounds()
        {
            var halfWidth = (this.Width / 2);
            var rightInnerPosition = halfWidth * (1 + EYE_X_INNER_RATIO);
            var rightOuterPosition = halfWidth * (1 + EYE_X_OUTER_RATIO);

            var halfHeight = (this.Height / 2);
            var eyeHeight = this.Height * EYE_HEIGHT_RATIO;
            var eyeHalfHeight = eyeHeight / 2.0D;
            var eyeHeightDelta = eyeHeight * EYE_OFFSET_RATIO;
            var bottomPosition = halfHeight + eyeHalfHeight - eyeHeightDelta;
            var topPosition = halfHeight - eyeHalfHeight - eyeHeightDelta;

            return new Rect(new Point(rightInnerPosition, topPosition),
                            new Point(rightOuterPosition, bottomPosition));
        }

        protected Rect GetMouthBounds()
        {
            var halfWidth = (this.Width / 2);
            var leftPosition = halfWidth * (1 - MOUTH_X_RATIO);
            var rightPosition = halfWidth * (1 + MOUTH_X_RATIO);

            var halfHeight = (this.Height / 2);
            var topLeft = new Point(leftPosition, halfHeight * (1 + MOUTH_Y_TOP_RATIO));
            var bottomRight = new Point(rightPosition, halfHeight * (1 + MOUTH_Y_BOTTOM_RATIO));
            return new Rect(topLeft, bottomRight);
        }

        public Smiley()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Forces the control to re-size it's features. This should never have to be called; but
        /// there's a problem initializing the control to render before it's set to a parent. If a
        /// dependency property is changed that forces it to render because it "affect the layout" or
        /// something else - it works. Otherwise, changing the expression would also acheive this.
        /// </summary>
        public void Initialize()
        {
            Draw();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            // SCALE BASED ON NEW SIZE
            base.OnRenderSizeChanged(sizeInfo);

            Draw();
        }

        private static void OnColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Smiley;
            var color = (Color)e.NewValue;

            if (control != null &&
                color != null)
            {
                control.BodyRectangle.Fill = new SolidColorBrush(color);
            }
        }

        private static void OnLineColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Smiley;
            var color = (Color)e.NewValue;

            if (control != null &&
                color != null)
            {
                control.LeftEyePath.Stroke = new SolidColorBrush(color);
                control.RightEyePath.Stroke = new SolidColorBrush(color);
                control.MouthPath.Stroke = new SolidColorBrush(color);
                control.BodyRectangle.Stroke = new SolidColorBrush(color);
            }
        }

        private static void OnExpressionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = o as Smiley;

            if (control != null)
            {
                control.Draw();
            }
        }

        protected void Draw()
        {
            // TODO: Figure out why this would happen and remove this check!!
            //       Why does the render size get set to NaN? How do you hook the
            //       visual events to do this at the right time?
            //
            if (double.IsNaN(this.Width) ||
                double.IsNaN(this.Height))
                return;

            var lineThickness = STROKE_THICKNESS_MULTIPLIER * (this.Width / ModelConstants.CellWidth);
            var radius = BODY_RADIUS_MULTIPLIER * (this.Width / ModelConstants.CellWidth);

            // Draw the body
            this.BodyRectangle.Height = this.Height;
            this.BodyRectangle.Width = this.Width;

            // Draw the facial features
            this.MouthPath.Data = DrawMouth();             // Offset by eye path thickness (no multiplier)
            this.LeftEyePath.Data = DrawLeftEye();
            this.RightEyePath.Data = DrawRightEye();

            // Scale the line thickness and corner radii
            this.MouthPath.StrokeThickness = lineThickness * MOUTH_STROKE_THICKNESS_MULTIPLIER;
            this.LeftEyePath.StrokeThickness = lineThickness;
            this.RightEyePath.StrokeThickness = lineThickness;
            this.BodyRectangle.StrokeThickness = lineThickness * BODY_STROKE_THICKNESS_MULTIPLIER;

            // Scale the body radius
            this.BodyRectangle.RadiusX = radius;
            this.BodyRectangle.RadiusY = radius;

            // Change the fill based on the expression
            switch (this.SmileyExpression)
            {
                // Path Fill (ALL) -> Transparent
                default:
                case SmileyExpression.Happy:
                case SmileyExpression.Blind:
                case SmileyExpression.Dead:
                case SmileyExpression.Disgruntled:
                case SmileyExpression.Emoji:
                case SmileyExpression.Frustrated:
                case SmileyExpression.Insane:
                case SmileyExpression.LeftWink:
                case SmileyExpression.MeanPumpkinFace:
                case SmileyExpression.Mischievous:
                case SmileyExpression.RightWink:
                case SmileyExpression.Scared:
                case SmileyExpression.Sad:
                case SmileyExpression.Sleeping:
                case SmileyExpression.Sour:
                case SmileyExpression.Unsure:
                    this.LeftEyePath.Fill = Brushes.Transparent;
                    this.RightEyePath.Fill = Brushes.Transparent;
                    this.MouthPath.Fill = Brushes.Transparent;
                    break;
                
                // Path Fill (MOUTH) -> Transparent
                case SmileyExpression.FreakedOut:
                    this.LeftEyePath.Fill = new SolidColorBrush(this.SmileyLineColor);
                    this.RightEyePath.Fill = new SolidColorBrush(this.SmileyLineColor);
                    this.MouthPath.Fill = Brushes.Transparent;
                    break;
                
                // Path Fill (ALL) -> [Line Color]
                case SmileyExpression.Shocked:
                case SmileyExpression.WeirdWhistler:
                    this.LeftEyePath.Fill = new SolidColorBrush(this.SmileyLineColor);
                    this.RightEyePath.Fill = new SolidColorBrush(this.SmileyLineColor);
                    this.MouthPath.Fill = new SolidColorBrush(this.SmileyLineColor);
                    break;
            }

            // Change stroke for those that are filled to prevent "big features"
            switch (this.SmileyExpression)
            {
                default:
                case SmileyExpression.Happy:
                case SmileyExpression.Blind:
                case SmileyExpression.Dead:
                case SmileyExpression.Disgruntled:
                case SmileyExpression.Emoji:
                case SmileyExpression.FreakedOut:
                case SmileyExpression.Frustrated:
                case SmileyExpression.Insane:
                case SmileyExpression.LeftWink:
                case SmileyExpression.MeanPumpkinFace:
                case SmileyExpression.Mischievous:
                case SmileyExpression.RightWink:
                case SmileyExpression.Sad:
                case SmileyExpression.Scared:
                case SmileyExpression.Sleeping:
                case SmileyExpression.Sour:
                case SmileyExpression.Unsure:
                    this.MouthPath.Stroke = new SolidColorBrush(this.SmileyLineColor);
                    this.LeftEyePath.Stroke = new SolidColorBrush(this.SmileyLineColor);
                    this.RightEyePath.Stroke = new SolidColorBrush(this.SmileyLineColor);
                    break;
                // The outline of the mouths made them a little too big
                case SmileyExpression.Shocked:
                case SmileyExpression.WeirdWhistler:
                    this.MouthPath.Stroke = Brushes.Transparent;
                    this.LeftEyePath.Stroke = Brushes.Transparent;
                    this.RightEyePath.Stroke = Brushes.Transparent;
                    break;
            }
        }

        protected Geometry DrawMouth()
        {
            // Procedure
            //
            // 1) Define the major points in drawing the mouth from the cavnas dimension:
            //    Left Corner, Right Corner, Center Bezier (Point)
            //
            // 2) Based on the mouth expression create offsets from those points
            //
            // 3) Draw the path figures based on those points and the expression
            //
            // (Using Golden Ratio to try and get the right facial feature offsets)
            //

            var bounds = GetMouthBounds();

            switch (this.SmileyExpression)
            {
                // Smiley Face (Happy) Bezier Curve
                default:
                case SmileyExpression.Happy:
                case SmileyExpression.Emoji:
                case SmileyExpression.Insane:
                case SmileyExpression.LeftWink:
                case SmileyExpression.Mischievous:
                case SmileyExpression.RightWink:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopLeft,new PathSegment[]
                        {
                            new QuadraticBezierSegment(bounds.BottomLeft, bounds.BottomMiddle(), true),
                            new QuadraticBezierSegment(bounds.BottomRight, bounds.TopRight, true)

                        }, false)
                    });
                // Straight-Mouthed (Unsure, maybe Scared, etc...)
                case SmileyExpression.Blind:
                case SmileyExpression.Dead:
                case SmileyExpression.Disgruntled:
                case SmileyExpression.FreakedOut:
                case SmileyExpression.Frustrated:
                case SmileyExpression.Sleeping:
                case SmileyExpression.Unsure:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.LeftMiddle(),new PathSegment[]
                        {
                            new LineSegment(bounds.RightMiddle(), true)
                        }, false)
                    });
                
                // Squggily poly-line mouth (Scared, Mean, .. Pumpkin..., etc...)
                case SmileyExpression.MeanPumpkinFace:
                case SmileyExpression.Scared:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopLeft,new PathSegment[]
                        {
                            new LineSegment(bounds.BottomAtFraction(1 / 8.0D), true),
                            new LineSegment(bounds.TopAtFraction(0.25), true),
                            new LineSegment(bounds.BottomAtFraction(3 / 8.0D), true),
                            new LineSegment(bounds.TopAtFraction(0.50), true),
                            new LineSegment(bounds.BottomAtFraction(5 / 8.0D), true),
                            new LineSegment(bounds.TopAtFraction(0.75), true),
                            new LineSegment(bounds.BottomAtFraction(7 / 8.0D), true),
                            new LineSegment(bounds.TopRight, true)

                        }, false)
                    });

                // Frown Face - bezier curve (Sad, Angry, Upset, etc...)
                case SmileyExpression.Sad:
                case SmileyExpression.Sour:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.BottomLeft,new PathSegment[]
                        {
                            new QuadraticBezierSegment(bounds.TopLeft, bounds.TopMiddle(), true),
                            new QuadraticBezierSegment(bounds.TopRight, bounds.BottomRight, true)

                        }, false)
                    });

                // Mouth Open - Ellipse (Screaming, Shocked, Shouting, maybe Angry, etc..)
                case SmileyExpression.Shocked:
                    return new EllipseGeometry(bounds.Center(), bounds.Height, bounds.Height);

                // Mouth Open Small - Circle (Whistline, maybe Ghost (?))
                case SmileyExpression.WeirdWhistler:
                    return new EllipseGeometry(bounds.Center(), bounds.Height / 2.0, bounds.Height / 2.0);
            }
        }

        protected Geometry DrawLeftEye()
        {
            // Procedure
            //
            // 1) Define the three points in drawing the mouth from the cavnas dimension. These
            //    will depend on the expression; but, generally, top, bottom, and average (center)
            //
            // 2) Based on the mouth expression create offsets from those points
            //
            // 3) Draw the path figures based on those points and the expression
            //

            var bounds = GetLeftEyeBounds();

            switch (this.SmileyExpression)
            {
                // Line Segments down the middle (Happy, Sad, Unsure, etc...)
                default:
                case SmileyExpression.Happy:
                case SmileyExpression.RightWink:
                case SmileyExpression.Sad:
                case SmileyExpression.Unsure:
                case SmileyExpression.Scared:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopMiddle(), new PathSegment[]
                        {
                            new LineSegment(bounds.BottomMiddle(), true)

                        }, false)
                    });

                // ) (
                // ---
                case SmileyExpression.Blind:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopLeft,new PathSegment[]
                        {
                            new QuadraticBezierSegment(bounds.TopRight, bounds.RightMiddle(), true),
                            new QuadraticBezierSegment(bounds.BottomRight, bounds.BottomLeft, true)

                        }, false)
                    });

                // X_X
                case SmileyExpression.Dead:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopLeft,new PathSegment[]
                        {
                            new LineSegment(bounds.BottomRight, true),
                            new LineSegment(bounds.BottomLeft, false),
                            new LineSegment(bounds.TopRight, true)
                        }, false)
                    });

                // -_-
                case SmileyExpression.Disgruntled:
                case SmileyExpression.LeftWink:
                    return new LineGeometry(bounds.LeftMiddle(), bounds.RightMiddle());

                // ^_^
                case SmileyExpression.Emoji:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.LeftMiddle(),new PathSegment[]
                        {
                            new LineSegment(bounds.TopMiddle(), true),
                            new LineSegment(bounds.RightMiddle(), true)
                        }, false)
                    });

                // O_O
                case SmileyExpression.FreakedOut:
                    return new EllipseGeometry(bounds);

                // >_<
                case SmileyExpression.Frustrated:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.LeftAtFraction(0.25),new PathSegment[]
                        {
                            new LineSegment(bounds.RightMiddle(), true),
                            new LineSegment(bounds.LeftAtFraction(0.75), true)
                        }, false)
                    });

                // @_@
                case SmileyExpression.Insane:

                    // TODO: To get the bezier curves to look (be) continuous - the intesecting point
                    //       must be co-linear to the two control points for the intesecting bezier curves.
                    //
                    //       Probably can't tell the difference here; but it should be smoothed out
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.LeftMiddle(),new PathSegment[]
                        {
                            new QuadraticBezierSegment(bounds.TopLeft, bounds.TopMiddle(), true),
                            new QuadraticBezierSegment(bounds.TopRight, bounds.RightMiddle(), true),
                            new QuadraticBezierSegment(bounds.BottomRight, bounds.PointAtFraction(0.5, 0.75), true),
                            new QuadraticBezierSegment(bounds.BottomLeft, bounds.PointAtFraction(0.25, 0.5), true),
                            new QuadraticBezierSegment(bounds.TopLeft, bounds.PointAtFraction(0.5, 0.35), true),
                            new QuadraticBezierSegment(bounds.TopRight, bounds.PointAtFraction(0.55, 0.5), true)

                        }, false)
                    });

                // \ /
                // ---
                case SmileyExpression.MeanPumpkinFace:
                case SmileyExpression.Mischievous:
                case SmileyExpression.Sour:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopLeft,new PathSegment[]
                        {
                            new LineSegment(bounds.BottomRight, true)
                        }, false)
                    });

                // O_O
                case SmileyExpression.Shocked:
                case SmileyExpression.WeirdWhistler:
                    return new EllipseGeometry(bounds);

                // ~_~
                case SmileyExpression.Sleeping:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.PointAtFraction(0, 0.2),new PathSegment[]
                        {
                            new QuadraticBezierSegment(bounds.PointAtFraction(0.1, 0.35), bounds.PointAtFraction(0.5, 0.4), true),
                            new QuadraticBezierSegment(bounds.PointAtFraction(0.9, 0.35), bounds.PointAtFraction(1, 0.2), true)

                        }, false)
                    });
            }
        }

        protected Geometry DrawRightEye()
        {
            // Procedure
            //
            // 1) Define the three points in drawing the mouth from the cavnas dimension. These
            //    will depend on the expression; but, generally, top, bottom, and average (center)
            //
            // 2) Based on the mouth expression create offsets from those points
            //
            // 3) Draw the path figures based on those points and the expression
            //

            var bounds = GetRightEyeBounds();

            switch (this.SmileyExpression)
            {
                // Line Segments down the middle (Happy, Sad, Unsure, etc...)
                default:
                case SmileyExpression.Happy:
                case SmileyExpression.LeftWink:
                case SmileyExpression.Sad:
                case SmileyExpression.Unsure:
                case SmileyExpression.Scared:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopMiddle(), new PathSegment[]
                        {
                            new LineSegment(bounds.BottomMiddle(), true)

                        }, false)
                    });

                // ) (
                // ---
                case SmileyExpression.Blind:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopRight,new PathSegment[]
                        {
                            new QuadraticBezierSegment(bounds.TopLeft, bounds.LeftMiddle(), true),
                            new QuadraticBezierSegment(bounds.BottomLeft, bounds.BottomRight, true)

                        }, false)
                    });

                // X_X
                case SmileyExpression.Dead:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopLeft,new PathSegment[]
                        {
                            new LineSegment(bounds.BottomRight, true),
                            new LineSegment(bounds.BottomLeft, false),
                            new LineSegment(bounds.TopRight, true)
                        }, false)
                    });

                // -_-
                case SmileyExpression.Disgruntled:
                case SmileyExpression.RightWink:
                    return new LineGeometry(bounds.LeftMiddle(), bounds.RightMiddle());

                // ^_^
                case SmileyExpression.Emoji:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.LeftMiddle(),new PathSegment[]
                        {
                            new LineSegment(bounds.TopMiddle(), true),
                            new LineSegment(bounds.RightMiddle(), true)
                        }, false)
                    });

                // O_O
                case SmileyExpression.FreakedOut:
                    return new EllipseGeometry(bounds);

                // >_<
                case SmileyExpression.Frustrated:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.RightAtFraction(0.25),new PathSegment[]
                        {
                            new LineSegment(bounds.LeftMiddle(), true),
                            new LineSegment(bounds.RightAtFraction(0.75), true)
                        }, false)
                    });

                // @_@
                case SmileyExpression.Insane:
                    // TODO: To get the bezier curves to look (be) continuous - the intesecting point
                    //       must be co-linear to the two control points for the intesecting bezier curves.
                    //
                    //       Probably can't tell the difference here; but it should be smoothed out
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.LeftMiddle(),new PathSegment[]
                        {
                            new QuadraticBezierSegment(bounds.TopLeft, bounds.TopMiddle(), true),
                            new QuadraticBezierSegment(bounds.TopRight, bounds.RightMiddle(), true),
                            new QuadraticBezierSegment(bounds.BottomRight, bounds.PointAtFraction(0.5, 0.75), true),
                            new QuadraticBezierSegment(bounds.BottomLeft, bounds.PointAtFraction(0.25, 0.5), true),
                            new QuadraticBezierSegment(bounds.TopLeft, bounds.PointAtFraction(0.5, 0.35), true),
                            new QuadraticBezierSegment(bounds.TopRight, bounds.PointAtFraction(0.55, 0.5), true)

                        }, false)
                    });

                // \ /
                // ---
                case SmileyExpression.MeanPumpkinFace:
                case SmileyExpression.Mischievous:
                case SmileyExpression.Sour:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopRight,new PathSegment[]
                        {
                            new LineSegment(bounds.BottomLeft, true)
                        }, false)
                    });

                // O_O
                case SmileyExpression.Shocked:
                case SmileyExpression.WeirdWhistler:
                    return new EllipseGeometry(bounds);

                // ~_~
                case SmileyExpression.Sleeping:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.PointAtFraction(0, 0.2),new PathSegment[]
                        {
                            new QuadraticBezierSegment(bounds.PointAtFraction(0.1, 0.35), bounds.PointAtFraction(0.5, 0.4), true),
                            new QuadraticBezierSegment(bounds.PointAtFraction(0.9, 0.35), bounds.PointAtFraction(1, 0.2), true)

                        }, false)
                    });
            }
        }
    }
}