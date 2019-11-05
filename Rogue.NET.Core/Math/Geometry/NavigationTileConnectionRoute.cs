using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Math.Geometry
{
    public class NavigationTileConnectionRoute
    {
        readonly NavigationTile _parentTile;

        List<VertexInt> _connectionPathway;

        public IEnumerable<VertexInt> ConnectionPathway
        {
            get { return _connectionPathway; }
        }

        /// <summary>
        /// Initializes the connection route by creating a pathway between the provided connection points. These points MUST be owned
        /// by the parent tile. This should be a subset of its connection points - just the ones that were chosen to route with.
        /// </summary>
        public NavigationTileConnectionRoute(NavigationTile parentTile)
        {
            _parentTile = parentTile;
            _connectionPathway = new List<VertexInt>();
        }

        /// <summary>
        /// Adds point to the connection pathway if it doesn't already contain it.
        /// </summary>
        public void IncludePoint(VertexInt point)
        {
            if (!_connectionPathway.Contains(point))
                _connectionPathway.Add(point);
        }
    }
}
