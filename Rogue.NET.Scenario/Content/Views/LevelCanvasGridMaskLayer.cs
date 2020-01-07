using Rogue.NET.Core.Processing.Model.Extension;

using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Content.Views
{
    public class LevelCanvasGridMaskLayer : Canvas
    {
        // Primary layer for rendering
        GeometryDrawing[,] _layer;

        static readonly Pen _borderPen = new Pen(Brushes.Transparent, 0.0);

        public LevelCanvasGridMaskLayer()
        {

        }

        public void SetGridLayer(GeometryDrawing[,] layer)
        {
            _layer = layer;

            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext context)
        {
            base.OnRender(context);

            var brushes = new Dictionary<DrawingImage, DrawingBrush>();

            // Apply the primary layer 
            if (_layer != null)
            {
                //var drawingGroup = new DrawingGroup();

                // Combine drawings into drawing group; and create separate drawing context
                //
                //using (var subContext = drawingGroup.Open())
                //{
                    _layer.Iterate((column, row) =>
                    {
                        if (_layer[column, row] == null)
                            return;

                        context.DrawDrawing(_layer[column, row]);
                    });

                    //subContext.Close();
                //}

                //RenderOptions.SetBitmapScalingMode(drawingGroup, BitmapScalingMode.Fant);
                //RenderOptions.SetCachingHint(drawingGroup, CachingHint.Cache);
                //drawingGroup.Freeze();

                // Draw the entire group together
                //context.DrawDrawing(drawingGroup);
            }
        }
    }
}
