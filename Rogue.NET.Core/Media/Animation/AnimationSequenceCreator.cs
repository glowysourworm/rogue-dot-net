using Rogue.NET.Core.Media.Animation.Interface;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Processing.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public IAnimationPlayer CreateScenarioImageProjectileAnimation(ScenarioImage scenarioImage, Point sourceLocation, Point targetLocation)
        {
            var drawingImage = _scenarioResourceService.GetImageSource(scenarioImage, 1.0) as DrawingImage;

            if (drawingImage == null)
                throw new Exception("Improper use of scenario image projectile animation");

            // Set default velocity
            var velocity = 250;    
            
            // Calculate animation time
            var animationTime = (int)((Math.Abs(Point.Subtract(sourceLocation, targetLocation).Length) / velocity) * 1000.0);   // milliseconds

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
            yAnimation.PathGeometry = pathGeometry;
            yAnimation.Duration = duration;

            var xClock = xAnimation.CreateClock();
            var yClock = yAnimation.CreateClock();
            xClock.Controller.Begin();
            yClock.Controller.Begin();
            xClock.Controller.Pause();
            yClock.Controller.Pause();

            var translateTransform = new TranslateTransform();
            translateTransform.ApplyAnimationClock(TranslateTransform.XProperty, xClock);
            translateTransform.ApplyAnimationClock(TranslateTransform.YProperty, yClock);

            // Create geometry to house the drawing source
            var cellBounds = new Rect(sourceLocation, new Size(ModelConstants.CellWidth, ModelConstants.CellHeight));
            var cellGeometry = new RectangleGeometry(cellBounds);
            var drawingBrush = new DrawingBrush(drawingImage.Drawing);
            drawingBrush.AlignmentX = AlignmentX.Center;
            drawingBrush.AlignmentY = AlignmentY.Center;
            drawingBrush.Viewbox = cellBounds;
            drawingBrush.ViewboxUnits = BrushMappingMode.Absolute;

            var primitive = new AnimationPrimitive(cellGeometry, new AnimationClock[] { xClock, yClock }, animationTime);
            primitive.RenderTransform = translateTransform;
            primitive.Fill = drawingBrush;

            return new AnimationQueue(new AnimationPrimitiveGroup[]
            {
                new AnimationPrimitiveGroup(new AnimationPrimitive[] { primitive }, animationTime)
            });
        }

        public IAnimationPlayer CreateTargetingAnimation(Point point, Color fillColor, Color strokeColor)
        {
            return new AnimationQueue(new AnimationPrimitiveGroup[] { _animationCreator.CreateTargetingAnimation(point, fillColor)});
        }
    }
}
