using Rogue.NET.Core.Media.Animation.Interface;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Service.Interface;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Rogue.NET.Core.Media.Animation
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IAnimationSequenceCreator))]
    public class AnimationSequenceCreator : IAnimationSequenceCreator
    {
        readonly IAnimationCreator _animationCreator;
        readonly IScenarioResourceService _scenarioResourceService;

        [ImportingConstructor]
        public AnimationSequenceCreator(IAnimationCreator animationCreator, IScenarioResourceService scenarioResourceService)
        {
            _animationCreator = animationCreator;
            _scenarioResourceService = scenarioResourceService;
        }

        public IAnimationPlayer CreateAnimation(AnimationSequence animation, Rect bounds, Point sourceLocation, Point[] targetLocations)
        {
            var animationGroups = animation.Animations.Select(x => _animationCreator.CreateAnimation(x, bounds, sourceLocation, targetLocations));

            return new AnimationQueue(animationGroups);
        }

        public IAnimationPlayer CreateThrowAnimation(ScenarioImage scenarioImage, Point sourceLocation, Point targetLocation)
        {
            return CreateRotatingProjectileAnimation(scenarioImage, sourceLocation, targetLocation);
        }

        public IAnimationPlayer CreateAmmoAnimation(ScenarioImage scenarioImage, Point sourceLocation, Point targetLocation)
        {
            return CreateOrientedProjectileAnimation(scenarioImage, sourceLocation, targetLocation);
        }

        public IAnimationPlayer CreateTargetingAnimation(Point point, Color fillColor, Color strokeColor)
        {
            return new AnimationQueue(new AnimationPrimitiveGroup[] { _animationCreator.CreateTargetingAnimation(point, fillColor)});
        }

        private IAnimationPlayer CreateRotatingProjectileAnimation(ScenarioImage scenarioImage, Point sourceLocation, Point targetLocation)
        {
            // Projectile Animations
            //
            // Non-Ammo:           Create a rotating image along the path
            // Ammo:               Create a non-rotating image directing the symbol "North" at the target
            //

            var drawingImage = _scenarioResourceService.GetImageSource(scenarioImage, 1.0, Light.White) as DrawingImage;

            if (drawingImage == null)
                throw new Exception("Improper use of scenario image projectile animation");

            // Set default velocity
            var velocity = 250;         // pixels / sec
            var angularVelocity = 2.5;  // rotations / sec

            // Calculate animation time
            var animationTime = (int)((System.Math.Abs(Point.Subtract(sourceLocation, targetLocation).Length) / velocity) * 1000.0);   // milliseconds
            var rotationAngle = angularVelocity * (animationTime / 1000.0) * 360.0;

            // Create path geometry for the line
            var pathGeometry = new PathGeometry(new PathFigure[]
            {
                new PathFigure(sourceLocation, new PathSegment[]{ new LineSegment(targetLocation, true) }, false)
            });

            //Generate Animations
            var duration = new Duration(TimeSpan.FromMilliseconds(animationTime));
            var xAnimation = new DoubleAnimationUsingPath();
            var yAnimation = new DoubleAnimationUsingPath();
            var angleAnimation = new DoubleAnimation(0, rotationAngle, duration);

            xAnimation.PathGeometry = pathGeometry;
            xAnimation.Duration = duration;
            xAnimation.Source = PathAnimationSource.X;
            xAnimation.RepeatBehavior = new RepeatBehavior(1);
            yAnimation.PathGeometry = pathGeometry;
            yAnimation.Duration = duration;
            yAnimation.Source = PathAnimationSource.Y;
            yAnimation.RepeatBehavior = new RepeatBehavior(1);
            angleAnimation.RepeatBehavior = new RepeatBehavior(1);


            var xClock = xAnimation.CreateClock();
            var yClock = yAnimation.CreateClock();
            var angleClock = angleAnimation.CreateClock();
            xClock.Controller.Begin();
            yClock.Controller.Begin();
            angleClock.Controller.Begin();
            xClock.Controller.Pause();
            yClock.Controller.Pause();
            angleClock.Controller.Pause();

            var transform = new TransformGroup();
            var translateTransform = new TranslateTransform();
            var rotateTransform = new RotateTransform(0, 0.5, 0.5);

            translateTransform.ApplyAnimationClock(TranslateTransform.XProperty, xClock);
            translateTransform.ApplyAnimationClock(TranslateTransform.YProperty, yClock);
            rotateTransform.ApplyAnimationClock(RotateTransform.AngleProperty, angleClock);

            transform.Children.Add(rotateTransform);
            transform.Children.Add(translateTransform);

            // Create geometry to house the drawing source
            var cellBounds = new Rect(new Point(-1 * (ModelConstants.CellWidth / 2.0D), -1 * (ModelConstants.CellHeight / 2.0D)),
                                      new Size(ModelConstants.CellWidth, ModelConstants.CellHeight));

            var cellGeometry = new RectangleGeometry(cellBounds);
            var drawingBrush = new DrawingBrush(drawingImage.Drawing);
            drawingBrush.Viewbox = cellBounds;
            drawingBrush.ViewboxUnits = BrushMappingMode.RelativeToBoundingBox;
            drawingBrush.Viewport = cellBounds;
            drawingBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            drawingBrush.Stretch = Stretch.Fill;

            var primitive = new AnimationPrimitive(cellGeometry, new AnimationClock[] { xClock, yClock, angleClock }, animationTime);
            primitive.Height = cellBounds.Height;
            primitive.Width = cellBounds.Width;
            primitive.RenderTransform = transform;
            primitive.Fill = drawingBrush;

            return new AnimationQueue(new AnimationPrimitiveGroup[]
            {
                new AnimationPrimitiveGroup(new AnimationPrimitive[] { primitive }, animationTime)
            });
        }
        private IAnimationPlayer CreateOrientedProjectileAnimation(ScenarioImage scenarioImage, Point sourceLocation, Point targetLocation)
        {
            // Projectile Animations
            //
            // Non-Ammo:           Create a rotating image along the path
            // Ammo:               Create a non-rotating image directing the symbol "North" at the target
            //

            var drawingImage = _scenarioResourceService.GetImageSource(scenarioImage, 1.0, Light.White) as DrawingImage;

            if (drawingImage == null)
                throw new Exception("Improper use of scenario image projectile animation");

            // Calculate orientation of image
            var angle = System.Math.Atan2(targetLocation.Y - sourceLocation.Y, targetLocation.X - sourceLocation.X);
            var orientationAngle = (angle * (180 / System.Math.PI)) + 90;

            // Set default velocity
            var velocity = 300;         // pixels / sec

            // Calculate animation time
            var animationTime = (int)((System.Math.Abs(Point.Subtract(sourceLocation, targetLocation).Length) / velocity) * 1000.0);   // milliseconds

            // Create path geometry for the line
            var pathGeometry = new PathGeometry(new PathFigure[]
            {
                new PathFigure(sourceLocation, new PathSegment[]{ new LineSegment(targetLocation, true) }, false)
            });

            //Generate Animations
            var duration = new Duration(TimeSpan.FromMilliseconds(animationTime));
            var xAnimation = new DoubleAnimationUsingPath();
            var yAnimation = new DoubleAnimationUsingPath();

            xAnimation.PathGeometry = pathGeometry;
            xAnimation.Duration = duration;
            xAnimation.Source = PathAnimationSource.X;
            xAnimation.RepeatBehavior = new RepeatBehavior(1);
            yAnimation.PathGeometry = pathGeometry;
            yAnimation.Duration = duration;
            yAnimation.Source = PathAnimationSource.Y;
            yAnimation.RepeatBehavior = new RepeatBehavior(1);

            var xClock = xAnimation.CreateClock();
            var yClock = yAnimation.CreateClock();
            xClock.Controller.Begin();
            yClock.Controller.Begin();
            xClock.Controller.Pause();
            yClock.Controller.Pause();

            var transform = new TransformGroup();
            var translateTransform = new TranslateTransform();
            var rotateTransform = new RotateTransform(orientationAngle, 0.5, 0.5);

            translateTransform.ApplyAnimationClock(TranslateTransform.XProperty, xClock);
            translateTransform.ApplyAnimationClock(TranslateTransform.YProperty, yClock);

            transform.Children.Add(rotateTransform);
            transform.Children.Add(translateTransform);

            // Create geometry to house the drawing source
            var cellBounds = new Rect(new Point(-1 * (ModelConstants.CellWidth / 2.0D), -1 * (ModelConstants.CellHeight / 2.0D)),
                                      new Size(ModelConstants.CellWidth, ModelConstants.CellHeight));

            var cellGeometry = new RectangleGeometry(cellBounds);
            var drawingBrush = new DrawingBrush(drawingImage.Drawing);
            drawingBrush.Viewbox = cellBounds;
            drawingBrush.ViewboxUnits = BrushMappingMode.RelativeToBoundingBox;
            drawingBrush.Viewport = cellBounds;
            drawingBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            drawingBrush.Stretch = Stretch.Fill;

            var primitive = new AnimationPrimitive(cellGeometry, new AnimationClock[] { xClock, yClock }, animationTime);
            primitive.Height = cellBounds.Height;
            primitive.Width = cellBounds.Width;
            primitive.RenderTransform = transform;
            primitive.Fill = drawingBrush;

            return new AnimationQueue(new AnimationPrimitiveGroup[]
            {
                new AnimationPrimitiveGroup(new AnimationPrimitive[] { primitive }, animationTime)
            });
        }
    }
}
