﻿using System;
using System.Runtime.Serialization;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout
{
    /// <summary>
    /// Serializable data structure to store calculated room information
    /// </summary>
    [Serializable]
    public class Room : ISerializable
    {
        public GridLocation[] Cells { get; private set; }
        public GridLocation[] EdgeCells { get; private set; }
        public CellRectangle Bounds { get; private set; }

        public Room(GridLocation[] cells, GridLocation[] edgeCells, CellRectangle bounds)
        {
            this.Cells = cells;
            this.EdgeCells = edgeCells;
            this.Bounds = bounds;
        }
        public Room(SerializationInfo info, StreamingContext context)
        {
            this.Cells = new GridLocation[info.GetInt32("CellsLength")];
            this.EdgeCells = new GridLocation[info.GetInt32("EdgeCellsLength")];
            this.Bounds = (CellRectangle)info.GetValue("Bounds", typeof(CellRectangle));

            for (int i = 0; i < this.Cells.Length; i++)
                this.Cells[i] = (GridLocation)info.GetValue("Cell" + i.ToString(), typeof(GridLocation));

            for (int i = 0; i < this.EdgeCells.Length; i++)
                this.EdgeCells[i] = (GridLocation)info.GetValue("EdgeCell" + i.ToString(), typeof(GridLocation));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CellsLength", this.Cells.Length);
            info.AddValue("EdgeCellsLength", this.EdgeCells.Length);
            info.AddValue("Bounds", this.Bounds);

            var counter = 0;

            foreach (var cell in this.Cells)
                info.AddValue("Cell" + counter++.ToString(), cell);

            counter = 0;

            foreach (var cell in this.EdgeCells)
                info.AddValue("EdgeCell" + counter++.ToString(), cell);
        }
    }
}
