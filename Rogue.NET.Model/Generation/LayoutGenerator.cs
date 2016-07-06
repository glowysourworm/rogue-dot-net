using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common;
using Rogue.NET.Scenario;
using Rogue.NET.Scenario.Model;
using Rogue.NET.Model;
using Rogue.NET.Model.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Rogue.NET.Model.Generation
{
    public class ScenarioLayoutGenerator : GeneratorBase
    {
        public ScenarioLayoutGenerator(IEventAggregator eventAggregator) : base(eventAggregator)
        {
        }
        public List<Level> CreateDungeonLayouts(ScenarioConfiguration config, Random rand)
        {
            if (config.DungeonTemplate.LayoutTemplates.Count <= 0)
                return new List<Level>();

            //layout percentage divided by 2
            double progress = 0;

            List<Level> layouts = new List<Level>();
            for (int i = 0; i < config.DungeonTemplate.NumberOfLevels; i++)
            {
                // Update progress
                progress = i / config.DungeonTemplate.NumberOfLevels / 2D;
                base.PublishLoadingMessage("Generating Scenario Level Layouts", progress);

                //Calucate list of templates that contain this level in their range
                List<LayoutTemplate> listInRange = new List<LayoutTemplate>();
                foreach (LayoutTemplate t in config.DungeonTemplate.LayoutTemplates)
                {
                    if (t.Level.Contains(i + 1))
                        listInRange.Add(t);
                }

                //Calculate sum of the weights
                double sumWeights = 0;
                List<double> weightList = new List<double>();
                foreach (LayoutTemplate t in listInRange)
                {
                    sumWeights += t.GenerationRate;
                    weightList.Add(t.GenerationRate);
                }

                //If not set, then set to 1
                if (sumWeights <= 0)
                    sumWeights = 1;

                //Normalize the weights
                for (int j = 0; j < weightList.Count; j++)
                    weightList[j] /= sumWeights;

                //Draw Uniform[0,1] and sum weights until it is reached.
                //Chose that index
                double d = rand.NextDouble();
                double cumulativeWeight = 0;
                for (int j = 0; j < weightList.Count; j++)
                {
                    cumulativeWeight += weightList[j];
                    if (cumulativeWeight >= d || weightList.Count == 1)
                    {
                        var layout = CreateLayout(listInRange[j], rand);
                        layout.Number = i + 1;
                        layouts.Add(layout);
                        break;
                    }
                }
            }
            return layouts;
        }
        private Level CreateLayout(LayoutTemplate t, Random rand)
        {
            LevelLayoutGenerator g = new LevelLayoutGenerator(t, rand);
            Level layout = g.CreateLayout();
            g = null;
            return layout;
        }
    }

    /// <summary>
    /// Object to manage creation of one Level Layout. 
    /// 
    /// Procedure:
    /// - Create Room Division Grid 
    /// - Create Room Rectangles 
    /// - Create Cells for those rooms
    /// - Connect Rooms
    /// - Create Cells for those halls
    /// - Find Doors
    /// - Set Walls / Doors for each cell accordingly - duplicates will exist for each wall and must
    /// be dealt with accordingly
    /// </summary>
    public class LevelLayoutGenerator
    {
        //Layout Components------
        LevelGrid _grid = null;
        //-----------------------

        LayoutTemplate _template = null;
        Random _rand = null;
        public LevelLayoutGenerator(LayoutTemplate template, Random r)
        {
            _template = template;
            _rand = r;
        }
        public Level CreateLayout()
        {
            _grid = new LevelGrid(
                _template.NumberRoomCols * (_template.RoomDivCellWidth + 4),
                _template.NumberRoomRows * (_template.RoomDivCellHeight + 4),
                _template.NumberRoomCols,
                _template.NumberRoomRows);

            CreateRoomGrid();
            switch (_template.Type)
            {
                case LayoutType.Normal:
                case LayoutType.Shop:
                    ConnectRooms(true);
                    //EdgeCells();
                    CreateRoomCells();
                    EdgeCells(true);
                    FinishDoors();
                    break;
                case LayoutType.Teleport:
                case LayoutType.TeleportRandom:
                    CreateRoomCells();
                    EdgeCells(false);
                    break;
                case LayoutType.Maze:
                    CreateMaze(_grid, _rand, _template.NumberExtraWallRemovals);
                    //EdgeCells(false);
                    break;
                case LayoutType.Hall:
                    {
                        ConnectRooms(true);
                        Cell[] doors = _grid.GetDoors();
                        //Connect doors together
                        for (int i = 0; i < doors.Length - 1; i++)
                        {
                            CreateHall(doors[i].Location, doors[i + 1].Location);
                            doors[i].Doors = Compass.Null;
                            if (i == doors.Length - 2)
                                doors[i + 1].Doors = Compass.Null;
                        }
                        EdgeCells(false);
                    }
                    break;
                case LayoutType.BigRoom:
                    {
                        CreateBigRoom();
                        ConnectRooms(true);
                        //EdgeCells();
                        CreateRoomCells();
                        EdgeCells(true);
                        FinishDoors();
                    }
                    break;
            }
            return new Level(_grid, _template.Type, 0);
        }
        private void CreateRoomGrid()
        {
            for (int i = 0; i < _template.NumberRoomCols; i++)
            {
                for (int j = 0; j < _template.NumberRoomRows; j++)
                {
                    int cellColLoc = _rand.Next(1, _template.RoomDivCellWidth - 2);
                    int cellRowLoc = _rand.Next(1, _template.RoomDivCellHeight - 2);
                    int cellCol = cellColLoc + (i * _template.RoomDivCellWidth + 1);
                    int cellRow = cellRowLoc + (j * _template.RoomDivCellHeight + 1);
                    int width = _rand.Next(2, _template.RoomDivCellWidth - cellColLoc);
                    int height = _rand.Next(2, _template.RoomDivCellHeight - cellRowLoc);

                    CellRectangle r = new CellRectangle(new CellPoint(cellRow, cellCol), width, height);
                    _grid.SetRoom(r, i, j);
                }
            }
        }
        private void CreateBigRoom()
        {
            int i = _rand.Next(0, _grid.GetRoomGridWidth());
            int j = _rand.Next(0, _grid.GetRoomGridHeight());
            CellRectangle r1 = _grid.GetRoom(i, j);
            CellRectangle r2 = null;
            int n = i;
            int m = j;
            while (r2 == null)
            {
                double d = _rand.NextDouble();
                if (d > 0.25 && j < _grid.GetRoomGridHeight() - 1)
                    r2 = _grid.GetRoom(i, ++j);
                else if (d > 0.5 && j > 0)
                    r2 = _grid.GetRoom(i, --j);
                else if (d > 0.75 && i < _grid.GetRoomGridWidth() - 1)
                    r2 = _grid.GetRoom(++i, j);
                else if (i > 0)
                    r2 = _grid.GetRoom(--i, j);
            }
            CellRectangle r = CellRectangle.Join(r1, r2);
            _grid.SetRoom(r, i, j);
            _grid.SetRoom(r, n, m);
        }
        private void ConnectRooms(bool createDoors)
        {
            //Connect N to S
            for (int i = 0; i < _grid.GetRoomGridWidth(); i++)
            {
                for (int j = 1; j < _grid.GetRoomGridHeight(); j++)
                {
                    CellRectangle r = _grid.GetRoom(i, j - 1);
                    CellRectangle r2 = _grid.GetRoom(i, j);

                    //if big room
                    if (r.Equals(r2))
                        continue;

                    int doorEndCol = _rand.Next(r2.CellWidth) + r2.Location.Column;
                    int doorEndRow = r2.Location.Row - 1;
                    int doorStartCol = _rand.Next(r.CellWidth) + r.Location.Column;
                    int doorStartRow = r.CellHeight + r.Location.Row;
                    CreateHall(new CellPoint(doorStartRow, doorStartCol), new CellPoint(doorEndRow, doorEndCol));
                    if (createDoors)
                    {
                        Cell door1 = _grid.GetCell(doorStartCol, doorStartRow);
                        Cell door2 = _grid.GetCell(doorEndCol, doorEndRow);
                        door1.Doors |= Compass.N;
                        door2.Doors |= Compass.S;
                    }
                }
            }

            //Connect W to E
            for (int i = 1; i < _grid.GetRoomGridWidth(); i++)
            {
                for (int j = 0; j < _grid.GetRoomGridHeight(); j++)
                {
                    CellRectangle r = _grid.GetRoom(i - 1, j);
                    CellRectangle r2 = _grid.GetRoom(i, j);

                    //if big room
                    if (r.Equals(r2))
                        continue;

                    int doorEndRow = _rand.Next(r2.Top, r2.Bottom);
                    int doorEndCol = r2.Location.Column - 1;
                    int doorStartRow = _rand.Next(r.Top, r.Bottom);
                    int doorStartCol = r.Right;
                    CreateHall(new CellPoint(doorStartRow, doorStartCol), new CellPoint(doorEndRow, doorEndCol));
                    if (createDoors)
                    {
                        Cell door1 = _grid.GetCell(doorStartCol, doorStartRow);
                        Cell door2 = _grid.GetCell(doorEndCol, doorEndRow);
                        door1.Doors |= Compass.W;
                        door2.Doors |= Compass.E;
                    }
                }
            }
        }
        private void CreateHall(CellPoint d1, CellPoint d2)
        {
            int midCol = (d1.Column + d2.Column) / 2;
            int midRow = (d1.Row + d2.Row) / 2;
            int col = d1.Column;
            int row = d1.Row;
            bool b = (_rand.NextDouble() > 0.5);

            if (b)
            {
                while (col < midCol)
                    _grid.AddCell(new Cell(col++, row, Compass.Null));
                while (row < midRow)
                    _grid.AddCell(new Cell(col, row++, Compass.Null));
                while (col > midCol)
                    _grid.AddCell(new Cell(col--, row, Compass.Null));
                while (row > midRow)
                    _grid.AddCell(new Cell(col, row--, Compass.Null));
                while (col < d2.Column)
                    _grid.AddCell(new Cell(col++, row, Compass.Null));
                while (row < d2.Row)
                    _grid.AddCell(new Cell(col, row++, Compass.Null));
                while (col > d2.Column)
                    _grid.AddCell(new Cell(col--, row, Compass.Null));
                while (row > d2.Row)
                    _grid.AddCell(new Cell(col, row--, Compass.Null));
            }
            else
            {
                while (row < midRow)
                    _grid.AddCell(new Cell(col, row++, Compass.Null));
                while (col < midCol)
                    _grid.AddCell(new Cell(col++, row, Compass.Null));
                while (row > midRow)
                    _grid.AddCell(new Cell(col, row--, Compass.Null));
                while (col > midCol)
                    _grid.AddCell(new Cell(col--, row, Compass.Null));
                while (row < d2.Row)
                    _grid.AddCell(new Cell(col, row++, Compass.Null));
                while (col < d2.Column)
                    _grid.AddCell(new Cell(col++, row, Compass.Null));
                while (row > d2.Row)
                    _grid.AddCell(new Cell(col, row--, Compass.Null));
                while (col > d2.Column)
                    _grid.AddCell(new Cell(col--, row, Compass.Null));
            }

            //TODO - finish creating doors
            Cell c1 = new Cell(d1.Column, d1.Row, Compass.Null);
            Cell c2 = new Cell(d2.Column, d2.Row, Compass.Null);



            _grid.AddCell(c1);
            _grid.AddCell(c2);
        }

        /// <summary>
        /// Creates Walls for cells in the rooms - does NOT take into account adjoining halls.
        /// Must further process the grid once room cells are created
        /// </summary>
        private void CreateRoomCells()
        {
            //Create Room Cells
            for (int ir = 0; ir < _grid.GetRoomGridWidth(); ir++)
            {
                for (int jr = 0; jr < _grid.GetRoomGridHeight(); jr++)
                {
                    CellRectangle r = _grid.GetRoom(ir, jr);
                    for (int i = r.Left; i < r.Right; i++)
                    {
                        for (int j = r.Top; j < r.Bottom; j++)
                        {
                            if (_grid.GetCell(i, j) == null)
                                _grid.AddCell(new Cell(i, j, Compass.Null));
                        }
                    }
                }
            }
        }
        private void EdgeCells(bool createRoomEdges)
        {
            //Create room walls first to make sure adjoining halls will be separated from the rooms
            for (int ri = 0; ri < _grid.GetRoomGridWidth() && createRoomEdges; ri++)
            {
                for (int rj = 0; rj < _grid.GetRoomGridHeight(); rj++)
                {
                    CellRectangle room = _grid.GetRoom(ri, rj);

                    //Top edge / Bottom edge
                    for (int ci = room.Left; ci < room.Right; ci++)
                    {
                        if (!_grid[ci, room.Top].IsDoor)
                            _grid[ci, room.Top].Walls |= Compass.N;

                        if (!_grid[ci, room.Bottom - 1].IsDoor)
                            _grid[ci, room.Bottom - 1].Walls |= Compass.S;
                    }

                    //Left edge / Right edge
                    for (int cj = room.Top; cj < room.Bottom; cj++)
                    {
                        if (!_grid[room.Left, cj].IsDoor)
                            _grid[room.Left, cj].Walls |= Compass.W;

                        if (!_grid[room.Right - 1, cj].IsDoor)
                            _grid[room.Right - 1, cj].Walls |= Compass.E;
                    }
                }
            }

            //set walls
            for (int i = 0; i < _grid.GetBounds().Right; i++)
            {
                for (int j = 0; j < _grid.GetBounds().Bottom; j++)
                {
                    if (_grid[i, j] == null)
                        continue;

                    //grid[i, j].Walls = Compass.Null;

                    if (i - 1 > 0)
                    {
                        if (_grid[i - 1, j] == null)
                            _grid[i, j].Walls |= Compass.W;
                        else if ((_grid[i - 1, j].Walls & Compass.E) != 0)
                            _grid[i, j].Walls |= Compass.W;
                    }
                    if (i + 1 < _grid.GetBounds().Right)
                    {
                        if (_grid[i + 1, j] == null)
                            _grid[i, j].Walls |= Compass.E;
                        else if ((_grid[i + 1, j].Walls & Compass.W) != 0)
                            _grid[i, j].Walls |= Compass.E;
                    }
                    if (i - 1 == 0)
                        _grid[i, j].Walls |= Compass.W;
                    if (i + 1 == _grid.GetBounds().Right)
                        _grid[i, j].Walls |= Compass.E;

                    if (j - 1 > 0)
                    {
                        if (_grid[i, j - 1] == null)
                            _grid[i, j].Walls |= Compass.N;
                        else if ((_grid[i, j - 1].Walls & Compass.S) != 0)
                            _grid[i, j].Walls |= Compass.N;
                    }
                    if (j + 1 < _grid.GetBounds().Bottom)
                    {
                        if (_grid[i, j + 1] == null)
                            _grid[i, j].Walls |= Compass.S;
                        else if ((_grid[i, j + 1].Walls & Compass.N) != 0)
                            _grid[i, j].Walls |= Compass.S;
                    }
                    if (j - 1 == 0)
                        _grid[i, j].Walls |= Compass.N;
                    if (j + 1 == _grid.GetBounds().Bottom)
                        _grid[i, j].Walls |= Compass.S;
                }
            }
        }
        private void FinishDoors()
        {
            //set walls
            for (int i = 0; i < _grid.GetBounds().Right; i++)
            {
                for (int j = 0; j < _grid.GetBounds().Bottom; j++)
                {
                    if (_grid[i, j] == null)
                        continue;

                    //Remove walls that overlap the door
                    //Set search counters at random
                    if (_grid[i, j].IsDoor)
                    {
                        _grid[i, j].Walls &= _grid[i, j].Walls & ~_grid[i, j].Doors;
                        Compass d = _grid[i, j].Doors;
                        if ((d & Compass.N) != 0 && _grid[i, j - 1] != null)
                        {
                            int ctr = _grid[i, j].NorthDoorSearchCounter;
                            if (ctr == 0 && _rand.NextDouble() < _template.HiddenDoorProbability)
                            {
                                ctr = _rand.Next(3, 20);
                                _grid[i, j].NorthDoorSearchCounter = ctr;
                            }

                            _grid[i, j - 1].Walls &= ~Compass.S;
                            _grid[i, j - 1].Doors |= Compass.S;
                            _grid[i, j - 1].SouthDoorSearchCounter = ctr;
                        }
                        if ((d & Compass.S) != 0 && _grid[i, j + 1] != null)
                        {
                            int ctr = _grid[i, j].SouthDoorSearchCounter;
                            if (ctr == 0 && _rand.NextDouble() < _template.HiddenDoorProbability)
                            {
                                ctr = _rand.Next(3, 20);
                                _grid[i, j].SouthDoorSearchCounter = ctr;
                            }

                            _grid[i, j + 1].Walls &= ~Compass.N;
                            _grid[i, j + 1].Doors |= Compass.N;
                            _grid[i, j + 1].NorthDoorSearchCounter = ctr;
                        }
                        if ((d & Compass.E) != 0 && _grid[i + 1, j] != null)
                        {
                            int ctr = _grid[i, j].EastDoorSearchCounter;
                            if (ctr == 0 && _rand.NextDouble() < _template.HiddenDoorProbability)
                            {
                                ctr = _rand.Next(3, 20);
                                _grid[i, j].EastDoorSearchCounter = ctr;
                            }

                            _grid[i + 1, j].Walls &= ~Compass.W;
                            _grid[i + 1, j].Doors |= Compass.W;
                            _grid[i + 1, j].WestDoorSearchCounter = ctr;
                        }
                        if ((d & Compass.W) != 0 && _grid[i - 1, j] != null)
                        {
                            int ctr = _grid[i, j].WestDoorSearchCounter;
                            if (ctr == 0 && _rand.NextDouble() < _template.HiddenDoorProbability)
                            {
                                ctr = _rand.Next(3, 20);
                                _grid[i, j].WestDoorSearchCounter = ctr;
                            }

                            _grid[i - 1, j].Walls &= ~Compass.E;
                            _grid[i - 1, j].Doors |= Compass.E;
                            _grid[i - 1, j].EastDoorSearchCounter = ctr;
                        }
                    }
                }
            }
        }
        private void CreateMaze(LevelGrid grid, Random rand, int numRemovals)
        {
            List<Cell> history = new List<Cell>();
            Compass tried = Compass.Null;
            grid[0, 0] = new Cell(0, 0, Compass.N | Compass.S | Compass.E | Compass.W);
            CellRectangle bnds = grid.GetBounds();

            for (int i = 0; i < bnds.CellWidth; i++)
            {
                for (int j = 0; j < bnds.CellHeight; j++)
                {
                    Cell s = grid[i, j];

                    if (s == null)
                    {
                        s = new Cell(i, j, Compass.N | Compass.S | Compass.E | Compass.W);
                        grid[i, j] = s;
                    }

                    history.Clear();
                    history.Add(s);

                    //Main loop - create the maze!
                    while (history.Count > 0)
                    {
                        double d = rand.NextDouble();

                        if ((int)tried == 15)
                        {
                            if (history.Count == 1)
                                break;

                            s = history[history.Count - 2];
                            history.RemoveAt(history.Count - 1);
                            tried = (Compass)(((int)(~s.Walls) & 0x0000000f));
                        }

                        //N
                        if (d < 0.25 && (tried & Compass.N) == 0)
                        {
                            if (s.Location.Row - 1 < 0)
                            {
                                tried |= Compass.N;
                                continue;
                            }

                            if (grid[s.Location.Column, s.Location.Row - 1] == null)
                            {
                                Cell next = new Cell(s.Location.Column, s.Location.Row - 1, Compass.N | Compass.E | Compass.W);

                                //Remove walls
                                s.Walls &= ~Compass.N;
                                next.Walls &= ~Compass.S;

                                grid[next.Location.Column, next.Location.Row] = next;
                                history.Add(next);
                                s = next;
                                tried = (Compass)(((int)(~s.Walls) & 0x0000000f));
                            }
                            else
                                tried |= Compass.N;
                        }
                        //S
                        else if (d < 0.5 && (tried & Compass.S) == 0)
                        {
                            if (s.Location.Row + 1 >= bnds.CellHeight)
                            {
                                tried |= Compass.S;
                                continue;
                            }

                            if (grid[s.Location.Column, s.Location.Row + 1] == null)
                            {
                                Cell next = new Cell(s.Location.Column, s.Location.Row + 1, Compass.S | Compass.E | Compass.W);

                                //Remove Walls
                                s.Walls &= ~Compass.S;
                                next.Walls &= ~Compass.N;

                                grid[next.Location.Column, next.Location.Row] = next;
                                history.Add(next);
                                s = next;
                                tried = (Compass)(((int)(~s.Walls) & 0x0000000f));
                            }
                            else
                                tried |= Compass.S;
                        }
                        //E
                        else if (d < 0.75 && (tried & Compass.E) == 0)
                        {
                            if (s.Location.Column + 1 >= bnds.CellWidth)
                            {
                                tried |= Compass.E;
                                continue;
                            }

                            if (grid[s.Location.Column + 1, s.Location.Row] == null)
                            {
                                Cell next = new Cell(s.Location.Column + 1, s.Location.Row, Compass.N | Compass.S | Compass.E);

                                //Remove Walls
                                s.Walls &= ~Compass.E;
                                next.Walls = ~Compass.W;

                                grid[next.Location.Column, next.Location.Row] = next;
                                history.Add(next);
                                s = next;
                                tried = (Compass)(((int)(~s.Walls) & 0x0000000f));
                            }
                            else
                                tried |= Compass.E;
                        }
                        //W
                        else if ((tried & Compass.W) == 0)
                        {
                            if (s.Location.Column - 1 < 0)
                            {
                                tried |= Compass.W;
                                continue;
                            }

                            if (grid[s.Location.Column - 1, s.Location.Row] == null)
                            {
                                Cell next = new Cell(s.Location.Column - 1, s.Location.Row, Compass.N | Compass.S | Compass.W);

                                //Remove walls
                                s.Walls &= ~Compass.W;
                                next.Walls &= ~Compass.E;

                                grid[next.Location.Column, next.Location.Row] = next;
                                history.Add(next);
                                s = next;
                                tried = (Compass)(((int)(~s.Walls) & 0x0000000f));
                            }
                            else
                                tried |= Compass.W;
                        }
                    }
                }
            }
            history.Clear();
            for (int i = 0; i < bnds.CellWidth; i++)
            {
                for (int j = 0; j < bnds.CellHeight; j++)
                {
                    if (grid[i, j] != null)
                        history.Add(grid[i, j]);
                }
            }

            //Make Maze easier..
            for (int i = 0; i < numRemovals; i++)
            {
                double d = rand.NextDouble();
                if (d < 0.25)
                {
                    Cell ce = history[rand.Next(0, history.Count)];
                    if (ce.Location.Row - 1 >= 0)
                    {
                        grid[ce.Location.Column, ce.Location.Row - 1].Walls &= ~Compass.S;
                        ce.Walls &= ~Compass.N;
                    }
                }
                else if (d < 0.5)
                {
                    Cell ce = history[rand.Next(0, history.Count)];
                    if (ce.Location.Row + 1 < bnds.CellHeight)
                    {
                        grid[ce.Location.Column, ce.Location.Row + 1].Walls &= ~Compass.N;
                        ce.Walls &= ~Compass.S;
                    }
                }
                else if (d < 0.75)
                {
                    Cell ce = history[rand.Next(0, history.Count)];
                    if (ce.Location.Column + 1 < bnds.CellWidth)
                    {
                        grid[ce.Location.Column + 1, ce.Location.Row].Walls &= ~Compass.W;
                        ce.Walls &= ~Compass.E;
                    }
                }
                else
                {
                    Cell ce = history[rand.Next(0, history.Count)];
                    if (ce.Location.Column - 1 > 0)
                    {
                        grid[ce.Location.Column - 1, ce.Location.Row].Walls &= ~Compass.E;
                        ce.Walls &= ~Compass.W;
                    }
                }
            }
        }
        private void GetUnvisitedCell(bool[,] grid, out int x, out int y)
        {
            x = -1;
            y = -1;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (!grid[i, j])
                    {
                        x = i;
                        y = j;
                        return;
                    }
                }
            }
        }
    }
}
