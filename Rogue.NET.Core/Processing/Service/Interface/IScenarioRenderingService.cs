using Rogue.NET.Core.Processing.Service.Rendering;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rogue.NET.Core.Processing.Service.Interface
{
    /// <summary>
    /// Component responsible for primary rendering routine of scenario levels.
    /// </summary>
    public interface IScenarioRenderingService
    {
        /// <summary>
        /// Renders primary drawing for the level
        /// </summary>
        WriteableBitmap Render(RenderingSpecification specification);

        /// <summary>
        /// Re-draws a portion of the layout specified by the invalidation map
        /// </summary>
        // void Invalidate(WriteableBitmap rendering);
    }
}
