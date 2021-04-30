using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Service.Rendering
{
    public abstract class RenderingLayer
    {
        public abstract SymbolDetailsTemplate GetSymbol(int column, int row);

        /// <summary>
        /// The width of the layer
        /// </summary>
        public abstract int CellWidth { get; protected set; }

        /// <summary>
        /// The height of the layer
        /// </summary>
        public abstract int CellHeight { get; protected set; }
    }
}
