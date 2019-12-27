using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Geometry;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Component for storing and mainintaining a 2D cell array for a layer of the level layout
    /// </summary>
    [Serializable]
    public class LayerMap : ISerializable
    {
        public string Name { get; private set; }

        // Keep a grid of region references per cell in the region
        Region<GridLocation>[,] _regionMap;

        // Also, keep a collection of the regions for this layer
        IEnumerable<Region<GridLocation>> _regions;

        // Also, keep region connections as a dictionary of id's and id lists
        Dictionary<string, List<string>> _regionConnections;

        /// <summary>
        /// Gets the region for the specified location
        /// </summary>
        public Region<GridLocation> this[int column, int row]
        {
            get { return _regionMap[column, row]; }
        }

        public IEnumerable<Region<GridLocation>> Regions
        {
            get { return _regions; }
        }

        public IEnumerable<Region<GridLocation>> Connections(int column, int row)
        {
            var region = this[column, row];

            if (region == null)
                return new Region<GridLocation>[] { };

            var connectionIds = _regionConnections[region.Id];

            return _regions.Where(otherRegion => _regionConnections[region.Id].Contains(otherRegion.Id))
                           .Actualize();
        }

        public IEnumerable<Region<GridLocation>> Connections(string regionId)
        {
            var region = _regions.FirstOrDefault(region => region.Id == regionId);

            if (region == null)
                return new Region<GridLocation>[] { };

            var connectionIds = _regionConnections[region.Id];

            return _regions.Where(otherRegion => _regionConnections[region.Id].Contains(otherRegion.Id))
                           .Actualize();
        }

        /// <summary>
        /// Initializes a layer map with region connections
        /// </summary>
        public LayerMap(string layerName, Graph<Region<GridLocation>> regionGraph, int width, int height)
        {
            // Get regions from the region graph
            var regions = regionGraph.Vertices
                                     .Select(vertex => vertex.Reference)
                                     .Distinct();

            var regionConnections = regions.ToDictionary(region => region.Id, region =>
            {
                // Fetch vertices with this region reference
                var vertices = regionGraph.Find(region);

                // Select all edges connecting this region to other regions
                var edges = vertices.SelectMany(vertex => regionGraph[vertex]);

                // Select all id's from all edges
                var allIds = edges.Select(edge => edge.Point1.Reference.Id)
                                  .Union(edges.Select(edge => edge.Point2.Reference.Id));

                // Return all distinct id's except for this region's id
                return allIds.Except(new string[] { region.Id })
                             .Distinct()
                             .ToList();
            });

            Initialize(layerName, regions, regionConnections, width, height);
        }

        public LayerMap(string layerName, IEnumerable<Region<GridLocation>> regions, int width, int height)
        {
            // Get regions from the region graph
            Initialize(layerName, regions, new Dictionary<string, List<string>>(), width, height);
        }

        public LayerMap(SerializationInfo info, StreamingContext context)
        {
            var name = info.GetString("Name");
            var width = info.GetInt32("Width");
            var height = info.GetInt32("Height");
            var regionCount = info.GetInt32("RegionCount");
            var regionConnections = (Dictionary<string, List<string>>)info.GetValue("RegionConnections", typeof(Dictionary<string, List<string>>));

            var regions = new List<Region<GridLocation>>();

            for (int i = 0; i < regionCount; i++)
            {
                var region = (Region<GridLocation>)info.GetValue("Region" + i.ToString(), typeof(Region<GridLocation>));

                regions.Add(region);
            }

            Initialize(name, regions, regionConnections, width, height);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", this.Name);
            info.AddValue("Width", _regionMap.GetLength(0));
            info.AddValue("Height", _regionMap.GetLength(1));
            info.AddValue("RegionCount", _regions.Count());
            info.AddValue("RegionConnections", _regionConnections);

            for (int i = 0; i < _regions.Count(); i++)
                info.AddValue("Region" + i.ToString(), _regions.ElementAt(i));
        }

        private void Initialize(string layerName, IEnumerable<Region<GridLocation>> regions, Dictionary<string, List<string>> regionConnections, int width, int height)
        {
            this.Name = layerName;

            _regions = regions;
            _regionConnections = regionConnections;
            _regionMap = new Region<GridLocation>[width, height];

            // Iterate regions and initialize the map
            foreach (var region in regions)
            {
                foreach (var location in region.Locations)
                {
                    if (_regionMap[location.Column, location.Row] != null)
                        throw new Exception("Invalid Region construction - duplicate cell locations");

                    _regionMap[location.Column, location.Row] = region;
                }
            }
        }
    }
}
