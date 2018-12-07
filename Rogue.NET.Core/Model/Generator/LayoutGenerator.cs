using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(ILayoutGenerator))]
    public class LayoutGenerator : ILayoutGenerator
    {
        private readonly IRandomSequenceGenerator _randomSequenceGenerator;

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

                //Calculate sum of the weights
                var sumWeights = layoutTemplates.Sum(y => y.GenerationRate);

                if (sumWeights <= 0)
                    sumWeights = 1;

                var weights = layoutTemplates.Select(x => x.GenerationRate / sumWeights)
                                             .ToList();

                //Draw Uniform[0,1] and sum weights until it is reached.
                //Chose that index
                var random = _randomSequenceGenerator.Get();
                var cumulativeWeight = 0.0D;

                for (int j = 0; j < weights.Count; j++)
                {
                    cumulativeWeight += weights[j];
                    if (cumulativeWeight >= random || weights.Count == 1)
                    {
                        var layout = CreateLayout(layoutTemplates[j], levelNumber);
                        levels.Add(layout);
                        break;
                    }
                }
            }
            return levels;
        }
        private Level CreateLayout(LayoutTemplate template, int levelNumber)
        {
            var grid = new LevelGrid(
                        template.NumberRoomCols * (template.RoomDivCellWidth + 4),
                        template.NumberRoomRows * (template.RoomDivCellHeight + 4),
                        template.NumberRoomCols,
                        template.NumberRoomRows);

            // ***This is (unfortunately) required to make the LevelGrid function (for now)
            CreateRoomGrid(grid, template);
            switch (template.Type)
            {
                case LayoutType.Normal:
                    ConnectRooms(grid, true);
                    CreateRoomCells(grid);
                    EdgeCells(grid, true);
                    FinishDoors(grid, template);
                    break;
                case LayoutType.Teleport:
                case LayoutType.TeleportRandom:
                    CreateRoomCells(grid);
                    EdgeCells(grid, false);
                    break;
                case LayoutType.Maze:
                    CreateMaze(grid, template.NumberExtraWallRemovals);
                    break;
                case LayoutType.Hall:
                    CreateHallwayLayout(grid, template);
                    EdgeCells(grid, false);
                    break;
                case LayoutType.BigRoom:
                    CreateBigRoom(grid);
                    ConnectRooms(grid, true);
                    CreateRoomCells(grid);
                    EdgeCells(grid, true);
                    FinishDoors(grid, template);
                    break;
            }
            return new Level(grid, template.Type, levelNumber, template.WallColor, template.DoorColor);
        }
        private void CreateRoomGrid(LevelGrid grid, LayoutTemplate template)
        {
            for (int i = 0; i < template.NumberRoomCols; i++)
            {
                for (int j = 0; j < template.NumberRoomRows; j++)
                {
                    int cellColLoc = _randomSequenceGenerator.Get(1, template.RoomDivCellWidth - 2);
                    int cellRowLoc = _randomSequenceGenerator.Get(1, template.RoomDivCellHeight - 2);
                    int cellCol = cellColLoc + (i * template.RoomDivCellWidth + 1);
                    int cellRow = cellRowLoc + (j * template.RoomDivCellHeight + 1);
                    int width = _randomSequenceGenerator.Get(2, template.RoomDivCellWidth - cellColLoc);
                    int height = _randomSequenceGenerator.Get(2, template.RoomDivCellHeight - cellRowLoc);

                    CellRectangle r = new CellRectangle(new CellPoint(cellRow, cellCol), width, height);
                    grid.SetRoom(r, i, j);
                }
            }
        }
        private void CreateBigRoom(LevelGrid grid)
        {
            int i = _randomSequenceGenerator.Get(0, grid.GetRoomGridWidth());
            int j = _randomSequenceGenerator.Get(0, grid.GetRoomGridHeight());
            CellRectangle r1 = grid.GetRoom(i, j);
            CellRectangle r2 = null;
            int n = i;
            int m = j;
            while (r2 == null)
            {
                double d = _randomSequenceGenerator.Get();
                if (d > 0.25 && j < grid.GetRoomGridHeight() - 1)
                    r2 = grid.GetRoom(i, ++j);
                else if (d > 0.5 && j > 0)
                    r2 = grid.GetRoom(i, --j);
                else if (d > 0.75 && i < grid.GetRoomGridWidth() - 1)
                    r2 = grid.GetRoom(++i, j);
                else if (i > 0)
                    r2 = grid.GetRoom(--i, j);
            }
            CellRectangle r = CellRectangle.Join(r1, r2);
            grid.SetRoom(r, i, j);
            grid.SetRoom(r, n, m);
        }
        private void ConnectRooms(LevelGrid grid, bool createDoors)
        {
            //Connect N to S
            for (int i = 0; i < grid.GetRoomGridWidth(); i++)
            {
                for (int j = 1; j < grid.GetRoomGridHeight(); j++)
                {
                    CellRectangle r = grid.GetRoom(i, j - 1);
                    CellRectangle r2 = grid.GetRoom(i, j);

                    //if big room
                    if (r.Equals(r2))
                        continue;

                    int doorEndCol = _randomSequenceGenerator.Get(0, r2.CellWidth) + r2.Location.Column;
                    int doorEndRow = r2.Location.Row - 1;
                    int doorStartCol = _randomSequenceGenerator.Get(0, r.CellWidth) + r.Location.Column;
                    int doorStartRow = r.CellHeight + r.Location.Row;
                    CreateHall(grid, new CellPoint(doorStartRow, doorStartCol), new CellPoint(doorEndRow, doorEndCol));
                    if (createDoors)
                    {
                        Cell door1 = grid.GetCell(doorStartCol, doorStartRow);
                        Cell door2 = grid.GetCell(doorEndCol, doorEndRow);
                        door1.Doors |= Compass.N;
                        door2.Doors |= Compass.S;
                    }
                }
            }

            //Connect W to E
            for (int i = 1; i < grid.GetRoomGridWidth(); i++)
            {
                for (int j = 0; j < grid.GetRoomGridHeight(); j++)
                {
                    CellRectangle r = grid.GetRoom(i - 1, j);
                    CellRectangle r2 = grid.GetRoom(i, j);

                    //if big room
                    if (r.Equals(r2))
                        continue;

                    int doorEndRow = _randomSequenceGenerator.Get(r2.Top, r2.Bottom);
                    int doorEndCol = r2.Location.Column - 1;
                    int doorStartRow = _randomSequenceGenerator.Get(r.Top, r.Bottom);
                    int doorStartCol = r.Right;
                    CreateHall(grid, new CellPoint(doorStartRow, doorStartCol), new CellPoint(doorEndRow, doorEndCol));
                    if (createDoors)
                    {
                        Cell door1 = grid.GetCell(doorStartCol, doorStartRow);
                        Cell door2 = grid.GetCell(doorEndCol, doorEndRow);
                        door1.Doors |= Compass.W;
                        door2.Doors |= Compass.E;
                    }
                }
            }
        }
        private void CreateHallwayLayout(LevelGrid grid, LayoutTemplate template)
        {
            var roomWidth = template.RoomDivCellWidth;
            var roomHeight = template.RoomDivCellHeight;
            var bounds = grid.GetBounds();

            CellPoint lastPoint = CellPoint.Empty;

            // Create random connected hallway using points in order
            for (int i=0;i<template.NumberHallwayPoints; i++)
            {
                if (i == 0)
                {
                    lastPoint = new CellPoint(_randomSequenceGenerator.Get(bounds.Top + 1, bounds.Bottom - 1),
                                              _randomSequenceGenerator.Get(bounds.Left + 1, bounds.Right - 1));
                }

                var nextPoint = new CellPoint(_randomSequenceGenerator.Get(bounds.Top + 1, bounds.Bottom - 1),
                                              _randomSequenceGenerator.Get(bounds.Left + 1, bounds.Right - 1));

                CreateHall(grid, lastPoint, nextPoint);

                lastPoint = nextPoint;
            }
        }
        private void CreateHall(LevelGrid grid, CellPoint d1, CellPoint d2)
        {
            int midCol = (d1.Column + d2.Column) / 2;
            int midRow = (d1.Row + d2.Row) / 2;
            int col = d1.Column;
            int row = d1.Row;
            bool b = (_randomSequenceGenerator.Get() > 0.5);

            if (b)
            {
                while (col < midCol)
                    grid.AddCell(new Cell(col++, row, Compass.Null));
                while (row < midRow)
                    grid.AddCell(new Cell(col, row++, Compass.Null));
                while (col > midCol)
                    grid.AddCell(new Cell(col--, row, Compass.Null));
                while (row > midRow)
                    grid.AddCell(new Cell(col, row--, Compass.Null));
                while (col < d2.Column)
                    grid.AddCell(new Cell(col++, row, Compass.Null));
                while (row < d2.Row)
                    grid.AddCell(new Cell(col, row++, Compass.Null));
                while (col > d2.Column)
                    grid.AddCell(new Cell(col--, row, Compass.Null));
                while (row > d2.Row)
                    grid.AddCell(new Cell(col, row--, Compass.Null));
            }
            else
            {
                while (row < midRow)
                    grid.AddCell(new Cell(col, row++, Compass.Null));
                while (col < midCol)
                    grid.AddCell(new Cell(col++, row, Compass.Null));
                while (row > midRow)
                    grid.AddCell(new Cell(col, row--, Compass.Null));
                while (col > midCol)
                    grid.AddCell(new Cell(col--, row, Compass.Null));
                while (row < d2.Row)
                    grid.AddCell(new Cell(col, row++, Compass.Null));
                while (col < d2.Column)
                    grid.AddCell(new Cell(col++, row, Compass.Null));
                while (row > d2.Row)
                    grid.AddCell(new Cell(col, row--, Compass.Null));
                while (col > d2.Column)
                    grid.AddCell(new Cell(col--, row, Compass.Null));
            }

            // Add the final cell to the hallway
            grid.AddCell(new Cell(d2.Column, d2.Row, Compass.Null));
        }

        /// <summary>
        /// Creates Walls for cells in the rooms - does NOT take into account adjoining halls.
        /// Must further process the grid once room cells are created
        /// </summary>
        private void CreateRoomCells(LevelGrid grid)
        {
            //Create Room Cells
            for (int ir = 0; ir < grid.GetRoomGridWidth(); ir++)
            {
                for (int jr = 0; jr < grid.GetRoomGridHeight(); jr++)
                {
                    CellRectangle r = grid.GetRoom(ir, jr);
                    for (int i = r.Left; i < r.Right; i++)
                    {
                        for (int j = r.Top; j < r.Bottom; j++)
                        {
                            if (grid.GetCell(i, j) == null)
                                grid.AddCell(new Cell(i, j, Compass.Null));
                        }
                    }
                }
            }
        }
        private void EdgeCells(LevelGrid grid, bool createRoomEdges)
        {
            //Create room walls first to make sure adjoining halls will be separated from the rooms
            for (int ri = 0; ri < grid.GetRoomGridWidth() && createRoomEdges; ri++)
            {
                for (int rj = 0; rj < grid.GetRoomGridHeight(); rj++)
                {
                    CellRectangle room = grid.GetRoom(ri, rj);

                    //Top edge / Bottom edge
                    for (int ci = room.Left; ci < room.Right; ci++)
                    {
                        if (!grid[ci, room.Top].IsDoor)
                            grid[ci, room.Top].Walls |= Compass.N;

                        if (!grid[ci, room.Bottom - 1].IsDoor)
                            grid[ci, room.Bottom - 1].Walls |= Compass.S;
                    }

                    //Left edge / Right edge
                    for (int cj = room.Top; cj < room.Bottom; cj++)
                    {
                        if (!grid[room.Left, cj].IsDoor)
                            grid[room.Left, cj].Walls |= Compass.W;

                        if (!grid[room.Right - 1, cj].IsDoor)
                            grid[room.Right - 1, cj].Walls |= Compass.E;
                    }
                }
            }

            //set walls
            for (int i = 0; i < grid.GetBounds().Right; i++)
            {
                for (int j = 0; j < grid.GetBounds().Bottom; j++)
                {
                    if (grid[i, j] == null)
                        continue;

                    //grid[i, j].Walls = Compass.Null;

                    if (i - 1 > 0)
                    {
                        if (grid[i - 1, j] == null)
                            grid[i, j].Walls |= Compass.W;
                        else if ((grid[i - 1, j].Walls & Compass.E) != 0)
                            grid[i, j].Walls |= Compass.W;
                    }
                    if (i + 1 < grid.GetBounds().Right)
                    {
                        if (grid[i + 1, j] == null)
                            grid[i, j].Walls |= Compass.E;
                        else if ((grid[i + 1, j].Walls & Compass.W) != 0)
                            grid[i, j].Walls |= Compass.E;
                    }
                    if (i - 1 == 0)
                        grid[i, j].Walls |= Compass.W;
                    if (i + 1 == grid.GetBounds().Right)
                        grid[i, j].Walls |= Compass.E;

                    if (j - 1 > 0)
                    {
                        if (grid[i, j - 1] == null)
                            grid[i, j].Walls |= Compass.N;
                        else if ((grid[i, j - 1].Walls & Compass.S) != 0)
                            grid[i, j].Walls |= Compass.N;
                    }
                    if (j + 1 < grid.GetBounds().Bottom)
                    {
                        if (grid[i, j + 1] == null)
                            grid[i, j].Walls |= Compass.S;
                        else if ((grid[i, j + 1].Walls & Compass.N) != 0)
                            grid[i, j].Walls |= Compass.S;
                    }
                    if (j - 1 == 0)
                        grid[i, j].Walls |= Compass.N;
                    if (j + 1 == grid.GetBounds().Bottom)
                        grid[i, j].Walls |= Compass.S;
                }
            }
        }
        private void FinishDoors(LevelGrid grid, LayoutTemplate template)
        {
            //set walls
            for (int i = 0; i < grid.GetBounds().Right; i++)
            {
                for (int j = 0; j < grid.GetBounds().Bottom; j++)
                {
                    if (grid[i, j] == null)
                        continue;

                    //Remove walls that overlap the door
                    //Set search counters at random
                    if (grid[i, j].IsDoor)
                    {
                        grid[i, j].Walls &= grid[i, j].Walls & ~grid[i, j].Doors;
                        Compass d = grid[i, j].Doors;
                        if ((d & Compass.N) != 0 && grid[i, j - 1] != null)
                        {
                            int ctr = grid[i, j].NorthDoorSearchCounter;
                            if (ctr == 0 && _randomSequenceGenerator.Get() < template.HiddenDoorProbability)
                            {
                                ctr = _randomSequenceGenerator.Get(3, 20);
                                grid[i, j].NorthDoorSearchCounter = ctr;
                            }

                            grid[i, j - 1].Walls &= ~Compass.S;
                            grid[i, j - 1].Doors |= Compass.S;
                            grid[i, j - 1].SouthDoorSearchCounter = ctr;
                        }
                        if ((d & Compass.S) != 0 && grid[i, j + 1] != null)
                        {
                            int ctr = grid[i, j].SouthDoorSearchCounter;
                            if (ctr == 0 && _randomSequenceGenerator.Get() < template.HiddenDoorProbability)
                            {
                                ctr = _randomSequenceGenerator.Get(3, 20);
                                grid[i, j].SouthDoorSearchCounter = ctr;
                            }

                            grid[i, j + 1].Walls &= ~Compass.N;
                            grid[i, j + 1].Doors |= Compass.N;
                            grid[i, j + 1].NorthDoorSearchCounter = ctr;
                        }
                        if ((d & Compass.E) != 0 && grid[i + 1, j] != null)
                        {
                            int ctr = grid[i, j].EastDoorSearchCounter;
                            if (ctr == 0 && _randomSequenceGenerator.Get() < template.HiddenDoorProbability)
                            {
                                ctr = _randomSequenceGenerator.Get(3, 20);
                                grid[i, j].EastDoorSearchCounter = ctr;
                            }

                            grid[i + 1, j].Walls &= ~Compass.W;
                            grid[i + 1, j].Doors |= Compass.W;
                            grid[i + 1, j].WestDoorSearchCounter = ctr;
                        }
                        if ((d & Compass.W) != 0 && grid[i - 1, j] != null)
                        {
                            int ctr = grid[i, j].WestDoorSearchCounter;
                            if (ctr == 0 && _randomSequenceGenerator.Get() < template.HiddenDoorProbability)
                            {
                                ctr = _randomSequenceGenerator.Get(3, 20);
                                grid[i, j].WestDoorSearchCounter = ctr;
                            }

                            grid[i - 1, j].Walls &= ~Compass.E;
                            grid[i - 1, j].Doors |= Compass.E;
                            grid[i - 1, j].EastDoorSearchCounter = ctr;
                        }
                    }
                }
            }
        }
        private void CreateMaze(LevelGrid grid, int numRemovals)
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
                        double d = _randomSequenceGenerator.Get();

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
                    if (ce.Location.Row + 1 < bnds.CellHeight)
                    {
                        grid[ce.Location.Column, ce.Location.Row + 1].Walls &= ~Compass.N;
                        ce.Walls &= ~Compass.S;
                    }
                }
                else if (d < 0.75)
                {
                    Cell ce = history[_randomSequenceGenerator.Get(0, history.Count)];
                    if (ce.Location.Column + 1 < bnds.CellWidth)
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
        }
    }
}
