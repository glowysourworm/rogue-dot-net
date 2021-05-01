using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;

namespace Rogue.NET.Core.Processing.Service.Rendering
{
    public class RenderingContentLayer : RenderingLayer
    {
        /// <summary>
        /// Callback for providing the symbol for a specific content layer
        /// </summary>
        public delegate SymbolDetailsTemplate SymbolDetailsCallback(int column, int row);

        public SymbolDetailsCallback SymbolCallback { get; set; }

        public RenderingContentLayer(int width, 
                                     int height, 
                                     SymbolDetailsCallback symbolCallback,
                                     TerrainLayer renderingOrder)
        {
            this.CellHeight = height;
            this.CellWidth = width;
            this.SymbolCallback = symbolCallback;
            this.RenderingOrder = renderingOrder;
        }

        public override int CellHeight { get; protected set; }
        public override int CellWidth { get; protected set; }
        public override SymbolDetailsTemplate GetSymbol(int column, int row)
        {
            return this.SymbolCallback(column, row);
        }
    }
}
