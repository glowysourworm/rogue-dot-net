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
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Service.Cache
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ISvgCache))]
    public class SvgCache : ISvgCache
    {
        // Default symbol for a failed SVG load
        readonly static DrawingGroup DEFAULT_DRAWING;
        readonly static IDictionary<string, DrawingGroup> _cache;
        readonly static IEnumerable<string> _gameResourceNames;
        readonly static IEnumerable<string> _symbolResourceNames;
        readonly static IEnumerable<string> _orientedSymbolResourceNames;
        readonly static IEnumerable<string> _terrainSymbolResourceNames;
        readonly static IDictionary<string, List<string>> _characterResourceNames;

        // These are paths that are absolute prefixes to the sub-folders
        const string SVG_PATH_PREFIX = "Rogue.NET.Common.Resource.Svg";

        // These are paths relative to the Svg folder in Rogue.NET.Common
        const string SVG_FOLDER_GAME = "Game";
        const string SVG_FOLDER_SCENARIO_CHARACTER = "Scenario.Character";
        const string SVG_FOLDER_SCENARIO_SYMBOL = "Scenario.Symbol";
        const string SVG_FOLDER_SCENARIO_ORIENTEDSYMBOL = "Scenario.OrientedSymbol";
        const string SVG_FOLDER_SCENARIO_TERRAIN = "Scenario.Terrain";

        static SvgCache()
        {
            // Create static SVG cache to feed other image caches
            _cache = new Dictionary<string, DrawingGroup>();

            // Load and store resource names
            _gameResourceNames = GetResourceNamesImpl(SymbolType.Game);
            _symbolResourceNames = GetResourceNamesImpl(SymbolType.Symbol);
            _orientedSymbolResourceNames = GetResourceNamesImpl(SymbolType.OrientedSymbol);
            _terrainSymbolResourceNames = GetResourceNamesImpl(SymbolType.Terrain);
            _characterResourceNames = GetCharacterResourceNamesImpl();

            // Load default drawing
            DEFAULT_DRAWING = LoadGameSVG(GameSymbol.Identify);
        }

        /// <summary>
        /// Pre-loads symbols from SVG resources into memory. THIS SHOULD ONLY BE CALLED ONCE ON STARTUP!
        /// </summary>
        public static void Load()
        {
            // Load game symbols
            foreach (var gameSymbol in _gameResourceNames)
                LoadGameSVG(gameSymbol);

            // Load symbols
            foreach (var symbol in _symbolResourceNames)
                LoadSymbolSVG(symbol);

            // Load oriented symbols
            foreach (var orientedSymbol in _orientedSymbolResourceNames)
                LoadOrientedSymbolSVG(orientedSymbol);

            // Load terrain symbols
            foreach (var terrainSymbol in _terrainSymbolResourceNames)
                LoadTerrainSymbolSVG(terrainSymbol);
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
            switch (cacheImage.Type)
            {
                case SymbolType.Smiley:
                default:
                    throw new Exception("Unsupported symbol type in the ISvgCache");
                case SymbolType.Character:
                    return string.Join(".", SVG_PATH_PREFIX,
                                            SVG_FOLDER_SCENARIO_CHARACTER,
                                            cacheImage.CharacterSymbolCategory,
                                            cacheImage.CharacterSymbol);
                case SymbolType.Symbol:
                    return string.Join(".", SVG_PATH_PREFIX,
                                            SVG_FOLDER_SCENARIO_SYMBOL,
                                            cacheImage.Symbol);
                case SymbolType.Game:
                    return string.Join(".", SVG_PATH_PREFIX,
                                            SVG_FOLDER_GAME,
                                            cacheImage.GameSymbol);
                case SymbolType.OrientedSymbol:
                    return string.Join(".", SVG_PATH_PREFIX,
                                            SVG_FOLDER_SCENARIO_ORIENTEDSYMBOL,
                                            cacheImage.Symbol);
                case SymbolType.Terrain:
                    return string.Join(".", SVG_PATH_PREFIX,
                                            SVG_FOLDER_SCENARIO_TERRAIN,
                                            cacheImage.Symbol);
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
        private static IDictionary<string, List<string>> GetCharacterResourceNamesImpl()
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
                                   .ToDictionary(x => x.Key,
                                                 x => x.Select(z => z.FileName)
                                                       .ToList());
        }
        private DrawingGroup LoadSVG(ScenarioCacheImage cacheImage)
        {
            switch (cacheImage.Type)
            {
                case SymbolType.Smiley:
                default:
                    throw new Exception("Unsupported symbol type in the ISvgCache");
                case SymbolType.Character:
                    return LoadCharacterSVG(cacheImage.CharacterSymbol, cacheImage.CharacterSymbolCategory);
                case SymbolType.Symbol:
                    return LoadSymbolSVG(cacheImage.Symbol);
                case SymbolType.Game:
                    return LoadGameSVG(cacheImage.GameSymbol);
                case SymbolType.OrientedSymbol:
                    return LoadOrientedSymbolSVG(cacheImage.Symbol);
                case SymbolType.Terrain:
                    return LoadTerrainSymbolSVG(cacheImage.Symbol);
            }
        }
        private static DrawingGroup LoadGameSVG(string gameSymbol)
        {
            return LoadSVG(SVG_FOLDER_GAME, "", gameSymbol);
        }
        private static DrawingGroup LoadSymbolSVG(string symbol)
        {
            return LoadSVG(SVG_FOLDER_SCENARIO_SYMBOL, "", symbol);
        }
        private static DrawingGroup LoadOrientedSymbolSVG(string symbol)
        {
            return LoadSVG(SVG_FOLDER_SCENARIO_ORIENTEDSYMBOL, "", symbol);
        }
        private static DrawingGroup LoadTerrainSymbolSVG(string symbol)
        {
            return LoadSVG(SVG_FOLDER_SCENARIO_TERRAIN, "", symbol);
        }
        private static DrawingGroup LoadCharacterSVG(string character, string characterCategory)
        {
            return LoadSVG(SVG_FOLDER_SCENARIO_CHARACTER, characterCategory, character);
        }

        /// <summary>
        /// SEE SVG_PATH_* ABOVE!!! SubPath is the symbol category (points to sub-folder for symbols)
        /// </summary>
        /// <param name="svgPath">Path to primary SVG folder (See SVG_PATH_*) { Game, Scenario -> Character, Scenario -> Symbol }</param>
        /// <param name="subPath">This is the character category (</param>
        /// <param name="svgName">This is the NAME of the svg file (WITHOUT THE EXTENSION)</param>
        /// <returns></returns>
        private static DrawingGroup LoadSVG(string svgPath, string subPath, string svgName)
        {
            // Calculate resource path
            var basePath = "Rogue.NET.Common.Resource.Svg";
            var path = !string.IsNullOrEmpty(subPath) ? string.Join(".", basePath, svgPath, subPath, svgName, "svg")
                                                      : string.Join(".", basePath, svgPath, svgName, "svg");

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
                        ApplyDrawingTransform(drawingGroup);

                        // Add SVG to static cache
                        _cache.Add(path, drawingGroup);

                        // Return the drawing group
                        return drawingGroup;
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
        private static void ApplyDrawingTransform(DrawingGroup group)
        {
            var transform = new TransformGroup();

            // HAVE TO MAINTAIN THE SYMBOL'S ASPECT RATIO WHILE FITTING IT TO OUR BOUNDING BOX
            //

            var scaleFactor = System.Math.Min((ModelConstants.CellWidth / group.Bounds.Width),
                                              (ModelConstants.CellHeight / group.Bounds.Height));

            transform.Children.Add(new ScaleTransform(scaleFactor, scaleFactor));
            transform.Children.Add(new TranslateTransform(group.Bounds.X * -1, group.Bounds.Y * -1));

            RecurseTransformDrawing(group, transform);
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
            else
                throw new Exception("Unhandled PathSegment Type ISvgCache");
        }
        #endregion
    }
}
