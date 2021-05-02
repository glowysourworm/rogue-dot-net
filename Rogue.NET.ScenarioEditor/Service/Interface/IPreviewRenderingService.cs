using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;

using System.Windows.Media.Imaging;

using static Rogue.NET.Core.Model.Scenario.Content.Layout.LayoutGrid;
using static System.Windows.Media.Imaging.WriteableBitmapExtensions;

namespace Rogue.NET.ScenarioEditor.Service.Interface
{
    /// <summary>
    /// Component for rendering layout previews for the Scenario Editor
    /// </summary>
    public interface IPreviewRenderingService
    {
        /// <summary>
        /// Renders terrain layer as separate bitmap
        /// </summary>
        WriteableBitmap RenderTerrainLayer(LayoutGrid layoutGrid, TerrainLayerTemplate terrainLayerTemplate, bool useLighting);

        /// <summary>
        /// Renders wall light layer as separate bitmap
        /// </summary>
        WriteableBitmap RenderWallLightLayer(LayoutGrid layoutGrid, SymbolDetailsTemplate symbol, bool useLighting);

        /// <summary>
        /// Renders one layer of the layout with the specified symbol
        /// </summary>
        WriteableBitmap RenderLayoutLayer(LayoutGrid layoutGrid, LayoutLayer layer, SymbolDetailsTemplate symbol, bool useLighting);

        /// <summary>
        /// Combines two layers of rendered bitmaps - overwriting the lower layer where pixels are set
        /// </summary>
        WriteableBitmap CombineRendering(WriteableBitmap lowerLayerRendering, WriteableBitmap upperLayerRendering, BlendMode blendMode);
    }
}
