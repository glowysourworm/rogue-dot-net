using QuickGraph;
using QuickGraph.Algorithms;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Extension;
using Rogue.NET.Core.Logic.Static;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Component;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using TriangleNet.Geometry;
using TriangleNet.Meshing;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(ILayoutGenerator))]
    public class LayoutGenerator : ILayoutGenerator
    {
        private readonly IRandomSequenceGenerator _randomSequenceGenerator;

        // Size is great enough for an upstairs, downstairs, and 2 teleport pods (in and out of the room)
        private const int ROOM_SIZE_MIN = 4;

        // NOTE** Not used for all layout types. It was not required for certain types
        //        that had other padding involved (Rectangular Grid); or no padding (Maze).
        //
        //        MUST BE GREATER THAN OR EQUAL TO 2.
        private const int CELLULAR_AUTOMATA_PADDING = 2;
        private const int CELLULAR_AUTOMATA_ITERATIONS = 5;
        

        [ImportingConstructor]
        public LayoutGenerator(IRandomSequenceGenerator randomSequenceGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public IEnumerable<Level> CreateDungeonLayouts(ScenarioConfigurationContainer configuration)
        {
            var levels = new List<Level>();

            for (int i = 0; i < configuration.DungeonTemplate.NumberOfLevels; i++)
            {
                var levelNumber = i + 1;

                // Layout templates in range
                var layoutTemplates = configuration.DungeonTemplate
                                                   .LayoutTemplates
                                                   .Where(x => x.Level.Contains(levelNumber))
                                                   .ToList();

                // Choose random layout template in the level range
                var template = _randomSequenceGenerator.GetWeightedRandom(layoutTemplates, x => x.GenerationRate);

                var grid = CreateLayout(template);
                var level = new Level(template.Name,
                                       grid,
                                       template.Type,
                                       template.ConnectionType,
                                       levelNumber,
                                       template.WallColor,
                                       template.DoorColor);
                levels.Add(level);
            }
            return levels;
        }
        private LevelGrid CreateLayout(LayoutTemplate template)
        {
            LevelGrid grid = null;

            // Procedure
            //
            // 0) Create cells in the LevelGrid - starting with either rooms or a maze layout type.
            // 1) For rooms - have to create connecting corridors (depending on type - some use teleporters which 
            //    are defined when filling in the level contents.
            // 2) Initialize the grid to create fast access data storage

            switch (template.Type)
            {
                case LayoutType.ConnectedRectangularRooms:
                case LayoutType.ConnectedCellularAutomata:
                    grid = CreateConnectedRooms(template);
                    break;
                case LayoutType.Maze:
                    grid = CreateMaze(template);
                    break;
                default:
                    throw new Exception("Unknown Layout Type: " + template.Type.ToString());
            }

            return grid;
        }
        private LevelGrid CreateConnectedRooms(LayoutTemplate template)
        {
            // The grid is constructed with the rooms already set. Corridors are added as 
            // required
            LevelGrid grid = null;

            // Used for rectilinear room creation
            Room[,] roomGrid = null;

            // Connected Rooms
            if (template.Type == LayoutType.ConnectedRectangularRooms)
            {
                switch (template.RoomPlacementType)
                {
                    case LayoutRoomPlacementType.RectangularGrid:
                        grid = CreateRectangularGridRooms(template, out roomGrid);
                        break;
                    case LayoutRoomPlacementType.Random:
                        grid = CreateRandomRooms(template);
                        break;
                    default:
                        throw new Exception("Unknown Room Placement Type");
                }
            }

            // Connected Cellular Automata
            else if (template.Type == LayoutType.ConnectedCellularAutomata)
            {
                switch (template.CellularAutomataType)
                {
                    case LayoutCellularAutomataType.Filled:
                        grid = CreateCellularAutomata(template, true);
                        break;
                    case LayoutCellularAutomataType.Open:
                        grid = CreateCellularAutomata(template, false);
                        break;
                    default:
                        throw new Exception("Unknown Cellular Automata Type");
                }
            }
            else
                throw new Exception("Unknown layout type");

            // Create room connections
            switch (template.ConnectionType)
            {
                case LayoutConnectionType.Corridor:
                case LayoutConnectionType.CorridorWithDoors:
                    CreateCorridors(grid, template, roomGrid);
                    break;

                // These are handled when filling in the contents.
                case LayoutConnectionType.Teleporter:
                case LayoutConnectionType.TeleporterRandom:
                default:
                    break;
            }

            // By this point cells with doors have been created
            CreateWalls(grid);

            return grid;
        }

        #region (private) Room Generation
        /// <summary>
        /// Creates rooms on a rectangular grid - returns the grid with which the rooms were structured
        /// </summary>
        private LevelGrid CreateRectangularGridRooms(LayoutTemplate template, out Room[,] roomGrid)
        {
            var bounds = new CellRectangle(new GridLocation(0, 0), template.Width, template.Height);
            var gridDivisionWidth = template.RoomWidthLimit + (2 * template.RectangularGridPadding);
            var gridDivisionHeight = template.RoomHeightLimit + (2 * template.RectangularGridPadding);

            // 2D Cell array used for primary LevelGrid object
            var grid = new Cell[bounds.CellWidth, bounds.CellHeight];

            // Calculate room grid here for use in corridor computation
            roomGrid = new Room[template.NumberRoomCols, template.NumberRoomRows];

            // Collect room data to pass to level grid constructor
            var rooms = new List<Room>();

            // Create rooms according to template parameters
            for (int i=0;i<template.NumberRoomCols;i++)
            {
                var divisionColumn = i * gridDivisionWidth;

                for (int j=0;j<template.NumberRoomRows;j++)
                {
                    var divisionRow = j * gridDivisionHeight;

                    // Draw a random room size
                    var roomWidth = _randomSequenceGenerator.Get(template.RoomWidthMin, template.RoomWidthLimit + 1);
                    var roomHeight = _randomSequenceGenerator.Get(template.RoomHeightMin, template.RoomHeightLimit + 1);

                    // Generate the upper left-hand corner for the room
                    var column = divisionColumn + 
                                 _randomSequenceGenerator.Get(template.RectangularGridPadding, 
                                                              gridDivisionWidth - (roomWidth + template.RectangularGridPadding) + 1);

                    var row = divisionRow + 
                              _randomSequenceGenerator.Get(template.RectangularGridPadding, 
                                                           gridDivisionHeight - (roomHeight + template.RectangularGridPadding) + 1);

                    var roomBounds = new CellRectangle(new GridLocation(row, column), roomWidth, roomHeight);
                    var roomCells = new List<GridLocation>();
                    var edgeCells = new List<GridLocation>();

                    // Create cells to fill the room
                    for (int roomCol = column; roomCol < column + roomWidth; roomCol++)
                    {
                        for (int roomRow = row; roomRow < row + roomHeight; roomRow++)
                        {
                            var cell = new Cell(roomCol, roomRow, Compass.Null);

                            // Store new cell in the grid
                            grid[roomCol, roomRow] = cell;

                            // Store cell location for room data
                            roomCells.Add(cell.Location);

                            // Store edge cells for room data
                            if (roomCol == column ||
                                roomCol == ((roomWidth + column) - 1) ||
                                roomRow == row ||
                                roomRow == ((row + roomHeight) - 1))
                                edgeCells.Add(cell.Location);
                        }
                    }

                    // Save room data
                    var room = new Room(roomCells.ToArray(), edgeCells.ToArray(), roomBounds);
                    rooms.Add(room);
                    roomGrid[i, j] = room;
                }
            }

            return new LevelGrid(grid, rooms.ToArray());
        }
        private LevelGrid CreateRandomRooms(LayoutTemplate template)
        {
            var bounds = new CellRectangle(new GridLocation(0, 0), template.Width, template.Height);

            // 2D Cell array used for primary LevelGrid object
            var grid = new Cell[bounds.CellWidth, bounds.CellHeight];

            // Create room cells first - then identify rooms (possibly overlapping)
            for (int i=0;i<template.RandomRoomCount;i++)
            {
                // First, generate the room height and width
                var roomWidth = _randomSequenceGenerator.Get(template.RoomWidthMin, template.RoomWidthLimit + 1);
                var roomHeight = _randomSequenceGenerator.Get(template.RoomHeightMin, template.RoomHeightLimit + 1);

                // Choose placement of room - LEAVING ONE CELL PADDING
                //
                // NOTE*** LEFT ONE CELL PADDING FOR RENDERING REASONS
                var column = _randomSequenceGenerator.Get(1, bounds.CellWidth - roomWidth);
                var row = _randomSequenceGenerator.Get(1, bounds.CellHeight - roomHeight);

                // Create cells to fill the room
                for (int roomCol = column; roomCol < column + roomWidth; roomCol++)
                {
                    for (int roomRow = row; roomRow < row + roomHeight; roomRow++)
                        grid[roomCol, roomRow] = new Cell(roomCol, roomRow, Compass.Null);
                }
            }

            return IdentifyRooms(grid);
        }
        private LevelGrid CreateCellularAutomata(LayoutTemplate template, bool filled)
        {
            // Procedure
            //
            // 0) Create random cells in grid
            // 1) Iterate grid several times using smoothing rules
            // 2) Locate rooms and store them in an array

            var bounds = new CellRectangle(new GridLocation(0, 0), template.Width, template.Height);

            // 2D Cell array used for primary LevelGrid object
            var grid = new Cell[bounds.CellWidth, bounds.CellHeight];

            // Create function to get count of adjacent empty cells
            //
            // NOTE*** CELLULAR_AUTOMATA_PADDING prevents out of bounds errors
            var emptyCellCountFunc = new Func<Cell[,], int, int, int, int, int>((gridArray, column, row, aggregator, distance) =>
            {
                // Use aggregator to add results from other distances
                var result = aggregator;

                if (distance == 0)
                    return result + (gridArray[column, row] == null ? 1 : 0);

                // Count Top and Bottom first
                for (int i = column - distance; i <= column + distance; i++)
                {
                    result += gridArray[i, row - distance] == null ? 1 : 0;
                    result += gridArray[i, row + distance] == null ? 1 : 0;
                }

                // Count Left and Right - excluding corners
                for (int j = (row - distance) + 1; j <= (row + distance) - 1; j++)
                {
                    result += gridArray[column - distance, j] == null ? 1 : 0;
                    result += gridArray[column + distance, j] == null ? 1 : 0;
                }

                return result;
            });

            // Iterate grid cells and fill them in randomly
            // 
            // Optimize:  Could cut down on the amount of iterations by using the fill ratio
            //            to give you a number of cells (to be chosen randomly)
            IterateCellularAutomata(grid, 1, (gridArray, currentCell, index) =>
            {
                // Fill Ratio => Empty Cell
                return _randomSequenceGenerator.Get() < template.CellularAutomataFillRatio;
            });

            // Iterate grid to apply Filled Rule - Apply this for several iterations before smoothing
            if (filled)
            {
                IterateCellularAutomata(grid, CELLULAR_AUTOMATA_ITERATIONS, (gridArray, column, row) =>
                {
                    var empty0Count = emptyCellCountFunc(gridArray, column, row, 0, 0);
                    var empty1Count = emptyCellCountFunc(gridArray, column, row, empty0Count, 1);

                    if (empty1Count >= 5)
                        return true;

                    var empty2Count = emptyCellCountFunc(gridArray, column, row, empty1Count, 2);

                    return (empty2Count <= 7);
                });
            }

            // Iterate grid to apply Open Rule - This will smooth out the noise in the grid. Also,
            // it is used in conjunction with the Filled rule to fill out large open spaces after
            // the Filled rule is applied
            IterateCellularAutomata(grid, filled ? 1 : CELLULAR_AUTOMATA_ITERATIONS, (gridArray, column, row) =>
            {
                return (emptyCellCountFunc(gridArray, column, row, 0, 0) + emptyCellCountFunc(gridArray, column, row, 0, 1)) >= 5;
            });


            return IdentifyRooms(grid);
        }

        /// <summary>
        /// Method that iterates the grid - filling each cell based on the provided function
        /// </summary>
        /// <param name="cellularAutomataRule">function that takes a Cell[,], and the iteration index - returning a bool to specfiy cell creation.</param>
        private void IterateCellularAutomata(Cell[,] grid, int numberOfIterations, Func<Cell[,], int, int, bool> cellularAutomataRule)
        {
            for (int k = 0; k < numberOfIterations; k++)
            {
                for (int i = CELLULAR_AUTOMATA_PADDING; i < grid.GetLength(0) - CELLULAR_AUTOMATA_PADDING; i++)
                {
                    for (int j = CELLULAR_AUTOMATA_PADDING; j < grid.GetLength(1) - CELLULAR_AUTOMATA_PADDING; j++)
                    {
                        if (cellularAutomataRule(grid, i, j))
                            grid[i, j] = null;

                        else
                            grid[i, j] = new Cell(i, j, Compass.Null);
                    }
                }
            }
        }

        /// <summary>
        /// Identifies rooms in the grid and returns a LevelGrid object with the rooms and room cells set. Also, creates
        /// a minimum sized room of 4 cells if there are no viable rooms available.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private LevelGrid IdentifyRooms(Cell[,] grid)
        {
            // Locate rooms and assign them inside the LevelGrid
            //
            // 0) Iterate cells
            // 1) First cell that's non-empty AND not part of an existing room
            // 2) Use "fill algorithm" to find connected cells
            // 3) Assign each to a new room
            //
            // Use the Cell[,] passed back from the fill algorithm to check for cell's room identity

            // Collect room data to pass to level grid constructor
            var rooms = new List<Room>();

            // Collect cell data on new rooms to know what locations have been found
            // to be in one of the rooms (during iteration)
            var roomGrids = new List<Cell[,]>();

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] != null &&
                       !roomGrids.Any(room => room[i, j] != null))
                    {
                        // Use "fill algorithm" to locate all room cells. (doens't include use of doors)
                        //
                        // ***REMOVE ANY ROOM THAT IS SMALLER THAN 4 CELLS. This is chosen so that there's room
                        //    for an upstairs, downstairs, and 2 teleporters (accomodates all level types).

                        Cell[,] roomGrid = null;
                        Room room = null;

                        var success = ApplyFloodFill(grid, grid[i, j].Location, true, out roomGrid, out room);

                        // Save the room data
                        if (success)
                        {
                            rooms.Add(room);
                            roomGrids.Add(roomGrid);
                        }
                    }
                }
            }

            // If there are no rooms of minimum size - then create one
            if (rooms.Count == 0)
            {
                var cell1 = new Cell(0, 0, Compass.Null);
                var cell2 = new Cell(0, 1, Compass.Null);
                var cell3 = new Cell(1, 0, Compass.Null);
                var cell4 = new Cell(1, 1, Compass.Null);

                grid[0, 0] = cell1;
                grid[0, 1] = cell2;
                grid[1, 0] = cell3;
                grid[1, 1] = cell4;

                var locations = new GridLocation[] { cell1.Location, cell2.Location, cell3.Location, cell4.Location };

                rooms.Add(new Room(locations, locations, new CellRectangle(0, 0, 1, 1)));
            }

            return new LevelGrid(grid, rooms.ToArray());
        }
        #endregion

        #region (private) Corridor Generation
        private void CreateCorridors(LevelGrid grid, LayoutTemplate template, Room[,] roomGrid)
        {
            switch (template.ConnectionGeometryType)
            {
                case LayoutConnectionGeometryType.Rectilinear:
                    CreateRectilinearCorridors(grid, template, roomGrid);
                    break;
                case LayoutConnectionGeometryType.MinimumSpanningTree:
                    CreateMinimumSpanningTreeCorridors(grid, template);
                    break;
                default:
                    throw new Exception("Invalid room connection geometry type");
            }
        }

        /// <summary>
        /// Creates corridors on a grid - horizontally and vertically between the rectangular grid
        /// layout
        /// </summary>
        private void CreateRectilinearCorridors(LevelGrid grid, LayoutTemplate template, Room[,] roomGrid)
        {
            if (grid.Rooms.Count() == 1)
                return;

            // Starting at the top-left, connect right and down
            for (int i = 0; i < template.NumberRoomCols; i++)
            {
                for (int j = 0; j < template.NumberRoomRows; j++)
                {
                    var room = roomGrid[i, j];

                    // Connect Right
                    if (i < template.NumberRoomCols - 1)
                    {
                        var otherRoom = roomGrid[i + 1, j];

                        ConnectRooms(grid, room, otherRoom, template);
                    }

                    // Connect Down
                    if (j < template.NumberRoomRows - 1)
                    {
                        var otherRoom = roomGrid[i, j + 1];

                        ConnectRooms(grid, room, otherRoom, template);
                    }
                }
            }
        }

        /// <summary>
        /// Creates corridors using the MST algorithm to calculate the best routes between rooms
        /// </summary>
        private void CreateMinimumSpanningTreeCorridors(LevelGrid grid, LayoutTemplate template)
        {
            // Procedure
            // 
            // 0) For grids with less than 3 rooms - Connect them directly
            // 1) Create Delaunay Triangulation to prevent overloading the MST algorithm
            // 2) Run MST algorithm to calculate the corridor topology
            // 3) Connect rooms via the tree using nearest neighbor calculation on edge cells

            if (grid.Rooms.Count() == 1)
                return;

            // The Delaunay Triangulation algorithm fails for less than 3 verticies
            else if (grid.Rooms.Count() < 3)
            {
                ConnectRooms(grid, grid.Rooms.ElementAt(0), grid.Rooms.ElementAt(1), template);
                return;
            }


            // Grid bounds
            var bounds = grid.Bounds;

            // Use QuickGraph to find the minimum spanning tree of room points
            var graph = new UndirectedGraph<Room, Edge<Room>>(
                            true,
                            (edge, room1, room2) =>
                            {
                                // Checks for equality of edges
                                return (edge.Source == room1 &&
                                        edge.Target == room2) ||
                                       (edge.Source == room2 &&
                                        edge.Target == room1);
                            },
                            grid.Rooms.Count(),
                            new RoomEqualityComparer());

            // APPROXIMATION** Use the center point for each bounding rectangle as a vertex
            graph.AddVertexRange(grid.Rooms);


            // Calculate Delauny Triangulation to reduce the calculation time of the minimum spanning tree
            var polygon = new Polygon();

            // Add room centers as points to the polygon
            foreach (var room in grid.Rooms)
            {
                polygon.Add(new Vertex(room.Bounds.Center.Column,
                                       room.Bounds.Center.Row)
                {
                    // Access rooms by hash code
                    ID = room.GetHashCode()
                });
            }

            // Create the Delaunay Triangulation
            var delaunayMesh = polygon.Triangulate(new ConstraintOptions()
            {
                Convex = false
            });

            // Add all edges from the Delaunay Triangulation - this should be a tennable number
            foreach (var edge in delaunayMesh.Edges)
            {
                var vertex1 = delaunayMesh.GetVertexById(edge.P0);
                var vertex2 = delaunayMesh.GetVertexById(edge.P1);

                var room1 = grid.Rooms.First(room => room.GetHashCode() == vertex1.ID);
                var room2 = grid.Rooms.First(room => room.GetHashCode() == vertex2.ID);

                // Add edge for a possible corridor
                graph.AddEdge(new Edge<Room>(room1, room2));
            }

            // Calculate minimum spanning tree (closest neighboring rooms) using the euclidean distance as the weight
            var corridorTopology = graph.MinimumSpanningTreePrim((edge) => Calculator.EuclideanDistance(edge.Source.Bounds.Center,
                                                                                                        edge.Target.Bounds.Center));

            // Connect rooms for each edge
            foreach (var edge in corridorTopology)
                ConnectRooms(grid, edge.Source, edge.Target, template);
        }

        /// <summary>
        /// Connects two rooms by calculating nearest neighbor cells
        /// </summary>
        private void ConnectRooms(LevelGrid grid, Room room1, Room room2, LayoutTemplate template)
        {
            // Locate the nearest neighbor cells
            Cell room1Cell = null;
            Cell room2Cell = null;
            CalculateNearestNeighborCells(template, grid, room1, room2, out room1Cell, out room2Cell);

            IEnumerable<Cell> corridor = null;

            // Create the corridor cells and assign them to the grid
            switch (template.CorridorGeometryType)
            {
                case LayoutCorridorGeometryType.Linear:
                    corridor = CreateLinearCorridor(grid, room1Cell, room2Cell, template);
                    break;
                default:
                    throw new Exception("Unknown Corridor Geometry Type");
            }
            

            // Add cells to the grid
            foreach (var cell in corridor)
                grid[cell.Location.Column, cell.Location.Row] = cell;
        }

        /// <summary>
        /// Creates new corridor cells for a linear corridor from cell1 to cell2
        /// </summary>
        private IEnumerable<Cell> CreateLinearCorridor(LevelGrid grid, Cell cell1, Cell cell2, LayoutTemplate template)
        {
            // Check for neighboring (cardinal) or same-identity cell. For this case, just return an empty array
            if (cell1.Location.Equals(cell2.Location) ||
                grid.GetCardinalAdjacentCells(cell1.Location)
                    .Any(x => x.Equals(cell2.Location)))
                return new Cell[] { };

            var corridor = new List<Cell>();
            var createDoors = template.ConnectionType == LayoutConnectionType.CorridorWithDoors;

            // Procedure
            //
            // 0) Create ray from center of cell1 to the center of cell2
            // 1) Create integer "grid lines" to locate intersections
            // 2) Add intersecting cells to the result

            var physicalLocation1 = LevelGridExtension.TransformToPhysicalLayout(cell1.Location);
            var physicalLocation2 = LevelGridExtension.TransformToPhysicalLayout(cell2.Location);

            var minX = (int)Math.Min(physicalLocation1.X, physicalLocation2.X);
            var minY = (int)Math.Min(physicalLocation1.Y, physicalLocation2.Y);
            var rangeX = (int)Math.Abs(physicalLocation2.X - physicalLocation1.X);
            var rangeY = (int)Math.Abs(physicalLocation2.Y - physicalLocation1.Y);

            // Offset to the center of the cell
            physicalLocation1.X += ModelConstants.CellWidth / 2.0F;
            physicalLocation1.Y += ModelConstants.CellHeight / 2.0F;

            physicalLocation2.X += ModelConstants.CellWidth / 2.0F;
            physicalLocation2.Y += ModelConstants.CellHeight / 2.0F;

            // Calculate ray parameters
            var slope = (physicalLocation2.Y - physicalLocation1.Y) / ((double)(physicalLocation2.X - physicalLocation1.X));
            var intercept = physicalLocation1.Y - (slope * physicalLocation1.X);

            var directionX = Math.Sign(physicalLocation2.X - physicalLocation1.X);
            var directionY = Math.Sign(physicalLocation2.Y - physicalLocation1.Y);

            // Grid lines don't include the originating cells
            var xLines = new List<float>();
            var yLines = new List<float>();

            // START ONE OFF THE MIN (Have to draw this out to see why...)
            var xCounter = minX + ModelConstants.CellWidth;
            var yCounter = minY + ModelConstants.CellHeight;

            while (xCounter <= minX + rangeX)
            {
                xLines.Add(xCounter);
                xCounter += ModelConstants.CellWidth;
            }

            while (yCounter <= minY + rangeY)
            {
                yLines.Add(yCounter);
                yCounter += ModelConstants.CellHeight;
            }

            // Store intersections in a list and use them to calculate the corridor
            var intersections = new List<PointF>();

            // Calculate all intersections of the ray defined above with the grid lines
            foreach (var xLine in xLines)
            {
                if (!double.IsInfinity(slope))
                    intersections.Add(new PointF(xLine, (float)((slope * xLine) + intercept)));
            }
            foreach (var yLine in yLines)
            {
                // For this case - add the horizontal lines as intersections
                if (double.IsInfinity(slope))
                    intersections.Add(new PointF(physicalLocation1.X, yLine));

                else if (slope != 0D)
                    intersections.Add(new PointF((float)((yLine - intercept) / slope), yLine));
            }


            // Order the intersections by their distance from the first point
            intersections = intersections.Where(point =>
                                         {
                                             // Need to filter out intersections that fell outside the bounds of the grid. This 
                                             // can happen because of an extraneous grid line. Extraneous grid lines are 
                                             // off-by-one errors from calculating the horizontal / vertical lines. These are
                                             // automatically filtered out below because the algorithm checks for cells already
                                             // alive in the grid before creating new ones. Also, no extra cells are built because
                                             // the error is only ever off-by-one.
                                             //
                                             // However, for small or near-infinity slopes, off-by-one errors can add a point that
                                             // is outside the bounds of the grid. So, those must filtered off prior to entering 
                                             // the loop below.
                                             var location = new GridLocation((int)Math.Floor(point.Y / ModelConstants.CellHeight),
                                                                          (int)Math.Floor(point.X / ModelConstants.CellWidth));

                                             if (location.Column < 0 ||
                                                 location.Column >= grid.Bounds.CellWidth)
                                                 return false;

                                             else if (location.Row < 0 ||
                                                      location.Row >= grid.Bounds.CellHeight)
                                                 return false;

                                             return true;
                                         })
                                         .OrderBy(point => Calculator.EuclideanSquareDistance(physicalLocation1, point))
                                         .ToList();

            // Trace along the ray and get the cells that are going to be part of the corridor
            foreach (var point in intersections)
            {
                // cell location of intersection
                var location = new GridLocation((int)Math.Floor(point.Y / ModelConstants.CellHeight), 
                                             (int)Math.Floor(point.X / ModelConstants.CellWidth));

                // intersection landed on one of the vertical lines
                var horizontal = (point.X % ModelConstants.CellWidth) <= double.Epsilon;

                Cell gridCell1 = null;
                Cell gridCell2 = null;
                Cell corridor1 = null;
                Cell corridor2 = null;

                // Using the ray direction and the intersection - calculate the next two points to include in the
                // result
                if (horizontal)
                {
                    var location1 = directionX > 0 ? new GridLocation(location.Row, location.Column - 1) :
                                                     new GridLocation(location.Row, location.Column);

                    var location2 = directionX > 0 ? new GridLocation(location.Row, location.Column) :
                                                     new GridLocation(location.Row, location.Column - 1);

                    gridCell1 = grid[location1.Column, location1.Row];
                    gridCell2 = grid[location2.Column, location2.Row];

                    corridor1 = gridCell1 ??
                                corridor.FirstOrDefault(cell => cell.Location.Equals(location1)) ??
                                new Cell(location1, Compass.Null);

                    corridor2 = gridCell2 ??
                                corridor.FirstOrDefault(cell => cell.Location.Equals(location2)) ??
                                new Cell(location2, Compass.Null);
                }
                else
                {
                    var location1 = directionY > 0 ? new GridLocation(location.Row - 1, location.Column) :
                                                     new GridLocation(location.Row, location.Column);

                    var location2 = directionY > 0 ? new GridLocation(location.Row , location.Column) :
                                                     new GridLocation(location.Row - 1, location.Column);

                    gridCell1 = grid[location1.Column, location1.Row];
                    gridCell2 = grid[location2.Column, location2.Row];

                    corridor1 = gridCell1 ?? 
                                corridor.FirstOrDefault(cell => cell.Location.Equals(location1)) ??  
                                new Cell(location1, Compass.Null);

                    corridor2 = gridCell2 ??
                                corridor.FirstOrDefault(cell => cell.Location.Equals(location2)) ?? 
                                new Cell(location2, Compass.Null);
                }

                // Create a door (if the location is on the edge of a room)
                if (createDoors)
                {
                    // Criteria for door
                    //
                    // 1) Grid cell is not yet in the corridor
                    // 2) Cardinally adjacent cell (N,S,E,W) is in a room (not the corridor)
                    if (gridCell1 == null)
                    {
                        foreach (var adjacentRoomCell in grid.GetCardinalAdjacentCells(corridor1.Location)
                                                             .Where(cell => grid[cell.Location.Column, cell.Location.Row] != null))
                        {
                            var direction = grid.GetDirectionOfAdjacentLocation(corridor1.Location, adjacentRoomCell.Location);
                            var oppositeDirection = LevelGridExtension.GetOppositeDirection(direction);

                            // Set up hidden door parameters for both "sides" of the door.
                            var hiddenDoor = _randomSequenceGenerator.Get() < template.HiddenDoorProbability;
                            var counter = hiddenDoor ? _randomSequenceGenerator.Get(1, 20) : 0;

                            // Set door on the corridor and the adjacent room cell
                            CreateDoor(corridor1, direction, counter);
                            CreateDoor(adjacentRoomCell, oppositeDirection, counter);
                        }
                    }
                    if (gridCell2 == null)
                    {
                        foreach (var adjacentRoomCell in grid.GetCardinalAdjacentCells(corridor2.Location)
                                                             .Where(cell => grid[cell.Location.Column, cell.Location.Row] != null))
                        {
                            var direction = grid.GetDirectionOfAdjacentLocation(corridor2.Location, adjacentRoomCell.Location);
                            var oppositeDirection = LevelGridExtension.GetOppositeDirection(direction);

                            // Set up hidden door parameters for both "sides" of the door.
                            var hiddenDoor = _randomSequenceGenerator.Get() < template.HiddenDoorProbability;
                            var counter = hiddenDoor ? _randomSequenceGenerator.Get(1, 20) : 0;

                            // Set door on the corridor and the adjacent room cell
                            CreateDoor(corridor2, direction, counter);
                            CreateDoor(adjacentRoomCell, oppositeDirection, counter);
                        }
                    }
                }

                // Store the results
                if (!corridor.Any(cell => cell == corridor1) && gridCell1 == null)
                    corridor.Add(corridor1);

                if (!corridor.Any(cell => cell == corridor2) && gridCell2 == null)
                    corridor.Add(corridor2);
            }

            // Validating the corridor
            if (corridor.Any(x => x == cell1 || x == cell2))
                throw new Exception("Invalid corridor created");

            if (createDoors && (!cell1.IsDoor || !cell2.IsDoor))
                throw new Exception("Invalid corridor created");

            if (grid[cell1.Location.Column, cell1.Location.Row] != cell1 ||
                grid[cell2.Location.Column, cell2.Location.Row] != cell2)
                throw new Exception("Invalid corridor created");

            return corridor;
        }
        
        private void CreateDoor(Cell cell, Compass direction, int hiddenDoorCounter)
        {
            

            switch (direction)
            {
                case Compass.N:
                    cell.SetDoorOR(direction, hiddenDoorCounter, 0, 0, 0);
                    break;
                case Compass.S:
                    cell.SetDoorOR(direction, 0, hiddenDoorCounter, 0, 0);
                    break;
                case Compass.E:
                    cell.SetDoorOR(direction, 0, 0, hiddenDoorCounter, 0);
                    break;
                case Compass.W:
                    cell.SetDoorOR(direction, 0, 0, 0, hiddenDoorCounter);
                    break;
                default:
                    throw new Exception("Invalid door creation direction");
            }
        }
        #endregion

        #region (private) Calculation Methods
        /// <summary>
        /// Calculates whether adjacent cell is connected by accessible path
        /// </summary>
        private bool IsAdjacentCellConnected(Cell[,] grid, GridLocation location, GridLocation adjacentLocation)
        {
            var direction = LevelGridExtension.GetDirectionBetweenAdjacentPoints(location, adjacentLocation);

            Compass cardinalDirection = Compass.Null;

            switch (direction)
            {
                case Compass.N:
                case Compass.S:
                case Compass.E:
                case Compass.W:
                    return true;

                case Compass.NW:
                case Compass.NE:
                case Compass.SE:
                case Compass.SW:
                    return grid.GetOffDiagonalCell1(location, direction, out cardinalDirection) != null ||
                           grid.GetOffDiagonalCell2(location, direction, out cardinalDirection) != null;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Creates room from cells connected to the test location (must be a cell). This helps to identify rooms; and
        /// does NOT take into account doors. Returns flag for success. Failure results from room not being of minimum size. 
        /// These cells are removed if specified by the provided flag (removeRoomErrors)
        /// <paramref name="roomError">Identifies error with room creation (ROOM SIZE MUST BE >= 4)</paramref>
        /// <paramref name="removeRoomErrors">This removes rooms that don't fit the criteria (Size >= 4)</paramref>
        /// </summary>
        private bool ApplyFloodFill(Cell[,] grid, GridLocation testLocation, bool removeRoomErrors, out Cell[,] roomGrid, out Room room)
        {
            if (grid[testLocation.Column, testLocation.Row] == null)
                throw new Exception("Trying to locate cell that is non-existent:  LayoutGenerator.GetConnectedCells");

            var bounds = new CellRectangle(new GridLocation(0, 0), grid.GetLength(0), grid.GetLength(1));

            // Room 2D Array
            roomGrid = new Cell[bounds.CellWidth, bounds.CellHeight];
            room = null;

            // Room Data
            var roomCells = new List<GridLocation>();
            var edgeCells = new List<GridLocation>();
            var roomBounds = new CellRectangle(testLocation, 1, 1);

            var resultCounter = 0;

            // Use stack to know what cells have been verified. Starting with test cell - continue 
            // until all connected cells have been added to the resulting room.
            var resultStack = new Stack<Cell>(bounds.CellWidth * bounds.CellHeight);

            // Process the first cell
            var testCell = grid[testLocation.Column, testLocation.Row];
            resultStack.Push(testCell);
            roomCells.Add(testCell.Location);
            roomGrid[testLocation.Column, testLocation.Row] = testCell;
            if (grid.IsEdgeCell(testCell))
                edgeCells.Add(testCell.Location);

            resultCounter++;

            while (resultStack.Count > 0)
            {
                var roomCell = resultStack.Pop();

                // Search cardinally adjacent cells (N,S,E,W)
                foreach (var cell in grid.GetCardinalAdjacentCells(roomCell))
                {
                    // Find connected cells that are not yet part of the room
                    if (IsAdjacentCellConnected(grid, roomCell.Location, cell.Location) &&
                        roomGrid[cell.Location.Column, cell.Location.Row] == null)
                    {
                        // Add cell to room immediately to prevent extra cells on stack
                        roomGrid[cell.Location.Column, cell.Location.Row] = cell;

                        // Add cell also to room data
                        roomCells.Add(cell.Location);

                        // Determine whether cell is an edge cell
                        if (grid.IsEdgeCell(cell))
                            edgeCells.Add(cell.Location);

                        // Re-calculate boundary
                        roomBounds.Expand(cell.Location);

                        // Push cell onto the stack to be iterated
                        resultStack.Push(cell);

                        // Increment result counter
                        resultCounter++;
                    }
                }
            }

            // REMOVE ANY ROOM THAT IS SMALLER THAN 4 CELLS. This is chosen so that there's room
            // for an upstairs, downstairs, and 2 teleporters (accomodates all level types).
            if (roomCells.Count < ROOM_SIZE_MIN &&
                removeRoomErrors)
            {
                foreach (var location in roomCells)
                    grid[location.Column, location.Row] = null;

                return false;
            }

            // Assign room data to new room
            room = new Room(roomCells.ToArray(), edgeCells.ToArray(), roomBounds);

            return true;
        }

        /// <summary>
        /// Finds the nearest neighbor room to the supplied room and calculates the cells involved in the
        /// linked corridor. This routine uses a RANDOM cell from the starting room. This was chosen to 
        /// greatly reduce the time involved trying to calculate edges.
        /// </summary>
        private void CalculateNearestNeighborCells(LayoutTemplate template, LevelGrid grid, Room room1, Room room2, out Cell room1Cell, out Cell room2Cell)
        {
            // Procedure
            //
            // Doing an O(n^2) search on the edge cells results in a total time that is computationally significant
            // (maybe as much as a couple seconds). A divide and conquer algorithm may be as good as O(n log n); but
            // the following should suffice.
            //
            // 1) Locate the relative position of room2's bounding rectangle (left, top, right, bottom)
            //
            // 2) For "corner located" (top-left, top-right, bottom-left, bottom-right) begin a search at
            //    the closest neighboring corners and iterate outward along the bounding box edge to find 
            //    the first candidate(s) for each room. Choose these at random for both rooms. (For rectangles
            //    this will be the corners 100% of the time).
            //
            // 3) For cardinally-located (top, left, right, bottom) cases - choose a point from the overlapping
            //    section of the rectangles (at random). Then, starting at room1, search directly upwards (or horizontally) 
            //    until you reach room2. This does not guarantee that the cell will be on the edge each time; but
            //    it will connect the rooms. Also, after finding the overlapping cell, start at the edge of room2
            //    to begin the search.
            //
            // 4) For overlapping or contained bounding boxes - just use a random cell from both rooms' edges. This
            //    could be improved using the O(n^2) brute force algorithm; but it may not be optimal for certain
            //    scenario generations - leaving a poor experience for those cases. (So, a TODO)

            // Have to force O(n^2) search for amorphous room shapes
            var bruteForceSearch = template.Type == LayoutType.ConnectedCellularAutomata ||
                                   (template.Type == LayoutType.ConnectedRectangularRooms &&
                                    template.RoomPlacementType == LayoutRoomPlacementType.Random);

            var room1Bounds = room1.Bounds;
            var room2Bounds = room2.Bounds;

            var top = room2Bounds.Bottom < room1Bounds.Top;
            var bottom = room2Bounds.Top > room1Bounds.Bottom;
            var left = room2Bounds.Right < room1Bounds.Left;
            var right = room2Bounds.Left > room1Bounds.Right;

            var room1Candidates = new List<Cell>();
            var room2Candidates = new List<Cell>();

            // Case 1: Corner-Located
            if (((top && left) ||
                 (top && right) ||
                 (bottom && left) ||
                 (bottom && right)) &&
                 !bruteForceSearch)
            {

                var sign1X = left ? 1 : -1;     // Search for room1-X is in the positive direction (for left-positioned room2)
                var sign1Y = top ? 1 : -1;      // Search for room1-Y is in the positive direction (for top-positioned room2)
                var sign2X = left ? -1 : 1;
                var sign2Y = top ? -1 : 1;
                var startLocation1 = top ?
                                     (left ? room1Bounds.TopLeft : room1Bounds.TopRight) :
                                     (left ? room1Bounds.BottomLeft : room1Bounds.BottomRight);
                var startLocation2 = top ?
                                     (left ? room2Bounds.BottomRight : room2Bounds.BottomLeft) :
                                     (left ? room2Bounds.TopRight : room2Bounds.TopLeft);

                var distance = 0;

                // This loop MUST exit because the bounding rectangle MUST contain at least one
                // cell from the room on each edge.
                while (room1Candidates.Count == 0 ||
                       room2Candidates.Count == 0)
                {
                    if (room1Candidates.Count == 0)
                    {
                        // Calculte the next pair of search cells using the provided parameters from above
                        var cell1X = grid[startLocation1.Column + (sign1X * distance), 
                                          startLocation1.Row];

                        var cell1Y = grid[startLocation1.Column,
                                          startLocation1.Row + (sign1Y * distance)];

                        if (cell1X != null)
                            room1Candidates.Add(cell1X);

                        if (cell1Y != null && cell1Y != cell1X)
                            room1Candidates.Add(cell1Y);
                    }

                    if (room2Candidates.Count == 0)
                    {
                        // Calculte the next pair of search cells using the provided parameters from above
                        var cell2X = grid[startLocation2.Column + (sign2X * distance),
                                           startLocation2.Row];

                        var cell2Y = grid[startLocation2.Column,
                                           startLocation2.Row    + (sign2Y * distance)];

                        if (cell2X != null)
                            room2Candidates.Add(cell2X);

                        if (cell2Y != null && cell2Y != cell2X)
                            room2Candidates.Add(cell2Y);
                    }

                    distance++;
                }
            }

            // Case 2: Edge-Located
            else if ((top || bottom || left || right) && !bruteForceSearch)
            {
                // Vertical
                if (top || bottom)
                {
                    // First, locate the overlap of the two rectangles
                    var overlapLeft = Math.Max(room1Bounds.Left, room2Bounds.Left);
                    var overlapRight = Math.Min(room1Bounds.Right, room2Bounds.Right);

                    var sign1 = top ? 1 : -1;
                    var sign2 = top ? -1 : 1;

                    var column = _randomSequenceGenerator.Get(overlapLeft, overlapRight + 1);
                    var cell1RowStart = top ? room1Bounds.Top : room1Bounds.Bottom;
                    var cell2RowStart = top ? room2Bounds.Bottom : room2Bounds.Top;
                    var distance = 0;

                    // Search from the edges until you reach the first cell
                    while (room1Candidates.Count == 0 ||
                           room2Candidates.Count == 0)
                    {
                        var cell1 = grid[column, cell1RowStart + (sign1 * distance)];
                        var cell2 = grid[column, cell2RowStart + (sign2 * distance)];

                        if (cell1 != null && room1Candidates.Count == 0)
                            room1Candidates.Add(cell1);

                        if (cell2 != null && room2Candidates.Count == 0)
                            room2Candidates.Add(cell2);

                        distance++;
                    }
                }

                // Horizontal
                else
                {
                    // First, locate the overlap of the two rectangles
                    var overlapTop = Math.Max(room1Bounds.Top, room2Bounds.Top);
                    var overlapBottom = Math.Min(room1Bounds.Bottom, room2Bounds.Bottom);

                    var sign1 = left ? 1 : -1;
                    var sign2 = left ? -1 : 1;

                    var row = _randomSequenceGenerator.Get(overlapTop, overlapBottom + 1);
                    var cell1ColumnStart = left ? room1Bounds.Left : room1Bounds.Right;
                    var cell2ColumnStart = left ? room2Bounds.Right : room2Bounds.Left;
                    var distance = 0;

                    // Search from the edges until you reach the first cell
                    while (room1Candidates.Count == 0 ||
                           room2Candidates.Count == 0)
                    {
                        var cell1 = grid[cell1ColumnStart + (sign1 * distance), row];
                        var cell2 = grid[cell2ColumnStart + (sign2 * distance), row];

                        if (cell1 != null && room1Candidates.Count == 0)
                            room1Candidates.Add(cell1);

                        if (cell2 != null && room2Candidates.Count == 0)
                            room2Candidates.Add(cell2);

                        distance++;
                    }
                }
            }
            
            // Case 3: Overlapping / Contained / Brute force
            else
            {
                var minimumDistance = int.MaxValue;

                foreach (var edgeLocation1 in room1.EdgeCells)
                {
                    foreach (var edgeLocation2 in room2.EdgeCells)
                    {
                        var distance = Calculator.RoguianDistance(edgeLocation1, edgeLocation2);

                        // Reset candidates
                        if (distance < minimumDistance)
                        {
                            minimumDistance = distance;

                            room1Candidates.Clear();
                            room2Candidates.Clear();

                            room1Candidates.Add(grid[edgeLocation1.Column, edgeLocation1.Row]);
                            room2Candidates.Add(grid[edgeLocation2.Column, edgeLocation2.Row]);
                        }

                        // Add to candidates
                        else if (distance == minimumDistance)
                        {
                            room1Candidates.Add(grid[edgeLocation1.Column, edgeLocation1.Row]);
                            room2Candidates.Add(grid[edgeLocation2.Column, edgeLocation2.Row]);
                        }
                    }
                }
            }

            // Assign a random cell from the candidates
            var location1 = _randomSequenceGenerator.GetRandomElement(room1Candidates);
            var location2 = _randomSequenceGenerator.GetRandomElement(room2Candidates);

            // Assign out variables
            room1Cell = grid[location1.Location.Column, location1.Location.Row];
            room2Cell = grid[location2.Location.Column, location2.Location.Row];
        }
        #endregion

        #region (private) Maze Creation       
        private LevelGrid CreateMaze(LayoutTemplate template)
        {
            var bounds = new CellRectangle(new GridLocation(0, 0), template.Width, template.Height);
            var grid = new Cell[bounds.CellWidth, bounds.CellHeight];

            // Initialize starting cell
            List<Cell> history = new List<Cell>();
            Compass tried = Compass.Null;
            grid[0, 0] = new Cell(0, 0, Compass.N | Compass.S | Compass.E | Compass.W);

            for (int i = 0; i < bounds.CellWidth; i++)
            {
                for (int j = 0; j < bounds.CellHeight; j++)
                {
                    var cell = grid[i, j];

                    // Create new cell if null
                    if (cell == null)
                    {
                        cell = new Cell(i, j, Compass.N | Compass.S | Compass.E | Compass.W);
                        grid[i, j] = cell;
                    }

                    history.Clear();
                    history.Add(cell);

                    //Main loop - create the maze!
                    while (history.Count > 0)
                    {
                        var random = _randomSequenceGenerator.Get();

                        if ((int)tried == 15)
                        {
                            if (history.Count == 1)
                                break;

                            cell = history[history.Count - 2];
                            history.RemoveAt(history.Count - 1);
                            tried = (Compass)(((int)(~cell.Walls) & 0x0000000f));
                        }

                        //N
                        if (random < 0.25 && (tried & Compass.N) == 0)
                        {
                            if (cell.Location.Row - 1 < 0)
                            {
                                tried |= Compass.N;
                                continue;
                            }

                            if (grid[cell.Location.Column, cell.Location.Row - 1] == null)
                            {
                                Cell next = new Cell(cell.Location.Column, cell.Location.Row - 1, Compass.N | Compass.E | Compass.W);

                                //Remove walls
                                cell.Walls &= ~Compass.N;
                                next.Walls &= ~Compass.S;

                                grid[next.Location.Column, next.Location.Row] = next;
                                history.Add(next);
                                cell = next;
                                tried = (Compass)(((int)(~cell.Walls) & 0x0000000f));
                            }
                            else
                                tried |= Compass.N;
                        }
                        //S
                        else if (random < 0.5 && (tried & Compass.S) == 0)
                        {
                            if (cell.Location.Row + 1 >= bounds.CellHeight)
                            {
                                tried |= Compass.S;
                                continue;
                            }

                            if (grid[cell.Location.Column, cell.Location.Row + 1] == null)
                            {
                                Cell next = new Cell(cell.Location.Column, cell.Location.Row + 1, Compass.S | Compass.E | Compass.W);

                                //Remove Walls
                                cell.Walls &= ~Compass.S;
                                next.Walls &= ~Compass.N;

                                grid[next.Location.Column, next.Location.Row] = next;
                                history.Add(next);
                                cell = next;
                                tried = (Compass)(((int)(~cell.Walls) & 0x0000000f));
                            }
                            else
                                tried |= Compass.S;
                        }
                        //E
                        else if (random < 0.75 && (tried & Compass.E) == 0)
                        {
                            if (cell.Location.Column + 1 >= bounds.CellWidth)
                            {
                                tried |= Compass.E;
                                continue;
                            }

                            if (grid[cell.Location.Column + 1, cell.Location.Row] == null)
                            {
                                Cell next = new Cell(cell.Location.Column + 1, cell.Location.Row, Compass.N | Compass.S | Compass.E);

                                //Remove Walls
                                cell.Walls &= ~Compass.E;
                                next.Walls = ~Compass.W;

                                grid[next.Location.Column, next.Location.Row] = next;
                                history.Add(next);
                                cell = next;
                                tried = (Compass)(((int)(~cell.Walls) & 0x0000000f));
                            }
                            else
                                tried |= Compass.E;
                        }
                        //W
                        else if ((tried & Compass.W) == 0)
                        {
                            if (cell.Location.Column - 1 < 0)
                            {
                                tried |= Compass.W;
                                continue;
                            }

                            if (grid[cell.Location.Column - 1, cell.Location.Row] == null)
                            {
                                Cell next = new Cell(cell.Location.Column - 1, cell.Location.Row, Compass.N | Compass.S | Compass.W);

                                //Remove walls
                                cell.Walls &= ~Compass.W;
                                next.Walls &= ~Compass.E;

                                grid[next.Location.Column, next.Location.Row] = next;
                                history.Add(next);
                                cell = next;
                                tried = (Compass)(((int)(~cell.Walls) & 0x0000000f));
                            }
                            else
                                tried |= Compass.W;
                        }
                    }
                }
            }
            history.Clear();

            // Store room data
            var roomCells = new List<GridLocation>();
            var edgeCells = new List<GridLocation>();
            var roomBounds = new CellRectangle(new GridLocation(0, 0), bounds.CellWidth, bounds.CellHeight);

            for (int i = 0; i < bounds.CellWidth; i++)
            {
                for (int j = 0; j < bounds.CellHeight; j++)
                {
                    if (grid[i, j] != null)
                    {
                        history.Add(grid[i, j]);

                        // Room data
                        roomCells.Add(grid[i, j].Location);

                        // Room edges (entire grid is one room)
                        if (i == 0 ||
                            j == 0 ||
                            i == bounds.CellWidth - 1 ||
                            j == bounds.CellHeight - 1)
                            edgeCells.Add(grid[i, j].Location);
                    }
                }
            }

            //Make Maze easier..
            for (int i = 0; i < template.NumberExtraWallRemovals; i++)
            {
                double d = _randomSequenceGenerator.Get();
                if (d < 0.25)
                {
                    Cell ce = history[_randomSequenceGenerator.Get(0, history.Count)];
                    if (ce.Location.Row - 1 >= 0)
                    {
                        grid[ce.Location.Column, ce.Location.Row - 1].Walls &= ~Compass.S;
                        ce.Walls &= ~Compass.N;
                    }
                }
                else if (d < 0.5)
                {
                    Cell ce = history[_randomSequenceGenerator.Get(0, history.Count)];
                    if (ce.Location.Row + 1 < bounds.CellHeight)
                    {
                        grid[ce.Location.Column, ce.Location.Row + 1].Walls &= ~Compass.N;
                        ce.Walls &= ~Compass.S;
                    }
                }
                else if (d < 0.75)
                {
                    Cell ce = history[_randomSequenceGenerator.Get(0, history.Count)];
                    if (ce.Location.Column + 1 < bounds.CellWidth)
                    {
                        grid[ce.Location.Column + 1, ce.Location.Row].Walls &= ~Compass.W;
                        ce.Walls &= ~Compass.E;
                    }
                }
                else
                {
                    Cell ce = history[_randomSequenceGenerator.Get(0, history.Count)];
                    if (ce.Location.Column - 1 > 0)
                    {
                        grid[ce.Location.Column - 1, ce.Location.Row].Walls &= ~Compass.E;
                        ce.Walls &= ~Compass.W;
                    }
                }
            }
            return new LevelGrid(grid, new Room[] { new Room(roomCells.ToArray(), edgeCells.ToArray(), roomBounds) });
        }
        #endregion

        #region (private) Layout Finishing
        private void CreateWalls(LevelGrid grid)
        {
            var bounds = grid.Bounds;

            // *** For the Maze layout - there's no padding around the outside.
            for (int i=0;i<bounds.CellWidth;i++)
            {
                for (int j=0;j<bounds.CellHeight;j++)
                {
                    if (grid[i, j] == null)
                        continue;

                    // North wall
                    if ((j - 1 >= 0) && grid[i, j - 1] == null)
                        grid[i, j].Walls |= Compass.N;

                    // South wall
                    if ((j + 1 < bounds.CellHeight) && grid[i, j + 1] == null)
                        grid[i, j].Walls |= Compass.S;

                    // West wall
                    if ((i - 1 >= 0) && grid[i - 1, j] == null)
                        grid[i, j].Walls |= Compass.W;

                    // East wall
                    if ((i + 1 < bounds.CellWidth) && grid[i + 1, j] == null)
                        grid[i, j].Walls |= Compass.E;
                }
            }
        }
        #endregion
    }
}
