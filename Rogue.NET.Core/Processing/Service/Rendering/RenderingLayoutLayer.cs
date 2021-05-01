using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;

namespace Rogue.NET.Core.Processing.Service.Rendering
{
    public class RenderingLayoutLayer : RenderingLayer
    {
        public SymbolDetailsTemplate Symbol { get; private set; }
        public SymbolDetailsTemplate EdgeSymbol { get; private set; }

        /// <summary>
        /// Layer for rendering
        /// </summary>
        public ILayerMap LayerMap { get; private set; }

        /// <summary>
        /// Does layer have a defined edge
        /// </summary>
        public bool IsEdged { get; private set; }
        
        /// <summary>
        /// Constructor used for layer with no edges
        /// </summary>
        public RenderingLayoutLayer(ILayerMap layerMap, 
                                    SymbolDetailsTemplate symbol, 
                                    TerrainLayer renderingOrder)
        {
            this.LayerMap = layerMap;
            this.EdgeSymbol = null;
            this.Symbol = symbol;
            this.IsEdged = false;
            this.RenderingOrder = renderingOrder;
        }

        /// <summary>
        /// Constructor for a layout with edges
        /// </summary>
        public RenderingLayoutLayer(ILayerMap layerMap, 
                                    SymbolDetailsTemplate symbol, 
                                    SymbolDetailsTemplate edgeSymbol, 
                                    bool isEdged,
                                    TerrainLayer renderingOrder)
        {
            this.Symbol = symbol;
            this.EdgeSymbol = edgeSymbol;
            this.LayerMap = layerMap;
            this.IsEdged = isEdged;
            this.RenderingOrder = renderingOrder;
        }

        public override int CellHeight
        {
            get { return this.LayerMap.Boundary.Height; }
            protected set { }
        }
        public override int CellWidth
        {
            get { return this.LayerMap.Boundary.Width; }
            protected set { }
        }
        public override SymbolDetailsTemplate GetSymbol(int column, int row)
        {
            // Get the Region for the location -> Get the cell from the region
            var cell = this.LayerMap.Get(column, row);

            if (cell != null)
            {
                if (this.IsEdged && this.LayerMap[column, row].IsEdge(column, row))
                    return this.EdgeSymbol;

                else
                    return this.Symbol;
            }

            else
                return null;
        }
    }
}
