using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Math.Algorithm;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Geometry
{
    public static class LargePolygonRegionGeometryCreator
    {
        static readonly IRandomSequenceGenerator _randomSequenceGenerator;

        static LargePolygonRegionGeometryCreator()
        {
            _randomSequenceGenerator = ServiceLocator.Current.GetInstance<IRandomSequenceGenerator>();
        }

        /// <summary>
        /// Creates large random-edge convex polygon in the middle of the level
        /// </summary>
        public static Polygon CreateRegion(LayoutTemplate template)
        {
            // TODO:TERRAIN Create parameters for this
            var paddingFrequency = 0.9;

            // Mapping function:  g(f) = (1 - f)(w / 2) + f  maps [0,1] -> [w/2, 1]
            //
            //                    This will create a minimum of 2 padding "cells" across
            //                    and a maximum of "w" cells (w := width)
            //

            var paddingWidth = (int)(((1 - paddingFrequency) * (template.Width / 2.0)) + paddingFrequency);
            var paddingHeight = (int)(((1 - paddingFrequency) * (template.Height / 2.0)) + paddingFrequency);

            // Procedure
            //
            // 0) Divide the level up into rectangles inside the padded region
            // 1) Generate a random point in each rectangle
            // 2) Return the convex hull of the resulting set of points
            //

            var points = new List<Vertex>();
            var xCursor = 0.0;

            // NOTE*** Creating entire mesh of points - but really should just create an edge
            //
            while ((xCursor + paddingWidth) <= template.Width)
            {
                var yCursor = 0.0;

                while ((yCursor + paddingHeight) <= template.Height)
                {
                    var column = _randomSequenceGenerator.Get((int)xCursor, (int)xCursor + paddingWidth);
                    var row = _randomSequenceGenerator.Get((int)yCursor, (int)yCursor + paddingHeight);

                    points.Add(new Vertex(column, row));

                    yCursor += paddingHeight;
                }

                xCursor += paddingWidth;
            }

            return GeometryUtility.GrahamScan(points);
        }
    }
}
