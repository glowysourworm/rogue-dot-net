using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rogue.NET.Core.Processing.Service.Cache.Interface
{
    /// <summary>
    /// Simple component that converts the DrawingImage instances produced by the image source factory into 
    /// bitmaps and caches them.
    /// </summary>
    public interface IScenarioBitmapSourceFactory
    {
        WriteableBitmap GetImageSource(DrawingImage drawingImage, double scale);
    }
}
