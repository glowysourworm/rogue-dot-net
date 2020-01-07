using Rogue.NET.Core.Model;
using Rogue.NET.Core.Processing.Service.Cache.Interface;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rogue.NET.Core.Processing.Service.Cache
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioBitmapSourceFactory))]
    public class ScenarioBitmapSourceFactory : IScenarioBitmapSourceFactory
    {
        readonly Dictionary<ScaledCacheImage, WriteableBitmap> _cache;

        protected struct ScaledCacheImage
        {
            public DrawingImage OriginalImage { get; set; }
            public double Scale { get; set; }
            
            public ScaledCacheImage(DrawingImage image, double scale)
            {
                this.OriginalImage = image;
                this.Scale = scale;
            }

            public override int GetHashCode()
            {
                var hash = 17;

                hash = (hash * 397) + this.Scale.GetHashCode();
                hash = (hash * 397) + this.OriginalImage.GetHashCode();

                return hash;
            }
        }


        public ScenarioBitmapSourceFactory()
        {
            _cache = new Dictionary<ScaledCacheImage, WriteableBitmap>();
        }

        public WriteableBitmap GetImageSource(DrawingImage drawingImage, double scale)
        {
            var cacheImage = new ScaledCacheImage(drawingImage, scale);

            if (_cache.ContainsKey(cacheImage))
                return _cache[cacheImage];

            // Draw image data to a visual
            var visual = new DrawingVisual();

            using (var context = visual.RenderOpen())
            {
                // Create scale transform for the visual to render at the current zoom factor
                context.PushTransform(new ScaleTransform(scale, scale));

                // Render the drawing
                context.DrawImage(drawingImage, new Rect(new Size(drawingImage.Width, drawingImage.Height)));
            }

            // Create the Bitmap and render the rectangle onto it.
            var bitmap = new RenderTargetBitmap((int)visual.ContentBounds.Width, (int)visual.ContentBounds.Height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
           
            var writeableBitmap = new WriteableBitmap(bitmap);

            // Cache the result
            _cache.Add(cacheImage, writeableBitmap);

            return writeableBitmap;
        }
    }
}
