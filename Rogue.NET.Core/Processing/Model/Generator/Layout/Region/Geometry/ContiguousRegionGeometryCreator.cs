using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Geometry
{
    public static class ContiguousRegionGeometryCreator
    {
        /// <summary>
        /// Combines region boundaries to create a contiguous subset by locating overlapping boundaries.
        /// </summary>
        public static IEnumerable<IEnumerable<RegionBoundary>> CreateContiguousRegionGeometry(IEnumerable<RegionBoundary> boundaries)
        {
            var result = new List<List<RegionBoundary>>();

            foreach (var boundary in boundaries)
            {
                // Any contiguous region boundaries will have already been
                // identified and added to one of the result lists
                if (result.Any(list => list.Contains(boundary)))
                    continue;

                // First, get the regions that overlap this region.
                var overlappingBoundaries = boundaries.Where(x => x != boundary && x.Touches(boundary))
                                                      .ToList();

                // Add the boundary in question to this list to start
                overlappingBoundaries.Add(boundary);

                IEnumerable<RegionBoundary> nextOverlappingBoundaries = null;

                do
                {
                    // While there are other boundaries to overlap with this subset - continue iterating
                    nextOverlappingBoundaries = boundaries.Where(x => overlappingBoundaries.Any(y => y.Touches(x) &&
                                                                     !overlappingBoundaries.Contains(x)));

                    if (nextOverlappingBoundaries.Any())
                        overlappingBoundaries.AddRange(nextOverlappingBoundaries);

                } while (nextOverlappingBoundaries.Any());

                // Add overlapping boundaries to the result
                result.Add(overlappingBoundaries);
            }

            if (result.Sum(x => x.Count) != boundaries.Count())
                throw new Exception("Improperly calculated contiguous region");

            return result;
        }
    }
}
