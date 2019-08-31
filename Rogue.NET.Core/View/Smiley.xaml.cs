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
        protected const double MOUTH_Y_TOP_RATIO = 0.45;     // Locates the top of the mouth region
        protected const double MOUTH_Y_BOTTOM_RATIO = 0.75;  // Locates the bottom of the mouth region

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

        public static readonly DependencyProperty SmileyMoodProperty =
            DependencyProperty.Register("SmileyMood", typeof(SmileyMoods), typeof(Smiley),
                new PropertyMetadata(new PropertyChangedCallback(Smiley.OnMoodChanged)));

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
        public SmileyMoods SmileyMood
        {
            get { return (SmileyMoods)GetValue(Smiley.SmileyMoodProperty); }
            set { SetValue(Smiley.SmileyMoodProperty, value); }
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

        private static void OnMoodChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            // SET FACIAL FEATURES BASED ON NEW MOOD ENUM
            var control = o as Smiley;
            var mood = (SmileyMoods)e.NewValue;

            if (control != null)
            {
                control.Draw();
            }
        }

        protected void Draw()
        {
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

            // Change the fill based on the mood
            switch (this.SmileyMood)
            {
                default:
                case SmileyMoods.None:
                case SmileyMoods.Happy:
                case SmileyMoods.Indifferent:
                case SmileyMoods.Sad:
                case SmileyMoods.Angry:
                case SmileyMoods.Drunk:
                case SmileyMoods.Mischievous:
                    this.LeftEyePath.Fill = Brushes.Transparent;
                    this.RightEyePath.Fill = Brushes.Transparent;
                    this.MouthPath.Fill = Brushes.Transparent;
                    break;
                case SmileyMoods.Shocked:
                    this.LeftEyePath.Fill = new SolidColorBrush(this.SmileyLineColor);
                    this.RightEyePath.Fill = new SolidColorBrush(this.SmileyLineColor);
                    this.MouthPath.Fill = new SolidColorBrush(this.SmileyLineColor);
                    break;
                case SmileyMoods.Scared:
                    this.LeftEyePath.Fill = new SolidColorBrush(this.SmileyLineColor);
                    this.RightEyePath.Fill = new SolidColorBrush(this.SmileyLineColor);
                    this.MouthPath.Fill = Brushes.Transparent;
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

            switch (this.SmileyMood)
            {
                default:
                case SmileyMoods.None:
                case SmileyMoods.Happy:
                case SmileyMoods.Mischievous:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.LeftMiddle(),new PathSegment[]
                        {
                            new QuadraticBezierSegment(bounds.BottomAtFraction(0.1), bounds.BottomMiddle(), true),
                            new QuadraticBezierSegment(bounds.BottomAtFraction(0.9), bounds.RightMiddle(), true)

                        }, false)
                    });
                case SmileyMoods.Indifferent:
                case SmileyMoods.Scared:
                case SmileyMoods.Drunk:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.LeftMiddle(),new PathSegment[]
                        {
                            new LineSegment(bounds.RightMiddle(), true)
                        }, false)
                    });
                case SmileyMoods.Sad:
                case SmileyMoods.Angry:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.LeftMiddle(),new PathSegment[]
                        {
                            new QuadraticBezierSegment(bounds.TopAtFraction(0.1), bounds.TopMiddle(), true),
                            new QuadraticBezierSegment(bounds.TopAtFraction(0.9), bounds.RightMiddle(), true)

                        }, false)
                    });
                case SmileyMoods.Shocked:
                    return new EllipseGeometry(bounds);
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

            switch (this.SmileyMood)
            {
                default:
                case SmileyMoods.None:
                case SmileyMoods.Happy:
                case SmileyMoods.Sad:
                case SmileyMoods.Indifferent:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopMiddle(), new PathSegment[]
                        {
                            new LineSegment(bounds.BottomMiddle(), true)

                        }, false)
                    });
                case SmileyMoods.Drunk:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopLeft,new PathSegment[]
                        {
                            new LineSegment(bounds.BottomRight, true),
                            new LineSegment(bounds.BottomLeft, false),
                            new LineSegment(bounds.TopRight, true)
                        }, false)
                    });
                case SmileyMoods.Mischievous:
                case SmileyMoods.Angry:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopLeft,new PathSegment[]
                        {
                            new LineSegment(bounds.BottomRight, true)
                        }, false)
                    });
                case SmileyMoods.Shocked:
                case SmileyMoods.Scared:
                    return new EllipseGeometry(bounds);
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

            switch (this.SmileyMood)
            {
                default:
                case SmileyMoods.None:
                case SmileyMoods.Happy:
                case SmileyMoods.Sad:
                case SmileyMoods.Indifferent:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopMiddle(), new PathSegment[]
                        {
                            new LineSegment(bounds.BottomMiddle(), true)

                        }, false)
                    }) ;
                case SmileyMoods.Drunk:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopLeft,new PathSegment[]
                        {
                            new LineSegment(bounds.BottomRight, true),
                            new LineSegment(bounds.BottomLeft, false),
                            new LineSegment(bounds.TopRight, true)
                        }, false)
                    });
                case SmileyMoods.Mischievous:
                case SmileyMoods.Angry:
                    return new PathGeometry(new PathFigure[]
                    {
                        new PathFigure(bounds.TopRight,new PathSegment[]
                        {
                            new LineSegment(bounds.BottomLeft, true)
                        }, false)
                    });
                case SmileyMoods.Shocked:
                case SmileyMoods.Scared:
                    return new EllipseGeometry(bounds);
            }
        }
    }
}