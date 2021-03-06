﻿using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Constant;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Service.Cache.Interface;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Service.Cache
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ISvgCache))]
    public class SvgCache : ISvgCache
    {
        // Default symbol for a failed SVG load
        readonly static DrawingGroup DEFAULT_DRAWING;
        readonly static SimpleDictionary<string, DrawingGroup> _cache;
        readonly static IEnumerable<string> _gameResourceNames;
        readonly static IEnumerable<string> _symbolResourceNames;
        readonly static IEnumerable<string> _orientedSymbolResourceNames;
        readonly static IEnumerable<string> _terrainSymbolResourceNames;
        readonly static SimpleDictionary<string, List<string>> _characterResourceNames;

        // These are paths that are absolute prefixes to the sub-folders
        const string SVG_PATH_PREFIX = "Rogue.NET.Common.Resource.Svg";

        // These are paths relative to the Svg folder in Rogue.NET.Common
        const string SVG_FOLDER_GAME = "Game";
        const string SVG_FOLDER_SCENARIO_CHARACTER = "Character";
        const string SVG_FOLDER_SCENARIO_SYMBOL = "Symbol";
        const string SVG_FOLDER_SCENARIO_ORIENTEDSYMBOL = "OrientedSymbol";
        const string SVG_FOLDER_SCENARIO_TERRAIN = "Terrain";

        static SvgCache()
        {
            // Create static SVG cache to feed other image caches
            _cache = new SimpleDictionary<string, DrawingGroup>();

            // Load and store resource names
            _gameResourceNames = GetResourceNamesImpl(SymbolType.Game);
            _symbolResourceNames = GetResourceNamesImpl(SymbolType.Symbol);
            _orientedSymbolResourceNames = GetResourceNamesImpl(SymbolType.OrientedSymbol);
            _terrainSymbolResourceNames = GetResourceNamesImpl(SymbolType.Terrain);
            _characterResourceNames = GetCharacterResourceNamesImpl();

            // Load default drawing
            DEFAULT_DRAWING = LoadSVG(SVG_FOLDER_GAME, GameSymbol.Identify);
        }

        /// <summary>
        /// Pre-loads symbols from SVG resources into memory. THIS SHOULD ONLY BE CALLED ONCE ON STARTUP!
        /// </summary>
        public static void Load()
        {
            // Load game symbols
            foreach (var gameSymbol in _gameResourceNames)
                LoadSVG(SVG_FOLDER_GAME, gameSymbol);

            // Load symbols
            foreach (var symbol in _symbolResourceNames)
                LoadSVG(SVG_FOLDER_SCENARIO_SYMBOL, symbol);

            // Load oriented symbols
            foreach (var orientedSymbol in _orientedSymbolResourceNames)
                LoadSVG(SVG_FOLDER_SCENARIO_ORIENTEDSYMBOL, orientedSymbol);

            // Load terrain symbols
            foreach (var terrainSymbol in _terrainSymbolResourceNames)
                LoadSVG(SVG_FOLDER_SCENARIO_TERRAIN, terrainSymbol);
        }
        public DrawingGroup GetDrawing(ScenarioCacheImage cacheImage)
        {
            // Calculate the cache key for this symbol
            var cacheKey = GetCacheKey(cacheImage);

            // Create and return a clone drawing
            if (_cache.ContainsKey(cacheKey))
                return _cache[cacheKey].Clone();

            // Create -> Cache -> return new drawing
            else
                return LoadSVG(cacheImage);
        }

        public IEnumerable<string> GetResourceNames(SymbolType type)
        {
            switch (type)
            {
                case SymbolType.Smiley:
                case SymbolType.Character:
                default:
                    throw new ArgumentException("Get Resource Names only returns Game / Symbol type names");
                case SymbolType.Symbol:
                    return _symbolResourceNames;
                case SymbolType.Game:
                    return _gameResourceNames;
                case SymbolType.OrientedSymbol:
                    return _orientedSymbolResourceNames;
                case SymbolType.Terrain:
                    return _terrainSymbolResourceNames;
            }
        }
        public IEnumerable<string> GetCharacterCategories()
        {
            return _characterResourceNames.Keys;
        }

        public IEnumerable<string> GetCharacterResourceNames(string category)
        {
            return _characterResourceNames[category];
        }
        private string GetCacheKey(ScenarioCacheImage cacheImage)
        {
            switch (cacheImage.SymbolType)
            {
                case SymbolType.Smiley:
                default:
                    throw new Exception("Unsupported symbol type in the ISvgCache");
                case SymbolType.Character:
                    return string.Join(".", SVG_PATH_PREFIX,
                                            SVG_FOLDER_SCENARIO_CHARACTER,
                                            cacheImage.SymbolPath);
                case SymbolType.Symbol:
                    return string.Join(".", SVG_PATH_PREFIX,
                                            SVG_FOLDER_SCENARIO_SYMBOL,
                                            cacheImage.SymbolPath);
                case SymbolType.Game:
                    return string.Join(".", SVG_PATH_PREFIX,
                                            SVG_FOLDER_GAME,
                                            cacheImage.SymbolPath);
                case SymbolType.OrientedSymbol:
                    return string.Join(".", SVG_PATH_PREFIX,
                                            SVG_FOLDER_SCENARIO_ORIENTEDSYMBOL,
                                            cacheImage.SymbolPath);
                case SymbolType.Terrain:
                    return string.Join(".", SVG_PATH_PREFIX,
                                            SVG_FOLDER_SCENARIO_TERRAIN,
                                            cacheImage.SymbolPath);
            }
        }
        private static IEnumerable<string> GetResourceNamesImpl(SymbolType type)
        {
            if (type != SymbolType.Game &&
                type != SymbolType.Symbol &&
                type != SymbolType.OrientedSymbol &&
                type != SymbolType.Terrain)
                throw new ArgumentException("Get Resource Names only returns Game / Symbol / Oriented Symbol / Terrain type names");

            var assembly = typeof(ZipEncoder).Assembly;

            // Parse the names like the folder structure
            //
            var folder = type == SymbolType.Symbol ? SVG_FOLDER_SCENARIO_SYMBOL :
                         type == SymbolType.OrientedSymbol ? SVG_FOLDER_SCENARIO_ORIENTEDSYMBOL :
                         type == SymbolType.Terrain ? SVG_FOLDER_SCENARIO_TERRAIN : SVG_FOLDER_GAME;

            var path = string.Join(".", SVG_PATH_PREFIX, folder);

            // Get resources from the character folder -> Parse out category names
            return assembly.GetManifestResourceNames()
                           .Where(x => x.Contains(path))
                           .Select(x => x.Replace(path, ""))
                           .Select(x =>
                           {
                               var pieces = x.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                               if (pieces.Length != 2)
                                   throw new Exception("Resource file-name format differs from expected");

                               // [FileName].svg
                               return pieces[0].Trim();
                           })
                           .Actualize();
        }
        private static SimpleDictionary<string, List<string>> GetCharacterResourceNamesImpl()
        {
            var assembly = typeof(ZipEncoder).Assembly;

            var categoryPath = string.Join(".", SVG_PATH_PREFIX, SVG_FOLDER_SCENARIO_CHARACTER);

            // Get resources from the character folder -> Parse out category names
            return assembly.GetManifestResourceNames()
                                   .Where(x => x.Contains(categoryPath))
                                   .Select(x => x.Replace(categoryPath, ""))
                                   .Select(x =>
                                   {
                                       var pieces = x.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                                       if (pieces.Length != 3)
                                           throw new Exception("Resource file-name format differs from expected");

                                       // [Category].[FileName].svg
                                       return new { Category = pieces[0].Trim(), FileName = pieces[1].Trim() };
                                   })
                                   .GroupBy(x => x.Category)
                                   .ToSimpleDictionary(x => x.Key,
                                                       x => x.Select(z => z.FileName).ToList());
        }
        private DrawingGroup LoadSVG(ScenarioCacheImage cacheImage)
        {
            switch (cacheImage.SymbolType)
            {
                case SymbolType.Smiley:
                default:
                    throw new Exception("Unsupported symbol type in the ISvgCache");
                case SymbolType.Character:
                    return LoadSVG(SVG_FOLDER_SCENARIO_CHARACTER, cacheImage.SymbolPath);
                case SymbolType.Symbol:
                    return LoadSVG(SVG_FOLDER_SCENARIO_SYMBOL, cacheImage.SymbolPath);
                case SymbolType.Game:
                    return LoadSVG(SVG_FOLDER_GAME, cacheImage.SymbolPath);
                case SymbolType.OrientedSymbol:
                    return LoadSVG(SVG_FOLDER_SCENARIO_ORIENTEDSYMBOL, cacheImage.SymbolPath);
                case SymbolType.Terrain:
                    return LoadSVG(SVG_FOLDER_SCENARIO_TERRAIN, cacheImage.SymbolPath);
            }
        }

        /// <summary>
        /// SEE SVG_PATH_* ABOVE!!!
        /// </summary>
        /// <param name="svgFolder">Path to primary SVG folder (See SVG_PATH_*) { Game, Scenario -> Character, Scenario -> Symbol }</param>
        /// <param name="svgPath">This is the path to the svg file AFTER the sub-folder:  .Svg.[Sub Folder].Path  (NO EXTENSION)</param>
        private static DrawingGroup LoadSVG(string svgFolder, string svgPath)
        {
            // Calculate resource path
            var path = string.Join(".", SVG_PATH_PREFIX, svgFolder, svgPath) + ".svg";

            // Check the static SVG cache for loaded symbol
            if (_cache.ContainsKey(path))
                return _cache[path].Clone();

            var assembly = Assembly.GetAssembly(typeof(ZipEncoder));
            var settings = new WpfDrawingSettings();
            settings.IncludeRuntime = true;
            settings.TextAsGeometry = true;
            settings.OptimizePath = false;

            try
            {
                using (var stream = assembly.GetManifestResourceStream(path))
                {
                    using (var reader = new FileSvgReader(settings))
                    {
                        // Read SVG from resource stream
                        var drawingGroup = reader.Read(stream);

                        // Apply transform to the base symbol
                        var finalDrawing = ApplyDrawingTransform(drawingGroup);

                        // Add SVG to static cache
                        _cache.Add(path, finalDrawing);

                        // Return the drawing group
                        return finalDrawing;
                    }
                }
            }
            catch (Exception)
            {
                // TODO: REMOVE THIS TRY / CATCH
                return DEFAULT_DRAWING;
            }
        }

        #region (private) SVG Geometry Transforms
        /// <summary>
        /// USED ONCE DURING SVG LOADING ONLY.  Applies a transform to the drawing group to fit it to a 
        /// bounding box with the supplied scale.
        /// </summary>
        private static DrawingGroup ApplyDrawingTransform(DrawingGroup group)
        {
            var transform = new TransformGroup();

            // HAVE TO MAINTAIN THE SYMBOL'S ASPECT RATIO WHILE FITTING IT TO OUR BOUNDING BOX
            //

            var scaleFactor = System.Math.Min((ModelConstants.CellWidth / group.Bounds.Width),
                                              (ModelConstants.CellHeight / group.Bounds.Height));

            transform.Children.Add(new TranslateTransform(group.Bounds.X * -1, group.Bounds.Y * -1));
            transform.Children.Add(new ScaleTransform(scaleFactor, scaleFactor));

            var visual = new DrawingVisual();

            using (var context = visual.RenderOpen())
            {
                context.PushTransform(transform);

                context.DrawDrawing(group);
            }

            return visual.Drawing;

            // TODO: REMOVE THIS AND RENDER  USING A TRANSFORM TO CREATE A NEW DRAWING
            // RecurseTransformDrawing(group, transform);

            // return group;
        }

        /// <summary>
        /// USED ONCE DURING SVG LOADING ONLY.  Recursively apply transform to drawing group points. 
        /// </summary>
        private static void RecurseTransformDrawing(DrawingGroup group, Transform transform)
        {
            foreach (var drawing in group.Children)
            {
                if (drawing is DrawingGroup)
                    RecurseTransformDrawing(drawing as DrawingGroup, transform);

                else if (drawing is GeometryDrawing)
                {
                    var geometryDrawing = drawing as GeometryDrawing;

                    RecurseTransformGeometry(geometryDrawing.Geometry, transform);
                }
                else
                    throw new Exception("Unhandled Drawing Type ISvgCache");
            }
        }
        private static void RecurseTransformGeometry(Geometry geometry, Transform transform)
        {
            if (geometry is PathGeometry)
            {
                var path = geometry as PathGeometry;

                foreach (var figure in path.Figures)
                {
                    // Transform Start Point
                    figure.StartPoint = transform.Transform(figure.StartPoint);

                    foreach (var segment in figure.Segments)
                        TransformPathSegment(segment, transform);
                }
            }
            else if (geometry is GeometryGroup)
            {
                foreach (var child in (geometry as GeometryGroup).Children)
                    RecurseTransformGeometry(child, transform);
            }
            else
                throw new Exception("Unhandled Geometry Type ISvgCache");
        }
        private static void TransformPathSegment(PathSegment segment, Transform transform)
        {
            if (segment is PolyBezierSegment)
            {
                var polyBezier = segment as PolyBezierSegment;

                polyBezier.Points = new PointCollection(polyBezier.Points.Select(x => transform.Transform(x)).Actualize());
            }
            else if (segment is PolyLineSegment)
            {
                var polyLine = segment as PolyLineSegment;

                polyLine.Points = new PointCollection(polyLine.Points.Select(x => transform.Transform(x)).Actualize());
            }
            else if (segment is PolyQuadraticBezierSegment)
            {
                var polyQuadBezier = segment as PolyQuadraticBezierSegment;

                polyQuadBezier.Points = new PointCollection(polyQuadBezier.Points.Select(x => transform.Transform(x)).Actualize());
            }
            else if (segment is QuadraticBezierSegment)
            {
                var quadBezier = segment as QuadraticBezierSegment;

                quadBezier.Point1 = transform.Transform(quadBezier.Point1);
                quadBezier.Point2 = transform.Transform(quadBezier.Point2);
            }
            else if (segment is BezierSegment)
            {
                var bezier = segment as BezierSegment;

                bezier.Point1 = transform.Transform(bezier.Point1);
                bezier.Point2 = transform.Transform(bezier.Point2);
                bezier.Point3 = transform.Transform(bezier.Point3);
            }
            else if (segment is LineSegment)
            {
                var line = segment as LineSegment;

                line.Point = transform.Transform(line.Point);
            }
            else if (segment is ArcSegment)
            {
                var arc = segment as ArcSegment;

                arc.Point = transform.Transform(arc.Point);

                var size = transform.Transform(new Point()
                {
                    X = arc.Size.Width,
                    Y = arc.Size.Height
                });

                arc.Size = new Size(size.X, size.Y);
            }
            else
                throw new Exception("Unhandled PathSegment Type ISvgCache");
        }
        #endregion
    }
}
