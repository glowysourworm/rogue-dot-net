using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Scenario.Processing.Service.Interface;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Content.Views
{
    public class LevelCanvasGridLayer : Canvas
    {
        // Primary layer for rendering
        DrawingImage[,] _layer;

        // TODO:RENDERING Make this readonly; and inject it
        IScenarioUIGeometryService _scenarioUIGeometryService;

        static readonly Pen _borderPen = new Pen(Brushes.Transparent, 0.0);

        public LevelCanvasGridLayer()
        {

        }

        public void SetGridLayer(DrawingImage[,] layer, IScenarioUIGeometryService scenarioUIGeometryService)
        {
            _layer = layer;
            _scenarioUIGeometryService = scenarioUIGeometryService;

            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext context)
        {
            base.OnRender(context);

            var brushes = new Dictionary<DrawingImage, DrawingBrush>(); 

            // Apply the primary layer 
            if (_layer != null)
            {
                var drawingGroup = new DrawingGroup();

                // Combine drawings into drawing group; and create separate drawing context
                //
                using (var subContext = drawingGroup.Open())
                {
                    _layer.Iterate((column, row) =>
                    {
                        if (_layer[column, row] == null ||
                            _layer[column, row].Drawing == null)
                            return;

                        if (!brushes.ContainsKey(_layer[column, row]))
                        {
                            var brush = new DrawingBrush(_layer[column, row].Drawing);

                            RenderOptions.SetBitmapScalingMode(brush, BitmapScalingMode.Fant);
                            RenderOptions.SetCachingHint(brush, CachingHint.Cache);
                            brush.Freeze();

                            brushes.Add(_layer[column, row], brush);
                        }

                        subContext.DrawRectangle(brushes[_layer[column, row]], _borderPen, _scenarioUIGeometryService.Cell2UIRect(column, row));
                    });

                    subContext.Close();
                }

                RenderOptions.SetBitmapScalingMode(drawingGroup, BitmapScalingMode.Fant);
                RenderOptions.SetCachingHint(drawingGroup, CachingHint.Cache);
                drawingGroup.Freeze();

                // Draw the entire group together
                context.DrawDrawing(drawingGroup);
            }          
        }
    }
}
